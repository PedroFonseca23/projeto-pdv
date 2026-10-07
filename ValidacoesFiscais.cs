using System;
using System.Linq;

namespace PDVCompleto
{
    /// <summary>
    /// Validações mínimas para reduzir rejeição de NF-e pela Sefaz:
    /// dígito verificador de CPF/CNPJ e presença dos campos de endereço
    /// que a nota exige (hoje o sistema mandava "Rua Padrão"/"00000000"
    /// fixos no código para todo cliente, o que não é válido).
    /// </summary>
    public static class ValidacoesFiscais
    {
        public static string SomenteDigitos(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            return new string(texto.Where(char.IsDigit).ToArray());
        }

        public static bool CpfValido(string cpf)
        {
            cpf = SomenteDigitos(cpf);
            if (cpf.Length != 11) return false;
            if (new string(cpf[0], 11) == cpf) return false; // todos os dígitos iguais

            int[] pesos1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] pesos2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            int dv1 = CalcularDigitoVerificador(cpf.Substring(0, 9), pesos1);
            int dv2 = CalcularDigitoVerificador(cpf.Substring(0, 9) + dv1, pesos2);

            return cpf == cpf.Substring(0, 9) + dv1.ToString() + dv2.ToString();
        }

        public static bool CnpjValido(string cnpj)
        {
            cnpj = SomenteDigitos(cnpj);
            if (cnpj.Length != 14) return false;
            if (new string(cnpj[0], 14) == cnpj) return false;

            int[] pesos1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] pesos2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int dv1 = CalcularDigitoVerificador(cnpj.Substring(0, 12), pesos1);
            int dv2 = CalcularDigitoVerificador(cnpj.Substring(0, 12) + dv1, pesos2);

            return cnpj == cnpj.Substring(0, 12) + dv1.ToString() + dv2.ToString();
        }

        /// <summary>Valida CPF (11 dígitos) ou CNPJ (14 dígitos) de acordo com o tamanho.</summary>
        public static bool DocumentoValido(string cpfOuCnpj)
        {
            string digitos = SomenteDigitos(cpfOuCnpj);
            if (digitos.Length == 11) return CpfValido(digitos);
            if (digitos.Length == 14) return CnpjValido(digitos);
            return false;
        }

        private static int CalcularDigitoVerificador(string baseNumeros, int[] pesos)
        {
            int soma = 0;
            for (int i = 0; i < pesos.Length; i++)
            {
                soma += (baseNumeros[i] - '0') * pesos[i];
            }
            int resto = soma % 11;
            return (resto < 2) ? 0 : 11 - resto;
        }

        /// <summary>
        /// Confere se os campos mínimos de endereço para NF-e estão preenchidos.
        /// Retorna null se estiver tudo certo, ou a mensagem do que falta.
        /// </summary>
        public static string ValidarEnderecoCompleto(string logradouro, string numero, string bairro,
            string municipio, string uf, string cep)
        {
            if (string.IsNullOrWhiteSpace(logradouro)) return "Endereço (rua/avenida) não informado.";
            if (string.IsNullOrWhiteSpace(numero)) return "Número do endereço não informado.";
            if (string.IsNullOrWhiteSpace(bairro)) return "Bairro não informado.";
            if (string.IsNullOrWhiteSpace(municipio)) return "Município não informado.";
            if (string.IsNullOrWhiteSpace(uf) || uf.Trim().Length != 2) return "UF inválida (use a sigla de 2 letras, ex.: RJ).";
            if (SomenteDigitos(cep).Length != 8) return "CEP inválido (precisa ter 8 dígitos).";
            return null;
        }
    }
}
