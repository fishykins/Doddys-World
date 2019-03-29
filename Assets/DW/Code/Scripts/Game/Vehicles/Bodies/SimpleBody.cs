using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;

namespace DW.Vehicles {
	public class SimpleBody : MonoBehaviour {
        #region Variables
        //Public & Serialized

        //Private
        private SceneInstance scene;
        #endregion;

        #region Properties
        public SceneInstance Scene { get { return scene; } }
        public Transform Transform { get { return transform; } }
        #endregion;

        #region Unity Methods
        void Start () {
			
		}

		void Update () {
			
		}
        #endregion;

        #region Custom Methods
        public string Initialize(SceneInstance scene)
        {
            this.scene = scene;
            return scene.name;
        }
        #endregion
    }
}
