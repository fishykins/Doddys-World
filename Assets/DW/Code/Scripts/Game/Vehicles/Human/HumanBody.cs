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
        public float balance = 60f; //How quickly we can right ourselves, once we are able to do so
        public float groundedRange = 2.5f; //How far we raycast to detect grounded
        public float stunRecoverySpeed = 1f; //how quickly we can recover from being stunned
        public float maxImpactForce = 2000f; //The largest impact we can experience without getting knocked down
        public float braceMaxTime = 0.2f;
        public float braceMaxAbsorb = 1000f;
        public float maxFlex = 0.4f;

        public float jumpPower = 1000f;
        public float movementSpeed = 5f;

        public Animator animator;

        public ConfigurableJoint[] joints;

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
