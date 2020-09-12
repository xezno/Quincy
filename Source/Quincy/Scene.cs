using OpenGL;
using Quincy.MathUtils;
using System;

namespace Quincy
{
    class Scene
    {
        private Model testModel;
        private Shader shader, depthShader;
        private Camera camera;
        private Light light;

        private DateTime lastUpdate;

        public Scene()
        {
            testModel = new Model("Content/Models/mcrn_tachi/scene.gltf");
            shader = new Shader("Content/Shaders/PBR/pbr.frag", "Content/Shaders/PBR/pbr.vert");
            depthShader = new Shader("Content/Shaders/Depth/depth.frag", "Content/Shaders/Depth/depth.vert");
            camera = new Camera();
            light = new Light(position: new Vector3f(-10f, 10f, -4f));
        }

        public void Render()
        {
            Update();

            Gl.Viewport(0, 0, 1280, 720);
            // Gl.ClearColor(100/255f, 149/255f, 237/255f, 1.0f);
            Gl.ClearColor(1f, 1f, 1f, 1f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            camera.Render();
            testModel.Draw(camera, shader, light);
        }

        public void RenderShadows()
        {
            Gl.Viewport(0, 0, (int)light.ShadowMap.Resolution.x, (int)light.ShadowMap.Resolution.y);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, light.ShadowMap.DepthMapFbo);
            Gl.Clear(ClearBufferMask.DepthBufferBit);
            
            light.Render();
            testModel.DrawShadows(light, depthShader);
            
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Update()
        {
            float deltaTime = (float)(DateTime.Now - lastUpdate).TotalSeconds;
            testModel.Update(deltaTime);
            camera.Update(deltaTime);

            lastUpdate = DateTime.Now;
        }
    }
}
