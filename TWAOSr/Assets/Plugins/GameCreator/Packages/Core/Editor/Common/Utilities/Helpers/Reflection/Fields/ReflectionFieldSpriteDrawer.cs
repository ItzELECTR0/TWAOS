using System;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEngine;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(ReflectionFieldSprite))]
    public class ReflectionFieldSpriteDrawer : TReflectionFieldDrawer
    {
        protected override Type AcceptType => typeof(Sprite);
    }
}