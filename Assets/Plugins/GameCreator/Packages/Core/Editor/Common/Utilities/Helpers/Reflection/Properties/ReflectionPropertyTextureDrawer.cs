using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertyTexture))]
    public class ReflectionPropertyTextureDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(Texture);
    }
}