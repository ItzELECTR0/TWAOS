using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Busy
    {
        /**
         * 
         * The limb system is split in 2 groups: arms and legs. Each of these groups is split in
         * two other categories: left and right.
         * 
         * +-------+-------+-------+-------+
         * |   3   |   2   |   1   |   0   |
         * +-------+-------+-------+-------+
         * | LEG-R | LEG-L | ARM-R | ARM-L |
         * +-------+-------+-------+-------+
         *
         * The Limb enum is used as a mask to detect which parts of the body are being used by
         * some other system.
         * 
        **/
        
        public enum Limb
        {
            ArmLeft  = 1,  // binary -> 0001
            ArmRight = 2,  // binary -> 0010
            LegLeft  = 4,  // binary -> 0100
            LegRight = 8,  // binary -> 1000
            Arms     = 3,  // binary -> 0011
            Legs     = 12, // binary -> 1100
            Every    = 15, // binary -> 1111
            None     = 0,  // binary -> 0000
        }
        
        // EVENTS: --------------------------------------------------------------------------------

        public event Action<Limb> EventChange;

        // MEMBERS: -------------------------------------------------------------------------------
        
        private Character m_Character;
        
        private Limb m_State = Limb.None;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Limb State
        {
            get => this.m_State;
            private set
            {
                this.m_State = value;
                this.EventChange?.Invoke(this.m_State);
            }
        }
        
        public bool IsArmLeftBusy  => ArmLeftBusy(this.m_State);
        public bool IsArmRightBusy => ArmRightBusy(this.m_State);
        
        public bool IsLegLeftBusy  => LegLeftBusy(this.m_State);
        public bool IsLegRightBusy => LegRightBusy(this.m_State);

        public bool AreArmsBusy => ArmsBusy(this.m_State);
        public bool AreLegsBusy => LegsBusy(this.m_State);

        public bool IsBusy => WholeBodyBusy(this.m_State);

        // INITIALIZERS: --------------------------------------------------------------------------

        internal void OnStartup(Character character)
        {
            this.m_Character = character;
        }
        
        internal void AfterStartup(Character character)
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetBusy() => this.AddState(Limb.Every);
        public void SetAvailable() => this.RemoveState(Limb.Every);

        public void MakeArmLeftBusy()  => this.AddState(Limb.ArmLeft);
        public void MakeArmRightBusy() => this.AddState(Limb.ArmRight);

        public void MakeLegLeftBusy()  => this.AddState(Limb.LegLeft);
        public void MakeLegRightBusy() => this.AddState(Limb.LegRight);
        
        public void MakeArmsBusy() => this.AddState(Limb.Arms);
        public void MakeLegsBusy() => this.AddState(Limb.Legs);
        
        public void RemoveArmLeftBusy()  => this.RemoveState(Limb.ArmLeft);
        public void RemoveArmRightBusy() => this.RemoveState(Limb.ArmRight);

        public void RemoveLegLeftBusy()  => this.RemoveState(Limb.LegLeft);
        public void RemoveLegRightBusy() => this.RemoveState(Limb.LegRight);
        
        public void RemoveArmsBusy() => this.RemoveState(Limb.Arms);
        public void RemoveLegsBusy() => this.RemoveState(Limb.Legs);
        
        public async Task Timeout(Limb limbs, float timeout)
        {
            this.AddState(limbs);
            float startTime = this.m_Character.Time.Time;
            while (!ApplicationManager.IsExiting && this.m_Character.Time.Time < startTime + timeout)
            {
                await Task.Yield();
            }

            this.RemoveState(limbs);
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static bool ArmLeftBusy(Limb state) => ((int)state & (int)Limb.ArmLeft) > 0;
        public static bool ArmRightBusy(Limb state) => ((int)state & (int)Limb.ArmRight) > 0;
        
        public static bool LegLeftBusy(Limb state) => ((int)state & (int)Limb.LegLeft) > 0;
        public static bool LegRightBusy(Limb state) => ((int)state & (int)Limb.LegRight) > 0;
        
        public static bool ArmsBusy(Limb state) => ((int)state & (int)Limb.Arms) > 0;
        public static bool LegsBusy(Limb state) => ((int)state & (int)Limb.Legs) > 0;
        
        public static bool WholeBodyBusy(Limb state) => ((int)state & (int)Limb.Every) > 0;

        public void AddState(Limb limbs)
        {
            int currState = (int) this.m_State;
            int nextState = (int) limbs;
            this.State = (Limb) (currState | nextState);
        }
        
        public void RemoveState(Limb limbs)
        {
            int currState = (int) this.m_State;
            int nextState = (int) limbs;
            this.State = (Limb) (currState & ~nextState);
        }
    }
}
