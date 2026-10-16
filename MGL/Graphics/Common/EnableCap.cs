namespace MGL.Graphics;

public enum EnableCap 
{
    Blend = 0x0BE2,
    DepthTest = 0x0B71,
    CullFace = 0x0B44,
    ScissorTest = 0x0C11,
    StencilTest = 0x0B90,
    PolygonOffsetFill = 0x8037,
    Multisample = 0x809D,
    ProgramPointSize = 0x8642,
    RasterizerDiscard = 0x8C89,
    PrimitiveRestart = 0x8F9D,
    PrimitiveRestartFixedIndex = 0x8D69,
    DebugOutput = 0x92E0
}