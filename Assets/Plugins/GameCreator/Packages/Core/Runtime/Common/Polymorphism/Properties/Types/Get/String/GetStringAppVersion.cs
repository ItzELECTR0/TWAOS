using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("App Version")]
    [Category("Application/App Version")]
    
    [Image(typeof(IconApplication), ColorTheme.Type.Blue)]
    [Description("Returns the current version of the Application")]
    
    [Serializable]
    public class GetStringAppVersion : PropertyTypeGetString
    {
        public override string Get(Args args) => Application.version;

        public override string Get(GameObject gameObject) => Application.version;

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringAppVersion()
        );

        public override string String => "App Version";

        public override string EditorValue => Application.version;
    }
}