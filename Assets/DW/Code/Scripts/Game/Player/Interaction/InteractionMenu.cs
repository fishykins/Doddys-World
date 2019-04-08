using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DW.Player
{
    public class InteractionMenu
    {
        #region Variables
        //public

        //private
        private Canvas canvas;
        private GameObject mainText;
        private float openTime;
        #endregion

        #region Preoperties

        #endregion

        #region Constructors
        public InteractionMenu()
        {
            canvas = PlayerLibrary.instance.interactionCanvas.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            mainText = new GameObject("mainText");
            mainText.transform.parent = canvas.transform;

            RectTransform rect = mainText.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0.5f, 0.5f);
            rect.localPosition = Vector3.zero;
            TextMeshProUGUI tmp = mainText.AddComponent<TextMeshProUGUI>();

            tmp.fontSize = 22;
            tmp.text = "Gooon";
            tmp.alignment = TextAlignmentOptions.Center;

            openTime = Time.fixedTime;
        }
        #endregion

        #region Custom Methods
        public void Destroy()
        {
            UnityEngine.Object.Destroy(mainText);
        }
        #endregion
    }
}