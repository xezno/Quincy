namespace Quincy
{
    class Scene
    {
        private Model testModel;
        private Shader shader;
        private Camera camera;

        public Scene()
        {
            testModel = new Model("Content/Models/vtech/scene.gltf");
            shader = new Shader("Content/Shaders/frag.glsl", "Content/Shaders/vert.glsl");
            camera = new Camera();
        }

        public void Render()
        {
            camera.Render();
            testModel.Draw(camera, shader);
        }
    }
}
