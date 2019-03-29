using UnityEngine;

namespace Fishy
{
    public class FollowCamera : MonoBehaviour
    {
        public Transform target;
        public float smoothSpeed = 10f;
        public Vector3 offset;
        public float minHeight = 5f;

        public enum behindAxis { x,y,z};
        public behindAxis followAxis;

        void LateUpdate()
        {
            Vector3 targetPosition = Vector3.zero;

            switch (followAxis) {
                case behindAxis.x:
                    targetPosition = target.position + target.right * offset.z;
                    targetPosition.y += offset.y;
                    targetPosition.x += offset.x;
                    break;
                case behindAxis.y:
                    targetPosition = target.position + target.forward * offset.z;
                    targetPosition.y += offset.y;
                    targetPosition.x += offset.x;
                    break;
                case behindAxis.z:
                    targetPosition = target.position + target.up * offset.z;
                    targetPosition.y += offset.y;
                    targetPosition.x += offset.x;
                    break;
            }

            RaycastHit hit;
            Color rayCol = Color.red;
            if (UnityEngine.Physics.Raycast(transform.position, -transform.up, out hit, minHeight)) {
                targetPosition.y = hit.point.y + minHeight;
            }

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            transform.LookAt(target);
        }
    }
}