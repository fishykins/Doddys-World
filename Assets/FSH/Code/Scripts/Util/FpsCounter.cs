using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy {
    [AddComponentMenu("Debug/FPS Counter")]
    public class FpsCounter : MonoBehaviour {

        public int fpsLimit = -1;

        private void Awake()
        {
            SetTargetFramerate();
        }

        private void OnValidate()
        {
            SetTargetFramerate();
        }

        private void SetTargetFramerate()
        {
            Application.targetFrameRate = fpsLimit;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(0, 0, 100, 100), "" + (int)(1.0f / Time.smoothDeltaTime));
        }
    }
}
