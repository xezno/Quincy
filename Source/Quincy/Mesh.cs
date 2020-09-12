using OpenGL;
using System;
using System.Collections.Generic;

namespace Quincy
{
    class Mesh
    {
        public List<Vertex> Vertices { get; set; }
        public List<uint> Indices { get; set; }
        public List<Texture> Textures { get; set; }

        private uint vao, vbo, ebo;

        private int rotation;
        private int Rotation { get => rotation; set => rotation = value % 360; }

        public Matrix4x4f ModelMatrix;

        public Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures, Matrix4x4f modelMatrix)
        {
            Vertices = vertices;
            Indices = indices;
            Textures = textures;
            ModelMatrix = modelMatrix;

            SetupMesh();
        }

        private void SetupMesh()
        {
            var vertexStructSize = 8 * sizeof(float);

            vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            vbo = Gl.GenBuffer();
            ebo = Gl.GenBuffer();

            var glVertices = new List<float>();
            foreach (var vertex in Vertices)
            {
                glVertices.AddRange(new[] { 
                    vertex.Position.x,
                    vertex.Position.y,
                    vertex.Position.z,

                    vertex.Normal.x,
                    vertex.Normal.y,
                    vertex.Normal.z,

                    vertex.TexCoords.x,
                    vertex.TexCoords.y
                });
            }
            
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)glVertices.Count * sizeof(float), glVertices.ToArray(), BufferUsage.StaticDraw);

            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)0);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(3 * sizeof(float)));

            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 2, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(6 * sizeof(float)));

            Gl.BindVertexArray(0);
        }

        public void Draw(Camera camera, Shader shader, Light light)
        {
            uint diffuseCount = 0, specularCount = 0;

            shader.Use();

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

                shader.SetInt($"material.{name}{number}", i);
                Gl.BindTexture(TextureTarget.Texture2d, texture.Id);
            }

            var tmpModelMatrix = ModelMatrix;
            tmpModelMatrix.RotateY(Rotation++);

            shader.SetMatrix("projectionMatrix", camera.ProjMatrix);
            shader.SetMatrix("viewMatrix", camera.ViewMatrix);
            shader.SetMatrix("modelMatrix", tmpModelMatrix);
            
            shader.SetMatrix("lightProjectionMatrix", light.ProjMatrix);
            shader.SetMatrix("lightViewMatrix", light.ViewMatrix);
            
            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count);
            Gl.BindTexture(TextureTarget.Texture2d, light.ShadowMap.DepthMap);
            shader.SetInt("shadowMap", Textures.Count);

            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.BindVertexArray(vao);
            Gl.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindVertexArray(0);
        }

        public void DrawShadows(Light light, Shader depthShader)
        {
            depthShader.Use();

            var tmpModelMatrix = ModelMatrix;
            tmpModelMatrix.RotateY(Rotation++);

            depthShader.SetMatrix("projectionMatrix", light.ProjMatrix);
            depthShader.SetMatrix("viewMatrix", light.ViewMatrix);
            depthShader.SetMatrix("modelMatrix", tmpModelMatrix);

            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.BindVertexArray(vao);
            Gl.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindVertexArray(0);
        }
    }
}
