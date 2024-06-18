using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class GradientFilter : MonoBehaviour
    {
        // Settings
        public Filter selectedFilter = Filter.Dawn;
        [Range(0.1f, 0.9f)] public float opacity = 0.5f;

        // Helpers
        Image bgImage;
        public List<Sprite> filters = new List<Sprite>();

        public enum Filter
        {
            Aqua,
            Dawn,
            Dusk,
            Emerald,
            Kylo,
            Memory,
            Mice,
            Pinky,
            Retro,
            Rock,
            Sunset,
            Violet,
            Warm,
            Random
        }

        void Awake()
        {
            bgImage = GetComponent<Image>();
        }

        void OnEnable()
        {
            UpdateFilter();
        }

        public void UpdateFilter()
        {
            if (selectedFilter == Filter.Random && Application.isPlaying) { bgImage.sprite = filters[Random.Range(0, filters.Count - 1)]; }
            else if (filters.Count >= (int)selectedFilter + 1) { bgImage.sprite = filters[(int)selectedFilter]; }

            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.g, opacity);
        }
    }
}