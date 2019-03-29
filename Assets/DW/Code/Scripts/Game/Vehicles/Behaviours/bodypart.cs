using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Vehicles {
	public class bodypart : MonoBehaviour {
        #region Variables
        //Public & Serialized
        public Transform follow;
        //Private
        private Vector3 offset;
		#endregion;

		#region Properties

		#endregion;

		#region Unity Methods
		void Awake () {
			if (follow) {
                offset = transform.position - follow.position;
            }
		}

		void Update () {
			if (follow) {
                Vector3 worldPos = follow.transform.TransformPoint(offset);
                transform.position = worldPos;
            }
		}
		#endregion;

        #region Custom Methods

        #endregion
	}
}
