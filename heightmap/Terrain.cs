using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace heightmap
{
    class Terrain
    {
        private readonly float[] _vertices;

        private Shader _shader;

        private int _vertexBufferObject;
        private Bitmap heightmap;

        public Terrain(FileInfo heightmapFile)
        {
            heightmap = new Bitmap(heightmapFile.FullName);
            _vertices = new float[heightmap.Width * heightmap.Height * 3 * 3 * 2];

            int a = 0;
            for(int i = 0;i < heightmap.Width - 1;i++)
            {
                for (int j = 0; j < heightmap.Width - 1; j++)
                {
                    Color c = heightmap.GetPixel(i, j);
                    _vertices[a++] = i;
                    _vertices[a++] = c.R / 4;
                    _vertices[a++] = j;

                    c = heightmap.GetPixel(i + 1, j);
                    _vertices[a++] = i + 1;
                    _vertices[a++] = c.R / 4;
                    _vertices[a++] = j;
                    
                    c = heightmap.GetPixel(i, j + 1);
                    _vertices[a++] = i;
                    _vertices[a++] = c.R /4;
                    _vertices[a++] = j + 1;

                    c = heightmap.GetPixel(i + 1, j + 1);
                    _vertices[a++] = i + 1;
                    _vertices[a++] = c.R / 4;
                    _vertices[a++] = j + 1;

                    c = heightmap.GetPixel(i + 1, j);
                    _vertices[a++] = i + 1;
                    _vertices[a++] = c.R / 4;
                    _vertices[a++] = j;

                    c = heightmap.GetPixel(i, j + 1);
                    _vertices[a++] = i;
                    _vertices[a++] = c.R / 4;
                    _vertices[a++] = j + 1;
                }
            }

            _vertexBufferObject = GL.GenBuffer();

            _shader = new Shader("shader.vert", "shader.frag");
        }

        public float getHeightAtPosition(int x, int y)
        {
            return heightmap.GetPixel(x, y).R / 4;
        }

        public void load()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _shader.Use();

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public void render(FrameEventArgs e, Matrix4 model)
        {
            _shader.SetMatrix4("model", model);

            _shader.Use();

            _shader.SetMatrix4("view", Program.camera.GetViewMatrix());
            _shader.SetMatrix4("projection", Program.camera.GetProjectionMatrix());

            _shader.SetMatrix4("model", model);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Length);
        }

        public void destroy(EventArgs e)
        {
            GL.DeleteProgram(_shader.Handle);
            GL.DeleteBuffer(_vertexBufferObject);
        }
    }
}
