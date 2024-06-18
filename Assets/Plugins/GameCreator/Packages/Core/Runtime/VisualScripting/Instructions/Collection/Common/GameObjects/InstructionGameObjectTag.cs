using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Tag")]
    [Description("Changes the Tag of a game object")]

    [Parameter(
        "Tag", 
        "The tag value which the game object belongs to"
    )]

    [Category("Game Objects/Change Tag")]
    [Keywords("MonoBehaviour", "Behaviour", "Script")]
    
    [Image(typeof(IconTag), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionGameObjectTag : TInstructionGameObject
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private TagValue m_Tag = new TagValue();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Change Tag to {0} on {1}",
            this.m_Tag,
            this.m_GameObject
        );

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;

            gameObject.tag = this.m_Tag.Value;
            return DefaultResult;
        }
    }
}