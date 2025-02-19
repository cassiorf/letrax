using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;

    [Header("Color Base")]
    public Color emptyColor;
    public Color wrongColor;
    public Color partialColor;
    public Color rightColor;

    [Header("Keyboard Color")]
    public Color keyBGBasicColor;
    public Color keyTextBasicColor;
    public Color keyBGWrongColor;
    public Color keyTextWrongColor;

    [Header("Color Selection")]
    public Color basicPartialColor;
    public Color basicRightColor;
    public Color deuteranopiaPartialColor;
    public Color deuteranopiaRightColor;
    public Color protanopiaPartialColor;
    public Color protanopiaRightColor;
    public Color tritanopiaPartialColor;
    public Color tritanopiaRightColor;

    [Header("After Game Screen Color")]
    public Color screenVictoryColor;
    public Color buttonVictoryColor;
    public Color screenDefeatColor;
    public Color buttonDefeatColor;

    [Header("Color Button Selection")]
    public Image[] colorButtonSelection;

    [Header("Languages Objects")]
    public Image ptButton;
    public Image enButton;

    [Header("Settings Objects")]
    public Image partialColorModel;
    public Image rightColorModel;
    public Image toggelButton;

    [Header("Help Objects")]
    public Image partialLetterOnExempleWord;
    public Image rightLetterOnExempleWord;
    public Image partialLetterOnExemple;
    public Image rightLetterOnExemple;

    [Header("After Game Objects")]
    public Image screenBG;
    public Image playAgainButton;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void UpdateSettingsColorButton(int index)
    {
        UpdateSettingsColorButton(index, true);
    }

    public void UpdateSettingsColorButton(int index, bool playSFX)
    {
        if (playSFX)
            AudioManager.instance.HoverSFX();

        switch (index)
        {
            case 0:
                partialColorModel.color = basicPartialColor;
                rightColorModel.color = basicRightColor;
                break;
            case 1:
                partialColorModel.color = deuteranopiaPartialColor;
                rightColorModel.color = deuteranopiaRightColor;
                break;
            case 2:
                partialColorModel.color = protanopiaPartialColor;
                rightColorModel.color = protanopiaRightColor;
                break;
            case 3:
                partialColorModel.color = tritanopiaPartialColor;
                rightColorModel.color = tritanopiaRightColor;
                break;
            default:
                partialColorModel.color = basicPartialColor;
                rightColorModel.color = basicRightColor;
                break;
        }

        GameManager.instance.colorPalette = index;
        PlayerPrefs.SetInt("colorPalette", index);

        for (int i = 0; i < colorButtonSelection.Length; i++)
            colorButtonSelection[i].color = (i == index) ? rightColorModel.color : ColorManager.instance.emptyColor;

        UpdateAllObjectsColor();
    }

    public void UpdateAllObjectsColor()
    {
        // update SFX toggle button
        toggelButton.color = AudioManager.instance.enableSFX ? rightColorModel.color : ColorManager.instance.emptyColor;

        // update help screen
        partialLetterOnExempleWord.color = partialColorModel.color;
        rightLetterOnExempleWord.color = rightColorModel.color;
        partialLetterOnExemple.color = partialColorModel.color;
        rightLetterOnExemple.color = rightColorModel.color;

        // update word grid
        foreach (Image[] row in GameManager.instance.bgMatrix)
        {
            foreach (Image img in row)
            {
                if (img.color == partialColor)
                    img.color = partialColorModel.color;
                else if (img.color == rightColor)
                    img.color = rightColorModel.color;
            }
        }

        // update keyboard color
        foreach (TextMeshProUGUI text in GameManager.instance.keyboardKeyText)
        {
            if (text.GetComponentInParent<Image>().color == partialColor)
                text.GetComponentInParent<Image>().color = partialColorModel.color;
            else if (text.GetComponentInParent<Image>().color == rightColor)
                text.GetComponentInParent<Image>().color = rightColorModel.color;
        }

        // update language screen
        if (ptButton.color == rightColor)
        {
            ptButton.color = rightColorModel.color;
            enButton.color = emptyColor;
        }
        if (enButton.color == rightColor)
        {
            enButton.color = rightColorModel.color;
            ptButton.color = emptyColor;
        }

        // update color database
        partialColor = partialColorModel.color;
        rightColor = rightColorModel.color;
    }

    public void UpdateAfterGameScreenColor(bool victory)
    {
        screenBG.color = victory ? screenVictoryColor : screenDefeatColor;
        playAgainButton.color = victory ? buttonVictoryColor : buttonDefeatColor;
    }

    public void UpdateLanguagesButtonsColor(string language)
    {
        switch (language)
        {
            case "pt":
                ptButton.color = rightColor;
                enButton.color = emptyColor;
                break;
            case "en":
                ptButton.color = emptyColor;
                enButton.color = rightColor;
                break;
            default:
                break;
        }
    }

}
