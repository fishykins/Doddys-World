using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;
using PlanetaryTerrain;

namespace DW.Worlds {
	public class WorldGravity : MonoBehaviour, IGravityBody {
        #region Variables
        //Public & Serialized

        //Private
        private float radius = 4000f;
        private float density = 1f;
        private Planet world;
        public float mass;
        private float gMass;
        private float radiusSqr;

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        void Awake () {
            world = GetComponent<Planet>();

            if (world) {
                radius = world.radius;
            }

            radiusSqr = radius * radius;
            mass = PhysicsLibrary.GetWorldMass(radius, density);
            gMass = mass * PhysicsLibrary.G;
        }
        #endregion;

        #region Custom Methods
        public Vector3 GetGravitationalForce(Vector3 position, float objectMass)
        {
            float rr = (position - transform.position).sqrMagnitude;
            Vector3 v = (position - transform.position).normalized;

            return -(gMass / rr) * objectMass * v;
        }
        #endregion
    }
}
