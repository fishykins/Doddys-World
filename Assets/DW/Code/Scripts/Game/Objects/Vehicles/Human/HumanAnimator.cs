using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Objects
{
    /// <summary>
    /// This automatically applies animations to the model, independant of body and controller. 
    /// This allows for a player to be controlled locally, or over network and still maintain a consistent animator
    /// </summary>
    public class HumanAnimator : MonoBehaviour
    {
        #region Variables
        //public
        public float maxSpeed = 5f;
        public float maxAngularSpeed = 180f;
        [Range(0f,1f)]
        public float lerpAmount = 0.2f;

        //private
        private Animator animator;
        private Rigidbody rb;
        private Vector3 lastPos;
        private Vector3 lastVector;
        private float direction;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            //Speed
            float velocity = (rb.position - lastPos).magnitude / Time.fixedDeltaTime;
            lastPos = rb.position;
            float speed = Mathf.InverseLerp(0f, maxSpeed, velocity);
            animator.SetFloat("Speed", velocity);

            //Direction
            float angle = Vector3.SignedAngle(lastVector, transform.forward, transform.up) / Time.fixedDeltaTime;
            lastVector = transform.forward;

            direction = Mathf.Lerp(direction, angle, lerpAmount);

            animator.SetFloat("Direction", direction / maxAngularSpeed);
        }


        #endregion

        #region Custom Methods

        #endregion

    }
}