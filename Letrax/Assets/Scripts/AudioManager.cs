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
    public AudioClip hoverSFX;
    public AudioClip errorSFX;
    public AudioClip sendButtonSFX;
    public AudioClip victorySFX;
    public AudioClip defeatSFX;

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
        AudioManager.instance.HoverSFX();
        toggelButton.color = enableSFX ? ColorManager.instance.rightColor : ColorManager.instance.emptyColor;
    }
}
