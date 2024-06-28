using System;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Core.Common
{
    [Title("Self hierarchy")]
    [Category("Self hierarchy")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Pink)]
    [Description("Reference to a game object in the parent hierarchy")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectInParent : PropertyTypeGetGameObject
    {
        private enum TypeOption
        {
            Character,
            Pawn
        }

        [SerializeField] private TypeOption m_TargetType;
        
        private Type TargetType
        {
            get
            {
                return m_TargetType switch
                {
                    TypeOption.Character => typeof(Character),
                    TypeOption.Pawn => typeof(Pawn),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        public override GameObject Get(Args args)
        {
            return FindGameObjectInParent(args.Self);
        }

        public override GameObject Get(GameObject gameObject)
        {
            return FindGameObjectInParent(gameObject);
        }

        private GameObject FindGameObjectInParent(GameObject args)
        {
            var parent = args.transform;
            while (parent != null)
            {
                var character = parent.GetComponent(TargetType);
                if (character) return character.gameObject;
                parent = parent.transform.parent;
            }

            return default;
        }

        public static PropertyGetGameObject Create => new(new GetGameObjectCharactersInstance());

        public override string String => $"{m_TargetType} in hierarchy";
    }
}