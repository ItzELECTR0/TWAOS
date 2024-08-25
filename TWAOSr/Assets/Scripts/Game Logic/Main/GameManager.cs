using System;
using System.Collections;
using System.Collections.Generic;
using ELECTRIS;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using TMPro;
using UnityEditor;

// ELECTRO - 11/08/2024 05:27 - Treat this as the heart of the game. I'm not kidding.
// ELECTRO - 16/08/2024 22:39 - Added support for enabling RTX. Also utility functions to reset the quality.
// ELECTRO - 16/08/2024 23:18 - For some reason the game starts playmode pause, so I added a function to autostart it, but can be disabled if needed straight within the editor.
// ELECTRO - 25/08/2024 14:32 - Should've probably mentioned this earlier but the unpauseOnStart thing doesn't really work.

namespace ELECTRIS
{
    public class GameManager : MonoBehaviour
    {
        [Header("Script Control")]
        public bool reInput = true;
#if UNITY_EDITOR
        public bool unpauseOnStart = true;
#endif
        public int customStartQuality = 3;
        public bool startWithRTX = false;
        public bool startWithRTXQuality = false;
        public bool startWithCustomQuality = false;

        [Header("Script Connectors")]
        public MenuManager menuCtl;
        public VideoManager vidCtl;
        public LevelLoader levelCtl;
        public PauseManager pauseCtl;

        [Header("Game Quality Levels")]
        public GameObject Minimal;
        public GameObject Pretty;
        public GameObject Amazing;
        public GameObject Wonderful;

        [Header("RTX Quality Levels")]
        public GameObject PrettyRTXPerformance;
        public GameObject PrettyRTXQuality;
        public GameObject AmazingRTXPerformance;
        public GameObject AmazingRTXQuality;
        public GameObject WonderfulRTXPerformance;
        public GameObject WonderfulRTXQuality;

        void Awake()
        {
            // Must always be active and must always be activated first
            Minimal.SetActive(true);

            // For Testing/Debugging ONLY | Do NOT use in production
            if (startWithCustomQuality)
            {
                QualitySettings.SetQualityLevel(customStartQuality);
            }

            // For Testing/Debugging ONLY | Do NOT use in production
            if (startWithRTX)
            {
                EnableRayTracing(startWithRTXQuality);
            }

#if UNITY_EDITOR
            // For Testing/Debugging ONLY | Do NOT use in production
            if (unpauseOnStart && EditorApplication.isPlaying)
            {
                EditorApplication.isPaused = false;
            }
#endif
        }

        void Update()
        {
            switch (QualitySettings.GetQualityLevel())
            {
                case 0:
                {
                    Minimal.SetActive(true);
                    Pretty.SetActive(false);
                    Amazing.SetActive(false);
                    Wonderful.SetActive(false);
                    break;
                }
                case 1:
                {
                    Minimal.SetActive(true);
                    Pretty.SetActive(true);
                    Amazing.SetActive(false);
                    Wonderful.SetActive(false);
                    break;
                }
                case 2:
                {
                    Minimal.SetActive(true);
                    Pretty.SetActive(true);
                    Amazing.SetActive(true);
                    Wonderful.SetActive(false);
                    break;
                }
                case 3:
                {
                    Minimal.SetActive(true);
                    Pretty.SetActive(true);
                    Amazing.SetActive(true);
                    Wonderful.SetActive(true);
                    break;
                }
            }
        }

        public void ResetQualityLevel()
        {
            QualitySettings.SetQualityLevel(0);
            Minimal.SetActive(true);
            Pretty.SetActive(false);
            Amazing.SetActive(false);
            Wonderful.SetActive(false);
        }

        public void ResetQuality()
        {
            Minimal.SetActive(true);
            Pretty.SetActive(false);
            Amazing.SetActive(false);
            Wonderful.SetActive(false);
        }

        public void ResetRayTracing()
        {
            PrettyRTXPerformance.SetActive(false);
            PrettyRTXQuality.SetActive(false);
            AmazingRTXPerformance.SetActive(false);
            AmazingRTXQuality.SetActive(false);
            WonderfulRTXPerformance.SetActive(false);
            WonderfulRTXQuality.SetActive(false);
        }

        public void EnableRayTracing(bool quality)
        {
            if (!quality)
            {
                switch (QualitySettings.GetQualityLevel())
                {
                    case 0:
                    {
                        ResetQuality();
                        break;
                    }
                    case 1:
                    {
                        ResetQuality();
                        PrettyRTXPerformance.SetActive(true);
                        Pretty.SetActive(true);
                        break;
                    }
                    case 2:
                    {
                        ResetQuality();
                        PrettyRTXPerformance.SetActive(true);
                        Pretty.SetActive(true);
                        AmazingRTXPerformance.SetActive(true);
                        Amazing.SetActive(true);
                        break;
                    }
                    case 3:
                    {
                        ResetQuality();
                        PrettyRTXPerformance.SetActive(true);
                        Pretty.SetActive(true);
                        AmazingRTXPerformance.SetActive(true);
                        Amazing.SetActive(true);
                        WonderfulRTXPerformance.SetActive(true);
                        Wonderful.SetActive(true);
                        break;
                    }
                }
            }else if (quality)
            {
                switch (QualitySettings.GetQualityLevel())
                {
                    case 0:
                    {
                        ResetQuality();
                        break;
                    }
                    case 1:
                    {
                        ResetQuality();
                        PrettyRTXQuality.SetActive(true);
                        Pretty.SetActive(true);
                        break;
                    }
                    case 2:
                    {
                        ResetQuality();
                        PrettyRTXQuality.SetActive(true);
                        Pretty.SetActive(true);
                        AmazingRTXQuality.SetActive(true);
                        Amazing.SetActive(true);
                        break;
                    }
                    case 3:
                    {
                        ResetQuality();
                        PrettyRTXQuality.SetActive(true);
                        Pretty.SetActive(true);
                        AmazingRTXQuality.SetActive(true);
                        Amazing.SetActive(true);
                        WonderfulRTXQuality.SetActive(true);
                        Wonderful.SetActive(true);
                        break;
                    }
                }
            }
        }

        public void DisableRayTracing()
        {
            ResetRayTracing();
            ResetQuality();
        }
    }
}