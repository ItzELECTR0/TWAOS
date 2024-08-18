using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldColor))]
    public class ReflectionFieldColorDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(Color);
    }
}