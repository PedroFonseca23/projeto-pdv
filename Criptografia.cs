using System;
using System.Security.Cryptography;
using System.Text;

namespace PDVCompleto
{
    /// <summary>
    /// Protege segredos (como a senha do certificado digital) usando DPAPI do
    /// Windows, no escopo de máquina local. Mesmo que o gestor.db seja copiado
    /// ou vazado, o valor criptografado só é legível no computador onde foi salvo.
    /// </summary>
    public static class Criptografia
    {
        private const string Prefixo = "DPAPI$";
        private static readonly byte[] Entropia = Encoding.UTF8.GetBytes("PDVPRO_CERT_V1");

        public static string Proteger(string textoPuro)
        {
            if (string.IsNullOrEmpty(textoPuro)) return "";

            byte[] dados = Encoding.UTF8.GetBytes(textoPuro);
            byte[] protegido = ProtectedData.Protect(dados, Entropia, DataProtectionScope.LocalMachine);
            return Prefixo + Convert.ToBase64String(protegido);
        }

        public static string Desproteger(string textoArmazenado)
        {
            if (string.IsNullOrEmpty(textoArmazenado)) return "";

            if (!textoArmazenado.StartsWith(Prefixo))
            {
                // Valor antigo, salvo em texto puro por uma versão anterior do sistema.
                return textoArmazenado;
            }

            try
            {
                byte[] protegido = Convert.FromBase64String(textoArmazenado.Substring(Prefixo.Length));
                byte[] dados = ProtectedData.Unprotect(protegido, Entropia, DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(dados);
            }
            catch
            {
                return "";
            }
        }
    }
}
