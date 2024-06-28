using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using DaimahouGames.Runtime.Core;
using UnityEngine;
using TState = DaimahouGames.Runtime.Core.Common.TState;

namespace DaimahouGames.Runtime.Pawns
{
    public class TPawnState: TState, IPawnState, IGenericItem
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        #region EditorInfo
        public virtual string Title { get; protected set; } = "feature - (no name)";
        public virtual Color Color => ColorTheme.Get(ColorTheme.Type.TextNormal);
        public bool IsExpanded { get; set; }
        public virtual string[] Info { get; } = Array.Empty<string>();
        #endregion
        
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        
        public Pawn Pawn { get; private set; }
        public CursorType Cursor { get; set; }
        public Character Character { get; private set; }
        
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        
        protected TPawnState(FiniteStateMachine ownerFiniteStateMachine) : base(ownerFiniteStateMachine) {}
        
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※
        // ※  Public Methods: --------------------------------------------------------------------------------------|※
        
        public async Task WaitUntilComplete()
        {
            await Awaiters.While(() => IsActive);
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※

        protected virtual void Initialize() { }

        protected void SetControllable(bool controllable)
        {
            if(Character) Character.Player.IsControllable = controllable;
        }

        protected void StopMovement()
        {
            if(Character) Character.Motion.MoveToDirection(Vector3.zero, Space.World, 0);
        }

        // ※  Private Methods: -------------------------------------------------------------------------------------|※

        void IPawnState.Initialize(Pawn pawn)
        {
            Pawn = pawn;
            Character = pawn.GetComponent<Character>();
            Initialize();
        }
        
        //============================================================================================================||
    }
}