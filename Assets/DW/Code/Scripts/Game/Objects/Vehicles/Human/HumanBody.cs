using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;

namespace DW.Objects
{
    public class HumanBody : PhysicsBody, IPhysicsBody
    {
        #region Variables
        //Public & Serialized
        [SerializeField]
        private float groundedRange = 3f;
        [SerializeField]
        private float movementSpeed = 5f;
        [SerializeField]
        private float jumpPower = 2000f;
        [SerializeField]
        private float turnSpeed = 180f;
        [SerializeField]
        private float turningSpeedBonus = 180f;
        [SerializeField]

        [HideInInspector]
        public float targetDirection;

        //Private
        private bool grounded = false;
        private bool jumping = false;
        private float stun = 0f; //how long a human is stunned for. 
        private float braceTime = 0f; //Time that player hit "crouch"
        private Vector3 down;
        private float yDirection;
        private Vector3 currentRotation;
        private Vector3 smmothVelocity;
        private Animator animator;
        #endregion;

        #region Properties
        public Vector3 Down { get { return down; } }
        #endregion;

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }
        protected override void Start()
        {
            base.Start();
            //Disable auto rotation- we will only want this if we are in ragdoll mode
            rb.freezeRotation = true;
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

            if (controller != null)
            {
                if (controller.Input != null)
                {
                    HandleInputThirdPerson(controller.Input, hit);
                }
                else
                {
                    Debug.Log("No input");
                }
            }
            else
            {
                Debug.Log("No Controller");
            }

            Color color = (grounded) ? Color.green : Color.red;
            Debug.DrawLine(feetPos, targetPos, color);
        }

        /// <summary>
        /// Third person mode input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hit"></param>
        protected virtual void HandleInputThirdPerson(IInput input, RaycastHit hit)
        {
            float targetSpeed = movementSpeed * Mathf.Clamp01(new Vector2(input.XAxis, input.ZAxis).magnitude);
            Vector3 forward = Vector3.Cross(transform.right, hit.normal);
            Vector3 movePoistion = rb.position + (forward * Time.fixedDeltaTime * targetSpeed);
            rb.MovePosition(movePoistion);

            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, -down);
            Vector2 walkingDirection = new Vector2(input.XAxis, input.ZAxis).normalized;
            float turnBonus = Mathf.InverseLerp(movementSpeed, 0f, targetSpeed) * turningSpeedBonus;
            float step = (turnSpeed + turnBonus) * Time.fixedDeltaTime;

            if (walkingDirection != Vector2.zero)
            {
                yDirection = (Mathf.Atan2(walkingDirection.x, walkingDirection.y) * Mathf.Rad2Deg) + targetDirection;
                targetRotation *= Quaternion.AngleAxis(yDirection, Vector3.up);
                rb.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            }
        }
        #endregion
    }
}
