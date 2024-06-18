using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyString))]
    public class ReflectionPropertyStringDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(string);
    }
}