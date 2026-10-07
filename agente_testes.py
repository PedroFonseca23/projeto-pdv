import sqlite3
import time
import json

DB_PATH = 'gestor.db'

def processar_notas_homologacao():
    conn = sqlite3.connect(DB_PATH)
    cursor = conn.cursor()
    
    cursor.execute("SELECT id, fk_cliente_id, valor_total FROM pedidos WHERE status_fiscal = 'PENDENTE'")
    pedidos = cursor.fetchall()
    
    cursor.execute("SELECT razao_social, cnpj, inscricao_estadual FROM empresa LIMIT 1")
    empresa = cursor.fetchone()
    
    if not empresa:
        if pedidos:
            print("Atencao: Cadastre os dados da Empresa no sistema antes de emitir notas.")
        conn.close()
        return
        
    for p in pedidos:
        pedido_id = p[0]
        cliente_id = p[1]
        valor_total = p[2]
        
        print(f"\n[+] Iniciando simulacao de NF-e para o Pedido ID: {pedido_id}")
        
        cursor.execute("SELECT nome, cpf_cnpj, inscricao_estadual FROM clientes WHERE id = ?", (cliente_id,))
        cliente = cursor.fetchone()
        
        cursor.execute("""
            SELECT pr.nome, pr.ncm, pr.cfop, pr.unidade, pr.preco, i.quantidade 
            FROM pedido_itens i 
            JOIN produtos pr ON i.fk_produto_id = pr.id 
            WHERE i.fk_pedido_id = ?
        """, (pedido_id,))
        itens = cursor.fetchall()
        
        lista_itens = []
        for idx, item in enumerate(itens):
            lista_itens.append({
                "numero_item": idx + 1,
                "descricao": item[0],
                "ncm": item[1],
                "cfop": item[2],
                "unidade_comercial": item[3],
                "valor_unitario": item[4],
                "quantidade": item[5]
            })
        
        payload_nfe = {
            "ambiente": 2, # 2 = Homologacao (Teste)
            "natureza_operacao": "Venda de Mercadoria",
            "empresa": {
                "razao_social": empresa[0],
                "cnpj": empresa[1],
                "inscricao_estadual": empresa[2]
            },
            "cliente": {
                "nome": cliente[0],
                "cpf_cnpj": cliente[1],
                "inscricao_estadual": cliente[2]
            },
            "itens": lista_itens,
            "valor_total": valor_total
        }
        
        print(">> Estrutura JSON gerada para envio:")
        print(json.dumps(payload_nfe, indent=4))
        
        try:
            cursor.execute("UPDATE pedidos SET status_fiscal = 'PROCESSANDO_SEFAZ' WHERE id = ?", (pedido_id,))
            conn.commit()
            print(">> Status atualizado para PROCESSANDO_SEFAZ...")
            
            time.sleep(3) 
            
            cursor.execute("UPDATE pedidos SET status_fiscal = 'AUTORIZADA_HOMOLOGACAO' WHERE id = ?", (pedido_id,))
            conn.commit()
            print(f">> SUCESSO! NF-e {pedido_id} autorizada em ambiente de simulacao!")
            
        except Exception as e:
            print(f"Erro no processamento da NF-e: {e}")
            
    conn.close()

if __name__ == "__main__":
    print("=== Servico de Simulacao de NF-e Iniciado ===")
    print("Aguardando novos pedidos marcados como PENDENTE no banco de dados...")
    while True:
        processar_notas_homologacao()
        time.sleep(5)