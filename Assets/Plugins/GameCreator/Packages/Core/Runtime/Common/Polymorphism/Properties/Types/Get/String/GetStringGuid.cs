using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Unique ID")]
    [Category("Random/Unique ID")]
    
    [Image(typeof(IconID), ColorTheme.Type.Yellow)]
    [Description("Returns a new globally unique ID string value")]
    
    [Serializable]
    public class GetStringGuid : PropertyTypeGetString
    {
        public override string Get(Args args) => UniqueID.GenerateID();
        
        public override string Get(GameObject gameObject) => UniqueID.GenerateID();

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringGuid()
        );
        
        public override string String => "UID";
    }
}