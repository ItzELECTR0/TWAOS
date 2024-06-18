using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldGameObject))]
    public class ReflectionFieldGameObjectDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(GameObject);
    }
}