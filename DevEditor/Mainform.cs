using PentaGE.Cameras;
using PentaGE.Core;
using PentaGE.Viewports;

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
            Engine.Invalidate += Engine_Invalidate;
        }

        private void Mainform_Paint(object? sender, PaintEventArgs e)
        {
            Engine.RenderGraphics(e.Graphics);
        }

        private void Engine_Invalidate(object sender, EventArgs e)
        {
            Invalidate();
            Application.DoEvents(); // Important
        }

        private void Mainform_FormClosing(object? sender, FormClosingEventArgs e)
        {
            Engine.Stop();
        }

        private void Mainform_Shown(object? sender, EventArgs e)
        {
            var camera = Camera.CreateTopDownCamera();
            var viewport1 = new Viewport(0, 0, ClientRectangle, camera);

            Engine.Viewports.Add(viewport1);
            Engine.Run();
        }
    }
}