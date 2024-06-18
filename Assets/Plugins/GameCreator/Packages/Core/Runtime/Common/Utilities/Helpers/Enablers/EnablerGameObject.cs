using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerGameObject : TEnablerValue<GameObject>
    {
        public EnablerGameObject()
        { }

        public EnablerGameObject(GameObject value) : base(false, value)
        { }
        
        public EnablerGameObject(bool isEnabled, GameObject value) : base(isEnabled, value)
        { }
    }
}