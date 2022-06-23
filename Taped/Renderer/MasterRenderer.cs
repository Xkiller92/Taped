using Taped.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Taped.Renderer
{
    internal static class MasterRenderer
    {
        internal static Dictionary<string, Texture2d> textures = new Dictionary<string, Texture2d>();
        private static int VertexArrayObject;
        private static int vertexBufferObject;
        private static float[] vertices;
        private static int maxBoundTextures;
        private static Shader shader;
        private static List<int> indices = new List<int>();
        private static int elementBufferObject;
        private static int tCount = 0;
        private static int gCount = 0;
        private static int maxObjectsPerBatch;

        static void Loadtexture(string path)
        {
            Texture2d texture = new Texture2d(path);
            textures.Add(path, texture);
        }

        internal static Sprite GenerateSprite(string path)
        {
            try
            {
                Texture2d texture = textures[path];
            }
            catch (Exception)
            {
                Loadtexture(path);
            }

            return new Sprite(path);
        }

        internal static void LoadResources(int maxObjects)
        {
            maxObjectsPerBatch = maxObjects;
            //allocate the vertices array
            vertices = new float[maxObjectsPerBatch * 24];
            //generating buffers
            //VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            //IBO
            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);

            //generating indices and passing them to the GPU 
            for (int i = 0; i < maxObjectsPerBatch; i += 4)
            {
                indices.Add(i);
                indices.Add(i + 1);
                indices.Add(i + 3);
                indices.Add(i + 1);
                indices.Add(i + 2);
                indices.Add(i + 3);
            }
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.DynamicDraw);
            //VBO
            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

            //allocating the verts buffer space necessary for batching
            GL.BufferData(BufferTarget.ArrayBuffer, maxObjectsPerBatch * 24 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            //mapping the vertex data layout
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 6 * sizeof(float), 5 * sizeof(float));

            //generating shader
            shader = new Shader(@"Renderer\Shaders\shader.vert", @"Renderer\Shaders\shader.frag");
            shader.Use();

            //setting the sampler2D array size depending on what is supported on your device
            maxBoundTextures = GL.GetInteger(GetPName.MaxTextureImageUnits);
            int[] a = new int[maxBoundTextures];
            for (int i = 0; i < maxBoundTextures; i++)
            {
                a[i] = i;
            }
            shader.SetIntArray("t[0]", a);

        }

        internal static void Render(Vector2i windowSize, FrameEventArgs args)
        {
            for (int i = 0; i < SceneManager.objects.Count; i++)
            {
                Batch(SceneManager.objects[i], windowSize);
            }

            Flush(windowSize, new Vector3(0, 0, -1));
        }

        internal static void Batch(GameObject gameObject, Vector2i windowSize)
        {
            if (gameObject.animator.texturePath == null)
            {
                return;
            }

            gameObject.animator.animate();

            Texture2d texture = textures[gameObject.animator.texturePath];

            if (!texture.isUsed)
            {
                texture.Use(tCount);
                texture.isUsed = true;
                texture.posInVBO = tCount;
                tCount++;
            }

            //generate the vetices on the CPU side
            float[] verts = BuildVetices(gameObject.transform.position, 40, 40, -gameObject.transform.rotationInAngles, gameObject.transform.scaleX, gameObject.transform.scaleY, texture.posInVBO, gameObject.animator.textureCoordinatesX, gameObject.animator.textureCoordinatesY);
            int length = verts.Length;
            for (int i = length * gCount; i < length + (length * gCount); i++)
            {
                vertices[i] = verts[i - (gCount * 24)];
            }

            gCount++;

            //the flushing system
            if (tCount >= maxBoundTextures || gCount >= maxObjectsPerBatch)
            {
                foreach (var tex in textures)
                {
                    tex.Value.isUsed = false;
                }

                Flush(windowSize, new Vector3(0, 0, -1));
            }
        }

        internal static void Flush(Vector2i windowSize, Vector3 camPos)
        {
            float aspect = 1;
            aspect = (float)windowSize.Y / (float)windowSize.X;

            //vertex (aka object) transform
            Matrix4 model = Matrix4.CreateTranslation(0, 0, -1f);
            //camera transform (scene shift) [INVERTE THE POS]
            Matrix4 view = Matrix4.CreateTranslation(0, 0, -1f);
            //world coordinate matrix
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(-1000.0f, 1000.0f, -1000.0f * aspect, 1000.0f * aspect, 0.1f, 100.0f);

            //assigning the matrices to their prespective uniforms to the shader inside the GPU
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);
            //drawing the bound VBO
            GL.DrawElements(PrimitiveType.Triangles, maxObjectsPerBatch * 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
            gCount = 0;
            tCount = 0;
            //GL.BufferData(BufferTarget.ArrayBuffer, 4000000 * Vector3.SizeInBytes, IntPtr.Zero, BufferUsageHint.StreamDraw);
            //GL.ClearBufferSubData(BufferTarget.ArrayBuffer, PixelInternalFormat.Rgba, IntPtr.Zero,maxObjectsPerBatch * 24 * sizeof(floa), PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        }

        //generate the vertices of a quad
        static float[] BuildVetices(Vector3 Center, float w, float h, float angle, float scaleY, float scaleX, float textureIndex, Vector4 texcooX, Vector4 texcooY)
        {
            float oa = angle;
            angle = MathHelper.DegreesToRadians(angle);
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            float height = h / 2;
            float width = w / 2;

            float[] tri =
            {
                (Center.X + (width) + (height)) * scaleX, (Center.Y + (width) + (height)) * scaleY, 0, texcooX.X, texcooY.X, textureIndex, //top right
                (Center.X + (width) + (height)) * scaleX, (Center.Y - (width) - (height)) * scaleY, 0, texcooX.Y, texcooY.Y, textureIndex, //bot right
                (Center.X - (width) - (height)) * scaleX, (Center.Y - (width) - (height)) * scaleY, 0, texcooX.Z, texcooY.Z, textureIndex, //bot left
                (Center.X - (width) - (height)) * scaleX, (Center.Y + (width) + (height)) * scaleY, 0, texcooX.W, texcooY.W, textureIndex  //top left
            };

            if (oa % 360 != 0)
            {
                //calculates the rotation for each vertex coordinate
                for (int i = 0; i < tri.Length; i += 6)
                {
                    float x = tri[i];
                    float y = tri[i + 1];

                    tri[i] = cos * (x - Center.X) - sin * (y - Center.Y) + Center.X;
                    tri[i + 1] = sin * (x - Center.X) + cos * (y - Center.Y) + Center.Y;
                }
            }

            return tri;
        }

    }

    internal class Sprite
    {
        public string path;
        public Sprite(string texPath)
        {
            path = texPath;
        }
    }
}
