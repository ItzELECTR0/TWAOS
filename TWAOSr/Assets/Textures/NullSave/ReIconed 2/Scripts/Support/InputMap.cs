using System;
using TMPro;
using UnityEngine;

namespace NullSave
{
    [Serializable]
    public class InputMap
    {

        #region Variables

        public string inputName;
        public Sprite unitySprite;
        public int tmpSpriteIndex;

        #endregion

        #region Properties

        public TMP_SpriteAsset TMPSpriteAsset { get; set; }

        #endregion

    }
}