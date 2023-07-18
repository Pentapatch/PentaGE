using PentaGE.Cameras;
using PentaGE.Core;
using PentaGE.Structs;
using PentaGE.Viewports;
using PentaGE.WorldGrid;
using System.Numerics;

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
            Engine.Render(e.Graphics);
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
            var e1 = EulerAngles.FromVector3(World.DownVector / 2);
            var e2 = EulerAngles.FromVector3(World.RightVector);
            var t1 = e1.GetForwardVector();

            var camera1 = Camera.CreateTopDownCamera();
            var camera2 = Camera.CreateTopDownCamera();
            camera2.Position = new(10, 0, 0);
            var viewport1 = new Viewport(0, 0, new(0, 0, ClientRectangle.Width / 2, ClientRectangle.Height), camera1);
            var viewport2 = new Viewport(ClientRectangle.Width / 2, 0, new(0, 0, ClientRectangle.Width / 2, ClientRectangle.Height), camera2);

            Engine.Viewports.Add(viewport1);
            Engine.Viewports.Add(viewport2);

            Engine.Run();
        }
    }
}