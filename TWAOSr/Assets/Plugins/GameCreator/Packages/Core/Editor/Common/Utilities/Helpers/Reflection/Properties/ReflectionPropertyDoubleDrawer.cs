using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyDouble))]
    public class ReflectionPropertyDoubleDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(double);
    }
}