using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Quincy
{
    [StructLayout(LayoutKind.Sequential)]
    struct Texture
    {
        public uint Id { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
    }

    class TextureContainer
    {
        public static List<Texture> Textures { get; } = new List<Texture>();
    }
}
