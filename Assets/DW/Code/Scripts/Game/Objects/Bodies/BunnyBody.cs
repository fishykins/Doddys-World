using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;

namespace DW.Objects
{
    public class BunnyBody : PhysicsBody, IPhysicsBody {
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
            if (Controller != null) {
                HandleBasicInput(Controller.Input);
            }

        }

        void OnCollisionEnter(Collision collision)
        {
            jumping = false;
            float impact = collision.impulse.magnitude;
            float braceLerp = (Time.time - braceTime) / braceMaxTime;
            float impactReduction = Mathf.Lerp(braceMaxAbsorb, 0f, braceLerp);

            //Scene.Log("Impact- " + collision.impulse.magnitude + ", reduction- " + impactReduction + " (" + braceLerp + ")");

            if (impact - impactReduction > maxImpactForce) {
                Stun(1f);
            }

            braceTime = 0f;
        }
        #endregion

        #region Custom Methods
        protected override void HandleWorldPhysics()
        {
            base.HandleWorldPhysics();

            Vector3 down = nearestWorld.Vector3Down(transform.position, false);

            HandleStability(down);
        }


        private void HandleStability(Vector3 down)
        {
            RaycastHit hit;
            Vector3 targetPos = transform.position + (down * groundedRange);

            foreach (var joint in joints) {
                joint.axis = joint.transform.InverseTransformDirection(transform.right);
                joint.secondaryAxis = joint.transform.InverseTransformDirection(transform.up);
            }

            //Do a raycast down, which we will use multiple times
            grounded = UnityEngine.Physics.Raycast(transform.position, down, out hit, groundedRange);

            Color color = (grounded) ? Color.green : Color.red;
            Debug.DrawLine(transform.position, targetPos, color);



            if (stun <= 0) {
                rb.freezeRotation = true;
                if (grounded) {
                    StandUp(balance, hit.normal); //This is for the 4 legged variant
                    //StandUp(balance, down);
                }

                if (Controller != null) {
                    handleMovementInput(Controller.Input, down, hit);
                }
            }
            else {
                rb.freezeRotation = false;
                stun -= Time.deltaTime * stunRecoverySpeed;
            }
        }

        private void HandleBasicInput(IInput input)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                braceTime = Time.time;
                SetFlexibility(maxFlex);
            }

            if (Input.GetKeyUp(KeyCode.LeftControl)) {
                //SetFlexibility(0);
            }

            if (animator) {
                if (grounded && Controller != null) {
                    animator.SetFloat("Speed", input.ZAxis);
                }
                else {
                    animator.SetFloat("Speed", 0);
                }
            }
        }

        private void SetFlexibility(float amount)
        {
            foreach (ConfigurableJoint joint in joints) {
                SoftJointLimit limit = joint.linearLimit;
                limit.limit = amount;
                joint.linearLimit = limit;
            }
        }

        /// <summary>
        /// Called when we have an input and are able to move!
        /// </summary>
        /// <param name="down"></param>
        /// <param name="hit"></param>
        private void handleMovementInput(IInput input, Vector3 down, RaycastHit hit)
        {
            transform.rotation *= Quaternion.AngleAxis(input.YRot, Vector3.up); //Does not work...
            //transform.Rotate(down, DWInput.YRot);

            Vector3 targetDirection = Vector3.zero;

            if (grounded) {
                if (input.ZAxis != 0) {
                    //targetDirection += transform.forward * Input.GetAxis("Vertical");
                    targetDirection += Vector3.Cross(transform.right, hit.normal) * input.ZAxis;
                }
                if (input.XAxis != 0) {
                    //targetDirection += transform.right * Input.GetAxis("Horizontal");
                    targetDirection += Vector3.Cross(transform.forward, hit.normal) * -input.XAxis;
                }
            }

            if (targetDirection != Vector3.zero) {
                //Make sure we dont abuse pythag
                if (targetDirection.magnitude > 1f) targetDirection = targetDirection.normalized;

                Vector3 movePoistion = transform.position + (targetDirection * Time.deltaTime * movementSpeed);

                rb.MovePosition(movePoistion);

                foreach (ConfigurableJoint joint in joints) {
                    Rigidbody jrb = joint.GetComponent<Rigidbody>();
                    jrb.MovePosition(jrb.transform.position + (targetDirection * Time.deltaTime * movementSpeed));
                }

                debug = rb.velocity;
            }

            if (input.YAxis > 0 && grounded && !jumping) {
                jumping = true;
                braceTime = 0f;
                rb.AddForce(-down * jumpPower, ForceMode.Impulse);
                rb.AddForce(targetDirection * movementSpeed, ForceMode.VelocityChange);
            }
        }

        private void StandUp(float speed, Vector3 down)
        {
            //Debug.DrawRay(transform.position, transform.position + down * 2f);

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, down) * transform.rotation;

            // The step size is equal to speed times frame time.
            float step = speed * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }

        public void Stun(float seconds)
        {
            stun += seconds;
        }
        #endregion
    }
}