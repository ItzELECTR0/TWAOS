using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyQuaternion))]
    public class ReflectionPropertyQuaternionDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(Quaternion);
    }
}