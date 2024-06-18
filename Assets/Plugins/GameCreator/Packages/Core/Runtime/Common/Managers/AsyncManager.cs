using System.Threading;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    public class AsyncManager : Singleton<AsyncManager>
    {
        private CancellationTokenSource ExitToken { get; set; }

        // STATIC ACCESSORS: ----------------------------------------------------------------------

        public static bool ExitRequest => 
            Instance == null || Instance.ExitToken.IsCancellationRequested;

        // INITIALIZER: ---------------------------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();
            this.ExitToken = new CancellationTokenSource();
        }

        // CONTROL METHODS: -----------------------------------------------------------------------

        private void OnApplicationQuit()
        {
            this.ExitToken?.Cancel();
        }
    }
}