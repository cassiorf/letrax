using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject gameplayScreen;
    public GameObject helpScreen;
    public GameObject statsScreen;
    public GameObject settingsScreen;
    public GameObject mainMenuScreen;
    public GameObject languagesScreen;
    public GameObject confirmLanguageChangePopUp;

    public void PlayGame()
    {
        AudioManager.instance.HoverSFX();

        GameManager.instance.alertText.text = "";

        if (PlayerPrefs.GetFloat("games") == 0)
            helpScreen.SetActive(true);

        gameplayScreen.SetActive(true);
        languagesScreen.SetActive(false);
        mainMenuScreen.SetActive(false);
    }

    public void MainMenu()
    {
        AudioManager.instance.HoverSFX();

        mainMenuScreen.SetActive(true);
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

    public void OpenLanguages()
    {
        AudioManager.instance.HoverSFX();
        ColorManager.instance.UpdateLanguagesButtonsColor(LocalizationManager.instance.currentLanguage);
        LocalizationManager.instance.preSelectionLanguage = LocalizationManager.instance.currentLanguage;
        languagesScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }

    public void CloseLanguages()
    {
        AudioManager.instance.HoverSFX();
        mainMenuScreen.SetActive(true);
        languagesScreen.SetActive(false);
    }

    public void ConfirmButton()
    {
        if (GameManager.instance.attemptNumber == 0)
        {
            // update language
            LocalizationManager.instance.StartLoadLocalizedText(LocalizationManager.instance.preSelectionLanguage);
            LocalizationManager.instance.currentLanguage = LocalizationManager.instance.preSelectionLanguage;

            // update word database
            GameManager.instance.LoadWordList(LocalizationManager.instance.currentLanguage);
            GameManager.instance.LoadSelectionWordList(LocalizationManager.instance.currentLanguage);

            // Reset grid + cursor + keyboard + variables
            GameManager.instance.ResetGameOnLanguageChange();

            // Manage screens
            mainMenuScreen.SetActive(true);
            languagesScreen.SetActive(false);

            // SFX
            AudioManager.instance.HoverSFX();
        }
        else
        {
            if (LocalizationManager.instance.preSelectionLanguage != LocalizationManager.instance.currentLanguage)
            {
                AudioManager.instance.HoverSFX();
                confirmLanguageChangePopUp.SetActive(true);
            }
            else
            {
                AudioManager.instance.HoverSFX();
                mainMenuScreen.SetActive(true);
                languagesScreen.SetActive(false);
            }
        }
    }

    public void YesButton()
    {
        // update language
        LocalizationManager.instance.StartLoadLocalizedText(LocalizationManager.instance.preSelectionLanguage);
        LocalizationManager.instance.currentLanguage = LocalizationManager.instance.preSelectionLanguage;

        // update worddatabase
        GameManager.instance.LoadWordList(LocalizationManager.instance.currentLanguage);
        GameManager.instance.LoadSelectionWordList(LocalizationManager.instance.currentLanguage);

        // Update stats with defeat
        GameManager.instance.UpdateStats(0);

        // Reset grid + cursor + keyboard + variables
        GameManager.instance.ResetGameOnLanguageChange();

        // Manage screens
        mainMenuScreen.SetActive(true);
        confirmLanguageChangePopUp.SetActive(false);
        languagesScreen.SetActive(false);

        // SFX
        AudioManager.instance.HoverSFX();
    }

    public void NoButton()
    {
        AudioManager.instance.HoverSFX();
        confirmLanguageChangePopUp.SetActive(false);
    }



}
