using System;
using System.Drawing;
using System.Windows.Forms;

namespace PDVCompleto
{
    public class MenuPrincipal : Form
    {
        private Panel painelMenu;
        private Panel painelLogo;
        private Label labelLogo;
        private Panel painelCabecalho;
        private Label labelTituloTela;
        private Panel painelCentral;
        private Panel painelStatusAgente;
        private Label labelStatusAgente;
        private Timer timerStatusAgente;
        private Panel indicadorAtivo;
        private string emailLogado;

        public MenuPrincipal(string emailLogado)
        {
            this.emailLogado = emailLogado ?? "";
            this.Text = "PDV PRO";
            this.Size = new Size(1280, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Cores.Fundo;
            this.MinimumSize = new Size(1024, 768);

            try { this.Icon = new Icon("icone.ico"); } catch { }

            this.painelCentral = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 20, 24, 24), BackColor = Cores.Fundo };

            this.painelCabecalho = new Panel { Height = 76, Dock = DockStyle.Top, BackColor = Cores.Superficie };
            this.labelTituloTela = new Label { Text = "Visão Geral", ForeColor = Cores.TextoPrincipal, Font = new Font("Segoe UI", 19, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true };

            Label labelUsuarioLogado = new Label
            {
                Text = "👤  " + this.emailLogado,
                ForeColor = Cores.TextoSecundario,
                Font = new Font("Segoe UI", 9.5f),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            labelUsuarioLogado.Location = new Point(this.painelCabecalho.Width - labelUsuarioLogado.PreferredWidth - 30, 30);
            this.painelCabecalho.Resize += (s, e) =>
                labelUsuarioLogado.Location = new Point(this.painelCabecalho.Width - labelUsuarioLogado.PreferredWidth - 30, 30);

            Panel linhaSeparadora = new Panel { Height = 1, Dock = DockStyle.Bottom, BackColor = Cores.Borda };
            this.painelCabecalho.Controls.Add(this.labelTituloTela);
            this.painelCabecalho.Controls.Add(labelUsuarioLogado);
            this.painelCabecalho.Controls.Add(linhaSeparadora);

            this.painelMenu = new Panel { Width = 264, Dock = DockStyle.Left, BackColor = Cores.SidebarFundo, AutoScroll = true };

            this.painelLogo = new Panel { Height = 76, Dock = DockStyle.Top, BackColor = Cores.SidebarFundoEscuro };
            this.labelLogo = new Label { Text = "◆  PDV PRO", ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            this.painelLogo.Controls.Add(this.labelLogo);

            indicadorAtivo = new Panel { Size = new Size(4, 50), BackColor = Cores.Primario, Visible = false };
            this.painelMenu.Controls.Add(indicadorAtivo);

            Button btnDashboard = CriarBotaoMenu("▦   Dashboard", 96);
            Button btnClientes = CriarBotaoMenu("◍   Clientes", 152);
            Button btnProdutos = CriarBotaoMenu("▧   Produtos", 208);
            Button btnPedidos = CriarBotaoMenu("▤   Pedidos", 264);
            Button btnNotas = CriarBotaoMenu("§   Notas Fiscais", 320);
            Button btnUsuarios = CriarBotaoMenu("☺   Usuários", 376);
            Button btnEmpresa = CriarBotaoMenu("⚙   Empresa", 432);

            btnDashboard.Click += delegate { AbrirTela(new TelaDashboard(), "Visão Geral", btnDashboard); };
            btnClientes.Click += delegate { AbrirTela(new TelaClientes(), "Gestão de Clientes", btnClientes); };
            btnProdutos.Click += delegate { AbrirTela(new TelaProdutos(), "Catálogo de Produtos", btnProdutos); };
            btnPedidos.Click += delegate { AbrirTela(new TelaPedidos(), "Central de Pedidos", btnPedidos); };
            btnNotas.Click += delegate { AbrirTela(new TelaNotasFiscais(), "Monitor de Notas Fiscais", btnNotas); };
            btnUsuarios.Click += delegate { AbrirTela(new TelaUsuarios(this.emailLogado), "Usuários do Sistema", btnUsuarios); };
            btnEmpresa.Click += delegate { AbrirTela(new TelaEmpresa(), "Configurações da Empresa", btnEmpresa); };

            this.painelMenu.Controls.Add(btnEmpresa);
            this.painelMenu.Controls.Add(btnUsuarios);
            this.painelMenu.Controls.Add(btnNotas);
            this.painelMenu.Controls.Add(btnPedidos);
            this.painelMenu.Controls.Add(btnProdutos);
            this.painelMenu.Controls.Add(btnClientes);
            this.painelMenu.Controls.Add(btnDashboard);

            this.painelStatusAgente = new Panel { Height = 60, Dock = DockStyle.Bottom, BackColor = Cores.SidebarFundoEscuro, Padding = new Padding(20, 0, 15, 0) };
            this.labelStatusAgente = new Label { Text = "●  Agente Fiscal: iniciando...", ForeColor = Cores.SidebarTextoInativo, Font = new Font("Segoe UI", 8, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true };
            this.painelStatusAgente.Controls.Add(this.labelStatusAgente);
            this.painelMenu.Controls.Add(this.painelStatusAgente);

            this.painelMenu.Controls.Add(this.painelLogo);

            this.Controls.Add(this.painelCentral);
            this.Controls.Add(this.painelCabecalho);
            this.Controls.Add(this.painelMenu);

            AbrirTela(new TelaDashboard(), "Visão Geral", btnDashboard);

            IniciarAgenteFiscal();

            this.FormClosing += MenuPrincipal_FormClosing;
        }

        private void IniciarAgenteFiscal()
        {
            AgenteFiscalManager.Iniciar();
            AtualizarStatusAgente();

            this.timerStatusAgente = new Timer();
            this.timerStatusAgente.Interval = 4000;
            this.timerStatusAgente.Tick += delegate { AtualizarStatusAgente(); };
            this.timerStatusAgente.Start();
        }

        private void AtualizarStatusAgente()
        {
            if (AgenteFiscalManager.Rodando)
            {
                this.labelStatusAgente.Text = "●  Agente Fiscal: ativo";
                this.labelStatusAgente.ForeColor = Cores.Sucesso;
            }
            else
            {
                this.labelStatusAgente.Text = "●  Agente Fiscal: parado (clique p/ detalhes)";
                this.labelStatusAgente.ForeColor = Color.FromArgb(248, 113, 113);
                this.labelStatusAgente.Cursor = Cursors.Hand;
                this.labelStatusAgente.Click -= LabelStatusAgente_Click;
                this.labelStatusAgente.Click += LabelStatusAgente_Click;
            }
        }

        private void LabelStatusAgente_Click(object sender, EventArgs e)
        {
            string msg = "O agente fiscal (responsável por autorizar as NF-e e gerar o DANFE) não está rodando.\n\n";
            msg += string.IsNullOrWhiteSpace(AgenteFiscalManager.UltimoErro)
                ? "Motivo desconhecido."
                : AgenteFiscalManager.UltimoErro;
            msg += "\n\nDeseja tentar iniciar novamente agora?";

            if (MessageBox.Show(msg, "Agente Fiscal parado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                AgenteFiscalManager.Iniciar();
                AtualizarStatusAgente();
            }
        }

        private void MenuPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.timerStatusAgente != null) this.timerStatusAgente.Stop();
            AgenteFiscalManager.Parar();
        }

        private Button CriarBotaoMenu(string texto, int posicaoY)
        {
            var btn = new BotaoModerno
            {
                Text = texto,
                Location = new Point(14, posicaoY),
                Size = new Size(236, 48),
                Raio = 10,
                BackColor = Cores.SidebarFundo,
                ForeColor = Cores.SidebarTextoInativo,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Feedback visual ao passar o mouse — só se aplica quando o botão
            // não é o item ativo no momento (guardado em Tag).
            btn.MouseEnter += (s, e) =>
            {
                if (!(btn.Tag is bool && (bool)btn.Tag))
                    btn.BackColor = Color.FromArgb(51, 65, 85);
            };
            btn.MouseLeave += (s, e) =>
            {
                if (!(btn.Tag is bool && (bool)btn.Tag))
                    btn.BackColor = Cores.SidebarFundo;
            };

            return btn;
        }

        private void AbrirTela(UserControl tela, string titulo, Button botaoAtivo)
        {
            this.labelTituloTela.Text = titulo;

            foreach (Control ctrl in this.painelMenu.Controls)
            {
                Button btn = ctrl as Button;
                if (btn != null)
                {
                    btn.Tag = false;
                    btn.BackColor = Cores.SidebarFundo;
                    btn.ForeColor = Cores.SidebarTextoInativo;
                    btn.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
                }
            }

            botaoAtivo.Tag = true;
            botaoAtivo.BackColor = Color.FromArgb(30, 41, 59);
            botaoAtivo.ForeColor = Color.White;
            botaoAtivo.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);

            indicadorAtivo.Visible = true;
            indicadorAtivo.Location = new Point(0, botaoAtivo.Location.Y - 1);
            indicadorAtivo.Size = new Size(4, botaoAtivo.Height + 2);
            indicadorAtivo.BringToFront();

            foreach (Control antigo in this.painelCentral.Controls)
            {
                antigo.Dispose();
            }
            this.painelCentral.Controls.Clear();
            tela.Dock = DockStyle.Fill;
            this.painelCentral.Controls.Add(tela);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            BancoDeDados.Inicializar();

            FormLogin login = new FormLogin();
            if (login.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new MenuPrincipal(login.EmailLogado));
            }
        }
    }
}
