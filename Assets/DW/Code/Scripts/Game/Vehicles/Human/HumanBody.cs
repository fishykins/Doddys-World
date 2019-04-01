using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;

namespace DW.Vehicles
{
    public class HumanBody : PhysicsBody, IPhysicsBody
    {
        #region Variables
        //Public & Serialized
        public Animator animator;

        //Private
        private bool grounded = false;
        private bool jumping = false;
        private float stun = 0f; //how long a human is stunned for. 
        private float braceTime = 0f; //Time that player hit "crouch"

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        protected override void Start()
        {
            base.Start();
            //Disable auto rotation- we will only want this if we are in ragdoll mode
            rb.freezeRotation = true;
        }

        protected override void Update()
        {
            base.Update();
        }

        void OnCollisionEnter(Collision collision)
        {

        }
        #endregion

        #region Custom Methods
        protected override void HandleNearestWorld()
        {
            base.HandleNearestWorld();

            Vector3 down = nearestWorld.Vector3Down(transform.position, false);
        }


        #endregion
    }
}
