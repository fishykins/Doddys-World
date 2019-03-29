using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Vehicles {
	public class AlignJoint : MonoBehaviour {
        #region Variables
        //Public & Serialized

        //Private
        private ConfigurableJoint joint;
		
		#endregion;

		#region Properties

		#endregion;

		#region Unity Methods
		void Awake () {
            joint = GetComponent<ConfigurableJoint>();

        }

		void Update () {
			if (joint) {
                joint.axis = joint.transform.InverseTransformDirection(transform.parent.right);
                joint.secondaryAxis = joint.transform.InverseTransformDirection(transform.parent.up);
            }
		}
		#endregion;
	}
}
