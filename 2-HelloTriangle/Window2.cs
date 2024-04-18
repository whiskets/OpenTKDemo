using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;
using System;

namespace LearnOpenTK
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    struct TestVertex
    {
        [FieldOffset(0)]
        public Vector3 Vertieces;
        [FieldOffset(12)]
        public int Id;
    }

    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Window : GameWindow
    {
       
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;

       float[] floorVertices = new[]
       {
            -0.5f,   0.5f,  -0.0f,
             0.5f,    0.5f, -0.0f,
            -0.5f,  -0.5f, -0.0f,

            0.5f,    0.5f,  -0.0f,
            0.5f,   -0.5f,-0.0f,
            -0.5f,   -0.5f, -0.0f,
        };

        TestVertex[] _vertices = new TestVertex[6];
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        // Now, we start initializing OpenGL.
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            for(int i =0;i< floorVertices.Length;i+=3)
            {
                TestVertex a =new TestVertex();
                a.Vertieces = new Vector3(floorVertices[i], floorVertices[i + 1], floorVertices[i + 2]);
                a.Id = i / 9 + 1;            
                _vertices[i/3] = a;
            }
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            // Now, enable the shader.
            // Just like the VBO, this is global, so every function that uses a shader will modify this one until a new one is bound instead.
            _shader.Use();
            _vertexBufferObject = GL.GenBuffer();    
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            unsafe
            {
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(TestVertex), _vertices, BufferUsageHint.StaticDraw);
               // GL.BufferData(BufferTarget.ArrayBuffer, floorVertices.Length * sizeof(float), floorVertices, BufferUsageHint.StaticDraw);
                _vertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(_vertexArrayObject);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(TestVertex), 0);
                // GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float)*3, 0);
                GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, sizeof(TestVertex), 12);
              
            }
            // Enable variable 0 in the shader.
            GL.EnableVertexAttribArray(0);

            // Setup is now complete! Now we move to the OnRenderFrame function to finally draw the triangle.
        }

        // Now that initialization is done, let's create our render loop.
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // This clears the image, using what you set as GL.ClearColor earlier.
            // OpenGL provides several different types of data that can be rendered.
            // You can clear multiple buffers by using multiple bit flags.
            // However, we only modify the color, so ColorBufferBit is all we need to clear.
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // To draw an object in OpenGL, it's typically as simple as binding your shader,
            // setting shader uniforms (not done here, will be shown in a future tutorial)
            // binding the VAO,
            // and then calling an OpenGL function to render.

            // Bind the shader
            _shader.Use();

            // Bind the VAO
            GL.BindVertexArray(_vertexArrayObject);

            // And then call our drawing function.
            // For this tutorial, we'll use GL.DrawArrays, which is a very simple rendering function.
            // Arguments:
            //   Primitive type; What sort of geometric primitive the vertices represent.
            //     OpenGL used to support many different primitive types, but almost all of the ones still supported
            //     is some variant of a triangle. Since we just want a single triangle, we use Triangles.
            //   Starting index; this is just the start of the data you want to draw. 0 here.
            //   How many vertices you want to draw. 3 for a triangle.
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            // OpenTK windows are what's known as "double-buffered". In essence, the window manages two buffers.
            // One is rendered to while the other is currently displayed by the window.
            // This avoids screen tearing, a visual artifact that can happen if the buffer is modified while being displayed.
            // After drawing, call this function to swap the buffers. If you don't, it won't display what you've rendered.
            SwapBuffers();

            // And that's all you have to do for rendering! You should now see a yellow triangle on a black screen.
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // When the window gets resized, we have to call GL.Viewport to resize OpenGL's viewport to match the new size.
            // If we don't, the NDC will no longer be correct.
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        // Now, for cleanup.
        // You should generally not do cleanup of opengl resources when exiting an application,
        // as that is handled by the driver and operating system when the application exits.
        // 
        // There are reasons to delete opengl resources, but exiting the application is not one of them.
        // This is provided here as a reference on how resource cleanup is done in opengl, but
        // should not be done when exiting the application.
        //
        // Places where cleanup is appropriate would be: to delete textures that are no
        // longer used for whatever reason (e.g. a new scene is loaded that doesn't use a texture).
        // This would free up video ram (VRAM) that can be used for new textures.
        //
        // The coming chapters will not have this code.
        protected override void OnUnload()
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);

            base.OnUnload();
        }
    }
}
