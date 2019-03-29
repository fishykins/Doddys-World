using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy {
	public class bunnyFollow : MonoBehaviour {
		#region Variables
		//Public & Serialized
		
		//Private
		
		#endregion;

		#region Properties

		#endregion;

		#region Unity Methods
		void Start () {
            Camera.main.transform.SetParent(this.transform);
            Camera.main.transform.localPosition = new Vector3(0f, 0f, -4f);

        }

		void Update () {
			
		}
		#endregion;

        #region Custom Methods

        #endregion
	}
}
