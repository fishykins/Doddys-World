using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Worlds.V1 {
	public class TrackedObject  {
        #region Variables
        //Public & Serialized
        public string name;
        public Transform transform;
        public float height;
        public Vector3 relativePosition;
        public Vector3 lastPosition = Vector3.zero;
        internal Vector3 worldToMeshVector;

        //Private
        private Transform world;

        #endregion;

        public TrackedObject(Transform transform, Transform world)
        {
            this.transform = transform;
            this.world = world;
            this.name = transform.gameObject.name;
        }


        #region Properties

        #endregion;

        #region Custom Methods
        public void Update()
        {
            //Vector used by Quads when computing distance
            worldToMeshVector = Quaternion.Inverse(world.rotation) * (transform.position - world.position);
            //Position relative to world
            relativePosition = transform.position - world.position;
            //height
            height = relativePosition.magnitude;
        }

        public bool BoundsChanged(float sqrRange)
        {
            if ((relativePosition - lastPosition).sqrMagnitude > sqrRange) {
                lastPosition = relativePosition;
                return true;
            }

            return false;
        }
        #endregion
    }
}
