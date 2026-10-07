using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PDVCompleto
{
    public static class Cores
    {
        public static readonly Color Fundo = Color.FromArgb(241, 245, 249);
        public static readonly Color Superficie = Color.White;
        public static readonly Color TextoPrincipal = Color.FromArgb(15, 23, 42);
        public static readonly Color TextoSecundario = Color.FromArgb(100, 116, 139);
        public static readonly Color Borda = Color.FromArgb(226, 232, 240);
        public static readonly Color Primario = Color.FromArgb(79, 70, 229);
        public static readonly Color PrimarioClaro = Color.FromArgb(238, 242, 255);
        public static readonly Color Sucesso = Color.FromArgb(16, 185, 129);
        public static readonly Color Perigo = Color.FromArgb(220, 38, 38);
        public static readonly Color PerigoClaro = Color.FromArgb(254, 242, 242);
        public static readonly Color Alerta = Color.FromArgb(217, 119, 6);
        public static readonly Color SidebarFundo = Color.FromArgb(15, 23, 42);
        public static readonly Color SidebarFundoEscuro = Color.FromArgb(2, 6, 23);
        public static readonly Color SidebarTextoInativo = Color.FromArgb(148, 163, 184);
    }

    internal static class DesenhoUtil
    {
        public static GraphicsPath CaminhoArredondado(Rectangle bounds, int raio)
        {
            int d = raio * 2;
            var path = new GraphicsPath();
            if (d <= 0 || bounds.Width <= d || bounds.Height <= d)
            {
                path.AddRectangle(bounds);
                return path;
            }
            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static Color Escurecer(Color cor, double fator)
        {
            int r = (int)(cor.R * (1 - fator));
            int g = (int)(cor.G * (1 - fator));
            int b = (int)(cor.B * (1 - fator));
            return Color.FromArgb(cor.A, Math.Max(r, 0), Math.Max(g, 0), Math.Max(b, 0));
        }
    }

    public class BotaoModerno : Button
    {
        private bool mouseOver = false;
        public int Raio { get; set; }

        public BotaoModerno()
        {
            Raio = 8;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            MouseEnter += (s, e) => { mouseOver = true; Invalidate(); };
            MouseLeave += (s, e) => { mouseOver = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Parent != null)
            {
                g.Clear(Parent.BackColor);
            }

            Color fundo = mouseOver ? DesenhoUtil.Escurecer(BackColor, 0.10) : BackColor;
            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            
            using (var path = DesenhoUtil.CaminhoArredondado(bounds, Raio))
            {
                using (var brush = new SolidBrush(fundo))
                {
                    g.FillPath(brush, path);
                }
            }

            Rectangle areaTexto = ClientRectangle;
            if (TextAlign == ContentAlignment.MiddleLeft) areaTexto.X += 16;
            if (TextAlign == ContentAlignment.MiddleRight) areaTexto.Width -= 16;
            TextRenderer.DrawText(g, Text, Font, areaTexto, ForeColor, ObterFlagsAlinhamento());
        }

        private TextFormatFlags ObterFlagsAlinhamento()
        {
            TextFormatFlags flags = TextFormatFlags.EndEllipsis;
            switch (TextAlign)
            {
                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
                    break;
                default:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                    break;
            }
            return flags;
        }
    }

    public class PainelArredondado : Panel
    {
        public int Raio { get; set; }
        public Color CorBorda { get; set; }
        public bool MostrarBorda { get; set; }

        public PainelArredondado()
        {
            Raio = 12;
            CorBorda = Cores.Borda;
            MostrarBorda = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Parent != null)
            {
                g.Clear(Parent.BackColor);
            }

            Rectangle bounds = new Rectangle(0, 0, Math.Max(Width - 1, 1), Math.Max(Height - 1, 1));
            
            using (var path = DesenhoUtil.CaminhoArredondado(bounds, Raio))
            {
                using (var brush = new SolidBrush(BackColor))
                {
                    g.FillPath(brush, path);
                }

                if (MostrarBorda)
                {
                    using (var pen = new Pen(CorBorda, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }
}