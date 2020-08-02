using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace heightmap
{
    class Program : GameWindow
    {
        // We need an instance of the new camera class so it can manage the view and projection matrix code
        // We also need a boolean set to true to detect whether or not the mouse has been moved for the first time
        // Finally we add the last position of the mouse so we can calculate the mouse offset easily
        public static Camera camera;

        private bool _firstMove = true;
        private Vector2 _lastPos;

        Terrain terrain;

        public Program()
            : base(800, 600, GraphicsMode.Default, "emptyTK")
        {
        }

        static void Main(string[] args)
        {
            using (Program program = new Program())
            {
                program.Run(60.0);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // We initialize the camera so that it is 3 units back from where the rectangle is
            // and give it the proper aspect ratio

           terrain = new Terrain(new FileInfo("./heightmap.png"));
            camera = new Camera(new Vector3(256, terrain.getHeightAtPosition(256, 256), 256), Width / (float)Height);

            // We make the mouse cursor invisible so we can have proper FPS-camera movement
            CursorVisible = false;

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrain.load();

            var transform = Matrix4.Identity;
            terrain.render(e, transform);

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            const float cameraSpeed = 10f;
            const float sensitivity = 0.2f;

            Vector3 front = camera.Front;
            front.Y = 0;
            if (input.IsKeyDown(Key.W))
            {
                camera.Position += front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Key.S))
            {
                camera.Position -= front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Key.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Key.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Key.Space))
            {
                camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Key.LShift))
            {
                camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = Mouse.GetState();

            if (_firstMove) // this bool variable is initially set to true
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused) // check to see if the window is focused
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            camera.AspectRatio = Width / (float)Height;
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            terrain.destroy(e);

            base.OnUnload(e);
        }
    }
}
