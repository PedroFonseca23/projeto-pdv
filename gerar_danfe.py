# -*- coding: utf-8 -*-
import os
import textwrap
from datetime import datetime
from reportlab.lib.pagesizes import A4
from reportlab.lib.units import mm
from reportlab.lib import colors
from reportlab.pdfgen import canvas
from reportlab.graphics.barcode.code128 import Code128

def formatar_moeda(valor):
    try:
        return f"{float(valor):.2f}".replace('.', ',')
    except:
        return "0,00"

def _quebrar_chave(chave):
    chave = (chave or "").replace(" ", "")
    blocos = textwrap.wrap(chave, 4)
    return " ".join(blocos)

def _caixa(c, x, y, w, h, titulo="", valor="", font_val="Helvetica", size_val=8, align="left"):
    """Função auxiliar para desenhar os quadros padrão do DANFE."""
    c.setLineWidth(0.5)
    c.setStrokeColor(colors.black)
    c.rect(x, y, w, h, stroke=1, fill=0)
    
    if titulo:
        c.setFont("Helvetica-Bold", 4.5)
        c.setFillColor(colors.black)
        c.drawString(x + 1*mm, y + h - 2*mm, titulo.upper())
        
    if valor:
        c.setFont(font_val, size_val)
        str_valor = str(valor)[:100] # Limite de segurança para não vazar do quadro
        if align == "left":
            c.drawString(x + 1*mm, y + 1.5*mm, str_valor)
        elif align == "center":
            c.drawCentredString(x + w/2.0, y + 1.5*mm, str_valor)
        elif align == "right":
            c.drawRightString(x + w - 1*mm, y + 1.5*mm, str_valor)

