using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyFloat))]
    public class ReflectionPropertyFloatDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(float);
    }
}