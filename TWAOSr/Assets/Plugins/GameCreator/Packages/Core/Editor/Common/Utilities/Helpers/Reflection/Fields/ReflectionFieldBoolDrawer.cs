using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldBool))]
    public class ReflectionFieldBoolDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(bool);
    }
}