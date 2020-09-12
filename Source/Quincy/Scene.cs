using OpenGL;
using Quincy.MathUtils;

namespace Quincy
{
    class Scene
    {
        private Model testModel;
        private Shader shader, depthShader;
        private Camera camera;
        private Light light;

        public Scene()
        {
            testModel = new Model("Content/Models/vtech/scene.gltf");
            shader = new Shader("Content/Shaders/frag.glsl", "Content/Shaders/vert.glsl");
            depthShader = new Shader("Content/Shaders/Depth/depth.frag", "Content/Shaders/Depth/depth.vert");
            camera = new Camera();
            light = new Light(position: new Vector3f(-5f, 5f, 0f));
        }

        public void Render()
        {
            Gl.Viewport(0, 0, 1280, 720);
            Gl.ClearColor(100/255f, 149/255f, 237/255f, 1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            camera.Render();
            testModel.Draw(camera, shader, light);
        }

        public void RenderShadows()
        {
            Gl.Viewport(0, 0, 1024, 1024);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, light.ShadowMap.DepthMapFbo);
            Gl.Clear(ClearBufferMask.DepthBufferBit);
            
            light.Render();
            testModel.DrawShadows(light, depthShader);
            
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
