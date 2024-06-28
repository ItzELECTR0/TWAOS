using DaimahouGames.Runtime.Core.Common;
using IState = DaimahouGames.Runtime.Core.Common.IState;

namespace DaimahouGames.Runtime.Pawns
{
    public interface IPawnState : IState
    {
        Pawn Pawn { get; }
        CursorType Cursor { get; set; }
        void Initialize(Pawn pawn);
        void ForceDefaultState();
    }
}