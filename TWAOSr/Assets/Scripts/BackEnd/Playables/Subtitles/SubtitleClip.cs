using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.Playables;

namespace ELECTRIS
{
    public class SubtitleClip : PlayableAsset
    {
        public string subtitleText;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
            SubtitleBehaviour subtitleBehaviour = playable.GetBehaviour();
            subtitleBehaviour.subtitleText = subtitleText;

            return playable;
        }
    }
}