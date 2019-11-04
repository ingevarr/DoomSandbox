using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

using Doom.DataPrimitives;
using Doom.GameMap;

using Veldrid;
using Veldrid.SPIRV;

namespace Doom 
{
    public sealed class Renderer
    {
        private readonly RenderingContext renderingContext;

        private MapData map;

        private Pipeline pipeline;
        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private ushort[] indexes;
        private ResourceSet resourceSet;


        public Renderer(RenderingContext renderingContext) => this.renderingContext = renderingContext;

        public void InitializeResources(MapData mapData)
        {
            map = mapData;

            vertexBuffer = renderingContext.ResourceFactory.CreateBuffer(new BufferDescription((uint) (map.VertexesCount * Unsafe.SizeOf<Vertex>()), BufferUsage.VertexBuffer));
            renderingContext.GraphicsDevice.UpdateBuffer(vertexBuffer, 0, map.Vertexes.Data);

            var orthographicOffCenter 
                = Matrix4x4.CreateOrthographicOffCenter(map.Vertexes.Left, 
                                                        map.Vertexes.Right, 
                                                        map.Vertexes.Bottom, 
                                                        map.Vertexes.Top, 
                                                        0.1f, 
                                                        100f);
            var ortho = renderingContext.ResourceFactory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            renderingContext.GraphicsDevice.UpdateBuffer(ortho, 0, orthographicOffCenter);


            indexBuffer = renderingContext.ResourceFactory.CreateBuffer(new BufferDescription((uint) (sizeof(ushort) * 2 * map.Linedefs.Length), BufferUsage.IndexBuffer));

            indexes = new ushort[map.Linedefs.Length * 2];
            var i = 0;
            foreach (var linedef in map.Linedefs)
            {
                indexes[i] = linedef.StartVertex;
                indexes[i + 1] = linedef.EndVertex;
                i += 2;
            }
            renderingContext.GraphicsDevice.UpdateBuffer(indexBuffer, 0, indexes);

            var shaders = renderingContext.ResourceFactory.CreateFromSpirv(new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(File.ReadAllText("Rendering/minimap-vertex.glsl")), "main"),
                                                          new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(File.ReadAllText("Rendering/minimap-fragment.glsl")), "main"));
            
            var resourceLayout = renderingContext.ResourceFactory.CreateResourceLayout(new ResourceLayoutDescription(new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            resourceSet = renderingContext.ResourceFactory.CreateResourceSet(new ResourceSetDescription(resourceLayout, ortho));
            pipeline = renderingContext.ResourceFactory.CreateGraphicsPipeline(new GraphicsPipelineDescription
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
                                                                  Outputs = renderingContext.GraphicsDevice.SwapchainFramebuffer.OutputDescription
                                                              });
            }

        public void Render()
        {
            renderingContext.MainCommandList.Begin();
            renderingContext.MainCommandList.SetFramebuffer(renderingContext.GraphicsDevice.SwapchainFramebuffer);
            renderingContext.MainCommandList.ClearColorTarget(0, RgbaFloat.Black);
            
            RenderMiniMap(renderingContext.MainCommandList);

            renderingContext.MainCommandList.End();

            renderingContext.GraphicsDevice.SubmitCommands(renderingContext.MainCommandList);
            renderingContext.GraphicsDevice.SwapBuffers();
            renderingContext.GraphicsDevice.WaitForIdle();
        }

        private void RenderMiniMap(CommandList commandList)
        {
            commandList.SetPipeline(pipeline);
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            commandList.SetGraphicsResourceSet(0, resourceSet);
            commandList.DrawIndexed((uint)indexes.Length);
        }
    }
}