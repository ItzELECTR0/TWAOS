using System.Threading.Tasks;

namespace DaimahouGames.Runtime.Core.Common
{
    public static class Async
    {
        public static async void Call(Task task) => await task;
    }
}