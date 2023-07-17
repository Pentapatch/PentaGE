using PentaGE.Core;

namespace DevEditor
{
    public partial class Mainform : Form
    {
        private readonly Engine _engine = new();

        private Engine Engine => _engine;

        public Mainform()
        {
            InitializeComponent();
            Shown += Mainform_Shown;
            FormClosing += Mainform_FormClosing;
            Paint += Mainform_Paint;
            Engine.Render += Engine_Render;
        }

        private void Mainform_Paint(object? sender, PaintEventArgs e)
        {
            Engine.RenderGraphics(e.Graphics);
        }

        private void Engine_Render(object sender, EventArgs e)
        {
            Invalidate();
            Application.DoEvents();
        }

        private void Mainform_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Engine.Stop();
        }

        private void Mainform_Shown(object? sender, EventArgs e)
        {
            Engine.Run();
        }
    }
}