using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy {
	public static class UnityScene  {
        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj) {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform) {
                if (null == child) {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        public static void SetLayerRecursively(GameObject obj, string newLayer)
        {
            SetLayerRecursively(obj, LayerMask.NameToLayer(newLayer));
        }


        public static void SetLayerRecursively(GameObject obj, LayerMask newLayer)
        {
            SetLayerRecursively(obj, newLayer.value);
        }
    }
}
