using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Player
{
    //TODO: Replace the millions of public floats with Vector3's. Much more fun
    public class PlayerController : MonoBehaviour, IInput
    {
        #region Variables
        //Public


        //Private
        private float xAxis = 0f;
        private float yAxis = 0f;
        private float zAxis = 0f;
        private float xRot = 0f;
        private float yRot = 0f;
        private float zRot = 0f;

        #endregion

        #region Properties
        public float XAxis { get { return xAxis; } }
        public float YAxis { get { return yAxis; } }
        public float ZAxis { get { return zAxis; } }
        public float XRot { get { return xRot; } }
        public float YRot { get { return yRot; } }
        public float ZRot { get { return zRot; } }
        public bool Interaction { get { return Input.GetKey(KeyCode.F); } }
        #endregion

        #region Unity Methods
        private void Update()
        {
            HandleInput();
        }
        #endregion

        #region Custom Methods
        private void HandleInput()
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                xAxis = Input.GetAxis("Horizontal");
                yAxis = Input.GetAxis("Jump");
                zAxis = Input.GetAxis("Vertical");
                yRot = Input.GetAxis("Mouse X");
                xRot = Input.GetAxis("Mouse Y");
            }
        }

        #endregion
    }
}
