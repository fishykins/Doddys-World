using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetaryTerrain;
using DW.Physics;

namespace DW.Worlds {
	public class WorldBuilder {
        #region Variables
        //Public & Serialized

        //Private
        private WorldManager manager;
        private SceneInstance scene;

        #endregion;

        #region Properties

        #endregion;

        #region Constructor
        public WorldBuilder(WorldManager manager)
        {
            this.manager = manager;
            this.scene = manager.Scene;
        }
        #endregion;

        #region Custom Methods
        public Planet GenerateWorld(WorldProfile profile)
        {
            if (scene == null) return null;

            GameObject worldObject = scene.SpawnGameObject(profile.worldName);
            worldObject.transform.position = profile.startPosition;
            Planet world = worldObject.AddComponent<Planet>();
            IGravityBody body = worldObject.AddComponent<WorldGravity>();

            scene.GravityBodies.Add(body);

            TransferData(world, profile);
            return world;
        }

        private void TransferData(Planet world, WorldProfile profile)
        {
            //General
            world.radius = profile.radius;
            world.rotation = profile.rotation;
            world.detailDistances = profile.detailDistances;
            world.calculateMsds = profile.calculateMsds;
            world.detailMsds = profile.detailMsds;
            world.lodModeBehindCam = profile.lodModeBehindCam;
            world.behindCameraExtraRange = profile.behindCameraExtraRange;
            world.planetMaterial = profile.planetMaterial;
            world.uvType = profile.uvType;
            world.uvScale = profile.uvScale;
            world.generateColliders = profile.generateColliders;
            world.visSphereRadiusMod = profile.visSphereRadiusMod;
            world.updateAllQuads = profile.updateAllQuads;
            world.maxQuadsToUpdate = profile.maxQuadsToUpdate;
            world.rotationUpdateThreshold = profile.rotationUpdateThreshold;
            world.recomputeQuadDistancesThreshold = profile.recomputeQuadDistancesThreshold;
            world.quadsSplittingSimultaneously = profile.quadsSplittingSimultaneously;

            //Scaled Space
            world.useScaledSpace = profile.useScaledSpace;
            world.createScaledSpaceCopy = profile.createScaledSpaceCopy;
            world.scaledSpaceDistance = profile.scaledSpaceDistance;
            world.scaledSpaceFactor = profile.scaledSpaceFactor;
            world.scaledSpaceMaterial = profile.scaledSpaceMaterial;
            world.scaledSpaceCopy = profile.scaledSpaceCopy;

            //Biomes
            world.useBiomeMap = profile.useBiomeMap;
            world.biomeMapTexture = profile.biomeMapTexture;
            world.textureHeights = profile.textureHeights;
            world.textureIds = profile.textureIds;
            world.useSlopeTexture = profile.useSlopeTexture;
            world.slopeAngle = profile.slopeAngle;
            world.slopeTexture = profile.slopeTexture;

            //Terrain generation
            world.mode = profile.mode;
            world.heightScale = profile.heightScale;
            world.heightmapSizeX = profile.heightmapSizeX;
            world.heightmapSizeY = profile.heightmapSizeY;
            world.heightmap16bit = profile.heightmap16bit;
            world.heightmapTextAsset = profile.heightmapTextAsset;
            world.useBicubicInterpolation = profile.useBicubicInterpolation;
            world.computeShader = profile.computeShader;
            world.noiseSerialized = profile.noiseSerialized;
            world.hybridModeNoiseDiv = profile.hybridModeNoiseDiv;
            world.constantHeight = profile.constantHeight;

            //Detail/Grass generation
            world.generateDetails = profile.generateDetails;
            world.generateGrass = profile.generateGrass;
            world.planetIsRotating = profile.planetIsRotating;
            world.grassMaterial = profile.grassMaterial;
            world.grassPerQuad = profile.grassPerQuad;
            world.grassLevel = profile.grassLevel;
            world.detailDistance = profile.detailDistance;
            world.detailMeshes = profile.detailMeshes;
            world.detailPrefabs = profile.detailPrefabs;
            world.generateFoliageInEveryBiome = profile.generateFoliageInEveryBiome;
            world.foliageBiomes = profile.foliageBiomes;
            world.detailObjectsGeneratingSimultaneously = profile.detailObjectsGeneratingSimultaneously;

            //Misc
            world.floatingOrigin = profile.floatingOrigin;
            world.hideQuads = profile.hideQuads;
            world.numQuads = profile.numQuads;
        }
        #endregion
    }
}
