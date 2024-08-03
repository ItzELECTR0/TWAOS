using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ELECTRIS;
using Discord;
using System;
using Unity.VisualScripting;

namespace ELECTRIS
{
    public class DiscordManager : MonoBehaviour
    {
        public Discord.Discord discord;
        public String ActivityDetails;
        public String ActivityState;
        private String ActivityLargeImageKey = "sip";

        void Start()
        {
            if (discord != null)
            {
                discord = new Discord.Discord(997984750155333753, (System.UInt64)Discord.CreateFlags.Default);
                var activityManager = discord.GetActivityManager();
                var activity = new Discord.Activity
                {
                    Details = ActivityDetails,
                    State = ActivityState,
                    Assets = {
                LargeText = "TWAOS",
                LargeImage = ActivityLargeImageKey
            }
                };
                activityManager.UpdateActivity(activity, (res) => {
                    if (res == Discord.Result.Ok)
                    {
                        Debug.Log(Discord.Result.Ok + "(Discord Presence Update Success!)");
                    }
                    else
                    {
                        Debug.LogError("(Discord Presence Update Failed!)");
                    }
                });
            }
            else
            {
                return;
            }
        }

        void Update()
        {
            if (discord != null)
            {
                discord.RunCallbacks();
            }
            else
            {
                return;
            }
        }
    }
}