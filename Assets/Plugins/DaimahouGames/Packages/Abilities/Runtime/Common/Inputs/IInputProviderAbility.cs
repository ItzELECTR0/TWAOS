using System.Threading.Tasks;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;

namespace DaimahouGames.Runtime.Abilities
{
    public interface IInputProviderAbility : IInputProvider
    {
        Task GetTargetLocation(ExtendedArgs args, Indicator indicator, float radius);
        Pawn GetTargetUnit();
        Task GetTargetDirection(ExtendedArgs args, float distance);
        string GetInputName(int slot);
    }
}