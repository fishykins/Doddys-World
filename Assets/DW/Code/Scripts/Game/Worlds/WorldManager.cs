using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetaryTerrain;

namespace DW.Worlds {
	public class WorldManager : MonoBehaviour {
        #region Variables
        //Public & Serialized

        //Private
        private SceneInstance scene;
        private WorldBuilder builder;
        private System.Random random = new System.Random();

        private List<Planet> worlds = new List<Planet>();

        private ManagerStatus status = ManagerStatus.initializing;
		#endregion;

		#region Properties
        public SceneInstance Scene { get { return scene; } }
        public List<Planet> Worlds { get { return worlds; } }
        public ManagerStatus Status { get { return status; } }
        #endregion;

        #region Unity Methods

        #endregion;

        #region Custom Methods
        public void Initialize(SceneInstance scene)
        {
            this.scene = scene;
            if (!Scene.Headless) {
                BuildWorlds();
            }

            StartCoroutine(Loading());
        }

        IEnumerator Loading()
        {
            while (status == ManagerStatus.initializing) {
                if (Worlds.Count > 0) {
                    int initialized = 0;
                    foreach (var world in Worlds) {
                        if (world.initialized) {
                            initialized++;
                        }
                    }

                    if (initialized == Worlds.Count) {
                        status = ManagerStatus.ready;
                    }
                }

                if (scene.Headless) {
                    status = ManagerStatus.ready;
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public Planet RandomWorld()
        {
            return (worlds.Count > 0) ? worlds[(int)random.Next(worlds.Count - 1)] : null;
        }

        public Vector2 RandomLatLon()
        {
            return new Vector2(random.Next(180) - 90, random.Next(360) - 180);
        }

        public void SpawnTestObjects()
        {
            foreach (var world in Worlds) {
                if (world.initialized) {
                    scene.SpawnVehicle("Bunny", world, new Vector2(20f, 20f));
                }
            }
        }

        public Planet GetNearestWorld(Vector3 worldPosition)
        {
            if (worlds.Count == 0) return null;
            if (worlds.Count == 1) return worlds[0];

            float nearestDist = float.MaxValue;
            Planet nearestPlanet = null;

            foreach (var world in worlds) {
                float dist = (world.transform.position - worldPosition).sqrMagnitude;
                if (dist < nearestDist) {
                    nearestDist = dist;
                    nearestPlanet = world;
                }
            }

            return nearestPlanet;
        }

        private void BuildWorlds()
        {
            builder = new WorldBuilder(this);

            if (WorldLibrary.instance.Worlds.Length > 0) {
                foreach (var world in WorldLibrary.instance.Worlds) {
                    Planet planet = builder.GenerateWorld(world);
                    if (planet != null) {
                        worlds.Add(planet);
                    }
                }
            }
        }
        #endregion
	}
}
