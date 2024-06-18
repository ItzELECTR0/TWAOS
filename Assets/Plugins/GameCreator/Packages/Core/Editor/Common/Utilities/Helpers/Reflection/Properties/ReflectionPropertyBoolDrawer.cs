using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyBool))]
    public class ReflectionPropertyBoolDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(bool);
    }
}