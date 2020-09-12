using OpenGL;
using System;

namespace Quincy
{
    class ShadowMap
    {
        public uint DepthMapFbo { get; private set; }
        public uint DepthMap { get; private set; }

        public ShadowMap(int width, int height)
        {
            DepthMap = Gl.GenTexture();
            DepthMapFbo = Gl.GenFramebuffer();

            Gl.BindTexture(TextureTarget.Texture2d, DepthMap);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.DepthComponent, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureBorderColor, new[] { 1.0f, 1.0f, 1.0f, 1.0f });
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, DepthMapFbo);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, DepthMap, 0);
            //Gl.DrawBuffer(0);
            //Gl.ReadBuffer(0);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
