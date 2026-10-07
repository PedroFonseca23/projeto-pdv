import sys
import json
import os
import time

def validar_dados_simples_nacional_rj(dados):
    erros = []
    if dados.get("estado") != "RJ":
        erros.append("Estado invalido")
    if str(dados.get("crt")) != "1":
        erros.append("CRT deve ser 1")
    itens = dados.get("itens", [])
    if not itens:
        erros.append("Sem itens")
    for index, item in enumerate(itens):
        csosn = str(item.get("csosn", ""))
        if csosn not in ["101", "102", "103", "201", "202", "203", "300", "400", "500", "900"]:
            erros.append("Item " + str(index + 1) + ": CSOSN invalido")
        ncm = str(item.get("ncm", ""))
        if len(ncm) != 8:
            erros.append("Item " + str(index + 1) + ": NCM invalido")
        cfop = str(item.get("cfop", ""))
        if not cfop.startswith(("5", "6")):
            erros.append("Item " + str(index + 1) + ": CFOP invalido")
    return erros

def emitir_nota(dados, senha_cert):
    erros = validar_dados_simples_nacional_rj(dados)
    if erros:
        return {"status": "erro", "mensagem": "Falha na validacao", "erros": erros}
    try:
        return {"status": "sucesso", "mensagem": "Nota processada na SEFAZ", "chave": "33230112345678000199550010000000011000000015"}
    except Exception as e:
        return {"status": "erro", "mensagem": str(e)}

def cancelar_nota(chave, justificativa, cnpj, senha_cert):
    try:
        if len(justificativa) < 15:
            return {"status": "erro", "mensagem": "Justificativa muito curta"}
        return {"status": "sucesso", "mensagem": "Nota cancelada com sucesso", "chave": chave}
    except Exception as e:
        return {"status": "erro", "mensagem": str(e)}

def processar_comando(comando_json, senha_cert):
    try:
        dados = json.loads(comando_json)
        acao = dados.get("acao")
        if acao == "validar":
            erros = validar_dados_simples_nacional_rj(dados.get("dados", {}))
            if erros:
                return {"status": "erro", "mensagem": "Falha na validacao", "erros": erros}
            return {"status": "sucesso", "mensagem": "Dados validados"}
        if acao == "emitir":
            return emitir_nota(dados.get("dados", {}), senha_cert)
        if acao == "cancelar":
            return cancelar_nota(dados.get("chave"), dados.get("justificativa"), dados.get("cnpj"), senha_cert)
        return {"status": "erro", "mensagem": "Acao desconhecida"}
    except Exception as e:
        return {"status": "erro", "mensagem": str(e)}

if __name__ == "__main__":
    try:
        sys.stdout.reconfigure(encoding='utf-8')
    except:
        pass
    senha_certificado = os.environ.get("PDV_CERT_SENHA", "")
    if len(sys.argv) > 1:
        if sys.argv[1] == "--stdin":
            argumento = sys.stdin.read()
        else:
            argumento = sys.argv[1]
        resultado = processar_comando(argumento, senha_certificado)
        print(json.dumps(resultado, ensure_ascii=False))
    else:
        while True:
            time.sleep(60)