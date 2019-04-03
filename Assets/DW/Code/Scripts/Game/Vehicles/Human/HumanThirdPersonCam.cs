using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Vehicles;

namespace DW
{
    public class HumanThirdPersonCam : MonoBehaviour, ICameraController
    {
        #region Variables
        //Public
        [SerializeField]
        private float mouseSensitivity = 10f;
        [SerializeField]
        private float orbitDistance = 4f;
        [SerializeField]
        private Vector2 pitchMinMax = new Vector2(-40, 80);
        [SerializeField]
        public float rotationSmoothTime = 1.2f;
        [SerializeField]
        private Transform target;
        [SerializeField]
        private bool invert = true;

        //Private
        private Transform cam;
        private float yaw, pitch;
        private HumanController controller;
        private HumanBody body;
        private Vector3 currentRotation;
        private Vector3 smmothVelocity;
        #endregion

        #region Properties
        public Quaternion Rotation { get { return cam.rotation; } }
        #endregion

        private void Awake()
        {
            controller = GetComponent<HumanController>();
            body = GetComponent<HumanBody>();
            if (!target)
                target = transform;

            if (!body || !controller) Debug.LogError(gameObject.name + " does not have a human body and/or a controller");
        }

        // Start is called before the first frame update
        public void AssignCamera(Transform cam)
        {
            this.cam = cam;
        }

        /// <summary>
        /// This is a sexy method that took way too long to write
        /// </summary>
        public void UpdateCamera()
        {
            IInput input = controller.Input;
            if (input == null || !cam) return;

            yaw += input.YRot * mouseSensitivity;
            pitch += input.XRot * ((invert) ? mouseSensitivity : -mouseSensitivity);
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref smmothVelocity, rotationSmoothTime);
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, -body.Down);

            cam.rotation = targetRotation * Quaternion.Euler(currentRotation);
            cam.position = target.position - cam.forward * orbitDistance;

            body.targetDirection = yaw;
        }
    }
}