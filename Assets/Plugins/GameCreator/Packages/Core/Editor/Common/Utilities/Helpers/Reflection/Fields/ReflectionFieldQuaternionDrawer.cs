using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldQuaternion))]
    public class ReflectionFieldQuaternionDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(Quaternion);
    }
}