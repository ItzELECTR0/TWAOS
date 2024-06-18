using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldFloat))]
    public class ReflectionFieldFloatDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(float);
    }
}