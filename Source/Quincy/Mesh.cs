using OpenGL;
using Quincy.MathUtils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Quincy
{
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct Vertex
    {
        public Vector3f Position { get; set; }
        public Vector3f Normal { get; set; }
        public Vector2f TexCoords { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Texture
    {
        public uint Id { get; set; }
        public string Type { get; set; }
    }

    class Mesh
    {
        public List<Vertex> Vertices { get; set; }
        public List<uint> Indices { get; set; }
        public List<Texture> Textures { get; set; }

        private uint vao, vbo, ebo;

        public Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures)
        {
            Vertices = vertices;
            Indices = indices;
            Textures = textures;
        }

        private void SetupMesh()
        {
            var vertexStructSize = Marshal.SizeOf(typeof(Vertex));

            vao = Gl.GenVertexArray();
            vbo = Gl.GenBuffer();
            ebo = Gl.GenBuffer();

            Gl.BindVertexArray(vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)Vertices.Count * (uint)vertexStructSize, Vertices.ToArray(), BufferUsage.StaticDraw);

            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)0);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(3 * sizeof(float)));

            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 2, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(5 * sizeof(float)));

            Gl.BindVertexArray(0);
        }

        public void Draw(Shader shader)
        {
            uint diffuseCount = 0, specularCount = 0;

            for (int i = 0; i < Textures.Count; ++i)
            {
                var texture = Textures[i];

                Gl.ActiveTexture(TextureUnit.Texture0 + i);

                string number = "";
                string name = texture.Type;

                // TODO: Make type an enum
                if (name == "texture_diffuse")
                {
                    number = (++diffuseCount).ToString();
                }
                else if (name == "texture_specular")
                {
                    number = (++specularCount).ToString();
                }

                shader.SetFloat($"material.{name}{number}", i);
                Gl.BindTexture(TextureTarget.Texture2d, texture.Id);
            }
        }
    }
}
