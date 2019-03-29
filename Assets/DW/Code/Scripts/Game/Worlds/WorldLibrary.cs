using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.DataTypes;

namespace DW.Worlds {
	public class WorldLibrary : MonoBehaviour {
        #region Classes

        #endregion

        #region Variables
        //Public
        public static WorldLibrary instance;

        [SerializeField]
        private WorldProfile[] worlds = new WorldProfile[1];

        //Private
		#endregion;

		#region Properties
        public WorldProfile[] Worlds { get { return worlds; } }
		#endregion;

		#region Unity Methods
		void Awake () {
			if (instance == null ) {
                instance = this;
            }
		}

		void Update () {
			
		}
        #endregion;

        #region Custom Methods

        #endregion

        #region Static Methods
        public static float GetTemperature(float altitude)
        {
            float celcius = 15f;
            return celcius;
        }
        #endregion

        public static Dictionary<int2, int[]> OrderOfQuadChildren = new Dictionary<int2, int[]>() {

                {new int2(0, 1), new int[] {2, 0, 1, 3}},
                {new int2(0, 0), new int[] {3, 1, 0, 2}},

                {new int2(1, 1), new int[] {1, 0, 2, 3}},
                {new int2(1, 0), new int[] {3, 2, 0, 1}},

                {new int2(2, 1), new int[] {3, 2, 0, 1}},
                {new int2(2, 0), new int[] {2, 3, 1, 0}},
            };
    }
}
