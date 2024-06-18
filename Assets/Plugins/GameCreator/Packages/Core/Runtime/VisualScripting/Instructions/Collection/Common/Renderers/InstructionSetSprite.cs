using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Change Sprite")]
    [Description("Sets the Sprite value")]

    [Category("Renderer/Change Sprite")]

    [Parameter("To", "Where to store the new Sprite value")]
    [Parameter("Sprite", "The Sprite value to be stored")]

    [Keywords("Texture", "Renderer")]
    [Image(typeof(IconSprite), ColorTheme.Type.Purple)]
    
    [Serializable]
    public class InstructionSetSprite : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected PropertySetSprite m_To = new PropertySetSprite();
        [SerializeField] private PropertyGetSprite m_Sprite = GetSpriteInstance.Create();

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_To} = {this.m_Sprite}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Sprite sprite = this.m_Sprite.Get(args);
            this.m_To.Set(sprite, args);
            
            return DefaultResult;
        }
    }
}