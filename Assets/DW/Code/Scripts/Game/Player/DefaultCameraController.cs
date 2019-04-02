using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW
{
    public class DefaultCameraController : MonoBehaviour, ICameraController
    {
        #region Variables
        //Public


        //Private
        private Transform camera;
        private Transform target;
        #endregion

        private void Awake()
        {
            target = transform;
        }

        // Start is called before the first frame update
        public void AssignCamera(Transform camera)
        {
            this.camera = camera;
        }

        public void UpdateCamera()
        {

        }
    }
}