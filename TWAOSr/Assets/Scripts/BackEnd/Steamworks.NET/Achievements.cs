#if !(DisableSteamworks)
using Steamworks;
#endif

using System.Collections;
using System.Collections.Generic;
using ELECTRIS;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    #if !DisableSteamworks
    public void DebugUnlockAll()
    {
        if (SteamManager.Initialized)
        {
            COMPLETE_CHAP_0(true);
            COMPLETE_CHAP_1(true);
            COMPLETE_CHAP_2(true);
            COMPLETE_CHAP_3(true);
            COMPLETE_CHAP_4(true);
            COMPLETE_CHAP_5(true);
            COMPLETE_CHAP_6(true);
            COMPLETE_CHAP_7(true);
            COMPLETE_CHAP_8(true);
            COMPLETE_CHAP_9(true);
            COMPLETE_CHAP_10(true);
            ODINSON(true);
            GHOST_OF_SPARTA(true);
            EYE_FOR_AN_EYE(true);
            READY_OR_NOT(true);
        }
    }

    public void DebugRemoveAll()
    {
        if (SteamManager.Initialized)
        {
            COMPLETE_CHAP_0(false);
            COMPLETE_CHAP_1(false);
            COMPLETE_CHAP_2(false);
            COMPLETE_CHAP_3(false);
            COMPLETE_CHAP_4(false);
            COMPLETE_CHAP_5(false);
            COMPLETE_CHAP_6(false);
            COMPLETE_CHAP_7(false);
            COMPLETE_CHAP_8(false);
            COMPLETE_CHAP_9(false);
            COMPLETE_CHAP_10(false);
            ODINSON(false);
            GHOST_OF_SPARTA(false);
            EYE_FOR_AN_EYE(false);
            READY_OR_NOT(false);
        }
    }

    public void COMPLETE_CHAP_0(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_0", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_0");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_0");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_1(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_1", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_1");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_1");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_2(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_2", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_2");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_2");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_3(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_3", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_3");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_3");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_4(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_4", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_4");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_4");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_5(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_5", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_6");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_6");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_6(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_6", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_7");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_7");
                }
                SteamUserStats.StoreStats();
            }

        }
    }

    public void COMPLETE_CHAP_7(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_7", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_7");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_7");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_8(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_8", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_8");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_8");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_9(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_9", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_9");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_9");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void COMPLETE_CHAP_10(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("COMPLETE_CHAP_10", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("COMPLETE_CHAP_10");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("COMPLETE_CHAP_10");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void ODINSON(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("ODINSON", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("ODINSON");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("ODINSON");
                }
                SteamUserStats.StoreStats();
            } 
        }
    }

    public void GHOST_OF_SPARTA(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("GHOST_OF_SPARTA", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("GHOST_OF_SPARTA");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("GHOST_OF_SPARTA");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void EYE_FOR_AN_EYE(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("EYE_FOR_AN_EYE", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("EYE_FOR_AN_EYE");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("EYE_FOR_AN_EYE");
                }
                SteamUserStats.StoreStats();
            }
        }
    }

    public void READY_OR_NOT(bool GiveOrTake)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement("READY_OR_NOT", out bool achievementCompleted);

            if (!achievementCompleted)
            {
                if (GiveOrTake == true)
                {
                    SteamUserStats.SetAchievement("READY_OR_NOT");
                }else if (GiveOrTake == false)
                {
                    SteamUserStats.ClearAchievement("READY_OR_NOT");
                }
                SteamUserStats.StoreStats();
            }
        }
    }
    #endif
}
