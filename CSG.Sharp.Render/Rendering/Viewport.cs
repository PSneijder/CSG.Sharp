using CSG.Sharp.Rendering;
using CSG.Sharp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CSG.Sharp
{
    internal sealed class Viewport
        : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private BasicEffect _effect;

        private readonly List<VertexPositionColor> _vertices = new List<VertexPositionColor>();
        private readonly List<short> _indices = new List<short>();

        private IndexBuffer _indexBuffer;
        private VertexBuffer _vertexBuffer;

        private static short indexId = 0;

        private readonly Camera _camera;

        public Viewport()
        {
            Content = new ContentManager(Services, "Content");

            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false
            };

            _camera = new Camera(this);

            Components.Add(_camera);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true
            };

            _camera.EyeHeightStanding = 20.0f;
            _camera.Acceleration = new Vector3(800.0f, 800.0f, 800.0f);
            _camera.VelocityWalking = new Vector3(200.0f, 200.0f, 200.0f);
            _camera.VelocityRunning = _camera.VelocityWalking * 2.0f;

            int width = GraphicsDevice.DisplayMode.Width / 2;
            int height = GraphicsDevice.DisplayMode.Height / 2;
            float aspectRatio = width / height;

            _camera.Perspective(90.0f, aspectRatio, 0.01f, 1000.0f);

            /*CSG cube = Cube.Create(new Vector(0, 0, -20), 10);
            CSG sphere = Sphere.Create(new Vector(30, 0, -20), 10);
            CSG cylinder = Cylinder.Create(new Vector(60, -10, -20), new Vector(60, 10, -20), 10);

            AddToVertices(cube.ToPolygons());
            AddToVertices(sphere.ToPolygons());
            AddToVertices(cylinder.ToPolygons());*/

            var cube = Cube.Create(radius: 10);
            var sphere = Sphere.Create(radius: 13);
            var polygons = cube.Subtract(sphere).ToPolygons();

            AddToVertices(polygons.ToArray());
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.M))
                _camera.EnableMouseSmoothing = !_camera.EnableMouseSmoothing;

            if (keyboardState.IsKeyDown(Keys.Add))
            {
                _camera.RotationSpeed += 0.01f;

                if (_camera.RotationSpeed > 1.0f)
                    _camera.RotationSpeed = 1.0f;
            }

            if (keyboardState.IsKeyDown(Keys.Subtract))
            {
                _camera.RotationSpeed -= 0.01f;

                if (_camera.RotationSpeed <= 0.0f)
                    _camera.RotationSpeed = 0.01f;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
                return;

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            GraphicsDevice.Indices = _indexBuffer;

            _effect.Parameters["World"].SetValue(Matrix.Identity);
            _effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Identity);
            _effect.Parameters["WorldViewProj"].SetValue(_camera.ViewMatrix * _camera.ProjectionMatrix);
            _effect.Parameters["EyePosition"].SetValue(_camera.Position);

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices.ToArray(), 0, _vertices.Count / 3, VertexPositionColor.VertexDeclaration);
            }

            base.Draw(gameTime);
        }

        private VertexPositionColor[] CreateTriangle(Polygon polygon, out short[] indices, int count)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[3];
            indices = new short[3];

            Color color = ColorUtils.GenerateRandomColor(Color.Wheat);

            vertices[0] = new VertexPositionColor(new Vector3((float)polygon.Vertices[0].Pos.x, (float)polygon.Vertices[0].Pos.y, (float)polygon.Vertices[0].Pos.z), color);
            vertices[1] = new VertexPositionColor(new Vector3((float)polygon.Vertices[1].Pos.x, (float)polygon.Vertices[1].Pos.y, (float)polygon.Vertices[1].Pos.z), color);
            vertices[2] = new VertexPositionColor(new Vector3((float)polygon.Vertices[2].Pos.x, (float)polygon.Vertices[2].Pos.y, (float)polygon.Vertices[2].Pos.z), color);

            indices[0] = (short)(indexId + 0);
            indices[1] = (short)(indexId + 1);
            indices[2] = (short)(indexId + 2);

            indexId = (short)(indexId + 3);

            return vertices;
        }

        private VertexPositionColor[] CreateRectangle(Polygon polygon, out short[] indices, int count)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[6];
            indices = new short[6];

            Color color = ColorUtils.GenerateRandomColor(Color.Wheat);

            vertices[0] = new VertexPositionColor(new Vector3((float)polygon.Vertices[1].Pos.x, (float)polygon.Vertices[1].Pos.y, (float)polygon.Vertices[1].Pos.z), color);
            vertices[1] = new VertexPositionColor(new Vector3((float)polygon.Vertices[2].Pos.x, (float)polygon.Vertices[2].Pos.y, (float)polygon.Vertices[2].Pos.z), color);
            vertices[2] = new VertexPositionColor(new Vector3((float)polygon.Vertices[3].Pos.x, (float)polygon.Vertices[3].Pos.y, (float)polygon.Vertices[3].Pos.z), color);
            vertices[3] = new VertexPositionColor(new Vector3((float)polygon.Vertices[0].Pos.x, (float)polygon.Vertices[0].Pos.y, (float)polygon.Vertices[0].Pos.z), color);
            vertices[4] = new VertexPositionColor(new Vector3((float)polygon.Vertices[1].Pos.x, (float)polygon.Vertices[1].Pos.y, (float)polygon.Vertices[1].Pos.z), color);
            vertices[5] = new VertexPositionColor(new Vector3((float)polygon.Vertices[3].Pos.x, (float)polygon.Vertices[3].Pos.y, (float)polygon.Vertices[3].Pos.z), color);

            indices[0] = (short)(indexId + 1);
            indices[1] = (short)(indexId + 2);
            indices[2] = (short)(indexId + 3);
            indices[3] = (short)(indexId + 0);
            indices[4] = (short)(indexId + 1);
            indices[5] = (short)(indexId + 3);

            indexId = (short)(indexId + 6);

            return vertices;
        }

        private VertexPositionColor[] Create(Polygon polygon, out short[] indices, int count)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[polygon.Vertices.Length];
            indices = new short[polygon.Vertices.Length];

            Color color = ColorUtils.GenerateRandomColor(Color.Wheat);

            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                vertices[i] = new VertexPositionColor(new Vector3((float)polygon.Vertices[1].Pos.x, (float)polygon.Vertices[1].Pos.y, (float)polygon.Vertices[1].Pos.z), color);
                indices[i] = (short)(indexId + 1);
            }

            return vertices;
        }

        private void AddToVertices(Polygon[] polygons)
        {
            var vertices = new List<VertexPositionColor>();
            var indices = new List<short>();

            foreach (var polygon in polygons)
            {
                var polygonVertices = new VertexPositionColor[0];
                var polygonIndices = new short[0];

                switch (polygon.Vertices.Length)
                {
                    case 3:
                        polygonVertices = CreateTriangle(polygon, out polygonIndices, vertices.Count);
                        break;
                    case 4:
                        polygonVertices = CreateRectangle(polygon, out polygonIndices, vertices.Count);
                        break;
                    default:
                        Debug.WriteLine("WARNING: {0}", polygon.Vertices.Length);
                        break;
                }

                vertices.AddRange(polygonVertices);
                indices.AddRange(polygonIndices);
            }

            _vertices.AddRange(vertices);
            _indices.AddRange(indices);

            _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), _vertices.Count, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(_vertices.ToArray());

            _indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), _indices.Count, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices.ToArray());
        }
    }
}