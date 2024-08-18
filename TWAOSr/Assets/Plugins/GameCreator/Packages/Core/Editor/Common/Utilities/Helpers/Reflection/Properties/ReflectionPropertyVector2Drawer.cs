using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyVector2))]
    public class ReflectionPropertyVector2Drawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(Vector2);
    }
}