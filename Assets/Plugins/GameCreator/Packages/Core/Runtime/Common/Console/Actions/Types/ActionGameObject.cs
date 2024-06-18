using System;
using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public class ActionGameObject : TAction<GameObject>
    {
        public ActionGameObject(string name, string description, Func<string, GameObject> method) 
            : base(name, description, method)
        { }
    }
}