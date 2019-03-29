namespace PlanetaryTerrain
{
    public enum LODModeBehindCam { NotComputed, ComputeRender } //How are quads behind the camera handled?
    public enum Mode { Heightmap, Noise, Hybrid, Const, ComputeShader }
    public enum UVType { Cube, Quad, Legacy, LegacyContinuous }
}
