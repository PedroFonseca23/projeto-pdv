using System;
using System.Security.Cryptography;
using System.Text;

namespace PDVCompleto
{
    /// <summary>
    /// Gera e verifica hashes de senha. O banco real desta instalação já tinha
    /// contas com senha em formato bcrypt (de uma versão anterior do sistema),
    /// que este código nunca soube ler — por isso o login não funcionava.
    ///
    /// Novas senhas usam PBKDF2-HMAC-SHA1 (formato "PBKDF2$iteracoes$sal$hash"),
    /// sem depender de nenhuma biblioteca externa. Hashes antigos em bcrypt
    /// ($2a$/$2b$/$2y$) são detectados e tratados como "precisa redefinir senha".
    /// </summary>
    public static class Seguranca
    {
        private const int TamanhoSalBytes = 16;
        private const int TamanhoHashBytes = 32;
        private const int Iteracoes = 200000;

        public static byte[] GerarHash(string senha)
        {
            if (senha == null) senha = "";

            byte[] sal = new byte[TamanhoSalBytes];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(sal);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(senha, sal, Iteracoes))
            {
                byte[] hash = pbkdf2.GetBytes(TamanhoHashBytes);
                string texto = string.Format("PBKDF2${0}${1}${2}",
                    Iteracoes, Convert.ToBase64String(sal), Convert.ToBase64String(hash));
                return Encoding.UTF8.GetBytes(texto);
            }
        }

        public static bool EhHashLegado(byte[] hashArmazenado)
        {
            if (hashArmazenado == null || hashArmazenado.Length == 0) return false;
            try
            {
                string texto = Encoding.UTF8.GetString(hashArmazenado);
                return texto.StartsWith("$2a$") || texto.StartsWith("$2b$") || texto.StartsWith("$2y$");
            }
            catch
            {
                return false;
            }
        }

        public static bool VerificarSenha(string senhaDigitada, byte[] hashArmazenado)
        {
            if (hashArmazenado == null || senhaDigitada == null) return false;

            string texto;
            try { texto = Encoding.UTF8.GetString(hashArmazenado); }
            catch { return false; }

            string[] partes = texto.Split('$');
            if (partes.Length != 4 || partes[0] != "PBKDF2") return false;

            int iteracoes;
            if (!int.TryParse(partes[1], out iteracoes) || iteracoes <= 0) return false;

            byte[] sal, hashEsperado;
            try
            {
                sal = Convert.FromBase64String(partes[2]);
                hashEsperado = Convert.FromBase64String(partes[3]);
            }
            catch
            {
                return false;
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(senhaDigitada, sal, iteracoes))
            {
                byte[] hashCalculado = pbkdf2.GetBytes(hashEsperado.Length);
                return ComparacaoTempoConstante(hashCalculado, hashEsperado);
            }
        }

        public static string GerarSenhaTemporaria()
        {
            const string alfabeto = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            byte[] buffer = new byte[10];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
            var sb = new StringBuilder();
            foreach (byte b in buffer)
            {
                sb.Append(alfabeto[b % alfabeto.Length]);
            }
            return sb.ToString();
        }

        private static bool ComparacaoTempoConstante(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diferenca = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diferenca |= a[i] ^ b[i];
            }
            return diferenca == 0;
        }
    }
}
