using OpenGL;
using Quincy.DebugUtils;
using StbiSharp;
using System.IO;
using System.Runtime.InteropServices;

namespace Quincy
{
    struct HdriTexture
    {
        public uint Id { get; }

        public HdriTexture(uint id)
        {
            Id = id;
        }

        public static HdriTexture LoadFromFile(string filePath)
        {
            var fileData = File.ReadAllBytes(filePath);
            var image = Stbi.LoadFromMemory(fileData, 3); // TODO: stb library may need patching to load floating-point

            var textureDataPtr = Marshal.AllocHGlobal(image.Data.Length);
            Marshal.Copy(image.Data.ToArray(), 0, textureDataPtr, image.Data.Length);

            uint texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texturePtr);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgb16f, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte /* hmm */, textureDataPtr);

            Marshal.FreeHGlobal(textureDataPtr);

            Logging.Log($"Loaded cubemap texture {filePath}, ptr {texturePtr}");

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            image.Dispose();

            return new HdriTexture(texturePtr);
        }
    }
}
