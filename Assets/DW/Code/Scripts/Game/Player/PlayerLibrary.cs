using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Player
{
    public class PlayerLibrary : MonoBehaviour
    {
        #region Variables
        //public
        public static PlayerLibrary instance;
        public Vector2 spawnPos = new Vector2(0f, 0f);
        public RectTransform interactionCanvas;

        //private

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }

        }
        #endregion;

        #region Custom Methods

        #endregion
    }
}
