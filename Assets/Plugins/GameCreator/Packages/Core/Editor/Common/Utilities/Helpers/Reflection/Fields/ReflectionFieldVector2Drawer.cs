using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldVector2))]
    public class ReflectionFieldVector2Drawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(Vector2);
    }
}