import sqlite3
import os

# Define os arquivos
banco_antigo = 'gestor_antigo.db'
banco_novo = 'gestor.db'

if not os.path.exists(banco_antigo):
    print("ERRO: O arquivo 'gestor_antigo.db' não foi encontrado na pasta.")
    exit()

print("Conectando aos bancos de dados...")
conn = sqlite3.connect(banco_novo)
cur = conn.cursor()

# Anexa o banco antigo para puxar as informações
cur.execute(f"ATTACH DATABASE '{banco_antigo}' AS antigo")

try:
    print("1. Copiando Clientes...")
    cur.execute("INSERT OR IGNORE INTO main.clientes (id, nome, telefone) SELECT id, nome, telefone FROM antigo.clientes")

    print("2. Copiando Usuários...")
    cur.execute("INSERT OR IGNORE INTO main.usuarios (id, nome, email, senha_hash) SELECT id, nome, email, senha_hash FROM antigo.usuarios")

    print("3. Copiando Produtos...")
    cur.execute("INSERT OR IGNORE INTO main.produtos (id, nome, descricao) SELECT id, nome, descricao FROM antigo.produtos")

    print("4. Copiando Pedidos e atualizando os Valores Pagos...")
    cur.execute("""
        INSERT OR IGNORE INTO main.pedidos 
        (id, fk_cliente_id, evento, data_entrega, data_criacao, valor_total, valor_sinal, descricao, valor_pago) 
        SELECT id, fk_cliente_id, evento, data_entrega, data_criacao, valor_total, valor_sinal, descricao, valor_sinal 
        FROM antigo.pedidos
    """)

    print("5. Copiando Itens dos Pedidos...")
    cur.execute("INSERT OR IGNORE INTO main.pedido_itens (id, fk_pedido_id, fk_produto_id, quantidade) SELECT id, fk_pedido_id, fk_produto_id, quantidade FROM antigo.pedido_itens")

    print("6. Gerando transações de Caixa para o Dashboard...")
    # Aqui nós pegamos o "valor_sinal" que você já tinha nos pedidos antigos 
    # e criamos uma entrada no Caixa para que o Dashboard não fique zerado.
    cur.execute("""
        INSERT OR IGNORE INTO main.pagamentos (fk_pedido_id, valor_pago, data_pagamento, metodo)
        SELECT id, valor_sinal, COALESCE(data_criacao, '2025-01-01'), 'Migração de Sistema'
        FROM antigo.pedidos 
        WHERE valor_sinal > 0
    """)

    conn.commit()
    print("\nSUCESSO! Migração concluída. Todos os dados foram unificados.")
    
except Exception as e:
    print(f"\nOcorreu um erro durante a migração: {e}")
finally:
    conn.close()