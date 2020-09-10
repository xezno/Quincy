using OpenGL;
using System.IO;

namespace Quincy
{
    class Shader
    {
        public uint Id { get; set; }

        public Shader(string fragGlslPath, string vertGlslPath)
        {
            var fragGlslContents = File.ReadAllText(fragGlslPath);
            var vertGlslContents = File.ReadAllText(vertGlslPath);

            var fragId = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragId, new[] { fragGlslContents });
            Gl.CompileShader(fragId);
            
            var vertId = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertId, new[] { vertGlslContents });
            Gl.CompileShader(vertId);

            Id = Gl.CreateProgram();
            Gl.AttachShader(Id, fragId);
            Gl.AttachShader(Id, vertId);
            Gl.LinkProgram(Id);

            Gl.DeleteShader(fragId);
            Gl.DeleteShader(vertId);
        }

        public void Use()
        {
            Gl.UseProgram(Id);
        }

        public void SetFloat(string name, float value)
        {
            var loc = Gl.GetUniformLocation(Id, name);
            if (loc < 0)
            {
                throw new System.Exception($"No variable {name}");
            }
            Gl.Uniform1(loc, value);
        }
    }
}