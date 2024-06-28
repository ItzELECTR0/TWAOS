using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DaimahouGames.Runtime.Core.Common
{
    public static class AwaitExtensions
    {
        public static TaskAwaiter<int> GetAwaiter(this Process process)
        {
            var tcs = new TaskCompletionSource<int>();
            process.EnableRaisingEvents = true;

            process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);

            if (process.HasExited)
            {
                tcs.TrySetResult(process.ExitCode);
            }

            return tcs.Task.GetAwaiter();
        }
    }
}
