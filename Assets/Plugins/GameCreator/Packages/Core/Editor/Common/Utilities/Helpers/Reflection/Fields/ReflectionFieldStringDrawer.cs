using System;
using GameCreator.Runtime.Common;
using UnityEditor;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldString))]
    public class ReflectionFieldStringDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(string);
    }
}