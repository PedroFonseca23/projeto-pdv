using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PDVCompleto
{
    public class ComponentesVisuais
    {
        public static DataGridView CriarGrid()
        {
            DataGridView grid = new DataGridView();
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 55;
            grid.GridColor = Cores.Borda;
            grid.Dock = DockStyle.Fill;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = Cores.TextoSecundario, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            grid.DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = Cores.TextoPrincipal, Font = new Font("Segoe UI", 11), SelectionBackColor = Cores.Fundo, SelectionForeColor = Cores.TextoPrincipal, Padding = new Padding(6) };
            grid.RowTemplate = new DataGridViewRow { Height = 55 };
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 250, 252) };
            return grid;
        }

        public static Panel CriarCardDashboard(string titulo, string query, bool moeda, string icone, Color? corAccent)
        {
            Color accent = corAccent.HasValue ? corAccent.Value : Cores.Primario;
            PainelArredondado card = new PainelArredondado { Dock = DockStyle.Fill, BackColor = Cores.Superficie, Margin = new Padding(8), Cursor = Cursors.Hand, Raio = 14 };
            PainelArredondado selo = new PainelArredondado { Size = new Size(28, 28), Location = new Point(14, 12), BackColor = ClarearCor(accent), Raio = 14, MostrarBorda = false };
            
            selo.Controls.Add(new Label { Text = icone, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = accent });
            card.Controls.Add(selo);
            card.Controls.Add(new Label { Text = titulo.ToUpper(), Location = new Point(52, 18), AutoSize = true, Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = Cores.TextoSecundario });

            string valor = moeda ? "R$ 0,00" : "0";
            try
            {
                DataTable dt = BancoDeDados.Buscar(query);
                if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                {
                    decimal num = Convert.ToDecimal(dt.Rows[0][0]);
                    valor = num.ToString(moeda ? "C2" : "N0", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"));
                }
            } 
            catch { }

            card.Controls.Add(new Label { Text = valor, Location = new Point(14, 48), AutoSize = true, AutoEllipsis = true, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Cores.TextoPrincipal });
            return card;
        }

        public static void AddCampo(Control parent, string texto, int x, int y, int width, out TextBox txt, bool senha, bool readOnly)
        {
            parent.Controls.Add(CriarLabel(texto, x, y));
            Panel wrapper = CriarTextBoxWrapper(x, y + 25, width, out txt);
            if (senha) txt.PasswordChar = '*';
            if (readOnly) txt.ReadOnly = true;
            parent.Controls.Add(wrapper);
        }

        public static void AddCombo(Control parent, string texto, int x, int y, int width, out ComboBox cb)
        {
            parent.Controls.Add(CriarLabel(texto, x, y));
            cb = new ComboBox { Location = new Point(x, y + 25), Size = new Size(width, 40), Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = Cores.TextoPrincipal };
            parent.Controls.Add(cb);
        }

        public static Button AddBotao(Control parent, string texto, int x, int y, int width, Color fundo, Color textoCor, EventHandler onClick)
        {
            BotaoModerno btn = new BotaoModerno { Text = texto, Location = new Point(x, y), Size = new Size(width, 40), BackColor = fundo, ForeColor = textoCor, Font = new Font("Segoe UI", 9, FontStyle.Bold), Raio = 8 };
            if (onClick != null) btn.Click += onClick;
            parent.Controls.Add(btn);
            return btn;
        }

        public static Button CriarBotao(string texto, int x, int y, int width, Color corFundo, Color corTexto)
        {
            return new BotaoModerno { Text = texto, Location = new Point(x, y), Size = new Size(width, 40), BackColor = corFundo, ForeColor = corTexto, Font = new Font("Segoe UI", 9, FontStyle.Bold), Raio = 8 };
        }

        public static void AddGrid(Control parent, out DataGridView g, DataGridViewCellEventHandler onClick)
        {
            g = CriarGrid();
            if (onClick != null) g.CellClick += onClick;
            Panel container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 20, 0, 0) };
            Panel card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(2) };
            card.Controls.Add(g);
            container.Controls.Add(card);
            parent.Controls.Add(container);
        }

        public static Label CriarLabel(string txt, int x, int y)
        {
            return new Label { Text = txt, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Cores.TextoSecundario };
        }

        public static Panel CriarCardContainer(int h)
        {
            return new PainelArredondado { Height = h, Dock = DockStyle.Top, BackColor = Cores.Superficie, Padding = new Padding(20), Raio = 14, Margin = new Padding(0, 0, 0, 16) };
        }

        public static Panel CriarTextBoxWrapper(int x, int y, int w, out TextBox txt)
        {
            PainelArredondado pnl = new PainelArredondado { Location = new Point(x, y), Size = new Size(w, 40), BackColor = Cores.Superficie, Raio = 8, CorBorda = Cores.Borda };
            TextBox t = new TextBox { BorderStyle = BorderStyle.None, Location = new Point(12, 11), Size = new Size(w - 24, 20), Font = new Font("Segoe UI", 11), ForeColor = Cores.TextoPrincipal, BackColor = Cores.Superficie };
            t.Enter += delegate { pnl.CorBorda = Cores.Primario; pnl.Invalidate(); };
            t.Leave += delegate { pnl.CorBorda = Cores.Borda; pnl.Invalidate(); };
            pnl.Controls.Add(t);
            txt = t; 
            return pnl;
        }

        private static Color ClarearCor(Color c)
        {
            return Color.FromArgb(Math.Min(c.R + (int)((255 - c.R) * 0.85), 255), Math.Min(c.G + (int)((255 - c.G) * 0.85), 255), Math.Min(c.B + (int)((255 - c.B) * 0.85), 255));
        }
    }

    public class TelaDashboard : UserControl
    {
        private decimal valorPago = 0;
        private decimal valorPendente = 0;
        private DataTable topProdutos = new DataTable();
        private DataTable topClientes = new DataTable();
        private DataGridView gridUltimosPedidos;

        public TelaDashboard()
        {
            this.BackColor = Cores.Fundo; 
            this.Dock = DockStyle.Fill;
            Panel container = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            TableLayoutPanel flow = new TableLayoutPanel { Dock = DockStyle.Top, Height = 300, ColumnCount = 3, RowCount = 3, Margin = new Padding(0) };
            for (int c = 0; c < 3; c++) flow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 3));
            for (int r = 0; r < 3; r++) flow.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 3));

            Panel[] cards = new Panel[] {
                ComponentesVisuais.CriarCardDashboard("Faturamento Total", "SELECT SUM(valor_total) FROM pedidos", true, "R$", Cores.Sucesso),
                ComponentesVisuais.CriarCardDashboard("Caixa Recebido", "SELECT SUM(valor_pago) FROM pedidos", true, "✓", Color.FromArgb(59, 130, 246)),
                ComponentesVisuais.CriarCardDashboard("Contas a Receber", "SELECT SUM(valor_total - valor_pago) FROM pedidos WHERE valor_total > valor_pago", true, "!", Cores.Perigo),
                ComponentesVisuais.CriarCardDashboard("Ticket Médio", "SELECT IFNULL(SUM(valor_total)/COUNT(*), 0) FROM pedidos", true, "≈", Color.FromArgb(139, 92, 246)),
                ComponentesVisuais.CriarCardDashboard("Total de Vendas", "SELECT COUNT(*) FROM pedidos", false, "#", Cores.TextoPrincipal),
                ComponentesVisuais.CriarCardDashboard("Clientes Ativos", "SELECT COUNT(*) FROM clientes", false, "◍", Cores.Alerta),
                ComponentesVisuais.CriarCardDashboard("Itens no Catálogo", "SELECT COUNT(*) FROM produtos", false, "▤", Color.FromArgb(14, 165, 233)),
                ComponentesVisuais.CriarCardDashboard("NF-e Autorizadas", "SELECT COUNT(*) FROM pedidos WHERE status_fiscal IN ('AUTORIZADA_HOMOLOGACAO','AUTORIZADA')", false, "NF", Cores.Sucesso),
                ComponentesVisuais.CriarCardDashboard("NF-e Pendentes/Erro", "SELECT COUNT(*) FROM pedidos WHERE status_fiscal IN ('PENDENTE','PROCESSANDO_SEFAZ','ERRO_EMISSAO')", false, "!", Cores.Perigo)
            };
            for (int i = 0; i < cards.Length; i++) flow.Controls.Add(cards[i], i % 3, i / 3);

            Panel linha2 = new Panel { Dock = DockStyle.Top, Height = 260, Margin = new Padding(0, 10, 0, 0), Padding = new Padding(0, 10, 0, 0) };
            Panel chartPagamentos = new Panel { Width = 380, Dock = DockStyle.Left, BackColor = Color.White };
            chartPagamentos.Paint += new PaintEventHandler(ChartPagamentos_Paint);
            Panel chartProdutos = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            chartProdutos.Paint += new PaintEventHandler(ChartProdutos_Paint);
            chartProdutos.SizeChanged += delegate { chartProdutos.Invalidate(); };
            linha2.Controls.Add(chartProdutos); 
            linha2.Controls.Add(new Panel { Width = 20, Dock = DockStyle.Left, BackColor = Color.Transparent }); 
            linha2.Controls.Add(chartPagamentos);

            Panel linha3 = new Panel { Dock = DockStyle.Top, Height = 280, Margin = new Padding(0, 10, 0, 0), Padding = new Padding(0, 10, 0, 0) };
            Panel chartClientes = new Panel { Width = 420, Dock = DockStyle.Left, BackColor = Color.White };
            chartClientes.Paint += new PaintEventHandler(ChartClientes_Paint);
            
            Panel painelUltimos = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            Panel headerUltimos = new Panel { Height = 45, Dock = DockStyle.Top, Padding = new Padding(20, 18, 0, 0) };
            headerUltimos.Controls.Add(ComponentesVisuais.CriarLabel("ÚLTIMOS PEDIDOS", 20, 18));
            ComponentesVisuais.AddBotao(headerUltimos, "Desta Semana", 200, 10, 120, Color.FromArgb(59, 130, 246), Color.White, delegate { this.gridUltimosPedidos.DataSource = BancoDeDados.BuscarPedidosDaSemanaAtual(); });
            ComponentesVisuais.AddBotao(headerUltimos, "Todos", 330, 10, 80, Color.White, Cores.TextoSecundario, delegate { CarregarDadosGraficos(); });
            
            this.gridUltimosPedidos = ComponentesVisuais.CriarGrid();
            this.gridUltimosPedidos.RowTemplate.Height = 32; 
            this.gridUltimosPedidos.ColumnHeadersHeight = 32;
            painelUltimos.Controls.Add(this.gridUltimosPedidos); 
            painelUltimos.Controls.Add(headerUltimos);
            
            linha3.Controls.Add(painelUltimos); 
            linha3.Controls.Add(new Panel { Width = 20, Dock = DockStyle.Left, BackColor = Color.Transparent }); 
            linha3.Controls.Add(chartClientes);
            
            container.Controls.Add(linha3); 
            container.Controls.Add(linha2); 
            container.Controls.Add(flow);
            this.Controls.Add(container);
            CarregarDadosGraficos();
        }

        private void ChartProdutos_Paint(object sender, PaintEventArgs e)
        {
            DesenharBarrasHorizontais(e.Graphics, (Panel)sender, "TOP 5 PRODUTOS MAIS VENDIDOS", this.topProdutos, "nome", "qtd", false);
        }

        private void ChartClientes_Paint(object sender, PaintEventArgs e)
        {
            DesenharBarrasHorizontais(e.Graphics, (Panel)sender, "TOP 5 CLIENTES (VALOR TOTAL)", this.topClientes, "nome", "total", true);
        }

        private void CarregarDadosGraficos()
        {
            try 
            {
                DataTable dtFin = BancoDeDados.Buscar("SELECT SUM(valor_pago), SUM(valor_total - valor_pago) FROM pedidos");
                if (dtFin.Rows.Count > 0 && dtFin.Rows[0][0] != DBNull.Value) 
                {
                    this.valorPago = Convert.ToDecimal(dtFin.Rows[0][0]);
                    decimal pend = Convert.ToDecimal(dtFin.Rows[0][1]);
                    this.valorPendente = pend > 0 ? pend : 0;
                }
                this.topProdutos = BancoDeDados.Buscar("SELECT pr.nome, SUM(i.quantidade) as qtd FROM pedido_itens i JOIN produtos pr ON i.fk_produto_id = pr.id GROUP BY pr.id ORDER BY qtd DESC LIMIT 5");
                this.topClientes = BancoDeDados.Buscar("SELECT c.nome, SUM(p.valor_total) as total FROM pedidos p JOIN clientes c ON p.fk_cliente_id = c.id GROUP BY c.id ORDER BY total DESC LIMIT 5");
                this.gridUltimosPedidos.DataSource = BancoDeDados.Buscar("SELECT p.id AS 'ID', c.nome AS 'Cliente', p.data_entrega AS 'Entrega', printf('R$ %.2f', p.valor_total) AS 'Valor' FROM pedidos p LEFT JOIN clientes c ON p.fk_cliente_id = c.id ORDER BY p.id DESC LIMIT 8");
            } 
            catch { }
        }

        private void ChartPagamentos_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawString("STATUS DE CRÉDITO E ENTRADAS", new Font("Segoe UI", 11, FontStyle.Bold), new SolidBrush(Cores.TextoSecundario), 25, 25);
            decimal total = this.valorPago + this.valorPendente; 
            if (total == 0) return;

            float angPago = (float)((this.valorPago / total) * 360);
            float angPend = 360 - angPago;
            Rectangle rect = new Rectangle(40, 80, 160, 160);
            Brush bPago = new SolidBrush(Cores.Sucesso);
            Brush bPend = new SolidBrush(Cores.Perigo);
            Brush tDark = new SolidBrush(Cores.TextoPrincipal);
            Font fLeg = new Font("Segoe UI", 10, FontStyle.Bold);

            if (angPago > 0) e.Graphics.FillPie(bPago, rect, -90, angPago);
            if (angPend > 0) e.Graphics.FillPie(bPend, rect, -90 + angPago, angPend);
            e.Graphics.FillEllipse(new SolidBrush(Color.White), new Rectangle(rect.X + 45, rect.Y + 45, rect.Width - 90, rect.Height - 90));
            
            e.Graphics.FillRectangle(bPago, 230, 120, 16, 16); 
            e.Graphics.DrawString(string.Format("Recebido ({0:P1})", this.valorPago / total), fLeg, tDark, 255, 118);
            
            e.Graphics.FillRectangle(bPend, 230, 160, 16, 16); 
            e.Graphics.DrawString(string.Format("A Receber ({0:P1})", this.valorPendente / total), fLeg, tDark, 255, 158);
        }

        private void DesenharBarrasHorizontais(Graphics g, Panel p, string titulo, DataTable dados, string colNome, string colValor, bool moeda)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawString(titulo, new Font("Segoe UI", 11, FontStyle.Bold), new SolidBrush(Cores.TextoSecundario), 25, 25);
            if (dados == null || dados.Rows.Count == 0) 
            { 
                g.DrawString("Sem dados suficientes.", new Font("Segoe UI", 9), new SolidBrush(Cores.Borda), 25, 70); 
                return; 
            }

            decimal maxV = 0;
            foreach(DataRow r in dados.Rows) 
            {
                decimal val = Convert.ToDecimal(r[colValor]);
                if (val > maxV) maxV = val;
            }
            if (maxV == 0) maxV = 1;

            int y = 80;
            int maxW = Math.Max(p.Width - 270, 100);
            Brush tColor = new SolidBrush(Cores.Fundo);
            Brush bColor = new SolidBrush(Color.FromArgb(59, 130, 246));
            Brush txtColor = new SolidBrush(Cores.TextoPrincipal);
            Font f = new Font("Segoe UI", 10, FontStyle.Bold);

            foreach (DataRow row in dados.Rows)
            {
                string nome = row[colNome].ToString(); 
                if (nome.Length > 20) nome = nome.Substring(0, 17) + "...";
                decimal valor = Convert.ToDecimal(row[colValor]);
                int w = (int)((valor * maxW) / maxV);

                g.DrawString(nome, f, txtColor, 25, y + 2);
                g.FillRectangle(tColor, 180, y, maxW, 24); 
                g.FillRectangle(bColor, 180, y, w, 24);
                
                string textoValor = moeda ? valor.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) : valor.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) + " un";
                g.DrawString(textoValor, f, txtColor, 180 + w + 10, y + 2);
                y += 45;
            }
        }
    }

    public class TelaProdutos : UserControl
    {
        private DataGridView grid;
        private TextBox txtNome;
        private TextBox txtDesc;
        private TextBox txtPreco;
        private TextBox txtNcm;
        private TextBox txtCfop;
        private TextBox txtUnidade;
        private string idSelecionado = "";

        public TelaProdutos()
        {
            this.BackColor = Cores.Fundo;
            Panel topo = ComponentesVisuais.CriarCardContainer(160);
            
            ComponentesVisuais.AddCampo(topo, "Nome do Produto", 20, 20, 250, out this.txtNome, false, false);
            ComponentesVisuais.AddCampo(topo, "Descrição do Item", 290, 20, 250, out this.txtDesc, false, false);
            ComponentesVisuais.AddCampo(topo, "Preço (R$)", 560, 20, 100, out this.txtPreco, false, false);
            ComponentesVisuais.AddCampo(topo, "NCM", 680, 20, 100, out this.txtNcm, false, false);
            ComponentesVisuais.AddCampo(topo, "CFOP", 800, 20, 70, out this.txtCfop, false, false); 
            this.txtCfop.Text = "5102";
            ComponentesVisuais.AddCampo(topo, "UN", 890, 20, 50, out this.txtUnidade, false, false); 
            this.txtUnidade.Text = "UN";

            ComponentesVisuais.AddBotao(topo, "Salvar Produto", 20, 100, 150, Cores.TextoPrincipal, Color.White, new EventHandler(Salvar));
            ComponentesVisuais.AddBotao(topo, "Atualizar", 180, 100, 120, Cores.Fundo, Cores.TextoPrincipal, new EventHandler(Editar));
            ComponentesVisuais.AddBotao(topo, "Excluir", 310, 100, 120, Cores.PerigoClaro, Cores.Perigo, new EventHandler(Deletar));
            ComponentesVisuais.AddBotao(topo, "Limpar", 440, 100, 120, Color.White, Cores.TextoSecundario, delegate { LimparCampos(); });

            ComponentesVisuais.AddGrid(this, out this.grid, new DataGridViewCellEventHandler(Grid_CellClick));
            this.Controls.Add(topo); 
            Carregar();
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow r = this.grid.Rows[e.RowIndex];
            this.idSelecionado = r.Cells["ID"].Value.ToString();
            this.txtNome.Text = r.Cells["Nome"].Value.ToString(); 
            this.txtDesc.Text = r.Cells["Descricao"].Value.ToString();
            this.txtPreco.Text = r.Cells["Preço"].Value.ToString().Replace("R$ ", ""); 
            this.txtNcm.Text = r.Cells["NCM"].Value.ToString();
            this.txtCfop.Text = r.Cells["CFOP"].Value.ToString(); 
            this.txtUnidade.Text = r.Cells["UN"].Value.ToString();
        }

        private void LimparCampos() 
        { 
            this.idSelecionado = ""; 
            this.txtNome.Clear(); 
            this.txtDesc.Clear(); 
            this.txtPreco.Clear(); 
            this.txtNcm.Clear(); 
            this.txtCfop.Text = "5102"; 
            this.txtUnidade.Text = "UN"; 
        }

        private void Carregar() 
        { 
            try 
            { 
                this.grid.DataSource = BancoDeDados.Buscar("SELECT id AS 'ID', nome AS 'Nome', descricao AS 'Descricao', ncm AS 'NCM', cfop AS 'CFOP', unidade AS 'UN', printf('R$ %.2f', preco) AS 'Preço' FROM produtos"); 
            } 
            catch { } 
        }

        private void Salvar(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtNome.Text)) return;
            decimal preco = 0; 
            decimal.TryParse(this.txtPreco.Text, out preco);
            try 
            {
                BancoDeDados.Executar("INSERT INTO produtos (nome, descricao, preco, ncm, cfop, unidade) VALUES (@n, @d, @p, @ncm, @cfop, @un)", 
                    new SQLiteParameter[] { new SQLiteParameter("@n", this.txtNome.Text), new SQLiteParameter("@d", this.txtDesc.Text), new SQLiteParameter("@p", preco), new SQLiteParameter("@ncm", this.txtNcm.Text), new SQLiteParameter("@cfop", this.txtCfop.Text), new SQLiteParameter("@un", this.txtUnidade.Text) });
                LimparCampos(); 
                Carregar();
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show("Erro: " + ex.Message); 
            }
        }

        private void Editar(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.idSelecionado) || string.IsNullOrWhiteSpace(this.txtNome.Text)) return;
            decimal preco = 0; 
            decimal.TryParse(this.txtPreco.Text, out preco);
            try 
            {
                BancoDeDados.Executar("UPDATE produtos SET nome=@n, descricao=@d, preco=@p, ncm=@ncm, cfop=@cfop, unidade=@un WHERE id=@id",
                    new SQLiteParameter[] { new SQLiteParameter("@n", this.txtNome.Text), new SQLiteParameter("@d", this.txtDesc.Text), new SQLiteParameter("@p", preco), new SQLiteParameter("@ncm", this.txtNcm.Text), new SQLiteParameter("@cfop", this.txtCfop.Text), new SQLiteParameter("@un", this.txtUnidade.Text), new SQLiteParameter("@id", this.idSelecionado) });
                LimparCampos(); 
                Carregar();
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show("Erro: " + ex.Message); 
            }
        }

        private void Deletar(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.idSelecionado)) return;
            try 
            { 
                BancoDeDados.Executar("DELETE FROM produtos WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", this.idSelecionado) }); 
                LimparCampos(); 
                Carregar(); 
            }
            catch 
            { 
                MessageBox.Show("Erro ao apagar. Produto vinculado a pedido."); 
            }
        }
    }

    public class TelaClientes : UserControl
    {
        private DataGridView grid;
        private TextBox txtPesquisa;
        private TextBox txtNome;
        private TextBox txtTel;
        private TextBox txtCpfCnpj;
        private TextBox txtIE;
        private TextBox txtEndereco;
        private TextBox txtNumero;
        private TextBox txtBairro;
        private TextBox txtMunicipio;
        private TextBox txtUf;
        private TextBox txtCep;
        private RadioButton rbPf;
        private RadioButton rbPj;
        private Label lblCpfCnpj;
        private Label lblIE;
        private string idSelecionado = "";

        public TelaClientes()
        {
            this.BackColor = Cores.Fundo;
            Panel topo = ComponentesVisuais.CriarCardContainer(320);

            this.rbPf = new RadioButton { Text = "Pessoa Física", Location = new Point(20, 15), AutoSize = true, Checked = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.rbPj = new RadioButton { Text = "Pessoa Jurídica", Location = new Point(160, 15), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.rbPf.CheckedChanged += new EventHandler(TipoPessoa_Changed); 
            this.rbPj.CheckedChanged += new EventHandler(TipoPessoa_Changed);
            topo.Controls.Add(this.rbPf); 
            topo.Controls.Add(this.rbPj);

            ComponentesVisuais.AddCampo(topo, "Pesquisar Cliente", 340, 10, 300, out this.txtPesquisa, false, false);
            this.txtPesquisa.TextChanged += delegate 
            { 
                if (string.IsNullOrWhiteSpace(this.txtPesquisa.Text)) Carregar(); 
                else this.grid.DataSource = BancoDeDados.PesquisarClientes(this.txtPesquisa.Text.Trim()); 
            };

            ComponentesVisuais.AddCampo(topo, "Nome Completo / Razão Social", 20, 80, 300, out this.txtNome, false, false);
            ComponentesVisuais.AddCampo(topo, "CPF", 340, 80, 200, out this.txtCpfCnpj, false, false); 
            this.lblCpfCnpj = (Label)topo.Controls[topo.Controls.Count - 2];
            
            ComponentesVisuais.AddCampo(topo, "Inscrição Estadual (IE)", 560, 80, 200, out this.txtIE, false, false); 
            this.lblIE = (Label)topo.Controls[topo.Controls.Count - 2];
            
            ComponentesVisuais.AddCampo(topo, "Telefone de Contato", 780, 80, 200, out this.txtTel, false, false);

            ComponentesVisuais.AddCampo(topo, "Endereço (rua/avenida)", 20, 160, 280, out this.txtEndereco, false, false);
            ComponentesVisuais.AddCampo(topo, "Número", 320, 160, 90, out this.txtNumero, false, false);
            ComponentesVisuais.AddCampo(topo, "Bairro", 430, 160, 220, out this.txtBairro, false, false);
            ComponentesVisuais.AddCampo(topo, "Município", 670, 160, 200, out this.txtMunicipio, false, false);
            ComponentesVisuais.AddCampo(topo, "UF", 890, 160, 60, out this.txtUf, false, false);
            ComponentesVisuais.AddCampo(topo, "CEP", 20, 240, 150, out this.txtCep, false, false);

            this.lblIE.Visible = false; 
            this.txtIE.Parent.Visible = false;

            ComponentesVisuais.AddBotao(topo, "Salvar Cliente", 190, 265, 150, Cores.TextoPrincipal, Color.White, new EventHandler(Salvar));
            ComponentesVisuais.AddBotao(topo, "Atualizar", 350, 265, 120, Cores.Fundo, Cores.TextoPrincipal, new EventHandler(Editar));
            ComponentesVisuais.AddBotao(topo, "Excluir", 480, 265, 120, Cores.PerigoClaro, Cores.Perigo, new EventHandler(Deletar));
            ComponentesVisuais.AddBotao(topo, "Limpar", 610, 265, 120, Color.White, Cores.TextoSecundario, delegate { LimparCampos(); });

            ComponentesVisuais.AddGrid(this, out this.grid, new DataGridViewCellEventHandler(Grid_CellClick));
            this.Controls.Add(topo); 
            Carregar();
        }

        private void TipoPessoa_Changed(object sender, EventArgs e)
        {
            this.lblCpfCnpj.Text = this.rbPf.Checked ? "CPF" : "CNPJ";
            this.lblIE.Visible = this.txtIE.Parent.Visible = this.rbPj.Checked;
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            this.idSelecionado = this.grid.Rows[e.RowIndex].Cells["ID"].Value.ToString();
            DataTable dt = BancoDeDados.Buscar("SELECT nome, telefone, cpf_cnpj, tipo_pessoa, inscricao_estadual, endereco, endereco_numero, endereco_bairro, endereco_municipio, endereco_uf, endereco_cep FROM clientes WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", this.idSelecionado) });
            if (dt.Rows.Count == 0) return;
            
            this.txtNome.Text = dt.Rows[0]["nome"].ToString(); 
            this.txtTel.Text = dt.Rows[0]["telefone"].ToString(); 
            this.txtCpfCnpj.Text = dt.Rows[0]["cpf_cnpj"].ToString();
            this.txtIE.Text = dt.Rows[0]["inscricao_estadual"].ToString(); 
            this.txtEndereco.Text = dt.Rows[0]["endereco"].ToString(); 
            this.txtNumero.Text = dt.Rows[0]["endereco_numero"].ToString();
            this.txtBairro.Text = dt.Rows[0]["endereco_bairro"].ToString(); 
            this.txtMunicipio.Text = dt.Rows[0]["endereco_municipio"].ToString(); 
            this.txtUf.Text = dt.Rows[0]["endereco_uf"].ToString(); 
            this.txtCep.Text = dt.Rows[0]["endereco_cep"].ToString();
            
            if (dt.Rows[0]["tipo_pessoa"].ToString() == "PJ") 
                this.rbPj.Checked = true; 
            else 
                this.rbPf.Checked = true;
        }

        private void LimparCampos() 
        { 
            this.idSelecionado = ""; 
            this.txtNome.Clear(); 
            this.txtTel.Clear(); 
            this.txtCpfCnpj.Clear(); 
            this.txtIE.Clear(); 
            this.txtEndereco.Clear(); 
            this.txtNumero.Clear(); 
            this.txtBairro.Clear(); 
            this.txtMunicipio.Clear(); 
            this.txtUf.Clear(); 
            this.txtCep.Clear(); 
            this.rbPf.Checked = true; 
        }

        private void Carregar() 
        { 
            try 
            { 
                this.grid.DataSource = BancoDeDados.Buscar("SELECT id AS 'ID', nome AS 'Nome', cpf_cnpj AS 'CPF/CNPJ', telefone AS 'Telefone', endereco_municipio AS 'Cidade', endereco_uf AS 'UF' FROM clientes"); 
            } 
            catch { } 
        }

        private string ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(this.txtNome.Text)) return "Informe o nome ou razão social do cliente.";
            if (!string.IsNullOrWhiteSpace(this.txtCpfCnpj.Text) && !ValidacoesFiscais.DocumentoValido(this.txtCpfCnpj.Text)) return this.rbPf.Checked ? "CPF inválido." : "CNPJ inválido.";
            if (!string.IsNullOrWhiteSpace(this.txtCep.Text) && ValidacoesFiscais.SomenteDigitos(this.txtCep.Text).Length != 8) return "CEP inválido.";
            if (!string.IsNullOrWhiteSpace(this.txtUf.Text) && this.txtUf.Text.Trim().Length != 2) return "UF inválida.";
            return null;
        }

        private SQLiteParameter[] Params(string id)
        {
            List<SQLiteParameter> l = new List<SQLiteParameter>();
            l.Add(new SQLiteParameter("@n", this.txtNome.Text));
            l.Add(new SQLiteParameter("@t", this.txtTel.Text));
            l.Add(new SQLiteParameter("@c", ValidacoesFiscais.SomenteDigitos(this.txtCpfCnpj.Text)));
            l.Add(new SQLiteParameter("@tp", this.rbPj.Checked ? "PJ" : "PF"));
            l.Add(new SQLiteParameter("@ie", this.txtIE.Text));
            l.Add(new SQLiteParameter("@end", this.txtEndereco.Text));
            l.Add(new SQLiteParameter("@num", this.txtNumero.Text));
            l.Add(new SQLiteParameter("@bai", this.txtBairro.Text));
            l.Add(new SQLiteParameter("@mun", this.txtMunicipio.Text));
            l.Add(new SQLiteParameter("@uf", this.txtUf.Text.Trim().ToUpper()));
            l.Add(new SQLiteParameter("@cep", ValidacoesFiscais.SomenteDigitos(this.txtCep.Text)));
            
            if (id != null) 
            {
                l.Add(new SQLiteParameter("@id", id));
            }
            return l.ToArray();
        }

        private void Salvar(object sender, EventArgs e) 
        { 
            string err = ValidarCampos(); 
            if (err != null) 
            { 
                MessageBox.Show(err, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return; 
            } 
            try 
            { 
                BancoDeDados.Executar("INSERT INTO clientes (nome, telefone, cpf_cnpj, tipo_pessoa, inscricao_estadual, endereco, endereco_numero, endereco_bairro, endereco_municipio, endereco_uf, endereco_cep) VALUES (@n, @t, @c, @tp, @ie, @end, @num, @bai, @mun, @uf, @cep)", Params(null)); 
                LimparCampos(); 
                Carregar(); 
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show("Erro: " + ex.Message); 
            } 
        }

        private void Editar(object sender, EventArgs e) 
        { 
            if (string.IsNullOrWhiteSpace(this.idSelecionado)) return; 
            string err = ValidarCampos(); 
            if (err != null) 
            { 
                MessageBox.Show(err, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return; 
            } 
            try 
            { 
                BancoDeDados.Executar("UPDATE clientes SET nome=@n, telefone=@t, cpf_cnpj=@c, tipo_pessoa=@tp, inscricao_estadual=@ie, endereco=@end, endereco_numero=@num, endereco_bairro=@bai, endereco_municipio=@mun, endereco_uf=@uf, endereco_cep=@cep WHERE id=@id", Params(this.idSelecionado)); 
                LimparCampos(); 
                Carregar(); 
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show("Erro: " + ex.Message); 
            } 
        }

        private void Deletar(object sender, EventArgs e) 
        { 
            if (string.IsNullOrWhiteSpace(this.idSelecionado)) return;
            if (MessageBox.Show("Excluir cliente?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return; 
            try 
            { 
                BancoDeDados.Executar("DELETE FROM clientes WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", this.idSelecionado) }); 
                LimparCampos(); 
                Carregar(); 
            } 
            catch 
            { 
                MessageBox.Show("Erro ao apagar. Cliente vinculado a pedido."); 
            } 
        }
    }

    public class TelaPedidos : UserControl
    {
        private DataGridView grid;
        private ComboBox cbClientes;
        private ComboBox cbProdutos;
        private TextBox txtQtd;
        private TextBox txtData;
        private TextBox txtTotal;
        private TextBox txtEvento;
        private TextBox txtEntrada;
        private TextBox txtPesquisa;
        private TextBox txtAbater;
        private Button btnSalvar;
        private DateTimePicker dtpInicio;
        private DateTimePicker dtpFim;
        private Label lblPendente;
        private string idSelecionado = "";
        private string idPedidoEmEdicao = "";
        private ListBox listCarrinho;
        private Dictionary<int, int> itensCarrinho = new Dictionary<int, int>();
        private Dictionary<int, decimal> precosProdutos = new Dictionary<int, decimal>();
        private Dictionary<int, string> nomesProdutos = new Dictionary<int, string>();
        private List<int> ordemCarrinho = new List<int>();

        public TelaPedidos()
        {
            this.BackColor = Cores.Fundo;
            Panel pnlNovoPedido = ComponentesVisuais.CriarCardContainer(240);

            ComponentesVisuais.AddCombo(pnlNovoPedido, "Cliente", 20, 10, 280, out this.cbClientes);
            ComponentesVisuais.AddCampo(pnlNovoPedido, "Observações do Pedido", 320, 10, 280, out this.txtEvento, false, false);
            ComponentesVisuais.AddCampo(pnlNovoPedido, "Data de Entrega", 620, 10, 130, out this.txtData, false, false); 
            this.txtData.Text = DateTime.Now.ToString("dd/MM/yyyy");

            ComponentesVisuais.AddCombo(pnlNovoPedido, "Adicionar Produto", 20, 80, 280, out this.cbProdutos);
            ComponentesVisuais.AddCampo(pnlNovoPedido, "Qtd", 320, 80, 60, out this.txtQtd, false, false);
            ComponentesVisuais.AddBotao(pnlNovoPedido, "+ Adicionar", 390, 105, 110, Cores.Fundo, Cores.TextoPrincipal, new EventHandler(AdicionarAoCarrinho));

            pnlNovoPedido.Controls.Add(ComponentesVisuais.CriarLabel("Itens do Pedido", 770, 10));
            this.listCarrinho = new ListBox { Location = new Point(770, 32), Size = new Size(240, 110), Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(248, 250, 252) };
            pnlNovoPedido.Controls.Add(this.listCarrinho);

            ComponentesVisuais.AddBotao(pnlNovoPedido, "Remover Selecionado", 770, 148, 240, Color.White, Cores.Perigo, new EventHandler(RemoverItemCarrinho));
            ComponentesVisuais.AddBotao(pnlNovoPedido, "Esvaziar Carrinho", 770, 192, 240, Cores.PerigoClaro, Cores.Perigo, delegate { this.itensCarrinho.Clear(); AtualizarListaCarrinho(); });

            ComponentesVisuais.AddCampo(pnlNovoPedido, "Total (R$)", 20, 150, 130, out this.txtTotal, false, true); 
            this.txtTotal.Text = "0,00";
            
            ComponentesVisuais.AddCampo(pnlNovoPedido, "Entrada (R$)", 160, 150, 130, out this.txtEntrada, false, false); 
            this.txtEntrada.Text = "0,00";

            this.btnSalvar = ComponentesVisuais.AddBotao(pnlNovoPedido, "Salvar Novo Pedido", 320, 175, 160, Cores.TextoPrincipal, Color.White, new EventHandler(Salvar));
            ComponentesVisuais.AddBotao(pnlNovoPedido, "Cancelar", 490, 175, 100, Color.White, Cores.TextoSecundario, delegate { LimparCampos(); });

            Panel pnlAcoes = new Panel { Dock = DockStyle.Top, Height = 160, BackColor = Color.FromArgb(248, 250, 252), Padding = new Padding(20) };
            ComponentesVisuais.AddCampo(pnlAcoes, "PESQUISAR (CLIENTE / OBS)", 20, 10, 280, out this.txtPesquisa, false, false); 
            this.txtPesquisa.TextChanged += delegate { CarregarHistorico(this.txtPesquisa.Text); };

            pnlAcoes.Controls.Add(ComponentesVisuais.CriarLabel("De:", 320, 10)); 
            this.dtpInicio = new DateTimePicker { Location = new Point(320, 35), Format = DateTimePickerFormat.Short, Width = 110 }; 
            pnlAcoes.Controls.Add(this.dtpInicio);
            
            pnlAcoes.Controls.Add(ComponentesVisuais.CriarLabel("Até:", 440, 10)); 
            this.dtpFim = new DateTimePicker { Location = new Point(440, 35), Format = DateTimePickerFormat.Short, Width = 110 }; 
            pnlAcoes.Controls.Add(this.dtpFim);

            ComponentesVisuais.AddBotao(pnlAcoes, "Filtrar Data", 560, 35, 100, Color.FromArgb(59, 130, 246), Color.White, delegate { this.grid.DataSource = BancoDeDados.PesquisarPedidosPorData(this.dtpInicio.Value, this.dtpFim.Value); });
            ComponentesVisuais.AddBotao(pnlAcoes, "Semana Atual", 670, 35, 110, Cores.Sucesso, Color.White, delegate { this.grid.DataSource = BancoDeDados.BuscarPedidosDaSemanaCompleto(); });
            ComponentesVisuais.AddBotao(pnlAcoes, "Limpar", 790, 35, 80, Color.White, Cores.TextoSecundario, delegate { this.txtPesquisa.Clear(); CarregarHistorico(); });

            this.lblPendente = ComponentesVisuais.CriarLabel("Selecione um pedido para gerenciar", 20, 110); 
            this.lblPendente.ForeColor = Cores.Perigo; 
            pnlAcoes.Controls.Add(this.lblPendente);
            
            ComponentesVisuais.AddCampo(pnlAcoes, "Valor Abater", 380, 85, 100, out this.txtAbater, false, false);
            
            ComponentesVisuais.AddBotao(pnlAcoes, "Pagar", 490, 110, 90, Cores.Sucesso, Color.White, new EventHandler(AbaterValor));
            ComponentesVisuais.AddBotao(pnlAcoes, "Editar Pedido", 590, 110, 120, Color.FromArgb(59, 130, 246), Color.White, new EventHandler(CarregarParaEdicao));
            ComponentesVisuais.AddBotao(pnlAcoes, "Recibo / Imprimir", 720, 110, 120, Color.FromArgb(71, 85, 105), Color.White, new EventHandler(ExportarRecibo));
            ComponentesVisuais.AddBotao(pnlAcoes, "Excluir", 850, 110, 90, Cores.PerigoClaro, Cores.Perigo, new EventHandler(DeletarPedido));

            ComponentesVisuais.AddGrid(this, out this.grid, new DataGridViewCellEventHandler(Grid_CellClick));

            this.Controls.Add(pnlAcoes); 
            this.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10, BackColor = Color.Transparent }); 
            this.Controls.Add(pnlNovoPedido);
            
            CarregarDropdowns(); 
            CarregarHistorico();
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e) 
        { 
            if (e.RowIndex >= 0) 
            { 
                DataGridViewRow r = this.grid.Rows[e.RowIndex]; 
                this.idSelecionado = r.Cells["ID"].Value.ToString(); 
                this.lblPendente.Text = "Falta Pagar: " + r.Cells["Pendente"].Value.ToString(); 
            } 
        }

        private void CarregarParaEdicao(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.idSelecionado)) return;
            DataTable dtPedido = BancoDeDados.Buscar("SELECT fk_cliente_id, evento, data_entrega, valor_pago FROM pedidos WHERE id = @id", new SQLiteParameter[] { new SQLiteParameter("@id", this.idSelecionado) });
            if (dtPedido.Rows.Count > 0)
            {
                this.cbClientes.SelectedValue = dtPedido.Rows[0]["fk_cliente_id"]; 
                this.txtEvento.Text = dtPedido.Rows[0]["evento"].ToString(); 
                this.txtData.Text = dtPedido.Rows[0]["data_entrega"].ToString(); 
                this.txtEntrada.Text = dtPedido.Rows[0]["valor_pago"].ToString();
                
                this.itensCarrinho.Clear();
                DataTable dtItens = BancoDeDados.Buscar("SELECT fk_produto_id, quantidade FROM pedido_itens WHERE fk_pedido_id = @id", new SQLiteParameter[] { new SQLiteParameter("@id", this.idSelecionado) });
                foreach (DataRow row in dtItens.Rows) 
                { 
                    int idProd = Convert.ToInt32(row["fk_produto_id"]);
                    int qtd = Convert.ToInt32(row["quantidade"]); 
                    
                    if (this.itensCarrinho.ContainsKey(idProd)) 
                        this.itensCarrinho[idProd] += qtd; 
                    else 
                        this.itensCarrinho.Add(idProd, qtd); 
                }
                AtualizarListaCarrinho(); 
                this.idPedidoEmEdicao = this.idSelecionado; 
                this.btnSalvar.Text = "Salvar Alterações"; 
                this.btnSalvar.BackColor = Color.FromArgb(59, 130, 246);
            }
        }

        private void LimparCampos() 
        { 
            this.idSelecionado = "";
            this.idPedidoEmEdicao = ""; 
            this.itensCarrinho.Clear(); 
            this.ordemCarrinho.Clear(); 
            this.listCarrinho.Items.Clear(); 
            this.txtEvento.Clear(); 
            this.txtTotal.Text = "0,00";
            this.txtEntrada.Text = "0,00"; 
            this.txtAbater.Clear(); 
            this.txtPesquisa.Clear(); 
            this.lblPendente.Text = "Selecione um pedido"; 
            this.txtData.Text = DateTime.Now.ToString("dd/MM/yyyy"); 
            this.btnSalvar.Text = "Salvar Novo Pedido"; 
            this.btnSalvar.BackColor = Cores.TextoPrincipal; 
        }

        private void CarregarDropdowns() 
        {
            try 
            {
                this.cbClientes.DataSource = BancoDeDados.Buscar("SELECT id, nome FROM clientes"); 
                this.cbClientes.DisplayMember = "nome"; 
                this.cbClientes.ValueMember = "id";
                
                DataTable dtP = BancoDeDados.Buscar("SELECT id, nome, preco FROM produtos"); 
                this.cbProdutos.DataSource = dtP; 
                this.cbProdutos.DisplayMember = "nome"; 
                this.cbProdutos.ValueMember = "id";
                
                this.precosProdutos.Clear(); 
                this.nomesProdutos.Clear();
                
                foreach (DataRow r in dtP.Rows) 
                { 
                    int id = Convert.ToInt32(r["id"]); 
                    this.precosProdutos[id] = r["preco"] != DBNull.Value ? Convert.ToDecimal(r["preco"]) : 0; 
                    this.nomesProdutos[id] = r["nome"].ToString(); 
                }
            } 
            catch { }
        }

        private void CarregarHistorico(string filtro = "") 
        {
            string sql = "SELECT p.id AS 'ID', c.nome AS 'Cliente', p.evento AS 'Obs', p.data_entrega AS 'Entrega', GROUP_CONCAT(i.quantidade || 'x ' || pr.nome, ', ') AS 'Itens', printf('R$ %.2f', p.valor_total) AS 'Total', printf('R$ %.2f', p.valor_pago) AS 'Pago', printf('R$ %.2f', (p.valor_total - p.valor_pago)) AS 'Pendente' FROM pedidos p LEFT JOIN clientes c ON p.fk_cliente_id = c.id LEFT JOIN pedido_itens i ON p.id = i.fk_pedido_id LEFT JOIN produtos pr ON i.fk_produto_id = pr.id";
            if (!string.IsNullOrWhiteSpace(filtro)) 
            {
                sql += " WHERE c.nome LIKE @f OR p.evento LIKE @f";
            }
            sql += " GROUP BY p.id ORDER BY p.id DESC";
            
            try 
            { 
                if (string.IsNullOrWhiteSpace(filtro))
                {
                    this.grid.DataSource = BancoDeDados.Buscar(sql); 
                }
                else
                {
                    this.grid.DataSource = BancoDeDados.Buscar(sql, new SQLiteParameter[] { new SQLiteParameter("@f", "%" + filtro + "%") }); 
                }
            } 
            catch { }
        }

        private void AdicionarAoCarrinho(object sender, EventArgs e) 
        {
            int qtd = 0;
            if (this.cbProdutos.SelectedValue == null || !int.TryParse(this.txtQtd.Text, out qtd) || qtd <= 0) return;
            
            int idProd = Convert.ToInt32(this.cbProdutos.SelectedValue);
            
            if (!this.precosProdutos.ContainsKey(idProd)) 
            { 
                DataTable dt = BancoDeDados.Buscar("SELECT preco FROM produtos WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", idProd) }); 
                this.precosProdutos[idProd] = dt.Rows.Count > 0 && dt.Rows[0]["preco"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["preco"]) : 0; 
                this.nomesProdutos[idProd] = this.cbProdutos.Text; 
            }
            
            if (this.itensCarrinho.ContainsKey(idProd)) 
                this.itensCarrinho[idProd] += qtd; 
            else 
                this.itensCarrinho.Add(idProd, qtd);
                
            this.txtQtd.Clear(); 
            AtualizarListaCarrinho();
        }

        private void RemoverItemCarrinho(object sender, EventArgs e) 
        { 
            int idx = this.listCarrinho.SelectedIndex; 
            if (idx < 0 || idx >= this.ordemCarrinho.Count) return; 
            
            this.itensCarrinho.Remove(this.ordemCarrinho[idx]); 
            AtualizarListaCarrinho(); 
        }

        private void AtualizarListaCarrinho() 
        {
            this.listCarrinho.Items.Clear(); 
            this.ordemCarrinho.Clear(); 
            decimal total = 0;
            
            foreach (KeyValuePair<int, int> item in this.itensCarrinho) 
            { 
                int id = item.Key;
                int qtd = item.Value; 
                decimal sub = (this.precosProdutos.ContainsKey(id) ? this.precosProdutos[id] : 0) * qtd; 
                total += sub; 
                
                string nomeProduto = this.nomesProdutos.ContainsKey(id) ? this.nomesProdutos[id] : "Prod #" + id;
                this.listCarrinho.Items.Add(qtd.ToString() + "x " + nomeProduto + "  —  " + sub.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))); 
                this.ordemCarrinho.Add(id); 
            }
            this.txtTotal.Text = total.ToString("F2");
        }

        private void Salvar(object sender, EventArgs e) 
        {
            if (this.cbClientes.SelectedValue == null || this.itensCarrinho.Count == 0) return;
            
            decimal total = 0;
            decimal entrada = 0;
            decimal.TryParse(this.txtTotal.Text, out total); 
            decimal.TryParse(this.txtEntrada.Text, out entrada);
            
            using (SQLiteConnection conn = BancoDeDados.Conectar()) 
            {
                using (SQLiteTransaction tr = conn.BeginTransaction()) 
                {
                    try 
                    {
                        long pedId = 0;
                        if (string.IsNullOrEmpty(this.idPedidoEmEdicao)) 
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO pedidos (fk_cliente_id, evento, data_entrega, data_criacao, valor_total, valor_pago) VALUES (@c, @e, @dt, datetime('now', 'localtime'), @t, @p); SELECT last_insert_rowid();", conn)) 
                            {
                                cmd.Transaction = tr; 
                                cmd.Parameters.AddWithValue("@c", this.cbClientes.SelectedValue); 
                                cmd.Parameters.AddWithValue("@e", this.txtEvento.Text); 
                                cmd.Parameters.AddWithValue("@dt", this.txtData.Text); 
                                cmd.Parameters.AddWithValue("@t", total); 
                                cmd.Parameters.AddWithValue("@p", entrada); 
                                pedId = (long)cmd.ExecuteScalar(); 
                            }
                        } 
                        else 
                        {
                            pedId = Convert.ToInt64(this.idPedidoEmEdicao);
                            using (SQLiteCommand cmd = new SQLiteCommand("UPDATE pedidos SET fk_cliente_id=@c, evento=@e, data_entrega=@dt, valor_total=@t, valor_pago=@p WHERE id=@id", conn)) 
                            { 
                                cmd.Transaction = tr; 
                                cmd.Parameters.AddWithValue("@c", this.cbClientes.SelectedValue); 
                                cmd.Parameters.AddWithValue("@e", this.txtEvento.Text); 
                                cmd.Parameters.AddWithValue("@dt", this.txtData.Text); 
                                cmd.Parameters.AddWithValue("@t", total); 
                                cmd.Parameters.AddWithValue("@p", entrada); 
                                cmd.Parameters.AddWithValue("@id", pedId); 
                                cmd.ExecuteNonQuery(); 
                            }
                            using (SQLiteCommand cmdDel = new SQLiteCommand("DELETE FROM pedido_itens WHERE fk_pedido_id=@id", conn)) 
                            { 
                                cmdDel.Transaction = tr; 
                                cmdDel.Parameters.AddWithValue("@id", pedId); 
                                cmdDel.ExecuteNonQuery(); 
                            }
                        }
                        
                        foreach (KeyValuePair<int, int> it in this.itensCarrinho) 
                        { 
                            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO pedido_itens (fk_pedido_id, fk_produto_id, quantidade) VALUES (@ped, @prod, @qtd)", conn)) 
                            { 
                                cmd.Transaction = tr; 
                                cmd.Parameters.AddWithValue("@ped", pedId); 
                                cmd.Parameters.AddWithValue("@prod", it.Key); 
                                cmd.Parameters.AddWithValue("@qtd", it.Value); 
                                cmd.ExecuteNonQuery(); 
                            } 
                        }
                        tr.Commit();
                    } 
                    catch (Exception ex) 
                    { 
                        tr.Rollback(); 
                        MessageBox.Show("Erro: " + ex.Message); 
                        return; 
                    }
                } 
            }
            LimparCampos(); 
            CarregarHistorico();
        }

        private void AbaterValor(object sender, EventArgs e) 
        { 
            if (string.IsNullOrWhiteSpace(this.idSelecionado)) return; 
            
            decimal pag = 0; 
            decimal.TryParse(this.txtAbater.Text, out pag); 
            if (pag <= 0) return; 
            
            try 
            { 
                BancoDeDados.Executar("UPDATE pedidos SET valor_pago = valor_pago + @v WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@v", pag), new SQLiteParameter("@id", this.idSelecionado) }); 
                CarregarHistorico(); 
                LimparCampos(); 
            } 
            catch { } 
        }
        
        private void DeletarPedido(object sender, EventArgs e) 
        { 
            if (string.IsNullOrWhiteSpace(this.idSelecionado)) return;
            if (MessageBox.Show("Excluir pedido?", "Excluir", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return; 
            
            try 
            { 
                BancoDeDados.Executar("DELETE FROM pedido_itens WHERE fk_pedido_id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", this.idSelecionado) }); 
                BancoDeDados.Executar("DELETE FROM pedidos WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", this.idSelecionado) }); 
                LimparCampos(); 
                CarregarHistorico(); 
            } 
            catch { } 
        }

        private void ExportarRecibo(object sender, EventArgs e) 
        {
            if (!string.IsNullOrWhiteSpace(this.idSelecionado) && this.grid.SelectedRows.Count > 0) 
            {
                DataGridViewRow r = this.grid.SelectedRows[0];
                string html = "<!DOCTYPE html><html><head><meta charset='utf-8'><title>Recibo #" + r.Cells["ID"].Value.ToString() + "</title><style>body{font-family:Segoe UI;background:#f1f5f9;padding:20px;}.container{max-width:800px;margin:auto;background:#fff;padding:30px;border-radius:8px;box-shadow:0 4px 6px rgba(0,0,0,0.1);}.header{text-align:center;border-bottom:2px solid #e2e8f0;padding-bottom:20px;margin-bottom:20px;}.table{width:100%;border-collapse:collapse;margin-bottom:20px;}.table th,.table td{border:1px solid #cbd5e1;padding:12px;text-align:left;}.totals{text-align:right;font-size:18px;}</style></head><body><div class='container'><div class='header'><h1>RECIBO #" + r.Cells["ID"].Value.ToString() + "</h1></div><p><strong>Cliente:</strong> " + r.Cells["Cliente"].Value.ToString() + "<br><strong>Entrega:</strong> " + r.Cells["Entrega"].Value.ToString() + "<br><strong>Obs:</strong> " + r.Cells["Obs"].Value.ToString() + "</p><table class='table'><tr><th>Itens do Pedido</th></tr><tr><td>" + r.Cells["Itens"].Value.ToString() + "</td></tr></table><div class='totals'><p>Total: " + r.Cells["Total"].Value.ToString() + "<br>Pago: " + r.Cells["Pago"].Value.ToString() + "<br><span style='color:red;'>Pendente: " + r.Cells["Pendente"].Value.ToString() + "</span></p></div></div><script>window.print();</script></body></html>";
                string folder = Application.StartupPath; 
                
                try 
                { 
                    DataTable dt = BancoDeDados.Buscar("SELECT pasta_salvamento FROM empresa LIMIT 1"); 
                    if (dt.Rows.Count > 0 && !string.IsNullOrWhiteSpace(dt.Rows[0][0].ToString())) 
                    {
                        folder = dt.Rows[0][0].ToString(); 
                    }
                } 
                catch { }
                
                string path = Path.Combine(folder, "Recibo_" + r.Cells["ID"].Value.ToString() + ".html");
                try 
                { 
                    File.WriteAllText(path, html); 
                    System.Diagnostics.Process.Start(path); 
                } 
                catch { }
            }
        }
    }

    public class TelaUsuarios : UserControl
    {
        private DataGridView grid;
        private TextBox txtNome;
        private TextBox txtEmail;
        private TextBox txtSenha;
        private TextBox txtConfirmarSenha;
        private string idSelecionado = "";
        private string emailProprio = "";

        public TelaUsuarios(string emailUsuarioLogado)
        {
            this.emailProprio = emailUsuarioLogado != null ? emailUsuarioLogado : ""; 
            this.BackColor = Cores.Fundo;
            Panel topo = ComponentesVisuais.CriarCardContainer(230);

            ComponentesVisuais.AddCampo(topo, "Nome Completo", 20, 20, 260, out this.txtNome, false, false);
            ComponentesVisuais.AddCampo(topo, "E-mail de acesso", 300, 20, 260, out this.txtEmail, false, false);
            ComponentesVisuais.AddCampo(topo, "Senha (mín. 8 caracteres)", 20, 100, 260, out this.txtSenha, true, false);
            ComponentesVisuais.AddCampo(topo, "Confirmar senha", 300, 100, 260, out this.txtConfirmarSenha, true, false);

            topo.Controls.Add(new Label { Location = new Point(20, 175), Size = new Size(540, 20), Font = new Font("Segoe UI", 8), ForeColor = Cores.TextoSecundario, Text = "Para EDITAR um usuário, deixe a senha em branco se não quiser alterá-la." });

            ComponentesVisuais.AddBotao(topo, "Cadastrar Usuário", 580, 45, 170, Cores.Sucesso, Color.White, new EventHandler(Salvar));
            ComponentesVisuais.AddBotao(topo, "Salvar Alterações", 580, 90, 170, Cores.Primario, Color.White, new EventHandler(Editar));
            ComponentesVisuais.AddBotao(topo, "Excluir Usuário", 580, 135, 170, Cores.PerigoClaro, Cores.Perigo, new EventHandler(Excluir));
            ComponentesVisuais.AddBotao(topo, "Limpar Campos", 580, 180, 170, Color.White, Cores.TextoSecundario, delegate { LimparCampos(); });

            ComponentesVisuais.AddGrid(this, out this.grid, new DataGridViewCellEventHandler(Grid_CellClick));
            this.Controls.Add(topo); 
            Carregar();
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e) 
        { 
            if (e.RowIndex < 0) return; 
            DataGridViewRow r = this.grid.Rows[e.RowIndex]; 
            this.idSelecionado = r.Cells["ID"].Value.ToString(); 
            this.txtNome.Text = r.Cells["Nome"].Value.ToString(); 
            this.txtEmail.Text = r.Cells["E-mail"].Value.ToString(); 
            this.txtSenha.Clear(); 
            this.txtConfirmarSenha.Clear(); 
        }
        
        private void LimparCampos() 
        { 
            this.idSelecionado = ""; 
            this.txtNome.Clear(); 
            this.txtEmail.Clear(); 
            this.txtSenha.Clear(); 
            this.txtConfirmarSenha.Clear(); 
        }
        
        private void Carregar() 
        { 
            try 
            { 
                this.grid.DataSource = BancoDeDados.Buscar("SELECT id AS 'ID', nome AS 'Nome', email AS 'E-mail' FROM usuarios ORDER BY nome"); 
            } 
            catch { } 
        }

        private void Salvar(object sender, EventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(this.txtNome.Text) || string.IsNullOrWhiteSpace(this.txtEmail.Text)) 
            { 
                MessageBox.Show("Informe nome e e-mail."); 
                return; 
            }
            
            if (this.txtSenha.Text.Length < 8 || this.txtSenha.Text != this.txtConfirmarSenha.Text) 
            { 
                MessageBox.Show("Senhas curtas ou não coincidem."); 
                return; 
            }
            
            try 
            { 
                BancoDeDados.Executar("INSERT INTO usuarios (nome, email, senha_hash) VALUES (@n, @e, @h)", new SQLiteParameter[] { new SQLiteParameter("@n", this.txtNome.Text), new SQLiteParameter("@e", this.txtEmail.Text.Trim().ToLower()), new SQLiteParameter("@h", Seguranca.GerarHash(this.txtSenha.Text)) }); 
                MessageBox.Show("Salvo!"); 
                LimparCampos(); 
                Carregar(); 
            }
            catch (Exception ex) 
            { 
                if (ex.Message.ToLower().Contains("unique"))
                {
                    MessageBox.Show("E-mail em uso.");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Editar(object sender, EventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(this.idSelecionado) || string.IsNullOrWhiteSpace(this.txtNome.Text)) return;
            bool troca = !string.IsNullOrEmpty(this.txtSenha.Text);
            
            if (troca && (this.txtSenha.Text.Length < 8 || this.txtSenha.Text != this.txtConfirmarSenha.Text)) 
            { 
                MessageBox.Show("Senhas curtas ou não coincidem."); 
                return; 
            }
            
            try 
            {
                if (troca) 
                {
                    BancoDeDados.Executar("UPDATE usuarios SET nome=@n, email=@e, senha_hash=@h WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@n", this.txtNome.Text), new SQLiteParameter("@e", this.txtEmail.Text.Trim().ToLower()), new SQLiteParameter("@h", Seguranca.GerarHash(this.txtSenha.Text)), new SQLiteParameter("@id", this.idSelecionado) });
                }
                else 
                {
                    BancoDeDados.Executar("UPDATE usuarios SET nome=@n, email=@e WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@n", this.txtNome.Text), new SQLiteParameter("@e", this.txtEmail.Text.Trim().ToLower()), new SQLiteParameter("@id", this.idSelecionado) });
                }
                LimparCampos(); 
                Carregar();
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }
        }

        private void Excluir(object sender, EventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(this.idSelecionado) || this.txtEmail.Text.Trim().Equals(this.emailProprio, StringComparison.OrdinalIgnoreCase)) 
            { 
                MessageBox.Show("Não pode excluir a si mesmo ou selecione alguém."); 
                return; 
            }
            
            try 
            { 
                if (Convert.ToInt32(BancoDeDados.Buscar("SELECT COUNT(*) FROM usuarios").Rows[0][0]) <= 1) 
                { 
                    MessageBox.Show("Não pode excluir o último usuário."); 
                    return; 
                } 
            } 
            catch { }
            
            if (MessageBox.Show("Excluir?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes) 
            { 
                try 
                { 
                    BancoDeDados.Executar("DELETE FROM usuarios WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", this.idSelecionado) }); 
                    LimparCampos(); 
                    Carregar(); 
                } 
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message); 
                } 
            }
        }
    }

    public class TelaEmpresa : UserControl
    {
        private TextBox txtRazao;
        private TextBox txtCnpj;
        private TextBox txtIE;
        private TextBox txtEndereco;
        private TextBox txtNumero;
        private TextBox txtBairro;
        private TextBox txtMunicipio;
        private TextBox txtUf;
        private TextBox txtCep;
        private TextBox txtTelefone;
        private TextBox txtCertificado;
        private TextBox txtSenha;
        private TextBox txtPasta;
        private ComboBox cbAmbiente;
        private string idConfig = "";

        public TelaEmpresa()
        {
            this.BackColor = Cores.Fundo; 
            this.Dock = DockStyle.Fill; 
            this.Padding = new Padding(20); 
            this.AutoScroll = true;
            Panel card = new Panel { Dock = DockStyle.Top, Height = 560, BackColor = Color.White, Padding = new Padding(20) };

            ComponentesVisuais.AddCampo(card, "Razão Social", 20, 20, 400, out this.txtRazao, false, false);
            ComponentesVisuais.AddCampo(card, "CNPJ", 440, 20, 200, out this.txtCnpj, false, false);
            ComponentesVisuais.AddCampo(card, "Inscrição Estadual (IE)", 660, 20, 200, out this.txtIE, false, false);

            ComponentesVisuais.AddCampo(card, "Endereço", 20, 100, 300, out this.txtEndereco, false, false);
            ComponentesVisuais.AddCampo(card, "Número", 340, 100, 90, out this.txtNumero, false, false);
            ComponentesVisuais.AddCampo(card, "Bairro", 450, 100, 200, out this.txtBairro, false, false);
            ComponentesVisuais.AddCampo(card, "Telefone", 670, 100, 190, out this.txtTelefone, false, false);

            ComponentesVisuais.AddCampo(card, "Município", 20, 180, 220, out this.txtMunicipio, false, false);
            ComponentesVisuais.AddCampo(card, "UF", 260, 180, 60, out this.txtUf, false, false);
            ComponentesVisuais.AddCampo(card, "CEP", 340, 180, 130, out this.txtCep, false, false);

            ComponentesVisuais.AddCampo(card, "Certificado Digital A1 (.pfx)", 20, 260, 300, out this.txtCertificado, false, true);
            ComponentesVisuais.AddBotao(card, "Localizar PFX", 330, 285, 120, Cores.Fundo, Cores.TextoPrincipal, delegate { using (OpenFileDialog o = new OpenFileDialog { Filter = "PFX|*.pfx" }) { if (o.ShowDialog() == DialogResult.OK) this.txtCertificado.Text = o.FileName; } });
            ComponentesVisuais.AddCampo(card, "Senha do Certificado", 470, 260, 170, out this.txtSenha, true, false);

            ComponentesVisuais.AddCampo(card, "Pasta Padrão DANFE/XML", 20, 340, 430, out this.txtPasta, false, true);
            ComponentesVisuais.AddBotao(card, "Escolher Pasta", 470, 365, 170, Cores.Fundo, Cores.TextoPrincipal, delegate { using (FolderBrowserDialog f = new FolderBrowserDialog()) { if (f.ShowDialog() == DialogResult.OK) this.txtPasta.Text = f.SelectedPath; } });

            ComponentesVisuais.AddCombo(card, "Ambiente", 20, 420, 300, out this.cbAmbiente);
            this.cbAmbiente.Items.Add("Homologação (teste)"); 
            this.cbAmbiente.Items.Add("Produção (nota real)"); 
            this.cbAmbiente.SelectedIndex = 0;

            card.Controls.Add(new Label { Location = new Point(340, 445), Size = new Size(500, 40), Font = new Font("Segoe UI", 8), ForeColor = Cores.Alerta, Text = "Em Produção a nota tem valor fiscal. Use após testar." });
            ComponentesVisuais.AddBotao(card, "Salvar Configurações", 20, 500, 200, Cores.Sucesso, Color.White, new EventHandler(Salvar));

            this.Controls.Add(card); 
            CarregarDados();
        }

        private void CarregarDados() 
        {
            DataTable dt = BancoDeDados.Buscar("SELECT * FROM empresa LIMIT 1");
            if (dt.Rows.Count > 0) 
            {
                DataRow r = dt.Rows[0]; 
                this.idConfig = r["id"].ToString(); 
                this.txtRazao.Text = r["razao_social"].ToString(); 
                this.txtCnpj.Text = r["cnpj"].ToString(); 
                this.txtIE.Text = r["inscricao_estadual"].ToString(); 
                this.txtEndereco.Text = r["endereco"].ToString(); 
                this.txtNumero.Text = LerColuna(dt, "endereco_numero"); 
                this.txtBairro.Text = LerColuna(dt, "endereco_bairro"); 
                this.txtMunicipio.Text = LerColuna(dt, "endereco_municipio"); 
                this.txtUf.Text = LerColuna(dt, "endereco_uf"); 
                this.txtCep.Text = LerColuna(dt, "endereco_cep"); 
                this.txtTelefone.Text = r["telefone"].ToString(); 
                this.txtCertificado.Text = r["caminho_certificado"].ToString(); 
                this.txtSenha.Text = Criptografia.Desproteger(r["senha_certificado"].ToString()); 
                this.txtPasta.Text = r["pasta_salvamento"].ToString(); 
                
                string amb = LerColuna(dt, "ambiente_emissao");
                this.cbAmbiente.SelectedIndex = (amb == "1") ? 1 : 0;
            }
        }
        
        private string LerColuna(DataTable d, string c) 
        { 
            if (d.Columns.Contains(c) && d.Rows[0][c] != DBNull.Value)
            {
                return d.Rows[0][c].ToString();
            }
            return ""; 
        }

        private void Salvar(object sender, EventArgs e) 
        {
            if (!string.IsNullOrWhiteSpace(this.txtCnpj.Text) && !ValidacoesFiscais.CnpjValido(this.txtCnpj.Text)) 
            { 
                MessageBox.Show("CNPJ inválido."); 
                return; 
            }
            if (this.cbAmbiente.SelectedIndex == 1)
            {
                if(MessageBox.Show("Ativar produção? Isso emite notas reais.", "Atenção", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
            }
            
            string amb = this.cbAmbiente.SelectedIndex == 1 ? "1" : "2";
            
            SQLiteParameter[] p = new SQLiteParameter[] { 
                new SQLiteParameter("@r", this.txtRazao.Text), 
                new SQLiteParameter("@c", ValidacoesFiscais.SomenteDigitos(this.txtCnpj.Text)), 
                new SQLiteParameter("@ie", this.txtIE.Text), 
                new SQLiteParameter("@e", this.txtEndereco.Text), 
                new SQLiteParameter("@num", this.txtNumero.Text), 
                new SQLiteParameter("@bai", this.txtBairro.Text), 
                new SQLiteParameter("@mun", this.txtMunicipio.Text), 
                new SQLiteParameter("@uf", this.txtUf.Text.Trim().ToUpper()), 
                new SQLiteParameter("@cep", ValidacoesFiscais.SomenteDigitos(this.txtCep.Text)), 
                new SQLiteParameter("@t", this.txtTelefone.Text), 
                new SQLiteParameter("@cert", this.txtCertificado.Text), 
                new SQLiteParameter("@s", Criptografia.Proteger(this.txtSenha.Text)), 
                new SQLiteParameter("@p", this.txtPasta.Text), 
                new SQLiteParameter("@amb", amb), 
                new SQLiteParameter("@id", this.idConfig) 
            };
            
            if (string.IsNullOrEmpty(this.idConfig)) 
            {
                BancoDeDados.Executar("INSERT INTO empresa (razao_social, cnpj, inscricao_estadual, endereco, endereco_numero, endereco_bairro, endereco_municipio, endereco_uf, endereco_cep, telefone, caminho_certificado, senha_certificado, pasta_salvamento, ambiente_emissao) VALUES (@r, @c, @ie, @e, @num, @bai, @mun, @uf, @cep, @t, @cert, @s, @p, @amb)", p);
            }
            else 
            {
                BancoDeDados.Executar("UPDATE empresa SET razao_social=@r, cnpj=@c, inscricao_estadual=@ie, endereco=@e, endereco_numero=@num, endereco_bairro=@bai, endereco_municipio=@mun, endereco_uf=@uf, endereco_cep=@cep, telefone=@t, caminho_certificado=@cert, senha_certificado=@s, pasta_salvamento=@p, ambiente_emissao=@amb WHERE id=@id", p);
            }
            
            MessageBox.Show("Salvo!"); 
            CarregarDados();
        }
    }

   public class FormEmissaoNFe : Form
    {
        private TextBox txtCpfCnpj, txtIe, txtNome, txtEndereco, txtNumero, txtBairro, txtMunicipio, txtUf, txtCep;
        private RadioButton rbPf, rbPj;
        private Label lblIe, lblCpfCnpj, lblErro;
        private string idPedido, idCliente;

        public FormEmissaoNFe(string idPedido, string idCliente)
        {
            this.idPedido = idPedido; this.idCliente = idCliente; Text = "Dados para Emissão";
            Size = new Size(470, 680); StartPosition = FormStartPosition.CenterParent; BackColor = Cores.Fundo;
            FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = MinimizeBox = false;

            Panel card = new Panel { Location = new Point(20, 20), Size = new Size(410, 540), BackColor = Color.White };
            rbPf = new RadioButton { Text = "PF", Location = new Point(20, 20), AutoSize = true, Checked = true };
            rbPj = new RadioButton { Text = "PJ", Location = new Point(100, 20), AutoSize = true };
            rbPf.CheckedChanged += delegate { lblCpfCnpj.Text = rbPf.Checked ? "CPF" : "CNPJ"; lblIe.Visible = txtIe.Parent.Visible = rbPj.Checked; };
            card.Controls.Add(rbPf); card.Controls.Add(rbPj);

            ComponentesVisuais.AddCampo(card, "Nome / Razão", 20, 60, 370, out txtNome, false, false);
            ComponentesVisuais.AddCampo(card, "CPF", 20, 135, 220, out txtCpfCnpj, false, false); lblCpfCnpj = (Label)card.Controls[card.Controls.Count - 2];
            ComponentesVisuais.AddCampo(card, "IE", 250, 135, 140, out txtIe, false, false); lblIe = (Label)card.Controls[card.Controls.Count - 2];
            lblIe.Visible = txtIe.Parent.Visible = false;

            ComponentesVisuais.AddCampo(card, "Endereço", 20, 215, 260, out txtEndereco, false, false);
            ComponentesVisuais.AddCampo(card, "Número", 290, 215, 100, out txtNumero, false, false);
            ComponentesVisuais.AddCampo(card, "Bairro", 20, 295, 200, out txtBairro, false, false);
            ComponentesVisuais.AddCampo(card, "CEP", 230, 295, 160, out txtCep, false, false);
            ComponentesVisuais.AddCampo(card, "Município", 20, 375, 270, out txtMunicipio, false, false);
            ComponentesVisuais.AddCampo(card, "UF", 300, 375, 90, out txtUf, false, false);

            lblErro = new Label { Location = new Point(20, 450), Size = new Size(370, 60), ForeColor = Cores.Perigo, Font = new Font("Segoe UI", 8, FontStyle.Bold) };
            card.Controls.Add(lblErro); Controls.Add(card);

            ComponentesVisuais.AddBotao(this, "Emitir NF-e", 20, 580, 200, Cores.Sucesso, Color.White, Emitir_Click);
            Carregar();
        }

        private void Carregar() {
            var dt = BancoDeDados.Buscar("SELECT * FROM clientes WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", idCliente) });
            if (dt.Rows.Count > 0) {
                var r = dt.Rows[0]; txtNome.Text = r["nome"].ToString(); txtCpfCnpj.Text = r["cpf_cnpj"].ToString(); txtIe.Text = r["inscricao_estadual"].ToString(); txtEndereco.Text = r["endereco"].ToString(); txtNumero.Text = r["endereco_numero"].ToString(); txtBairro.Text = r["endereco_bairro"].ToString(); txtMunicipio.Text = r["endereco_municipio"].ToString(); txtUf.Text = r["endereco_uf"].ToString(); txtCep.Text = r["endereco_cep"].ToString();
                if (r["tipo_pessoa"].ToString() == "PJ") rbPj.Checked = true; else rbPf.Checked = true;
            }
        }

        private void Emitir_Click(object sender, EventArgs e) {
            lblErro.Text = ""; string tipo = rbPf.Checked ? "PF" : "PJ";
            
            if (!ValidacoesFiscais.DocumentoValido(txtCpfCnpj.Text)) { lblErro.Text = "Doc inválido."; return; }
            
            string errE = ValidacoesFiscais.ValidarEnderecoCompleto(txtEndereco.Text, txtNumero.Text, txtBairro.Text, txtMunicipio.Text, txtUf.Text, txtCep.Text);
            if (errE != null) { lblErro.Text = errE; return; }
            
            if (string.IsNullOrWhiteSpace(txtNome.Text)) { lblErro.Text = "Informe nome."; return; }

            // ---------------------------------------------------------------------
            // NOVA VALIDAÇÃO DE CERTIFICADO DIRETO NO C# (EVITA TRAVAR A FILA)
            // ---------------------------------------------------------------------
            DataTable dtEmpresa = BancoDeDados.Buscar("SELECT caminho_certificado, senha_certificado FROM empresa LIMIT 1");
            if (dtEmpresa.Rows.Count == 0 || string.IsNullOrWhiteSpace(dtEmpresa.Rows[0]["caminho_certificado"].ToString()) || !System.IO.File.Exists(dtEmpresa.Rows[0]["caminho_certificado"].ToString()))
            {
                lblErro.Text = "Certificado digital não configurado na tela Empresa.";
                return;
            }
            if (string.IsNullOrWhiteSpace(Criptografia.Desproteger(dtEmpresa.Rows[0]["senha_certificado"].ToString())))
            {
                lblErro.Text = "Senha do certificado não configurada na tela Empresa.";
                return;
            }

            // Grava os dados do cliente e libera a nota para a fila
            BancoDeDados.Executar("UPDATE clientes SET nome=@n, cpf_cnpj=@c, tipo_pessoa=@t, inscricao_estadual=@i, endereco=@end, endereco_numero=@num, endereco_bairro=@bai, endereco_municipio=@mun, endereco_uf=@uf, endereco_cep=@cep WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@n", txtNome.Text), new SQLiteParameter("@c", ValidacoesFiscais.SomenteDigitos(txtCpfCnpj.Text)), new SQLiteParameter("@t", tipo), new SQLiteParameter("@i", txtIe.Text), new SQLiteParameter("@end", txtEndereco.Text), new SQLiteParameter("@num", txtNumero.Text), new SQLiteParameter("@bai", txtBairro.Text), new SQLiteParameter("@mun", txtMunicipio.Text), new SQLiteParameter("@uf", txtUf.Text.Trim().ToUpper()), new SQLiteParameter("@cep", ValidacoesFiscais.SomenteDigitos(txtCep.Text)), new SQLiteParameter("@id", idCliente) });
            BancoDeDados.Executar("UPDATE pedidos SET status_fiscal='PENDENTE', mensagem_erro='' WHERE id=@id", new SQLiteParameter[] { new SQLiteParameter("@id", idPedido) });
            
            DialogResult = DialogResult.OK; 
            Close();
        }
    }

    public class TelaNotasFiscais : UserControl
    {
        private DataGridView grid;
        private string idPedidoSelecionado = "";
        private string idClienteSelecionado = "";
        private string caminhoDanfe = "";
        private string caminhoXml = "";
        private string status = "";
        private string chave = "";
        private string msgErro = "";
        private Label lblStatusAgente;
        private Timer timerAtualizacao;

        public TelaNotasFiscais()
        {
            this.BackColor = Cores.Fundo; 
            this.Dock = DockStyle.Fill; 
            this.Padding = new Padding(20);
            
            Panel topo = ComponentesVisuais.CriarCardContainer(140);
            topo.Controls.Add(ComponentesVisuais.CriarLabel("Monitoramento e Emissão", 20, 20));

            ComponentesVisuais.AddBotao(topo, "Atualizar", 20, 45, 140, Color.FromArgb(59, 130, 246), Color.White, delegate { Carregar(); });
            ComponentesVisuais.AddBotao(topo, "Emitir NF-e", 170, 45, 130, Cores.Sucesso, Color.White, new EventHandler(SolicitarEmissao));
            ComponentesVisuais.AddBotao(topo, "Abrir DANFE", 310, 45, 160, Color.FromArgb(71, 85, 105), Color.White, new EventHandler(AbrirDanfe));
            ComponentesVisuais.AddBotao(topo, "Baixar XML", 480, 45, 160, Color.FromArgb(71, 85, 105), Color.White, new EventHandler(BaixarXml));
            ComponentesVisuais.AddBotao(topo, "Cancelar Nota", 20, 90, 140, Cores.PerigoClaro, Cores.Perigo, new EventHandler(CancelarNota));
            ComponentesVisuais.AddBotao(topo, "Ver Motivo do Erro", 170, 90, 160, Color.FromArgb(255, 251, 235), Cores.Alerta, new EventHandler(VerMotivoErro));

            this.lblStatusAgente = ComponentesVisuais.CriarLabel("● Verificando agente...", 340, 100); 
            topo.Controls.Add(this.lblStatusAgente);
            
            ComponentesVisuais.AddGrid(this, out this.grid, new DataGridViewCellEventHandler(Grid_CellClick)); 
            this.Controls.Add(topo);
            
            Carregar(); 
            AtualizarStatus();
            
            this.timerAtualizacao = new Timer();
            this.timerAtualizacao.Interval = 4000;
            this.timerAtualizacao.Tick += delegate { Carregar(); AtualizarStatus(); };
            this.timerAtualizacao.Start(); 
            this.Disposed += delegate { this.timerAtualizacao.Stop(); };
        }

        private void AtualizarStatus() 
        { 
            if (AgenteFiscalManager.Rodando) 
            { 
                this.lblStatusAgente.Text = "● Agente ativo"; 
                this.lblStatusAgente.ForeColor = Cores.Sucesso; 
            } 
            else 
            { 
                this.lblStatusAgente.Text = "● Agente parado"; 
                this.lblStatusAgente.ForeColor = Cores.Perigo; 
            } 
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e) 
        {
            if (e.RowIndex < 0) return; 
            DataGridViewRow r = this.grid.Rows[e.RowIndex];
            this.idPedidoSelecionado = r.Cells["ID_Pedido"].Value != null ? r.Cells["ID_Pedido"].Value.ToString() : "";
            this.idClienteSelecionado = r.Cells["ID_Cliente"].Value != null ? r.Cells["ID_Cliente"].Value.ToString() : "";
            this.caminhoDanfe = r.Cells["ID_Danfe"].Value != null ? r.Cells["ID_Danfe"].Value.ToString() : "";
            this.caminhoXml = r.Cells["ID_Xml"].Value != null ? r.Cells["ID_Xml"].Value.ToString() : "";
            this.status = r.Cells["ID_StatusBruto"].Value != null ? r.Cells["ID_StatusBruto"].Value.ToString() : "";
            this.chave = r.Cells["ID_Chave"].Value != null ? r.Cells["ID_Chave"].Value.ToString() : "";
            this.msgErro = r.Cells["ID_Erro"].Value != null ? r.Cells["ID_Erro"].Value.ToString() : "";
        }

        private void Carregar() 
        {
            try { BancoDeDados.Executar("ALTER TABLE pedidos ADD COLUMN caminho_xml TEXT"); } catch { }
            try 
            {
                int l = this.grid.FirstDisplayedScrollingRowIndex;
                this.grid.DataSource = BancoDeDados.Buscar("SELECT p.id AS 'ID_Pedido', c.id AS 'ID_Cliente', c.nome AS 'Cliente', p.data_entrega AS 'Data', printf('R$ %.2f', p.valor_total) AS 'Valor', CASE p.status_fiscal WHEN 'AGUARDANDO' THEN 'Não iniciado' WHEN 'PENDENTE' THEN 'Na fila' WHEN 'PROCESSANDO_SEFAZ' THEN 'Processando...' WHEN 'AUTORIZADA_HOMOLOGACAO' THEN 'Autorizada (homologação)' WHEN 'AUTORIZADA' THEN 'Autorizada' WHEN 'ERRO_EMISSAO' THEN 'Erro na emissão ⚠' WHEN 'CANCELADA' THEN 'Cancelada' ELSE p.status_fiscal END AS 'Status_NFe', p.numero_protocolo AS 'Protocolo', p.caminho_danfe AS 'ID_Danfe', p.caminho_xml AS 'ID_Xml', p.status_fiscal AS 'ID_StatusBruto', p.chave_acesso AS 'ID_Chave', p.mensagem_erro AS 'ID_Erro' FROM pedidos p LEFT JOIN clientes c ON p.fk_cliente_id = c.id ORDER BY p.id DESC");
                
                foreach (string col in new string[] { "ID_Cliente", "ID_Danfe", "ID_Xml", "ID_StatusBruto", "ID_Chave", "ID_Erro" }) 
                {
                    if (this.grid.Columns.Contains(col)) 
                    {
                        this.grid.Columns[col].Visible = false;
                    }
                }
                if (l >= 0 && l < this.grid.Rows.Count) 
                {
                    this.grid.FirstDisplayedScrollingRowIndex = l;
                }
            } 
            catch { }
        }

        private void SolicitarEmissao(object sender, EventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(this.idPedidoSelecionado)) 
            { 
                MessageBox.Show("Selecione pedido."); 
                return; 
            }
            if (!AgenteFiscalManager.Rodando) 
            { 
                AgenteFiscalManager.Iniciar(); 
                AtualizarStatus(); 
            }
            using (FormEmissaoNFe f = new FormEmissaoNFe(this.idPedidoSelecionado, this.idClienteSelecionado)) 
            {
                if (f.ShowDialog() == DialogResult.OK) 
                {
                    Carregar();
                }
            }
        }

        private void VerMotivoErro(object sender, EventArgs e) 
        {
            if (this.status != "ERRO_EMISSAO") 
            { 
                MessageBox.Show("Sem erro."); 
                return; 
            }
            MessageBox.Show("Falha:\n\n" + (string.IsNullOrWhiteSpace(this.msgErro) ? "Erro genérico." : this.msgErro));
        }

        private void CancelarNota(object sender, EventArgs e) 
        {
            if (this.status != "AUTORIZADA") 
            { 
                MessageBox.Show("Apenas notas AUTORIZADAS podem ser canceladas."); 
                return; 
            }
            if (string.IsNullOrWhiteSpace(this.chave)) return;
            
            string just = "";
            using (Form d = new Form { Text = "Justificativa", Size = new Size(460, 260), StartPosition = FormStartPosition.CenterParent, BackColor = Cores.Fundo }) 
            {
                d.Controls.Add(new Label { Location = new Point(20, 15), Size = new Size(410, 40), Text = "Motivo (mínimo 15 caracteres):" });
                TextBox t = new TextBox { Location = new Point(20, 60), Size = new Size(410, 80), Multiline = true }; 
                d.Controls.Add(t);
                
                ComponentesVisuais.AddBotao(d, "Confirmar", 20, 160, 190, Cores.Primario, Color.White, delegate { just = t.Text; d.DialogResult = DialogResult.OK; });
                
                if (d.ShowDialog() != DialogResult.OK || just.Length < 15) 
                { 
                    MessageBox.Show("Justificativa curta."); 
                    return; 
                }
            }
            
            string cnpj = BancoDeDados.Buscar("SELECT cnpj FROM empresa LIMIT 1").Rows[0][0].ToString();
            try 
            {
                string r = AgenteFiscalManager.CancelarNotaFiscal(this.chave, just, cnpj);
                if (Regex.IsMatch(r != null ? r : "", "\"sucesso\"\\s*:\\s*true")) 
                {
                    MessageBox.Show("Cancelada! Protocolo: " + Regex.Match(r, "\"protocolo_cancelamento\"\\s*:\\s*\"([^\"]*)\"").Groups[1].Value);
                }
                else 
                {
                    MessageBox.Show("Erro: " + Regex.Match(r != null ? r : "", "\"erro\"\\s*:\\s*\"([^\"]*)\"").Groups[1].Value);
                }
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            } 
            Carregar();
        }

        private void AbrirDanfe(object sender, EventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(this.idPedidoSelecionado)) return;
            if (File.Exists(this.caminhoDanfe)) 
            {
                System.Diagnostics.Process.Start(this.caminhoDanfe); 
            }
            else 
            {
                MessageBox.Show("DANFE não encontrado.");
            }
        }

        private void BaixarXml(object sender, EventArgs e) 
        {
            if (!File.Exists(this.caminhoXml)) 
            { 
                MessageBox.Show("XML não encontrado."); 
                return; 
            }
            using (SaveFileDialog s = new SaveFileDialog { Filter = "XML|*.xml", FileName = Path.GetFileName(this.caminhoXml) }) 
            {
                if (s.ShowDialog() == DialogResult.OK) 
                {
                    File.Copy(this.caminhoXml, s.FileName, true);
                }
            }
        }
    }
}