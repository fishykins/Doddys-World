using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;

namespace DW.Vehicles {
    public class CarBody : PhysicsBody, IPhysicsBody
    {
        #region Variables
        //Public & Serialized
        public Transform body;
        public Transform[] wheels;

        //Private

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        protected override void Update()
        {
            base.Update();
            foreach (var wheel in wheels) {
                //wheel.rotation = body.rotation;
            }
        }

        #endregion;

        #region Custom Methods

        #endregion
    }
}
