using UnityEngine;
using System.Collections;
using DW;
using DW.Vehicles;

namespace UMA
{
    public class PhysicsBodySlotScript : MonoBehaviour
    {
        // Start is called before the first frame update
        public void OnDnaApplied(UMAData umaData)
        {
            HumanBody body = umaData.gameObject.GetComponent<HumanBody>();
            if (body == null)
            {
                body = umaData.gameObject.AddComponent<HumanBody>();
                body.applyGravity = true;
            }

            IVehicleController controller = umaData.transform.parent.GetComponent<IVehicleController>();
            if (controller != null)
            {
                controller.AddPhysicsBody(body);
            }

        }
    }
}