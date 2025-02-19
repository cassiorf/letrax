using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;
    private Dictionary<string, string> localizedText;
    public string currentLanguage;
    public string preSelectionLanguage;

    public delegate void LanguageChanged();
    public static event LanguageChanged OnLanguageChanged;

    void Awake()
    {
        if (instance == null)
            instance = this;

        currentLanguage = PlayerPrefs.GetString("currentLanguage", "pt");
        StartLoadLocalizedText(currentLanguage);
    }

    public void StartLoadLocalizedText(string languageCode)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, $"lang_{languageCode}.json");

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            localizedText = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataAsJson);
            currentLanguage = languageCode;
            PlayerPrefs.SetString("currentLanguage", languageCode);

            OnLanguageChanged?.Invoke(); // Dispara o evento para atualizar todos os textos
        }
        else
        {
            Debug.LogError($"Arquivo de localização não encontrado: {filePath}");
        }
    }

    public void LoadLocalizedText(string languageCode)
    {
        preSelectionLanguage = languageCode;
        AudioManager.instance.HoverSFX();
        ColorManager.instance.UpdateLanguagesButtonsColor(languageCode);
    }

    public string GetLocalizedText(string key)
    {
        if (localizedText == null)
        {
            Debug.LogError("LocalizationManager: localizedText não foi inicializado!");
            return key; // Retorna a chave original para evitar erro
        }

        return localizedText.ContainsKey(key) ? localizedText[key] : key;
    }



}
