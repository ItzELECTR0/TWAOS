using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyColor))]
    public class ReflectionPropertyColorDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(Color);
    }
}