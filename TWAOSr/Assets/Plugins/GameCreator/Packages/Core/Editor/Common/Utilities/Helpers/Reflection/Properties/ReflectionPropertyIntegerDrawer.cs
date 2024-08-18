using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyInteger))]
    public class ReflectionPropertyIntegerDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(int);
    }
}