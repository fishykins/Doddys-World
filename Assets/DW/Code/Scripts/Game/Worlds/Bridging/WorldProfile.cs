using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetaryTerrain;
using PlanetaryTerrain.DoubleMath;
using PlanetaryTerrain.Foliage;

namespace DW.Worlds {
    [CreateAssetMenu]
    public class WorldProfile : ScriptableObject
    {
        //General
        public string worldName = "newWorld";
        public float radius = 10000;
        public QuaternionD rotation;
        public Vector3 startPosition = Vector3.zero;
        public float[] detailDistances = { 50000, 25000, 12500, 6250, 3125 };
        public bool calculateMsds;
        public float[] detailMsds = { 0f, 0f, 0f, 0f, 0f };
        public LODModeBehindCam lodModeBehindCam = LODModeBehindCam.ComputeRender;
        public float behindCameraExtraRange;
        public Material planetMaterial;
        public UVType uvType = UVType.Legacy;
        public float uvScale = 1f;
        public bool[] generateColliders = { false, false, false, false, false, true };
        public float visSphereRadiusMod = 1f;
        public bool updateAllQuads = false;
        public int maxQuadsToUpdate = 250;
        public float rotationUpdateThreshold = 1f;
        public float recomputeQuadDistancesThreshold = 10f;
        public int quadsSplittingSimultaneously = 2;

        //Scaled Space
        public bool useScaledSpace;
        public bool createScaledSpaceCopy;
        public float scaledSpaceDistance = 1500f;
        public float scaledSpaceFactor = 100000f;
        public Material scaledSpaceMaterial;
        public GameObject scaledSpaceCopy;

        //Biomes
        public bool useBiomeMap;
        public Texture2D biomeMapTexture;
        public float[] textureHeights = { 0f, 0.01f, 0.02f, 0.75f, 1f };
        public byte[] textureIds = { 0, 1, 2, 3, 4, 5 };
        public bool useSlopeTexture;
        public float slopeAngle = 60;
        public byte slopeTexture = 5;

        //Terrain generation
        public Mode mode = Mode.Const;
        public float heightScale = 0.02f;
        public int heightmapSizeX = 8192;
        public int heightmapSizeY = 4096;
        public bool heightmap16bit;
        public TextAsset heightmapTextAsset;
        public bool useBicubicInterpolation = true;
        public ComputeShader computeShader;
        public TextAsset noiseSerialized;
        public float hybridModeNoiseDiv = 50f;
        public float constantHeight = 0f;

        //Detail/Grass generation
        public bool generateDetails;
        public bool generateGrass;
        public bool planetIsRotating;
        public Material grassMaterial;
        public int grassPerQuad = 10000;
        public int grassLevel = 5;
        public float detailDistance;
        public List<DetailMesh> detailMeshes = new List<DetailMesh>();
        public List<DetailPrefab> detailPrefabs = new List<DetailPrefab>();
        public bool generateFoliageInEveryBiome;
        public List<byte> foliageBiomes = new List<byte> { 1 };
        public int detailObjectsGeneratingSimultaneously = 3;

        //Misc
        public FloatingOrigin floatingOrigin;
        public bool hideQuads = true;
        public int numQuads;
    }
}
