using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;

    [Header("Color")]
    public Color emptyColor;
    public Color wrongColor;
    public Color partialColor;
    public Color rightColor;

    [Header("Color Selection")]
    public Color basicPartialColor;
    public Color basicRightColor;
    public Color deuteranopiaPartialColor;
    public Color deuteranopiaRightColor;
    public Color protanopiaPartialColor;
    public Color protanopiaRightColor;
    public Color tritanopiaPartialColor;
    public Color tritanopiaRightColor;

    [Header("Color Button Selection")]
    public Image[] colorButtonSelection;

    [Header("Settings Objects")]
    public Image partialColorModel;
    public Image rightColorModel;
    public Image toggelButton;

    [Header("Help Objects")]
    public Image partialLetterOnExempleWord;
    public Image rightLetterOnExempleWord;
    public Image partialLetterOnExemple;
    public Image rightLetterOnExemple;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void UpdateSettingsColorButton(int index)
    {
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

        // update color database
        partialColor = partialColorModel.color;
        rightColor = rightColorModel.color;
    }
}
