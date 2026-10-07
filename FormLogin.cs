using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace PDVCompleto
{
    public class FormLogin : Form
    {
        private TextBox txtEmailLogin;
        private TextBox txtSenhaLogin;
        private TextBox txtNomeCad;
        private TextBox txtEmailCad;
        private TextBox txtSenhaCad;
        private TextBox txtConfirmarSenhaCad;
        private Label lblAvisoLogin;
        private Label lblAvisoCad;
        private PainelArredondado cardLogin;
        private PainelArredondado cardCadastro;
        
        // Transformamos os botões em variáveis globais da classe
        private Button btnEntrar;
        private Button btnCadastrar;

        public string EmailLogado { get; private set; }

        public FormLogin()
        {
            this.EmailLogado = "";
            this.Text = "Login - PDV PRO";
            this.Size = new Size(800, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Cores.Fundo;

            try
            {
                this.BackgroundImage = Image.FromFile("fundo.jpg");
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.Icon = new Icon("icone.ico");
            }
            catch { }

            MontarTelaLogin();
            MontarTelaCadastro();

            bool estaVazio = VerificarSeEstaVazio();
            cardLogin.Visible = !estaVazio;
            cardCadastro.Visible = estaVazio;
            this.AcceptButton = estaVazio ? btnCadastrar : btnEntrar;
        }

        private bool VerificarSeEstaVazio()
        {
            try
            {
                DataTable dt = BancoDeDados.Buscar("SELECT COUNT(*) FROM usuarios");
                return Convert.ToInt32(dt.Rows[0][0]) == 0;
            }
            catch
            {
                return false;
            }
        }

        private void MontarTelaLogin()
        {
            cardLogin = new PainelArredondado { Location = new Point(250, 90), Size = new Size(300, 380), BackColor = Cores.Superficie, Raio = 16 };

            Label lblTitulo = new Label { Text = "  PDV PRO", ForeColor = Cores.Primario, Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            Label lblSubtitulo = new Label { Text = "Acesse sua conta", ForeColor = Cores.TextoSecundario, Font = new Font("Segoe UI", 9), Location = new Point(20, 55), AutoSize = true };

            cardLogin.Controls.Add(lblTitulo);
            cardLogin.Controls.Add(lblSubtitulo);

            cardLogin.Controls.Add(ComponentesVisuais.CriarLabel("E-mail", 20, 90));
            cardLogin.Controls.Add(ComponentesVisuais.CriarTextBoxWrapper(20, 115, 260, out txtEmailLogin));

            cardLogin.Controls.Add(ComponentesVisuais.CriarLabel("Senha", 20, 170));
            cardLogin.Controls.Add(ComponentesVisuais.CriarTextBoxWrapper(20, 195, 260, out txtSenhaLogin));
            txtSenhaLogin.PasswordChar = '*';

            // Usando a variável da classe
            btnEntrar = ComponentesVisuais.CriarBotao("Entrar", 20, 250, 260, Cores.Primario, Color.White);
            btnEntrar.Click += (s, e) => FazerLogin();
            cardLogin.Controls.Add(btnEntrar);

            Label lblAlternar = new Label { Text = "Não tem conta? Cadastre-se", ForeColor = Cores.Primario, Font = new Font("Segoe UI", 9, FontStyle.Underline), Location = new Point(20, 305), Size = new Size(260, 20), TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };
            lblAlternar.Click += (s, e) => AlternarTelas(false);
            cardLogin.Controls.Add(lblAlternar);

            lblAvisoLogin = new Label { Location = new Point(20, 335), Size = new Size(260, 40), ForeColor = Color.FromArgb(220, 38, 38), Font = new Font("Segoe UI", 8), Text = "" };
            cardLogin.Controls.Add(lblAvisoLogin);

            this.Controls.Add(cardLogin);
        }

        private void MontarTelaCadastro()
        {
            cardCadastro = new PainelArredondado { Location = new Point(250, 60), Size = new Size(300, 560), BackColor = Cores.Superficie, Raio = 16 };

            Label lblTitulo = new Label { Text = "  PDV PRO", ForeColor = Cores.Primario, Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            Label lblSubtitulo = new Label { Text = "Crie sua conta", ForeColor = Cores.TextoSecundario, Font = new Font("Segoe UI", 9), Location = new Point(20, 55), AutoSize = true };

            cardCadastro.Controls.Add(lblTitulo);
            cardCadastro.Controls.Add(lblSubtitulo);

            cardCadastro.Controls.Add(ComponentesVisuais.CriarLabel("Seu nome", 20, 90));
            cardCadastro.Controls.Add(ComponentesVisuais.CriarTextBoxWrapper(20, 115, 260, out txtNomeCad));

            cardCadastro.Controls.Add(ComponentesVisuais.CriarLabel("E-mail", 20, 170));
            cardCadastro.Controls.Add(ComponentesVisuais.CriarTextBoxWrapper(20, 195, 260, out txtEmailCad));

            cardCadastro.Controls.Add(ComponentesVisuais.CriarLabel("Senha (mínimo 8)", 20, 250));
            cardCadastro.Controls.Add(ComponentesVisuais.CriarTextBoxWrapper(20, 275, 260, out txtSenhaCad));
            txtSenhaCad.PasswordChar = '*';

            cardCadastro.Controls.Add(ComponentesVisuais.CriarLabel("Confirmar senha", 20, 330));
            cardCadastro.Controls.Add(ComponentesVisuais.CriarTextBoxWrapper(20, 355, 260, out txtConfirmarSenhaCad));
            txtConfirmarSenhaCad.PasswordChar = '*';

            // Usando a variável da classe
            btnCadastrar = ComponentesVisuais.CriarBotao("Cadastrar", 20, 415, 260, Cores.Primario, Color.White);
            btnCadastrar.Click += (s, e) => CriarConta();
            cardCadastro.Controls.Add(btnCadastrar);

            Label lblAlternar = new Label { Text = "Já tem conta? Entre aqui", ForeColor = Cores.Primario, Font = new Font("Segoe UI", 9, FontStyle.Underline), Location = new Point(20, 470), Size = new Size(260, 20), TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand };
            lblAlternar.Click += (s, e) => AlternarTelas(true);
            cardCadastro.Controls.Add(lblAlternar);

            lblAvisoCad = new Label { Location = new Point(20, 500), Size = new Size(260, 50), ForeColor = Color.FromArgb(220, 38, 38), Font = new Font("Segoe UI", 8), Text = "" };
            cardCadastro.Controls.Add(lblAvisoCad);

            this.Controls.Add(cardCadastro);
        }

        private void AlternarTelas(bool irParaLogin)
        {
            cardLogin.Visible = irParaLogin;
            cardCadastro.Visible = !irParaLogin;
            
            // Agora apontamos diretamente para os botões corretos
            this.AcceptButton = irParaLogin ? btnEntrar : btnCadastrar;
            
            lblAvisoLogin.Text = "";
            lblAvisoCad.Text = "";
        }

        private void FazerLogin()
        {
            lblAvisoLogin.Text = "";
            string email = (txtEmailLogin.Text ?? "").Trim();
            string senhaDigitada = txtSenhaLogin.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senhaDigitada))
            {
                lblAvisoLogin.Text = "Preencha e-mail e senha.";
                return;
            }

            try
            {
                DataTable dt = BancoDeDados.Buscar("SELECT id, nome, senha_hash FROM usuarios WHERE email = @email", new[] { new SQLiteParameter("@email", email) });

                if (dt.Rows.Count == 0)
                {
                    lblAvisoLogin.Text = "E-mail ou senha incorretos.";
                    return;
                }

                int idUsuario = Convert.ToInt32(dt.Rows[0]["id"]);
                string nomeUsuario = dt.Rows[0]["nome"].ToString();
                byte[] hashSalvo = (byte[])dt.Rows[0]["senha_hash"];

                if (Seguranca.EhHashLegado(hashSalvo))
                {
                    bool redefiniu = SolicitarRedefinicaoDeSenha(idUsuario, email, nomeUsuario);
                    if (redefiniu)
                    {
                        this.EmailLogado = email;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    return;
                }

                if (Seguranca.VerificarSenha(senhaDigitada, hashSalvo))
                {
                    this.EmailLogado = email;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                lblAvisoLogin.Text = "E-mail ou senha incorretos.";
            }
            catch (Exception ex)
            {
                lblAvisoLogin.Text = "Erro ao acessar o banco: " + ex.Message;
            }
        }

        private void CriarConta()
        {
            lblAvisoCad.Text = "";
            string nome = (txtNomeCad.Text ?? "").Trim();
            string email = (txtEmailCad.Text ?? "").Trim().ToLower();
            string senha = txtSenhaCad.Text ?? "";
            string confirmar = txtConfirmarSenhaCad.Text ?? "";

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email))
            {
                lblAvisoCad.Text = "Preencha seu nome e e-mail.";
                return;
            }

            if (senha.Length < 8)
            {
                lblAvisoCad.Text = "A senha precisa ter pelo menos 8 caracteres.";
                return;
            }

            if (senha != confirmar)
            {
                lblAvisoCad.Text = "As senhas não coincidem.";
                return;
            }

            try
            {
                DataTable dt = BancoDeDados.Buscar("SELECT id FROM usuarios WHERE email = @email", new[] { new SQLiteParameter("@email", email) });
                if (dt.Rows.Count > 0)
                {
                    lblAvisoCad.Text = "Este e-mail já está cadastrado.";
                    return;
                }

                byte[] hash = Seguranca.GerarHash(senha);
                BancoDeDados.Executar(
                    "INSERT INTO usuarios (nome, email, senha_hash) VALUES (@n, @e, @h)",
                    new[]
                    {
                        new SQLiteParameter("@n", nome),
                        new SQLiteParameter("@e", email),
                        new SQLiteParameter("@h", hash)
                    });

                this.EmailLogado = email;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblAvisoCad.Text = "Não foi possível criar a conta: " + ex.Message;
            }
        }

        private bool SolicitarRedefinicaoDeSenha(int idUsuario, string email, string nomeUsuario)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = "Atualização necessária";
                dlg.Size = new Size(420, 300);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;

                Label lblInfo = new Label
                {
                    Location = new Point(20, 15),
                    Size = new Size(370, 60),
                    Text = "A conta \"" + nomeUsuario + "\" foi migrada de uma versão anterior do sistema " +
                           "e precisa de uma nova senha antes de continuar.",
                    Font = new Font("Segoe UI", 9)
                };
                dlg.Controls.Add(lblInfo);

                dlg.Controls.Add(ComponentesVisuais.CriarLabel("Nova senha (mínimo 8)", 20, 85));
                TextBox txtNova;
                dlg.Controls.Add(ComponentesVisuais.CriarTextBoxWrapper(20, 108, 360, out txtNova));
                txtNova.PasswordChar = '*';

                dlg.Controls.Add(ComponentesVisuais.CriarLabel("Confirmar nova senha", 20, 155));
                TextBox txtConfirma;
                dlg.Controls.Add(ComponentesVisuais.CriarTextBoxWrapper(20, 178, 360, out txtConfirma));
                txtConfirma.PasswordChar = '*';

                Label lblErro = new Label { Location = new Point(20, 222), Size = new Size(360, 20), ForeColor = Color.FromArgb(220, 38, 38), Font = new Font("Segoe UI", 8) };
                dlg.Controls.Add(lblErro);

                Button btnSalvar = ComponentesVisuais.CriarBotao("Salvar nova senha", 20, 245, 360, Color.FromArgb(15, 23, 42), Color.White);
                dlg.Controls.Add(btnSalvar);
                dlg.AcceptButton = btnSalvar;

                bool sucesso = false;

                btnSalvar.Click += delegate
                {
                    if (txtNova.Text.Length < 8)
                    {
                        lblErro.Text = "A senha precisa ter pelo menos 8 caracteres.";
                        return;
                    }

                    if (txtNova.Text != txtConfirma.Text)
                    {
                        lblErro.Text = "As senhas não coincidem.";
                        return;
                    }

                    try
                    {
                        byte[] novoHash = Seguranca.GerarHash(txtNova.Text);
                        BancoDeDados.Executar(
                            "UPDATE usuarios SET senha_hash = @hash WHERE id = @id",
                            new[]
                            {
                                new SQLiteParameter("@hash", novoHash),
                                new SQLiteParameter("@id", idUsuario)
                            });

                        sucesso = true;
                        dlg.DialogResult = DialogResult.OK;
                        dlg.Close();
                    }
                    catch (Exception ex)
                    {
                        lblErro.Text = "Erro ao salvar: " + ex.Message;
                    }
                };

                dlg.ShowDialog(this);
                return sucesso;
            }
        }
    }
}