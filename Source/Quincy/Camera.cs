using OpenGL;
using Quincy.MathUtils;
using System;

namespace Quincy
{
    class Camera
    {
        public Vector3f Position { get; set; }
        public Vector3f Rotation { get; set; }

        public float FieldOfView { get; set; } = 90f;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 2500f;

        private Matrix4x4f viewMatrix;
        public Matrix4x4f ViewMatrix { get => viewMatrix; set => viewMatrix = value; }

        private Matrix4x4f projMatrix;
        public Matrix4x4f ProjMatrix { get => projMatrix; set => projMatrix = value; }

        public Camera()
        {
            ProjMatrix = CreateInfReversedZProj(FieldOfView,
                1280f / 720f,
                NearPlane);
        }

        private Matrix4x4f CreateInfReversedZProj(float fov, float aspectRatio, float nearPlane)
        {
            float f = 1.0f / (float)Math.Tan(Angle.ToRadians(fov) / 2.0f);
            return new Matrix4x4f(f / aspectRatio, 0f, 0f, 0f,
                0f, f, 0f, 0f,
                0f, 0f, 0f, -1f,
                0f, 0f, nearPlane, 0f);
        }

        public void Render()
        {
            viewMatrix = Matrix4x4f.Identity;
            viewMatrix.RotateX(Rotation.x);
            viewMatrix.RotateY(Rotation.y);
            viewMatrix.RotateZ(Rotation.z);
            viewMatrix *= Matrix4x4f.LookAtDirection(new Vertex3f(Position.x, Position.y, Position.z), new Vertex3f(0f, 0f, -1f), new Vertex3f(0f, 1f, 0f));
        }
    }
}
