using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PDVCompleto
{
    public static class AgenteFiscalManager
    {
        private static Process processo;
        public static string UltimoErro = "";
        public static string ModoIniciado = "";

        public static bool Rodando
        {
            get
            {
                try { return processo != null && !processo.HasExited; }
                catch { return false; }
            }
        }

        public static bool Iniciar()
        {
            if (Rodando) return true;

            string pasta = Application.StartupPath;
            string exePath = Path.Combine(pasta, "agente_fiscal.exe");
            string scriptPath = Path.Combine(pasta, "agente_fiscal.py");

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();

                if (File.Exists(exePath))
                {
                    psi.FileName = exePath;
                    ModoIniciado = "exe";
                }
                else if (File.Exists(scriptPath))
                {
                    string python = LocalizarPython();
                    if (python == null)
                    {
                        UltimoErro = "Erro: Python ou executavel nao localizado.";
                        return false;
                    }
                    psi.FileName = python;
                    psi.Arguments = "\"" + scriptPath + "\"";
                    ModoIniciado = "python";
                }
                else
                {
                    UltimoErro = "Erro: agente_fiscal nao encontrado.";
                    return false;
                }

                psi.WorkingDirectory = pasta;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                string senhaCert = ObterSenhaCertificadoDescriptografada();
                if (senhaCert != null)
                {
                    psi.EnvironmentVariables["PDV_CERT_SENHA"] = senhaCert;
                }
                else
                {
                    psi.EnvironmentVariables["PDV_CERT_SENHA"] = "";
                }

                processo = Process.Start(psi);
                
                if (string.IsNullOrEmpty(senhaCert)) 
                {
                    UltimoErro = "Aviso: Senha do certificado nao configurada.";
                }
                else
                {
                    UltimoErro = "";
                }

                return true;
            }
            catch (Exception ex)
            {
                UltimoErro = ex.Message;
                return false;
            }
        }

        private static string ObterSenhaCertificadoDescriptografada()
        {
            try
            {
                DataTable dt = BancoDeDados.Buscar("SELECT senha_certificado FROM empresa LIMIT 1");
                if (dt.Rows.Count == 0 || dt.Rows[0]["senha_certificado"] == DBNull.Value) return "";
                return Criptografia.Desproteger(dt.Rows[0]["senha_certificado"].ToString());
            }
            catch
            {
                return "";
            }
        }

        private static string LocalizarPython()
        {
            string[] candidatos = new string[] { "python", "python3", "py" };
            foreach (string cand in candidatos)
            {
                try
                {
                    ProcessStartInfo testeInfo = new ProcessStartInfo(cand, "--version");
                    testeInfo.UseShellExecute = false;
                    testeInfo.CreateNoWindow = true;
                    testeInfo.RedirectStandardOutput = true;
                    testeInfo.RedirectStandardError = true;

                    using (Process teste = Process.Start(testeInfo))
                    {
                        teste.WaitForExit(3000);
                        if (teste.ExitCode == 0) return cand;
                    }
                }
                catch { }
            }
            return null;
        }

        public static void Parar()
        {
            try
            {
                if (processo != null && !processo.HasExited)
                {
                    processo.Kill();
                }
            }
            catch { }
        }

        public static string ExecutarComando(string argumento)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            string pasta = Application.StartupPath;

            if (File.Exists(Path.Combine(pasta, "agente_fiscal.exe")))
            {
                startInfo.FileName = Path.Combine(pasta, "agente_fiscal.exe");
                startInfo.Arguments = "--stdin";
            }
            else
            {
                string pyPath = LocalizarPython();
                if (pyPath != null)
                {
                    startInfo.FileName = pyPath;
                }
                else
                {
                    startInfo.FileName = "python";
                }
                startInfo.Arguments = "\"agente_fiscal.py\" --stdin";
            }

            startInfo.WorkingDirectory = pasta;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            
            string senhaCert = ObterSenhaCertificadoDescriptografada();
            if (senhaCert != null)
            {
                startInfo.EnvironmentVariables["PDV_CERT_SENHA"] = senhaCert;
            }
            else
            {
                startInfo.EnvironmentVariables["PDV_CERT_SENHA"] = "";
            }

            try
            {
                using (Process p = Process.Start(startInfo))
                {
                    using (StreamWriter sw = new StreamWriter(p.StandardInput.BaseStream, System.Text.Encoding.UTF8))
                    {
                        sw.Write(argumento);
                    }
                    string output = p.StandardOutput.ReadToEnd();
                    string error = p.StandardError.ReadToEnd();
                    p.WaitForExit();
                    
                    if (string.IsNullOrEmpty(output) && !string.IsNullOrEmpty(error))
                    {
                        return "{\"status\": \"erro\", \"mensagem\": \"Erro interno: " + error.Replace("\"", "'").Replace("\r", "").Replace("\n", " ") + "\"}";
                    }
                    
                    return output;
                }
            }
            catch (Exception ex)
            {
                return "{\"status\": \"erro\", \"mensagem\": \"" + ex.Message.Replace("\"", "'").Replace("\r", "").Replace("\n", " ") + "\"}";
            }
        }

        public static string CancelarNotaFiscal(string chaveAcesso, string justificativa, string cnpj)
        {
            string argumento = string.Format("{{\"acao\": \"cancelar\", \"chave\": \"{0}\", \"justificativa\": \"{1}\", \"cnpj\": \"{2}\"}}", chaveAcesso, justificativa, cnpj);
            return ExecutarComando(argumento);
        }
        
        public static string EmitirNotaFiscal(string jsonNota)
        {
            string argumento = string.Format("{{\"acao\": \"emitir\", \"dados\": {0}}}", jsonNota);
            return ExecutarComando(argumento);
        }
    }
}