using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy {
    [AddComponentMenu("Camera/Free Camera")]
    public class FreeCamera : MonoBehaviour {
        public float cameraSensitivity = 90;
        public float climbSpeed = 4;
        public float normalMoveSpeed = 10;
        public float slowMoveFactor = 0.25f;
        public float fastMoveFactor = 3;

        public bool altLook = true;

        private float rotationX = 0.0f;
        private float rotationY = 0.0f;
        private float rotationZ = 0.0f;

        private bool cameraEnabled = true;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            bool canLook = (altLook && Input.GetKey(KeyCode.LeftAlt)) || !altLook;

            if (cameraEnabled && canLook) {
                rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
                rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
                rotationZ += Input.GetAxis("Roll") * cameraSensitivity * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, -90, 90);

                transform.localRotation *= Quaternion.AngleAxis(rotationZ, Vector3.forward);
                transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

                float climbSpeedFactor = 1f;

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    climbSpeedFactor = fastMoveFactor;
                    transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                    transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
                }
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                    climbSpeedFactor = slowMoveFactor;
                    transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
                    transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
                }
                else {
                    transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
                    transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                }


                if (Input.GetKey(KeyCode.X)) rotationZ = 0f;

                if (Input.GetKey(KeyCode.Space)) { transform.position += transform.up * climbSpeed * climbSpeedFactor * Time.deltaTime; }
                if (Input.GetKey(KeyCode.C)) { transform.position -= transform.up * climbSpeed * climbSpeedFactor * Time.deltaTime; }
            }

            if (Input.GetKeyDown(KeyCode.End)) {
                cameraEnabled = !cameraEnabled;
                Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}
