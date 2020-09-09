using OpenGL.CoreUI;
using Quincy.MathUtils;
using System;
using System.Collections.Generic;
using System.Threading;

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

            window.Create(0, 0, (uint)windowResX + 16, (uint)windowResY + 16, NativeWindowStyles.Caption | NativeWindowStyles.Border);
            window.Caption = windowTitle;

            window.Show();
            window.Run();
        }

        public void RenderImGui() { }

        private void Render(object sender, NativeWindowEventArgs e)
        {
            // TODO
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
