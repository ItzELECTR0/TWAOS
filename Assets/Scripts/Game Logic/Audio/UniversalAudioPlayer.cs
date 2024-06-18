using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Audio;

namespace ELECTRIS
{
    public class UniversalAudioPlayer : MonoBehaviour
    {
        [SerializeField]private AudioSource audioPlayer;
        [SerializeField]private AudioClip[] audioFile;
        public int SoundOnStart;
        public bool PlayOnStart = false;
        public bool LoopOnStart = false;
        public bool RandomOnStart = false;
        public bool ShuffleOnStart = false;
        private int GetUniqueRandomIndex(List<int> playedIndices)
        {
            List<int> unplayedIndices = new List<int>();
            for (int i = 0; i < audioFile.Length; i++)
            {
                if (!playedIndices.Contains(i))
                {
                    unplayedIndices.Add(i);
                }
            }

            if (unplayedIndices.Count == 0)
            {
                return -1; // All songs have been played
            }

            int randomIndex = unplayedIndices[Random.Range(0, unplayedIndices.Count)];
            return randomIndex;
        }

        private void Awake()
        {
            audioPlayer = GetComponent<AudioSource>();
            if (PlayOnStart == true)
            {
                Play(SoundOnStart, LoopOnStart, RandomOnStart, ShuffleOnStart);
            }
        }

        // Randomized Play
        public void PlayRandomAudio(bool Loop, bool Shuffle)
        {

            if (Shuffle)
            {
                StartCoroutine(RandomShuffle(Loop));
                return;
            }else if (!Shuffle)
            {
                int randomIndex = Random.Range(0, audioFile.Length);
                Play(randomIndex, Loop);
            }
        }

        // Randomized Shuffle Play
        private IEnumerator RandomShuffle(bool Loop)
        {
            List<int> playedIndices = new List<int>();

            while (true)
            {
                int randomIndex = GetUniqueRandomIndex(playedIndices);
                if (randomIndex == -1)
                {
                    Debug.Log("All songs have been played.");

                    if (Loop) //Continue shuffling
                    {
                        playedIndices.Clear();
                        yield return null;
                        continue;
                    }else if (!Loop) // Stop playing all together
                    {
                        audioPlayer.Stop();
                        yield break;
                    }
                }

                Play(randomIndex, Loop);
                yield return new WaitForSeconds(audioPlayer.clip.length);
                playedIndices.Add(randomIndex);
            }
        }


        public void Play(int soundIndex, bool loop = false, bool random = false, bool shuffle = false, float volume = 1f, bool fadeIn = false, float duration = 0.5f)
        {
            if (random == true) // Play using Random instead of regular sound index
            {
                if (RandomOnStart == true) 
                {
                    PlayRandomAudio(LoopOnStart, ShuffleOnStart);
                    return;
                }else
                {
                    PlayRandomAudio(loop, shuffle);
                    return;
                }
            }

            if (soundIndex < 0 || soundIndex >= audioFile.Length)
            {
                Debug.LogError("Invalid sound index");
                return;
            }

            audioPlayer.loop = loop;
            audioPlayer.volume = Mathf.Clamp01(volume);

            if (fadeIn)
            {
                StartCoroutine(FadeIn(duration));
            }

            audioPlayer.clip = audioFile[soundIndex];
            audioPlayer.Play();
        }

        public void PlayByButton(int soundIndex)
        {
            audioPlayer.clip = audioFile[soundIndex];
            audioPlayer.Play();
        }

        public IEnumerator FadeIn(float duration)
        {
            float targetVolume = audioPlayer.volume;
            audioPlayer.volume = 0f;

            while (audioPlayer.volume < targetVolume)
            {
                audioPlayer.volume += Time.deltaTime / duration;
                yield return null;
            }
        }

        public IEnumerator FadeOut(float duration)
        {
            float startVolume = audioPlayer.volume;

            while (audioPlayer.volume > 0)
            {
                audioPlayer.volume -= startVolume * Time.deltaTime / duration;
                yield return null;
            }

            audioPlayer.loop = false;
            audioPlayer.Stop();
        }

        public void SwitchLoop(bool Loop)
        {
            audioPlayer.loop = Loop;
        }

        public void StopAudio()
        {
            audioPlayer.Stop();
        }

        public void PauseOrResume(bool pauseOrResume)
        {
            if (audioPlayer.isPlaying)
            {
                if (pauseOrResume)
                {
                    audioPlayer.UnPause();
                }
                else
                {
                    audioPlayer.Pause();
                }
            }
            else
            {
                Debug.LogError("Cannot pause or resume. Audio player is not playing.");
            }
        }
    }
}
