using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject gameplayScreen;
    public GameObject helpScreen;
    public GameObject statsScreen;
    public GameObject settingsScreen;
    public GameObject mainMenuScreen;

    public void PlayGame()
    {
        AudioManager.instance.HoverSFX();

        if (PlayerPrefs.GetFloat("games") == 0)
            helpScreen.SetActive(true);

        gameplayScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void MainMenu()
    {
        AudioManager.instance.HoverSFX();

        mainMenuScreen.SetActive(true);
        gameplayScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT GAME");
    }

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
