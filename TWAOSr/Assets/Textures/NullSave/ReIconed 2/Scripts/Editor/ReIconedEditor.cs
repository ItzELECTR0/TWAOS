using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

namespace NullSave
{
    [CustomEditor(typeof(ReIconed))]
    public class ReIconedEditor : CogEditor
    {

        #region Unity Methods

        public override void OnInspectorGUI()
        {
            MainContainerBegin();

            SectionHeader("Controller Maps", "controllerMaps", typeof(ControllerMap));
            SimpleList("controllerMaps", false);

            MainContainerEnd();
        }

        #endregion

    }
}