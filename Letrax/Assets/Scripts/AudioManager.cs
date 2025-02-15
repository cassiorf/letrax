    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Core")]
    public static AudioManager instance;
    public AudioSource effectSource;
    public bool enableSFX = true;

    [Header("Audio Clips")]
    public AudioClip keyboardSFX;
    public AudioClip hoverSFX;
    public AudioClip errorSFX;
    public AudioClip sendButtonSFX;
    public AudioClip victorySFX;
    public AudioClip defeatSFX;
    public AudioClip screenChangeSFX;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void HoverSFX()
    {
        if (enableSFX)
            effectSource.PlayOneShot(hoverSFX);
    }

    public void KeyboardSFX()
    {
        if (enableSFX)
            effectSource.PlayOneShot(keyboardSFX);
    }

    public void ErrorSFX()
    {
        if (enableSFX)
            effectSource.PlayOneShot(errorSFX);
    }

    public void SendButtonSFX()
    {
        if (enableSFX)
            effectSource.PlayOneShot(sendButtonSFX);
    }

    public void VictorySFX()
    {
        if (enableSFX)
            effectSource.PlayOneShot(victorySFX);
    }

    public void DefeatSFX()
    {
        if (enableSFX)
            effectSource.PlayOneShot(defeatSFX);
    }

    public void ToggleSFX(Image toggelButton)
    {
        enableSFX = !enableSFX;
        PlayerPrefs.SetInt("enableSFX", enableSFX ? 2 : 1);

        AudioManager.instance.HoverSFX();
        toggelButton.color = enableSFX ? ColorManager.instance.rightColor : ColorManager.instance.emptyColor;
    }
}
