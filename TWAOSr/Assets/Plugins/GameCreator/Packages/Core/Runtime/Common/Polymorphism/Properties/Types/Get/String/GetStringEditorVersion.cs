using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Editor Version")]
    [Category("Application/Editor Version")]
    
    [Image(typeof(IconUnity), ColorTheme.Type.Blue)]
    [Description("Returns the current version of the Unity Editor")]
    
    [Serializable]
    public class GetStringEditorVersion : PropertyTypeGetString
    {
        public override string Get(Args args) => Application.unityVersion;

        public override string Get(GameObject gameObject) => Application.unityVersion;

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringEditorVersion()
        );

        public override string String => "App Version";
        
        public override string EditorValue => Application.unityVersion;
    }
}