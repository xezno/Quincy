using System;
using System.Collections.Generic;
using System.Text;

namespace Quincy.Primitives
{
    class Cube
    {
        float[] cubeVertices = new[] {      
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
        };

        public List<Vertex> Vertices
        {
            get
            {
                List<Vertex> tmp = new List<Vertex>();

                for (int i = 0; i < cubeVertices.Length; i += 3)
                {
                    var x = cubeVertices[i];
                    var y = cubeVertices[i + 1];
                    var z = cubeVertices[i + 2];

                    tmp.Add(new Vertex()
                    {
                        Position = new MathUtils.Vector3f(x, y, z),

                        // TODO:
                        TexCoords = new MathUtils.Vector2f(0, 0),
                        BiTangent = new MathUtils.Vector3f(0, 0, 0),
                        Normal = new MathUtils.Vector3f(0, 0, 0),
                        Tangent = new MathUtils.Vector3f(0, 0, 0),
                    });
                }

                return tmp;
            }
        }
    }
}
