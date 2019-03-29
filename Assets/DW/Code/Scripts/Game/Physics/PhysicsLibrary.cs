using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW {
	public class PhysicsLibrary : MonoBehaviour {
        #region Variables
        //Public
        public static PhysicsLibrary instance;

        [Header("World Physics")]
        [SerializeField]
        private float StandardG = 9.81f;
        [SerializeField]
        private float StandardWorldRadius = 4000f;

        //Private
        private float earthLikeDensity;

        #endregion;

        #region Constants
        public const float G = 6.674f * 1e-11f;
        #endregion

        #region Properties
        public float EarthLikeDensity { get { return earthLikeDensity; } }
        #endregion;

        #region Unity Methods
        private void Awake()
        {
            if (!instance) {
                instance = this;
                CalculateWorldDensity();
            }
            
        }
		#endregion;

        #region Custom Methods
        private void CalculateWorldDensity()
        {
            float earthLikeMass = (StandardG * StandardWorldRadius * StandardWorldRadius) / G;
            float earthLikeVolume = MathDW.GetSphereVolume(StandardWorldRadius);
            earthLikeDensity = earthLikeMass / earthLikeVolume;
        }

        public static float GetWorldMass(float radius, float earthLikeDensity)
        {
            float density = earthLikeDensity * instance.EarthLikeDensity; //GOOD
            float volume = MathDW.GetSphereVolume(radius);

            return density * volume;
        }
        #endregion
	}
}
