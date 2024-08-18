using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldDouble))]
    public class ReflectionFieldDoubleDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(double);
    }
}