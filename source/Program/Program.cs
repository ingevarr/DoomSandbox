using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

using Doom.DataPrimitives;
using Doom.GameMap;
using Doom.WadFile;

using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace Doom
{
    public static class Program
    {
        public static void Main()
        {
            var wad = Wad.FromFile("DOOM.WAD");
            var mapLoader = new MapLoader(wad);

            var map = mapLoader.LoadByName("E1M1");

            var windowCreateInfo = new WindowCreateInfo
                                   {
                                       X = 100,
                                       Y = 200,
                                       WindowWidth = 1280,
                                       WindowHeight = 800,
                                       WindowTitle = "Doom",
                                   };

            var window = VeldridStartup.CreateWindow(ref windowCreateInfo);
            var graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, GraphicsBackend.OpenGL);

            var resourceFactory = graphicsDevice.ResourceFactory;
            var vertexBuffer = resourceFactory.CreateBuffer(new BufferDescription((uint) (map.VertexesCount * Unsafe.SizeOf<Vertex>()), BufferUsage.VertexBuffer));
            graphicsDevice.UpdateBuffer(vertexBuffer, 0, map.Vertexes.Data);

            var orthographicOffCenter 
                = Matrix4x4.CreateOrthographicOffCenter(map.Vertexes.Left, 
                                                        map.Vertexes.Right, 
                                                        map.Vertexes.Bottom, 
                                                        map.Vertexes.Top, 
                                                        0.1f, 
                                                        100f);
            var ortho = resourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            graphicsDevice.UpdateBuffer(ortho, 0, orthographicOffCenter);


            var indexBuffer = resourceFactory.CreateBuffer(new BufferDescription((uint) (sizeof(ushort) * 2 * map.Linedefs.Length), BufferUsage.IndexBuffer));

            var indexes = new ushort[map.Linedefs.Length * 2];
            var i = 0;
            foreach (var linedef in map.Linedefs)
            {
                indexes[i] = linedef.StartVertex;
                indexes[i + 1] = linedef.EndVertex;
                i += 2;
            }
            graphicsDevice.UpdateBuffer(indexBuffer, 0, indexes);

            var shaders = resourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(File.ReadAllText("Rendering/minimap-vertex.glsl")), "main"),
                                                            new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(File.ReadAllText("Rendering/minimap-fragment.glsl")), "main"));
            
            var resourceLayout = resourceFactory.CreateResourceLayout(new ResourceLayoutDescription(new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            var resourceSet = resourceFactory.CreateResourceSet(new ResourceSetDescription(resourceLayout, ortho));
            var pipeline = resourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription
                                                                          {
                                                                              BlendState = BlendStateDescription.SingleOverrideBlend,
                                                                              PrimitiveTopology = PrimitiveTopology.LineList,
                                                                              DepthStencilState = new DepthStencilStateDescription
                                                                                                  {
                                                                                                      DepthTestEnabled = false,
                                                                                                      StencilTestEnabled = false,
                                                                                                      DepthComparison = ComparisonKind.Always
                                                                                                  },
                                                                              RasterizerState = RasterizerStateDescription.CullNone,
                                                                              ShaderSet = new ShaderSetDescription(new []
                                                                                                                   {
                                                                                                                       new VertexLayoutDescription(new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Short2))
                                                                                                                   }, shaders),
                                                                              ResourceLayouts = new[] {resourceLayout},
                                                                              Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription
                                                                          });

            var cl = resourceFactory.CreateCommandList();
            while (window.Exists)
            {
                window.PumpEvents();

                cl.Begin();
                cl.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
                //cl.SetViewport(0, new Viewport(0,0, window.Width / 10f, window.Height / 10f, 0.1f, 100f));
                cl.ClearColorTarget(0, RgbaFloat.Black);
                cl.SetPipeline(pipeline);
                cl.SetVertexBuffer(0, vertexBuffer);
                cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
                cl.SetGraphicsResourceSet(0, resourceSet);
                cl.DrawIndexed((uint) indexes.Length);
                cl.End();

                graphicsDevice.SubmitCommands(cl);
                graphicsDevice.SwapBuffers();
                graphicsDevice.WaitForIdle();
            }
        }
    }
}
