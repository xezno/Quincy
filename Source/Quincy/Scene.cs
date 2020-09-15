using OpenGL;
using Quincy.MathUtils;
using Quincy.Primitives;
using System;

namespace Quincy
{
    class Scene
    {
        private Model testModel;
        private Shader shader, depthShader;
        private Camera camera;
        private Light light;

        private Framebuffer mainFramebuffer;
        private Plane framebufferRenderPlane;
        private Shader framebufferRenderShader;

        private HdriTexture skyHdri;

        private DateTime lastUpdate;

        public Scene()
        {
            testModel = new Model("Content/Models/mcrn_tachi/scene.gltf");
            shader = new Shader("Content/Shaders/PBR/pbr.frag", "Content/Shaders/PBR/pbr.vert");
            depthShader = new Shader("Content/Shaders/Depth/depth.frag", "Content/Shaders/Depth/depth.vert");
            camera = new Camera(position: new Vector3f(0, 0f, 1f));
            light = new Light(position: new Vector3f(0, 10f, 0));

            skyHdri = HdriTexture.LoadFromFile("Content/HDRIs/gamrig_4k.hdr");
            
            framebufferRenderShader = new Shader("Content/Shaders/Framebuffer/framebuffer.frag", "Content/Shaders/Framebuffer/framebuffer.vert");
            framebufferRenderPlane = new Plane();
            mainFramebuffer = new Framebuffer();
        }

        public void Render()
        {
            Update();

            // Render scene to framebuffer
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, mainFramebuffer.Fbo);
            Gl.Viewport(0, 0, 1280, 720);
            Gl.ClearDepth(0.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.ZeroToOne);
            Gl.DepthFunc(DepthFunction.Greater);

            camera.Render();
            testModel.Draw(camera, shader, light);
            Gl.DepthFunc(DepthFunction.Less);
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.NegativeOneToOne);

            // Render framebuffer to screen

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.ClearDepth(1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            framebufferRenderShader.SetFloat("exposure", 1.0f);
            framebufferRenderPlane.Draw(framebufferRenderShader, mainFramebuffer.ColorTexture);
        }

        public void RenderShadows()
        {
            Gl.Disable(EnableCap.CullFace); //Gl.CullFace(CullFaceMode.Front);

            Gl.Viewport(0, 0, (int)light.ShadowMap.Resolution.x, (int)light.ShadowMap.Resolution.y);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, light.ShadowMap.DepthMapFbo);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            light.Render();
            testModel.DrawShadows(light, depthShader);

            Gl.Enable(EnableCap.CullFace); //Gl.CullFace(CullFaceMode.Back);
                        
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
