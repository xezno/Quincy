using Assimp;
using Assimp.Unmanaged;
using OpenGL;
using Quincy.DebugUtils;
using Quincy.MathUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Quincy
{
    class Model
    {
        private List<Mesh> meshes = new List<Mesh>();
        private string directory;

        public Model(string path)
        {
            LoadModel(path);
        }

        public void Draw(Camera camera, Shader shader)
        {
            foreach (var mesh in meshes)
            {
                mesh.Draw(camera, shader);
            }
        }

        private void LoadModel(string path)
        {
            var importer = new AssimpContext();
            var scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.PreTransformVertices);

            directory = Path.GetDirectoryName(path);

            ProcessNode(scene.RootNode, scene);
        }

        private void ProcessNode(Node node, Assimp.Scene scene)
        {
            for (int i = 0; i < node.MeshCount; ++i)
            {
                var mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, scene, node.Transform));
            }

            foreach (var child in node.Children)
            {
                ProcessNode(child, scene);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Assimp.Scene scene, Matrix4x4 transform)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < mesh.VertexCount; ++i)
            {
                var vertex = new Vertex()
                {
                    Position = new Vector3f(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z),
                    Normal = new Vector3f(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z)
                };

                if (mesh.HasTextureCoords(0))
                {
                    var texCoords = new Vector2f(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y);
                    vertex.TexCoords = texCoords;
                }
                else
                {
                    vertex.TexCoords = new Vector2f(0, 0);
                }

                vertices.Add(vertex);
            }

            for (int i = 0; i < mesh.FaceCount; ++i)
            {
                var face = mesh.Faces[i];
                for (int f = 0; f < face.IndexCount; ++f)
                {
                    indices.Add((uint)face.Indices[f]);
                }
            }

            if (mesh.MaterialIndex >= 0)
            {
                var material = scene.Materials[mesh.MaterialIndex];
                var diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
                textures.AddRange(diffuseMaps);
                
                var specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
                textures.AddRange(specularMaps);
            }

            var oglTransform = new Matrix4x4f(
                transform.A1, transform.A2, transform.A3, transform.A4,
                transform.B1, transform.B2, transform.B3, transform.B4,
                transform.C1, transform.C2, transform.C3, transform.C4,
                transform.D1, transform.D2, transform.D3, transform.D4
            );

            return new Mesh(vertices, indices, textures, oglTransform);
        }

        List<Texture> LoadMaterialTextures(Material material, TextureType textureType, string typeName)
        {
            var textures = new List<Texture>();

            for (int i = 0; i < material.GetMaterialTextureCount(textureType); ++i)
            {
                material.GetMaterialTexture(textureType, i, out var textureSlot);

                if (string.IsNullOrEmpty(textureSlot.FilePath))
                    continue;

                var texture = new Texture()
                {
                    Id = TextureFromFile(textureSlot.FilePath, directory),
                    Type = typeName,
                    Path = $"{directory}/{textureSlot.FilePath}"
                };

                // Add to texture container so that we don't reload it later
                TextureContainer.Textures.Add(texture);
                textures.Add(texture);
            }

            return textures;
        }

        private uint TextureFromFile(string fileName, string directory)
        {
            // Check if already loaded
            if (TextureContainer.Textures.Any(t => t.Path == $"{directory}/{fileName}"))
            {
                // Already loaded, we'll just use that
                return TextureContainer.Textures.First(t => t.Path == $"{directory}/{fileName}").Id;
            }

            // Not loaded, load from scratch
            var texturePtr = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texturePtr);
            using var textureStream = new MemoryStream();
            var image = Image.FromFile($"{directory}/{fileName}");

            var imageFormat = OpenGL.PixelFormat.Bgra;
            if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ||
                image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                imageFormat = OpenGL.PixelFormat.Bgr;

            image.Save(textureStream, ImageFormat.Bmp);

            var textureData = new byte[textureStream.Length];
            textureStream.Read(textureData, 0, (int)textureStream.Length);

            var textureDataPtr = Marshal.AllocHGlobal(textureData.Length);
            Marshal.Copy(textureData, 0, textureDataPtr, textureData.Length);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height - 1/* fixes black pixel row*/, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            image.Dispose();
            Marshal.FreeHGlobal(textureDataPtr);

            Logging.Log($"Loaded texture {fileName} ({directory}), ptr {texturePtr}");
            Gl.BindTexture(TextureTarget.Texture2d, 0);

            return texturePtr;
        }
    }
}
