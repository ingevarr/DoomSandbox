using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

using Doom.DataPrimitives;
using Doom.GameMap;
using Doom.Input;
using Doom.WadFile;

using Veldrid;
using Veldrid.SPIRV;

namespace Doom 
{
    public sealed class Game
    {
        private readonly Wad wad;
        private readonly MapLoader mapLoader;
        
        private readonly IInputTracker input;

        private readonly GraphicsDevice graphicsDevice;
        private readonly ResourceFactory resourceFactory;
        private Pipeline pipeline;

        private CommandList cl;
        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private ushort[] indexes;
        private ResourceSet resourceSet;

        public Game(IInputTracker input, RenderingContext renderingContext)
        {
            wad = Wad.FromFile("DOOM.WAD");
            mapLoader = new MapLoader(wad);
            this.input = input;

            graphicsDevice = renderingContext.GraphicsDevice;
            resourceFactory = renderingContext.ResourceFactory;
            cl = renderingContext.MainCommandList;
        }

        public void Initialize()
        {
            var map = mapLoader.LoadByName("E1M1");

            vertexBuffer = resourceFactory.CreateBuffer(new BufferDescription((uint) (map.VertexesCount * Unsafe.SizeOf<Vertex>()), BufferUsage.VertexBuffer));
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


            indexBuffer = resourceFactory.CreateBuffer(new BufferDescription((uint) (sizeof(ushort) * 2 * map.Linedefs.Length), BufferUsage.IndexBuffer));

            indexes = new ushort[map.Linedefs.Length * 2];
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
            resourceSet = resourceFactory.CreateResourceSet(new ResourceSetDescription(resourceLayout, ortho));
            pipeline = resourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription
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
        }

        public void Update(float deltaTime)
        {
            Console.WriteLine($"{1f / deltaTime}");
        }

        public void Render(RenderingContext renderingContext)
        {
            cl.Begin();
            cl.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            cl.ClearColorTarget(0, RgbaFloat.Black);
            cl.SetPipeline(pipeline);
            cl.SetVertexBuffer(0, vertexBuffer);
            cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            cl.SetGraphicsResourceSet(0, resourceSet);
            cl.DrawIndexed((uint)indexes.Length);
            cl.End();

            graphicsDevice.SubmitCommands(cl);
            graphicsDevice.SwapBuffers();
            graphicsDevice.WaitForIdle();
        }

        public bool IsRunning { get; private set; } = true;
    }
}