using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Taped.Renderer;
using Taped.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Taped
{
    internal class MainWindow : GameWindow
    {

        //make a window option class for more user control
        public MainWindow(GameWindowSettings settings) : base(settings, NativeWindowSettings.Default)
        {
            MasterRenderer.LoadResources(1000);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(Color4.DarkGoldenrod);

            for (int i = 0; i < SceneManager.objects.Count; i++)
            {
                SceneManager.objects[i].Start();
            }
            base.OnLoad();
        }

       protected override void OnUpdateFrame(FrameEventArgs args)
        {
            for (int i = 0; i < SceneManager.objects.Count; i++)
            {
                SceneManager.objects[i].Update(args.Time, KeyboardState);
            }

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            MasterRenderer.Render(Size, args);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }
    }
}
