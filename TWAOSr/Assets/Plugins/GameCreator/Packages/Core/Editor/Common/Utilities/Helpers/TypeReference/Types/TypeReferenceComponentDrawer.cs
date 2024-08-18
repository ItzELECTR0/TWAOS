using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(TypeReferenceComponent))]
    public class TypeReferenceComponentDrawer : TypeReferenceDrawer
    {
        protected override Type Base => typeof(Component);
    }
}