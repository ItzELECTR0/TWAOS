using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldInteger))]
    public class ReflectionFieldIntegerDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(int);
    }
}