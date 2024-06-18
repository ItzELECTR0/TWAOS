using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyVector3))]
    public class ReflectionPropertyVector3Drawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(Vector3);
    }
}