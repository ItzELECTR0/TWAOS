using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyGameObject))]
    public class ReflectionPropertyGameObjectDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(GameObject);
    }
}