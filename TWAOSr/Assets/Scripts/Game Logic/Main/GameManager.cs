using System;
using System.Collections;
using System.Collections.Generic;
using ELECTRIS;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using TMPro;

// ELECTRO - 11/08/2024 05:27 - Treat this as the heart of the game. I'm not kidding.

namespace ELECTRIS
{
    public class GameManager : MonoBehaviour
    {
        [Header("Script Control")]
        public int customStartQuality = 3;
        public bool startWithCustomQuality = false;

        [Header("Qualities")]
        public GameObject Minimal;
        public GameObject Pretty;
        public GameObject Amazing;
        public GameObject Wonderful;

        void Awake()
        {
            // For Testing/Debugging ONLY | Do NOT use in production
            if (startWithCustomQuality)
            {
                QualitySettings.SetQualityLevel(customStartQuality);
            }

            // Must always be active and must always be activated first
            Minimal.SetActive(true);
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
    }
}

