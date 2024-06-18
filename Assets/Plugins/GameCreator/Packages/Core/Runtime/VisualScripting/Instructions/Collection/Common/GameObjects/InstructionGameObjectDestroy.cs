using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Destroy")]
    [Description("Destroys a game object scene instance")]

    [Category("Game Objects/Destroy")]

    [Keywords("Remove", "Delete", "Flush", "MonoBehaviour", "Behaviour", "Script")]
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Red, typeof(OverlayMinus))]
    
    [Serializable]
    public class InstructionGameObjectDestroy : TInstructionGameObject
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Destroy {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return DefaultResult;
            
            UnityEngine.Object.Destroy(gameObject);
            return DefaultResult;
        }
    }
}