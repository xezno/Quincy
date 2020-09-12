using OpenGL;
using Quincy.MathUtils;
using System.Runtime.InteropServices;

namespace Quincy
{
    [StructLayout(LayoutKind.Sequential)]
    struct Light
    {
        public Light(Vector3f position)
        {
            Position = position;
            ProjMatrix = Matrix4x4f.Ortho(-10f, 10f, -10f, 10f, 1.0f, 20f);
            ViewMatrix = Matrix4x4f.Identity;
            ShadowMap = new ShadowMap(1024, 1024);
        }

        public Vector3f Position { get; set; }
        public Matrix4x4f ViewMatrix { get; set; }
        public Matrix4x4f ProjMatrix { get; set; }
        public ShadowMap ShadowMap { get; set; }

        public void Render()
        {
            ViewMatrix = Matrix4x4f.LookAt(new Vertex3f(Position.x, Position.y, Position.z), new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 1f, 0f));
        }
    }
}
