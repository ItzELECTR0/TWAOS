using System.Threading.Tasks;
using UnityEngine;

namespace GameCreator.Runtime.Common.Audio
{
    internal interface IAudioChannel
    {
        Task Play(AudioClip audioClip, IAudioConfig audioConfig, Args args);
        Task Stop(AudioClip audioClip, float transitionOut);

        Task StopAll(float transition);
    }
}