using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject helpScreen;
    public GameObject statsScreen;
    public GameObject settingsScreen;

    public void OpenHelp()
    {
        AudioManager.instance.HoverSFX();
        helpScreen.SetActive(true);
    }

    public void CloseHelp()
    {
        AudioManager.instance.HoverSFX();
        helpScreen.SetActive(false);
    }

    public void OpenStats()
    {
        AudioManager.instance.HoverSFX();
        statsScreen.SetActive(true);
    }

    public void CloseStats()
    {
        AudioManager.instance.HoverSFX();
        statsScreen.SetActive(false);
    }

    public void OpenSettings()
    {
        AudioManager.instance.HoverSFX();
        settingsScreen.SetActive(true);
    }

    public void CloseSettings()
    {
        AudioManager.instance.HoverSFX();
        settingsScreen.SetActive(false);
    }
}
