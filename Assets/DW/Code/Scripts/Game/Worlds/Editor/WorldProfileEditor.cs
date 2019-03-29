using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PlanetaryTerrain;
using PlanetaryTerrain.Foliage;

namespace DW.Worlds
{
    [CustomEditor(typeof(WorldProfile))]
    public class WorldProfileEditor : Editor
    {
        public enum Tab
        {
            General, Generation, Visual, Foliage, Debug
        }
        Tab tab = Tab.General;
        WorldProfile profile;
        SerializedProperty detailDistances, generateColliders, detailMSDs;
        SerializedProperty scaledSpaceMaterial, planetMaterial;
        SerializedProperty heightmap;
        SerializedProperty heights, textureIds;
        SerializedProperty detailObjects;
        SerializedProperty foliageBiomes;


        string[] tabNames = { "General", "Generation", "Visual", "Foliage", "Debug" };
        public void OnEnable()
        {
            profile = (WorldProfile)target;

            detailDistances = serializedObject.FindProperty("detailDistances");
            generateColliders = serializedObject.FindProperty("generateColliders");
            detailMSDs = serializedObject.FindProperty("detailMsds");
            scaledSpaceMaterial = serializedObject.FindProperty("scaledSpaceMaterial");
            planetMaterial = serializedObject.FindProperty("planetMaterial");
            heights = serializedObject.FindProperty("textureHeights");
            textureIds = serializedObject.FindProperty("textureIds");
            foliageBiomes = serializedObject.FindProperty("foliageBiomes");

        }
        public override void OnInspectorGUI()
        {

            EditorGUILayout.Space();
            tab = (Tab)GUILayout.Toolbar((int)tab, tabNames, EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            switch (tab) {
                case Tab.General:
                    profile.worldName = EditorGUILayout.TextField("Name", profile.worldName);
                    profile.radius = EditorGUILayout.FloatField("Radius", profile.radius);
                    EditorGUILayout.PropertyField(detailDistances, true);
                    profile.calculateMsds = EditorGUILayout.Toggle(new GUIContent("Calculate MSDs", "The MSD is the bumpiness of the quad. When calculated, bumpyness thresholds can be set for splitting quads."), profile.calculateMsds);
                    if (profile.calculateMsds)
                        EditorGUILayout.PropertyField(detailMSDs, true);
                    EditorGUILayout.PropertyField(generateColliders, true);
                    GUILayout.Space(5f);
                    profile.lodModeBehindCam = (LODModeBehindCam)EditorGUILayout.EnumPopup(new GUIContent("LOD Mode behind Camera", "How are quads behind the camera handled?"), profile.lodModeBehindCam);
                    if (profile.lodModeBehindCam == LODModeBehindCam.NotComputed)
                        profile.behindCameraExtraRange = EditorGUILayout.FloatField(new GUIContent("LOD Extra Range", "Extra Range for quads behind the Camera. Increase for large planets."), profile.behindCameraExtraRange);
                    GUILayout.Space(5f);
                    profile.recomputeQuadDistancesThreshold = EditorGUILayout.FloatField(new GUIContent("Recompute Quad Threshold", "Threshold for recomputing all quad distances. Increase for better performance while moving with many quads."), profile.recomputeQuadDistancesThreshold);
                    profile.rotationUpdateThreshold = EditorGUILayout.FloatField(new GUIContent("Rotation Update Threshold", "Degrees of rotation after which Quads are updated."), profile.rotationUpdateThreshold);
                    profile.updateAllQuads = EditorGUILayout.Toggle(new GUIContent("Update all Quads simultaneously", "Update all Quads in one frame or over multiple frames? Only turn on when player is very fast and planet has few quads."), profile.updateAllQuads);
                    if (!profile.updateAllQuads)
                        profile.maxQuadsToUpdate = EditorGUILayout.IntField(new GUIContent("Max Quads to update per frame", "Max Quads to update in one frame. Lower value means process of updating all Quads takes longer, fewer spikes of lower framerates. If it takes too long, the next update tries to start while the last one is still running, warning and suggestion to increase maxQuadsToUpdate will be logged."), profile.maxQuadsToUpdate);
                    profile.floatingOrigin = (FloatingOrigin)EditorGUILayout.ObjectField("Floating Origin (if used)", profile.floatingOrigin, typeof(FloatingOrigin), true);
                    profile.hideQuads = EditorGUILayout.Toggle("Hide Quads in Hierarchy", profile.hideQuads);
                    break;

                case Tab.Generation:
                    profile.mode = (Mode)EditorGUILayout.EnumPopup("Generation Mode", profile.mode);
                    GUILayout.Space(5f);

                    switch (profile.mode) {
                        case Mode.Heightmap:
                            Heightmap();
                            break;

                        case Mode.Noise:
                            profile.noiseSerialized = (TextAsset)EditorGUILayout.ObjectField("Noise", profile.noiseSerialized, typeof(TextAsset), false);
                            break;

                        case Mode.Hybrid:
                            Heightmap();
                            GUILayout.Space(5f);
                            profile.noiseSerialized = (TextAsset)EditorGUILayout.ObjectField("Noise", profile.noiseSerialized, typeof(TextAsset), false);

                            GUILayout.Space(5f);
                            profile.hybridModeNoiseDiv = EditorGUILayout.FloatField(new GUIContent("Noise Divisor", "Increase for noise to be less pronounced."), profile.hybridModeNoiseDiv);
                            break;
                        case Mode.Const:
                            profile.constantHeight = EditorGUILayout.FloatField("Constant Height", profile.constantHeight);
                            break;

                        case Mode.ComputeShader:
                            profile.computeShader = (ComputeShader)EditorGUILayout.ObjectField("Compute Shader", profile.computeShader, typeof(ComputeShader), false);
                            break;
                    }

                    GUILayout.Space(10f);
                    profile.heightScale = EditorGUILayout.FloatField("Height Scale", profile.heightScale);
                    profile.useScaledSpace = EditorGUILayout.Toggle("Use Scaled Space", profile.useScaledSpace);
                    if (profile.useScaledSpace) {
                        profile.createScaledSpaceCopy = EditorGUILayout.Toggle("Create Scaled Space Copy", profile.createScaledSpaceCopy);
                        profile.scaledSpaceFactor = EditorGUILayout.FloatField("Scaled Space Factor", profile.scaledSpaceFactor);
                    }
                    profile.quadsSplittingSimultaneously = EditorGUILayout.IntField(new GUIContent("Quads Splitting Simultaneously", "Number of quads that can split at the same time. Higher means shorter loading time but more CPU usage."), profile.quadsSplittingSimultaneously);
                    break;

                case Tab.Visual:
                    EditorGUILayout.PropertyField(planetMaterial);
                    profile.uvType = (UVType)EditorGUILayout.EnumPopup("UV Type", (System.Enum)profile.uvType);
                    if (profile.uvType == UVType.Cube)
                        profile.uvScale = EditorGUILayout.FloatField("UV Scale", profile.uvScale);
                    if (profile.useScaledSpace) {
                        profile.scaledSpaceDistance = EditorGUILayout.FloatField(new GUIContent("Scaled Space Distance", "Distance at which the planet disappears and the Scaled Space copy of the planet is shown if enabled."), profile.scaledSpaceDistance);
                        if (profile.createScaledSpaceCopy)
                            EditorGUILayout.PropertyField(scaledSpaceMaterial);
                    }
                    GUILayout.Space(5f);

                    profile.useBiomeMap = EditorGUILayout.Toggle(new GUIContent("Use biome map", "Override height map used for texture selection."), profile.useBiomeMap);
                    if (profile.useBiomeMap)
                        profile.biomeMapTexture = (Texture2D)EditorGUILayout.ObjectField("Biome Map Texture", profile.biomeMapTexture, typeof(Texture2D), false);

                    GUILayout.Space(5f);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(textureIds, true);
                    EditorGUILayout.PropertyField(heights, true);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5f);
                    profile.useSlopeTexture = EditorGUILayout.Toggle("Use Slope Texture", profile.useSlopeTexture);
                    if (profile.useSlopeTexture) {
                        profile.slopeAngle = EditorGUILayout.Slider("Slope Angle", profile.slopeAngle, 0f, 90f);
                        profile.slopeTexture = (byte)EditorGUILayout.IntField(new GUIContent("Slope Texture", "Texture ID (0-5) used for slope."), profile.slopeTexture);
                    }
                    GUILayout.Space(5f);
                    profile.visSphereRadiusMod = EditorGUILayout.FloatField("Visibilty Sphere Radius Mod", profile.visSphereRadiusMod);

                    break;
                case Tab.Foliage:

                    profile.generateDetails = EditorGUILayout.Toggle("Generate Details", profile.generateDetails);

                    if (profile.generateDetails) {
                        profile.generateFoliageInEveryBiome = EditorGUILayout.Toggle("Generate Foliage in every biome", profile.generateFoliageInEveryBiome);
                        if (!profile.generateFoliageInEveryBiome)
                            EditorGUILayout.PropertyField(foliageBiomes, true);

                        profile.planetIsRotating = EditorGUILayout.Toggle(new GUIContent("Planet is Rotating", "If the planet is rotating, all points for a quad have to generated in one frame."), profile.planetIsRotating);
                        profile.grassPerQuad = EditorGUILayout.IntSlider(new GUIContent("Points per Quad", "How many random positions on each quad? Points are used for both grass and meshes."), profile.grassPerQuad, 0, 60000);
                        GUILayout.Space(5f);
                        profile.generateGrass = EditorGUILayout.Toggle("Generate Grass", profile.generateGrass);
                        if (profile.generateGrass)
                            profile.grassMaterial = (Material)EditorGUILayout.ObjectField("Grass Material", profile.grassMaterial, typeof(Material), false);

                        profile.grassLevel = EditorGUILayout.IntField(new GUIContent("Detail Level", "Level at and after which details are generated"), profile.grassLevel);
                        profile.detailDistance = EditorGUILayout.FloatField(new GUIContent("Detail Distance", "Distance at which grass and meshes are generated"), profile.detailDistance);
                        profile.detailObjectsGeneratingSimultaneously = EditorGUILayout.IntField(new GUIContent("Details generating simultaneously", "How many quads can generate details at the same time."), profile.detailObjectsGeneratingSimultaneously);
                        GUILayout.Space(5f);

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Mesh"))
                            profile.detailMeshes.Add(new DetailMesh());
                        if (GUILayout.Button("Add Prefab"))
                            profile.detailPrefabs.Add(new DetailPrefab());
                        EditorGUILayout.EndHorizontal();

                        for (int i = 0; i < profile.detailMeshes.Count; i++) {
                            var dM = profile.detailMeshes[i];

                            if (dM.isGrass)
                                continue;

                            GUILayout.Label("Detail Mesh:", EditorStyles.boldLabel);

                            dM.meshFraction = EditorGUILayout.Slider(new GUIContent("Fraction", "Fraction of generated points used for meshes instead of grass"), dM.meshFraction, 0f, 1f);
                            dM.meshOffsetUp = EditorGUILayout.FloatField("Offset Up", dM.meshOffsetUp);
                            dM.meshScale = EditorGUILayout.Vector3Field("Scale", dM.meshScale);
                            dM.mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", dM.mesh, typeof(Mesh), false);
                            dM.material = (Material)EditorGUILayout.ObjectField("Material", dM.material, typeof(Material), false);
                            dM.useGPUInstancing = EditorGUILayout.Toggle("Use GPU Instancing", dM.useGPUInstancing);

                            profile.detailMeshes[i] = dM;

                            if (GUILayout.Button("Remove"))
                                profile.detailMeshes.RemoveAt(i);


                            GUILayout.Space(10f);
                        }

                        for (int i = 0; i < profile.detailPrefabs.Count; i++) {

                            var dP = (DetailPrefab)profile.detailPrefabs[i];

                            GUILayout.Label("Detail Prefab:", EditorStyles.boldLabel);

                            dP.meshFraction = EditorGUILayout.Slider(new GUIContent("Fraction", "Fraction of generated points used for prefab instead of grass"), dP.meshFraction, 0f, 1f);
                            dP.meshOffsetUp = EditorGUILayout.FloatField("Offset Up", dP.meshOffsetUp);
                            dP.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", dP.prefab, typeof(GameObject), true);

                            if (GUILayout.Button("Remove"))
                                profile.detailPrefabs.RemoveAt(i);

                            GUILayout.Space(10f);
                        }

                    }


                    break;

                case Tab.Debug:
                    DrawDefaultInspector();
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }

        void Heightmap()
        {
            profile.heightmapTextAsset = (TextAsset)EditorGUILayout.ObjectField("Heightmap", profile.heightmapTextAsset, typeof(TextAsset), false);
            EditorGUILayout.BeginHorizontal();
            profile.heightmapSizeX = EditorGUILayout.IntField("Width", profile.heightmapSizeX);
            profile.heightmapSizeY = EditorGUILayout.IntField("Height", profile.heightmapSizeY);
            EditorGUILayout.EndHorizontal();
            profile.heightmap16bit = EditorGUILayout.Toggle(new GUIContent("16bit", "16bit mode for more elevation levels. Enable if you generated a 16bit heightmap in the generator."), profile.heightmap16bit);
            profile.useBicubicInterpolation = EditorGUILayout.Toggle("Use Bicubic interpolation", profile.useBicubicInterpolation);
        }
        [MenuItem("GameObject/3D Object/Planet")]
        public static void CreatePlanet()
        {
            var go = new GameObject();
            go.AddComponent<Planet>().planetMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
            go.name = "Planet";
        }
    }
}