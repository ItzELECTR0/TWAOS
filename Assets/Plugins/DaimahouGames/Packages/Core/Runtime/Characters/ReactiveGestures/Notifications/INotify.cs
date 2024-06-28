using System.Threading.Tasks;
using GameCreator.Runtime.Characters;

namespace DaimahouGames.Runtime.Characters
{
    public interface INotify
    {
        Task Update(Character character, float progressLastFrame, float currentProgress);
    }
}