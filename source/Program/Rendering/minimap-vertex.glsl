#version 450

layout(location = 0) in ivec2 Position;

layout(set = 0, binding = 0) uniform ProjectionBuffer
{
    mat4 Projection;
};

void main()
{
 	gl_Position = Projection * vec4(Position, 0, 1);
}
