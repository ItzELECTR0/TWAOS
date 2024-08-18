using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldTexture))]
    public class ReflectionFieldTextureDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(Texture);
    }
}