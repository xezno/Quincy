using OpenGL;
using Quincy.MathUtils;
using System;

namespace Quincy
{
    class Camera
    {
        public Vector3f Position { get; set; } = new Vector3f(0, 7.5f, 10f);
        public Vector3f Rotation { get; set; }

        private float angle;

        public float FieldOfView { get; set; } = 70f;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 2500f;

        private Matrix4x4f viewMatrix;
        public Matrix4x4f ViewMatrix { get => viewMatrix; set => viewMatrix = value; }

        private Matrix4x4f projMatrix;
        public Matrix4x4f ProjMatrix { get => projMatrix; set => projMatrix = value; }

        public Camera()
        {
            ProjMatrix = Matrix4x4f.Perspective(FieldOfView,
                1280f / 720f,
                NearPlane,
                FarPlane);
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
        { }

        public void Update(float deltaTime)
        {
            viewMatrix = Matrix4x4f.Identity;

            var position = new Vertex3f();
            position.x = MathF.Cos(angle) * 10f;
            position.y = 10f;
            position.z = MathF.Sin(angle) * 10f;

            angle += deltaTime;
            angle %= 360;

            viewMatrix *= Matrix4x4f.LookAt(position, new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 1f, 0f));
        }
    }
}
