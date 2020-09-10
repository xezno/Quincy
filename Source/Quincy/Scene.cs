namespace Quincy
{
    class Scene
    {
        private Model testModel;
        private Shader shader;
        private Camera camera;

        public Scene()
        {
            testModel = new Model("vtech/scene.gltf");
            shader = new Shader("shaders/frag.glsl", "shaders/vert.glsl");
            camera = new Camera();
        }

        public void Render()
        {
            shader.Use();
            testModel.Draw(shader);
        }
    }
}
