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
        [SerializeField]
        private float groundedRange = 3f;
        [SerializeField]
        private float movementSpeed = 5f;
        [SerializeField]
        private float jumpPower = 2000f;
        [SerializeField]
        private float turnSpeed = 5f;
        [SerializeField]
        private float turnSmoothing = 0.1f;

        [HideInInspector]
        public Quaternion targetDirection;

        //Private
        private bool grounded = false;
        private bool jumping = false;
        private float stun = 0f; //how long a human is stunned for. 
        private float braceTime = 0f; //Time that player hit "crouch"
        private Vector3 down;
        private float yaw;
        private Vector3 currentRotation;
        private Vector3 smmothVelocity;
        #endregion;

        #region Properties
        public Vector3 Down { get { return down; } }
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
            jumping = false;
            float impact = collision.impulse.magnitude;
        }
        #endregion

        #region Custom Methods
        protected override void HandleWorldPhysics()
        {
            base.HandleWorldPhysics();

            down = nearestWorld.Vector3Down(transform.position, false);

            Vector3 feetPos = transform.position - down * (groundedRange / 2);

            RaycastHit hit;
            Vector3 targetPos = feetPos + (down * groundedRange);

            grounded = UnityEngine.Physics.Raycast(feetPos, down, out hit, groundedRange);

            if (grounded)
            {
                if (controller != null)
                {
                    if (controller.Input != null)
                        HandleWorldInput(controller.Input, hit);
                }

                OrientateToWorld(100f);
            }

            Color color = (grounded) ? Color.green : Color.red;
            Debug.DrawLine(feetPos, targetPos, color);
        }

        protected virtual void HandleWorldInput(IInput input, RaycastHit hit)
        {
            Vector3 targetDirection = Vector3.zero;

            //Running!
            if (input.ZAxis != 0)
            {
                targetDirection += Vector3.Cross(transform.right, hit.normal) * input.ZAxis;
            }

            //Jumping!
            if (input.YAxis > 0 && grounded && !jumping)
            {
                jumping = true;
                braceTime = 0f;
                rb.AddForce(-down * jumpPower, ForceMode.Impulse);
                rb.AddForce(targetDirection * movementSpeed, ForceMode.VelocityChange);
            }


            

            //Moving at the end
            Vector3 movePoistion = transform.position + (targetDirection * Time.fixedDeltaTime * movementSpeed);
            rb.MovePosition(movePoistion);
        }

        private void OrientateToWorld(float speed)
        {
            //Debug.DrawRay(transform.position, transform.position + down * 2f);

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -down) * transform.rotation;

            // The step size is equal to speed times frame time.
            float step = speed * Time.fixedDeltaTime;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }
        #endregion
    }
}