def gerar_danfe(caminho_saida, empresa, cliente, pedido, itens, chave_acesso, protocolo, numero_nfe, ambiente="HOMOLOGACAO"):
    os.makedirs(os.path.dirname(caminho_saida) or ".", exist_ok=True)
    c = canvas.Canvas(caminho_saida, pagesize=A4)
    largura, altura = A4
    margem = 5 * mm
    x0 = margem
    largura_util = largura - 2 * margem
    
    # ---------------- MARCA D'ÁGUA ----------------
    if ambiente == "HOMOLOGACAO":
        c.saveState()
        c.setFont("Helvetica-Bold", 45)
        c.setFillColor(colors.Color(0.85, 0.1, 0.1, alpha=0.15))
        c.translate(largura / 2, altura / 2)
        c.rotate(45)
        c.drawCentredString(0, 0, "SEM VALOR FISCAL")
        c.drawCentredString(0, -50, "AMBIENTE DE HOMOLOGACAO")
        c.restoreState()

    y = altura - margem

    # ---------------- CANHOTO ----------------
    h_canhoto = 15 * mm
    y -= h_canhoto
    c.setFont("Helvetica", 5)
    _caixa(c, x0, y, 165*mm, h_canhoto, 
           f"RECEBEMOS DE {empresa.get('razao_social', '')} OS PRODUTOS CONSTANTES DA NOTA FISCAL INDICADA AO LADO")
    _caixa(c, x0, y, 35*mm, 7.5*mm, "DATA DE RECEBIMENTO")
    _caixa(c, x0 + 35*mm, y, 130*mm, 7.5*mm, "IDENTIFICACAO E ASSINATURA DO RECEBEDOR")
    _caixa(c, x0 + 165*mm, y, 35*mm, h_canhoto, "NF-e", f"Nº {numero_nfe}", "Helvetica-Bold", 11, "center")
    
    # Linha tracejada
    y -= 3*mm
    c.setDash([2, 2])
    c.line(x0, y, x0 + largura_util, y)
    c.setDash([])
    
    # ---------------- CABEÇALHO ----------------
    y -= 32 * mm
    # Quadro Emitente
    _caixa(c, x0, y, 85*mm, 32*mm)
    c.setFont("Helvetica-Bold", 9)
    c.drawString(x0 + 2*mm, y + 26*mm, (empresa.get("razao_social") or "")[:40])
    c.setFont("Helvetica", 7)
    endereco_emit = f"{empresa.get('endereco', '')}, {empresa.get('endereco_numero', '')}"
    c.drawString(x0 + 2*mm, y + 20*mm, endereco_emit[:55])
    c.drawString(x0 + 2*mm, y + 16*mm, f"{empresa.get('endereco_bairro', '')} - {empresa.get('endereco_cep', '')}")
    c.drawString(x0 + 2*mm, y + 12*mm, f"{empresa.get('endereco_municipio', '')} - {empresa.get('endereco_uf', '')}")
    c.drawString(x0 + 2*mm, y + 8*mm, f"Fone: {empresa.get('telefone', '')}")
    
    # Quadro DANFE Central
    _caixa(c, x0 + 85*mm, y, 30*mm, 32*mm)
    c.setFont("Helvetica-Bold", 12)
    c.drawCentredString(x0 + 100*mm, y + 25*mm, "DANFE")
    c.setFont("Helvetica", 5)
    c.drawCentredString(x0 + 100*mm, y + 21*mm, "Documento Auxiliar da Nota")
    c.drawCentredString(x0 + 100*mm, y + 18*mm, "Fiscal Eletronica")
    c.setFont("Helvetica", 7)
    c.drawString(x0 + 87*mm, y + 13*mm, "0 - Entrada")
    c.drawString(x0 + 87*mm, y + 9*mm, "1 - Saida")
    c.rect(x0 + 108*mm, y + 9.5*mm, 4*mm, 4*mm)
    c.drawCentredString(x0 + 110*mm, y + 10.5*mm, "1") # Fixo Saída para Vendas
    c.setFont("Helvetica-Bold", 9)
    c.drawCentredString(x0 + 100*mm, y + 4*mm, f"Nº {numero_nfe}")
    c.setFont("Helvetica-Bold", 8)
    c.drawCentredString(x0 + 100*mm, y + 1*mm, "SERIE: 1")
    
    # Quadro Código de Barras / Chave
    _caixa(c, x0 + 115*mm, y, 85*mm, 32*mm)
    # Desenhar código de barras 128
    chave_digitos = (chave_acesso or "").replace(" ", "")
    if len(chave_digitos) == 44 and chave_digitos.isdigit():
        barra = Code128(chave_digitos, barHeight=11*mm, barWidth=0.3*mm, humanReadable=False)
        barra.drawOn(c, x0 + 118*mm, y + 18*mm)
        
    _caixa(c, x0 + 115*mm, y + 8*mm, 85*mm, 8*mm, "CHAVE DE ACESSO", _quebrar_chave(chave_acesso), "Helvetica-Bold", 9, "center")
    c.setFont("Helvetica", 6)
    c.drawCentredString(x0 + 157.5*mm, y + 4*mm, "Consulta de autenticidade no portal nacional da NF-e")
    c.drawCentredString(x0 + 157.5*mm, y + 1.5*mm, "www.nfe.fazenda.gov.br/portal ou no site da Sefaz Autorizadora")

    # Natureza da Operação / Protocolo
    y -= 7 * mm
    _caixa(c, x0, y, 115*mm, 7*mm, "NATUREZA DA OPERACAO", "VENDA DE MERCADORIA")
    _caixa(c, x0 + 115*mm, y, 85*mm, 7*mm, "PROTOCOLO DE AUTORIZACAO DE USO", f"{protocolo} - {datetime.now().strftime('%d/%m/%Y %H:%M:%S')}", "Helvetica", 8, "center")
    
    # Inscrições e CNPJ
    y -= 7 * mm
    _caixa(c, x0, y, 66*mm, 7*mm, "INSCRICAO ESTADUAL", empresa.get('inscricao_estadual', 'ISENTO'))
    _caixa(c, x0 + 66*mm, y, 66*mm, 7*mm, "INSC. ESTADUAL DO SUBST. TRIB.")
    _caixa(c, x0 + 132*mm, y, 68*mm, 7*mm, "CNPJ", empresa.get('cnpj', ''), "Helvetica-Bold", 9, "center")

    # ---------------- DESTINATÁRIO ----------------
    y -= 5 * mm
    c.setFont("Helvetica-Bold", 6)
    c.drawString(x0, y + 1*mm, "DESTINATARIO / REMETENTE")
    y -= 7 * mm
    _caixa(c, x0, y, 120*mm, 7*mm, "NOME / RAZAO SOCIAL", cliente.get('nome', ''))
    _caixa(c, x0 + 120*mm, y, 50*mm, 7*mm, "CNPJ / CPF", cliente.get('cpf_cnpj', ''))
    _caixa(c, x0 + 170*mm, y, 30*mm, 7*mm, "DATA DA EMISSAO", pedido.get('data_entrega', ''), "Helvetica", 8, "center")
    
    y -= 7 * mm
    end_cli = f"{cliente.get('endereco', '')}, {cliente.get('endereco_numero', '')}"
    _caixa(c, x0, y, 90*mm, 7*mm, "ENDERECO", end_cli)
    _caixa(c, x0 + 90*mm, y, 50*mm, 7*mm, "BAIRRO / DISTRITO", cliente.get('endereco_bairro', ''))
    _caixa(c, x0 + 140*mm, y, 30*mm, 7*mm, "CEP", cliente.get('endereco_cep', ''))
    _caixa(c, x0 + 170*mm, y, 30*mm, 7*mm, "DATA DA SAIDA", pedido.get('data_entrega', ''), "Helvetica", 8, "center")
    
    y -= 7 * mm
    _caixa(c, x0, y, 90*mm, 7*mm, "MUNICIPIO", cliente.get('endereco_municipio', ''))
    _caixa(c, x0 + 90*mm, y, 35*mm, 7*mm, "FONE / FAX", "-")
    _caixa(c, x0 + 125*mm, y, 15*mm, 7*mm, "UF", cliente.get('endereco_uf', ''), "Helvetica", 8, "center")
    _caixa(c, x0 + 140*mm, y, 30*mm, 7*mm, "INSCRICAO ESTADUAL", cliente.get('inscricao_estadual', 'ISENTO'))
    _caixa(c, x0 + 170*mm, y, 30*mm, 7*mm, "HORA DA SAIDA", "08:00:00", "Helvetica", 8, "center")

    # ---------------- IMPOSTOS (ZERADOS NO SIMPLES/HOMOLOGAÇÃO) ----------------
    y -= 5 * mm
    c.setFont("Helvetica-Bold", 6)
    c.drawString(x0, y + 1*mm, "CALCULO DO IMPOSTO")
    y -= 7 * mm
    _caixa(c, x0, y, 35*mm, 7*mm, "BASE DE CALC. DO ICMS", "0,00", "Helvetica", 8, "right")
    _caixa(c, x0 + 35*mm, y, 35*mm, 7*mm, "VALOR DO ICMS", "0,00", "Helvetica", 8, "right")
    _caixa(c, x0 + 70*mm, y, 35*mm, 7*mm, "BASE CALC. ICMS SUBST.", "0,00", "Helvetica", 8, "right")
    _caixa(c, x0 + 105*mm, y, 35*mm, 7*mm, "VALOR DO ICMS SUBST.", "0,00", "Helvetica", 8, "right")
    
    valor_total_itens = sum((float(i.get("quantidade") or 0) * float(i.get("valor_unitario") or 0)) for i in itens)
    _caixa(c, x0 + 140*mm, y, 60*mm, 7*mm, "VALOR TOTAL DOS PRODUTOS", formatar_moeda(valor_total_itens), "Helvetica", 8, "right")
    
    y -= 7 * mm
    _caixa(c, x0, y, 35*mm, 7*mm, "VALOR DO FRETE", "0,00", "Helvetica", 8, "right")
    _caixa(c, x0 + 35*mm, y, 35*mm, 7*mm, "VALOR DO SEGURO", "0,00", "Helvetica", 8, "right")
    _caixa(c, x0 + 70*mm, y, 35*mm, 7*mm, "DESCONTO", "0,00", "Helvetica", 8, "right")
    _caixa(c, x0 + 105*mm, y, 30*mm, 7*mm, "OUTRAS DESPESAS", "0,00", "Helvetica", 8, "right")
    _caixa(c, x0 + 135*mm, y, 25*mm, 7*mm, "VALOR DO IPI", "0,00", "Helvetica", 8, "right")
    
    total_nota = float(pedido.get("valor_total") or valor_total_itens)
    _caixa(c, x0 + 160*mm, y, 40*mm, 7*mm, "VALOR TOTAL DA NOTA", formatar_moeda(total_nota), "Helvetica-Bold", 10, "right")

    # ---------------- TRANSPORTADOR ----------------
    y -= 5 * mm
    c.setFont("Helvetica-Bold", 6)
    c.drawString(x0, y + 1*mm, "TRANSPORTADOR / VOLUMES TRANSPORTADOS")
    y -= 7 * mm
    _caixa(c, x0, y, 90*mm, 7*mm, "RAZAO SOCIAL", "O MESMO (FRETE PROPRIO)")
    _caixa(c, x0 + 90*mm, y, 30*mm, 7*mm, "FRETE POR CONTA", "0 - EMITENTE", "Helvetica", 7, "center")
    _caixa(c, x0 + 120*mm, y, 20*mm, 7*mm, "COD. ANTT")
    _caixa(c, x0 + 140*mm, y, 20*mm, 7*mm, "PLACA VEIC.")
    _caixa(c, x0 + 160*mm, y, 10*mm, 7*mm, "UF")
    _caixa(c, x0 + 170*mm, y, 30*mm, 7*mm, "CNPJ / CPF")
    
    y -= 7 * mm
    _caixa(c, x0, y, 90*mm, 7*mm, "ENDERECO")
    _caixa(c, x0 + 90*mm, y, 50*mm, 7*mm, "MUNICIPIO")
    _caixa(c, x0 + 140*mm, y, 10*mm, 7*mm, "UF")
    _caixa(c, x0 + 150*mm, y, 50*mm, 7*mm, "INSCRICAO ESTADUAL")

    # ---------------- ITENS DA NOTA ----------------
    y -= 5 * mm
    c.setFont("Helvetica-Bold", 6)
    c.drawString(x0, y + 1*mm, "DADOS DO PRODUTO / SERVICO")
    y -= 5 * mm
    
    # Cabeçalho da Tabela
    colunas = [
        ("CODIGO", 15), ("DESCRICAO", 70), ("NCM/SH", 15), ("CST", 8), 
        ("CFOP", 9), ("UN", 7), ("QTD", 11), ("VLR UNIT", 15), ("VLR TOTAL", 15), 
        ("BC ICMS", 13), ("VLR ICMS", 12), ("ALICMS", 10)
    ]
    
    xx = x0
    c.setLineWidth(0.5)
    c.rect(x0, y, largura_util, 5*mm, fill=1, stroke=1) # Fundo preto pro cabeçalho
    c.setFillColor(colors.white)
    
    for titulo, w in colunas:
        c.drawCentredString(xx + (w*mm)/2, y + 1.5*mm, titulo)
        xx += w * mm
        
    c.setFillColor(colors.black)
    
    # Linhas de Itens
    linha_h = 5 * mm
    y_itens_topo = y
    y -= linha_h
    c.setFont("Helvetica", 6)
    
    for idx, item in enumerate(itens):
        if y < 35 * mm:  # Quebra de página se passar do limite
            # Fecha a tabela atual
            c.rect(x0, y + linha_h, largura_util, y_itens_topo - (y + linha_h))
            c.showPage()
            y = altura - margem - 10*mm
            y_itens_topo = y
            # Redesenha cabeçalho
            xx = x0
            c.rect(x0, y, largura_util, 5*mm, fill=1)
            c.setFillColor(colors.white)
            for titulo, w in colunas:
                c.drawCentredString(xx + (w*mm)/2, y + 1.5*mm, titulo)
                xx += w * mm
            c.setFillColor(colors.black)
            y -= linha_h

        qtd = float(item.get("quantidade") or 0)
        vunit = float(item.get("valor_unitario") or 0)
        vtotal = qtd * vunit
        
        # Posição X para cada valor
        vals = [
            (f"{idx+1:03d}", 15, "center"),
            (item.get("descricao", "")[:45], 70, "left"),
            (item.get("ncm", "00000000"), 15, "center"),
            ("0102", 8, "center"), # CST fixo Simples Nacional
            (item.get("cfop", "5102"), 9, "center"),
            (item.get("unidade", "UN"), 7, "center"),
            (formatar_moeda(qtd), 11, "right"),
            (formatar_moeda(vunit), 15, "right"),
            (formatar_moeda(vtotal), 15, "right"),
            ("0,00", 13, "right"),
            ("0,00", 12, "right"),
            ("0,00", 10, "right")
        ]
        
        xx = x0
        for val, w, align in vals:
            if align == "left":
                c.drawString(xx + 1*mm, y + 1.5*mm, val)
            elif align == "center":
                c.drawCentredString(xx + (w*mm)/2, y + 1.5*mm, val)
            elif align == "right":
                c.drawRightString(xx + (w*mm) - 1*mm, y + 1.5*mm, val)
            xx += w * mm
            
        y -= linha_h

    # Desenha o grid externo da tabela e colunas
    altura_tabela = y_itens_topo - (y + linha_h)
    c.rect(x0, y + linha_h, largura_util, altura_tabela)
    xx = x0
    for titulo, w in colunas:
        xx += w * mm
        c.line(xx, y + linha_h, xx, y_itens_topo + 5*mm)

    # ---------------- DADOS ADICIONAIS / RODAPÉ ----------------
    y = 30 * mm # Fixa os dados adicionais no final da página
    c.setFont("Helvetica-Bold", 6)
    c.drawString(x0, y + 1*mm, "DADOS ADICIONAIS")
    y -= 25 * mm
    _caixa(c, x0, y, 130*mm, 25*mm, "INFORMACOES COMPLEMENTARES")
    c.setFont("Helvetica", 6)
    c.drawString(x0 + 2*mm, y + 20*mm, "DOCUMENTO EMITIDO POR ME OU EPP OPTANTE PELO SIMPLES NACIONAL.")
    c.drawString(x0 + 2*mm, y + 17*mm, "NAO GERA DIREITO A CREDITO FISCAL DE IPI.")
    
    if ambiente == "HOMOLOGACAO":
        c.setFillColor(colors.red)
        c.drawString(x0 + 2*mm, y + 12*mm, "NOTA EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL")
        c.setFillColor(colors.black)
    else:
        c.drawString(x0 + 2*mm, y + 12*mm, f"Referente ao Pedido Interno Nº {pedido.get('id', '')}")

    _caixa(c, x0 + 130*mm, y, 70*mm, 25*mm, "RESERVADO AO FISCO")

    c.showPage()
    c.save()
    return caminho_saida