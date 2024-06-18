using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Console
{
    public class ActionGameObjectsCollection : TActionCollection<ActionGameObject>
    {
        public override IEnumerable<ActionGameObject> Get => new []
        {
            new ActionGameObject(
                "name",
                "Finds a game object by name",
                GameObject.Find
            ),
            new ActionGameObject(
                "tag",
                "Finds a game object by its tag",
                GameObject.FindWithTag
            ),
            new ActionGameObject(
                "type",
                "Finds a game object by its component type",
                value =>
                {
                    Type type = Type.GetType(value);
                    if (type == null) return null;
                    
                    Component result = UnityEngine.Object.FindAnyObjectByType(type) as Component;
                    return result != null ? result.gameObject : null;
                }
            )
        };
    }
}