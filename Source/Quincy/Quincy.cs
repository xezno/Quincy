using OpenGL;
using OpenGL.CoreUI;
using System;

namespace Quincy
{
    public class Quincy
    {
        #region Variables
        protected NativeWindow window;

        public bool isRunning = true;
        #endregion

        #region Methods
        public Quincy(string windowTitle, int windowResX, int windowResY)
        {
            window = NativeWindow.Create();
            
            window.Render += Render;
            window.Close += Closing;
            window.ContextDestroying += ContextDestroyed;

            window.Animation = false;
            window.CursorVisible = true;
            window.DepthBits = 24;
            window.SwapInterval = 0;

            window.Create(128, 128, (uint)windowResX + 16, (uint)windowResY + 16, NativeWindowStyles.Caption | NativeWindowStyles.Border);
            window.Caption = windowTitle;

            window.Show();
            window.Run();
        }

        public void RenderImGui() { }

        private void Render(object sender, NativeWindowEventArgs e)
        {
            Gl.ClearColor(100/255f, 149/255f, 237/255f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Close()
        {
            window.Stop();
            window.Destroy();
        }
        #endregion

        private void Closing(object sender, EventArgs e)
        {
            isRunning = false;
        }

        private void ContextDestroyed(object sender, NativeWindowEventArgs e)
        {
            isRunning = false;
        }
    }
}
