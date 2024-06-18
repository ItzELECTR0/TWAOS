using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(TypeReferenceBehaviour))]
    public class TypeReferenceBehaviourDrawer : TypeReferenceDrawer
    {
        protected override Type Base => typeof(Behaviour);
    }
}