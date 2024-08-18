using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldVector3))]
    public class ReflectionFieldVector3Drawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(Vector3);
    }
}