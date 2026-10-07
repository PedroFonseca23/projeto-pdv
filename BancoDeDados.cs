using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace PDVCompleto
{
    public class BancoDeDados
    {
        public static string Caminho = "gestor.db";

        public static SQLiteConnection Conectar()
        {
            SQLiteConnection conexao = new SQLiteConnection("Data Source=" + Caminho + ";Version=3;Foreign Keys=True;Default Timeout=5;");
            conexao.Open();
            return conexao;
        }

        public static void Inicializar()
        {
            if (!File.Exists(Caminho))
            {
                SQLiteConnection.CreateFile(Caminho);
            }

            using (SQLiteConnection conexao = Conectar())
            {
                string sql =
                    "CREATE TABLE IF NOT EXISTS clientes (" +
                    "   id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "   nome TEXT NOT NULL," +
                    "   telefone TEXT," +
                    "   cpf_cnpj TEXT DEFAULT ''," +
                    "   tipo_pessoa TEXT DEFAULT 'PF'," +
                    "   inscricao_estadual TEXT DEFAULT ''" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS produtos (" +
                    "   id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "   nome TEXT NOT NULL UNIQUE," +
                    "   descricao TEXT," +
                    "   preco REAL DEFAULT 0," +
                    "   ncm TEXT DEFAULT '00000000'," +
                    "   cfop TEXT DEFAULT '5102'," +
                    "   unidade TEXT DEFAULT 'UN'" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS pedidos (" +
                    "   id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "   fk_cliente_id INTEGER NOT NULL," +
                    "   evento TEXT," +
                    "   data_entrega TEXT NOT NULL," +
                    "   data_criacao TEXT," +
                    "   valor_total REAL NOT NULL DEFAULT 0.0," +
                    "   valor_sinal REAL NOT NULL DEFAULT 0.0," +
                    "   identificador_pagamento TEXT UNIQUE," +
                    "   descricao TEXT," +
                    "   valor_pago REAL DEFAULT 0," +
                    "   status_fiscal TEXT DEFAULT 'AGUARDANDO'," +
                    "   chave_acesso TEXT DEFAULT ''," +
                    "   numero_protocolo TEXT DEFAULT ''," +
                    "   data_autorizacao TEXT DEFAULT ''," +
                    "   caminho_danfe TEXT DEFAULT ''," +
                    "   numero_nfe TEXT DEFAULT ''," +
                    "   mensagem_erro TEXT DEFAULT ''," +
                    "   caminho_xml TEXT," +
                    "   FOREIGN KEY (fk_cliente_id) REFERENCES clientes (id)" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS pedido_itens (" +
                    "   id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "   fk_pedido_id INTEGER NOT NULL," +
                    "   fk_produto_id INTEGER NOT NULL," +
                    "   quantidade INTEGER NOT NULL," +
                    "   FOREIGN KEY (fk_pedido_id) REFERENCES pedidos (id) ON DELETE CASCADE," +
                    "   FOREIGN KEY (fk_produto_id) REFERENCES produtos (id)" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS pagamentos (" +
                    "   id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "   fk_pedido_id INTEGER NOT NULL," +
                    "   valor_pago REAL NOT NULL," +
                    "   data_pagamento TEXT NOT NULL," +
                    "   metodo TEXT," +
                    "   FOREIGN KEY (fk_pedido_id) REFERENCES pedidos (id)" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS empresa (" +
                    "   id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "   razao_social TEXT," +
                    "   cnpj TEXT," +
                    "   endereco TEXT," +
                    "   telefone TEXT," +
                    "   caminho_certificado TEXT," +
                    "   senha_certificado TEXT," +
                    "   pasta_salvamento TEXT," +
                    "   inscricao_estadual TEXT DEFAULT ''" +
                    ");" +
                    "CREATE TABLE IF NOT EXISTS usuarios (" +
                    "   id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "   nome TEXT NOT NULL," +
                    "   email TEXT NOT NULL UNIQUE," +
                    "   senha_hash BLOB NOT NULL," +
                    "   caminho_foto TEXT" +
                    ");";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    cmd.ExecuteNonQuery();
                }

                AdicionarColuna(conexao, "empresa", "endereco_numero", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "empresa", "endereco_bairro", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "empresa", "endereco_municipio", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "empresa", "endereco_uf", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "empresa", "endereco_cep", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "empresa", "ambiente_emissao", "TEXT DEFAULT '2'");
                AdicionarColuna(conexao, "empresa", "ultimo_numero_nfce", "INTEGER DEFAULT 0");
                AdicionarColuna(conexao, "clientes", "endereco", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "clientes", "endereco_numero", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "clientes", "endereco_bairro", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "clientes", "endereco_municipio", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "clientes", "endereco_uf", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "clientes", "endereco_cep", "TEXT DEFAULT ''");
                AdicionarColuna(conexao, "pedidos", "mensagem_erro", "TEXT DEFAULT ''");
                
                // Força a inserção de data de criação nos pedidos antigos usando a mesma conexão
                using (SQLiteCommand cmdDt = new SQLiteCommand("UPDATE pedidos SET data_criacao = datetime('now', 'localtime') WHERE data_criacao IS NULL;", conexao))
                {
                    cmdDt.ExecuteNonQuery();
                }
            }
        }

        private static void AdicionarColuna(SQLiteConnection conexao, string tabela, string coluna, string definicao)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(
                    string.Format("ALTER TABLE {0} ADD COLUMN {1} {2}", tabela, coluna, definicao), conexao))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SQLiteException)
            {
            }
        }

        public static DataTable Buscar(string sql)
        {
            return Buscar(sql, null);
        }

        public static DataTable Buscar(string sql, SQLiteParameter[] parametros)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conexao = Conectar())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    if (parametros != null)
                    {
                        cmd.Parameters.AddRange(parametros);
                    }
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static void Executar(string sql)
        {
            Executar(sql, null);
        }

        public static void Executar(string sql, SQLiteParameter[] parametros)
        {
            using (SQLiteConnection conexao = Conectar())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    if (parametros != null)
                    {
                        cmd.Parameters.AddRange(parametros);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static long ExecutarRetornandoId(string sql, SQLiteParameter[] parametros)
        {
            using (SQLiteConnection conexao = Conectar())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conexao))
                {
                    if (parametros != null)
                    {
                        cmd.Parameters.AddRange(parametros);
                    }
                    cmd.ExecuteNonQuery();
                    return conexao.LastInsertRowId;
                }
            }
        }

        public static void ImportarBanco()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQLite Database|*.db";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string caminhoOrigem = openFileDialog.FileName;
                string caminhoDestino = Path.Combine(Application.StartupPath, Caminho);

                if (MessageBox.Show(
                        "Isso vai SUBSTITUIR o banco de dados atual (clientes, pedidos, notas, etc.) pelo arquivo selecionado.\n\n" +
                        "Essa ação não pode ser desfeita. Deseja continuar?",
                        "Confirmar importação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                File.Copy(caminhoOrigem, caminhoDestino, true);
                MessageBox.Show("Banco de dados carregado com sucesso.");
            }
        }

        public static DataTable PesquisarClientes(string termoBusca)
        {
            string sql = "SELECT id AS 'ID', nome AS 'Nome', cpf_cnpj AS 'CPF/CNPJ', telefone AS 'Telefone', endereco_municipio AS 'Cidade', endereco_uf AS 'UF' FROM clientes WHERE nome LIKE @termo OR telefone LIKE @termo";
            SQLiteParameter[] parametros = new SQLiteParameter[]
            {
                new SQLiteParameter("@termo", "%" + termoBusca + "%")
            };
            return Buscar(sql, parametros);
        }

        public static DataTable PesquisarPedidosPorData(DateTime dataInicio, DateTime dataFim)
        {
            string sql = @"SELECT p.id AS 'ID', c.nome AS 'Cliente', p.evento AS 'Obs', p.data_entrega AS 'Entrega', 
                           GROUP_CONCAT(i.quantidade || 'x ' || pr.nome, ', ') AS 'Itens', 
                           printf('R$ %.2f', p.valor_total) AS 'Total', 
                           printf('R$ %.2f', p.valor_pago) AS 'Pago', 
                           printf('R$ %.2f', (p.valor_total - p.valor_pago)) AS 'Pendente' 
                           FROM pedidos p 
                           LEFT JOIN clientes c ON p.fk_cliente_id = c.id 
                           LEFT JOIN pedido_itens i ON p.id = i.fk_pedido_id 
                           LEFT JOIN produtos pr ON i.fk_produto_id = pr.id 
                           WHERE (p.data_criacao >= @inicio AND p.data_criacao <= @fim)
                              OR (CASE 
                                    WHEN substr(p.data_entrega, 3, 1) = '/' THEN substr(p.data_entrega, 7, 4) || '-' || substr(p.data_entrega, 4, 2) || '-' || substr(p.data_entrega, 1, 2)
                                    ELSE p.data_entrega 
                                  END >= @inicio_dt AND 
                                  CASE 
                                    WHEN substr(p.data_entrega, 3, 1) = '/' THEN substr(p.data_entrega, 7, 4) || '-' || substr(p.data_entrega, 4, 2) || '-' || substr(p.data_entrega, 1, 2)
                                    ELSE p.data_entrega 
                                  END <= @fim_dt)
                           GROUP BY p.id ORDER BY p.id DESC";

            SQLiteParameter[] parametros = new SQLiteParameter[]
            {
                new SQLiteParameter("@inicio", dataInicio.ToString("yyyy-MM-dd 00:00:00")),
                new SQLiteParameter("@fim", dataFim.ToString("yyyy-MM-dd 23:59:59")),
                new SQLiteParameter("@inicio_dt", dataInicio.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@fim_dt", dataFim.ToString("yyyy-MM-dd"))
            };
            return Buscar(sql, parametros);
        }

        public static DataTable BuscarPedidosDaSemanaAtual()
        {
            string sql = @"SELECT p.id AS 'ID', c.nome AS 'Cliente', p.data_entrega AS 'Entrega', printf('R$ %.2f', p.valor_total) AS 'Valor'
                           FROM pedidos p LEFT JOIN clientes c ON p.fk_cliente_id = c.id 
                           WHERE p.data_criacao >= datetime('now', '-7 days', 'localtime')
                              OR CASE 
                                    WHEN substr(p.data_entrega, 3, 1) = '/' THEN substr(p.data_entrega, 7, 4) || '-' || substr(p.data_entrega, 4, 2) || '-' || substr(p.data_entrega, 1, 2)
                                    ELSE p.data_entrega 
                                 END >= date('now', '-7 days', 'localtime')
                           ORDER BY p.id DESC LIMIT 8";
            return Buscar(sql);
        }

        public static DataTable BuscarPedidosDaSemanaCompleto()
        {
            string sql = @"SELECT p.id AS 'ID', c.nome AS 'Cliente', p.evento AS 'Obs', p.data_entrega AS 'Entrega', 
                           GROUP_CONCAT(i.quantidade || 'x ' || pr.nome, ', ') AS 'Itens', 
                           printf('R$ %.2f', p.valor_total) AS 'Total', 
                           printf('R$ %.2f', p.valor_pago) AS 'Pago', 
                           printf('R$ %.2f', (p.valor_total - p.valor_pago)) AS 'Pendente' 
                           FROM pedidos p 
                           LEFT JOIN clientes c ON p.fk_cliente_id = c.id 
                           LEFT JOIN pedido_itens i ON p.id = i.fk_pedido_id 
                           LEFT JOIN produtos pr ON i.fk_produto_id = pr.id 
                           WHERE p.data_criacao >= datetime('now', '-7 days', 'localtime')
                              OR CASE 
                                    WHEN substr(p.data_entrega, 3, 1) = '/' THEN substr(p.data_entrega, 7, 4) || '-' || substr(p.data_entrega, 4, 2) || '-' || substr(p.data_entrega, 1, 2)
                                    ELSE p.data_entrega 
                                 END >= date('now', '-7 days', 'localtime')
                           GROUP BY p.id ORDER BY p.id DESC";
            return Buscar(sql);
        }
    }
}