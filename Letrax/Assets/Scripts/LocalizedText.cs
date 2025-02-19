using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    public string key;
    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        if (LocalizationManager.instance != null)
        {
            textComponent.text = LocalizationManager.instance.GetLocalizedText(key);
        }
    }
}
