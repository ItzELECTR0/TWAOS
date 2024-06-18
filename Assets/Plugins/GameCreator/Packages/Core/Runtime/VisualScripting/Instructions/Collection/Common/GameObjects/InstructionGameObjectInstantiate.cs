using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Instantiate")]
    [Description("Creates a new instance of a referenced game object")]

    [Category("Game Objects/Instantiate")]
    
    [Parameter("Game Object", "Game Object reference that is instantiated")]
    [Parameter("Position", "The position of the new game object instance")]
    [Parameter("Rotation", "The rotation of the new game object instance")]
    [Parameter("Save", "Optional value where the newly instantiated game object is stored")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayPlus))]
    
    [Keywords("Create", "New", "Game Object")]
    [Serializable]
    public class InstructionGameObjectInstantiate : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetInstantiate m_GameObject = new PropertyGetInstantiate();

        [SerializeField]
        private PropertyGetPosition m_Position = GetPositionCharactersPlayer.Create;

        [SerializeField]
        private PropertyGetRotation m_Rotation = GetRotationCharactersPlayer.Create;

        [SerializeField] 
        private PropertyGetGameObject m_Parent = GetGameObjectNone.Create();

        [SerializeField]
        private PropertySetGameObject m_Save = SetGameObjectNone.Create;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Instantiate {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Vector3 position = this.m_Position.Get(args);
            Quaternion rotation = this.m_Rotation.Get(args);
            
            GameObject instance = this.m_GameObject.Get(args, position, rotation);

            if (instance != null)
            {
                Transform parent = this.m_Parent.Get<Transform>(args);
                if (parent != null) instance.transform.SetParent(parent);
                
                this.m_Save.Set(instance, args);
            }

            return DefaultResult;
        }
    }
}