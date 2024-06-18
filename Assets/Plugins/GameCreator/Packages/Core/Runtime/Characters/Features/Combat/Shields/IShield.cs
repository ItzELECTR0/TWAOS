using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    public interface IShield
    {
        int Priority { get; }
        string Name  { get; }
        
        // METHODS: -------------------------------------------------------------------------------

        float GetDefense(Args args);
        float GetCooldown(Args args);
        float GetRecovery(Args args);
        float GetAngle(Args args);
        
        ShieldOutput CanDefend(Character character, Args args, ShieldInput input);
        void OnDefend(Args args, ShieldOutput shieldOutput, ReactionInput reaction);

        void OnRaise(Character character);
        void OnLower(Character character);
    }
}