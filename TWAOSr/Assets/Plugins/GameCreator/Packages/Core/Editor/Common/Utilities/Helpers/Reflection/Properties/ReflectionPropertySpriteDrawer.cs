using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionPropertySprite))]
    public class ReflectionPropertySpriteDrawer : TReflectionPropertyDrawer
    {
        protected override Type AcceptType => typeof(Sprite);
    }
}