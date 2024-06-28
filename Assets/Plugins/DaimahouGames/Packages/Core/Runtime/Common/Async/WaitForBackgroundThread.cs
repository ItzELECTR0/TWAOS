using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DaimahouGames.Runtime.Core.Common
{
    public class WaitForBackgroundThread
    {
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => {}).ConfigureAwait(false).GetAwaiter();
        }
    }
}
