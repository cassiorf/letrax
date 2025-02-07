using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Stats")]
    public string[] baseWordPool;
    public string baseWord;
    public string attemptWord;
    public int letterIndex;
    public int attemptNumber;
    public int maxAttempts;

    [Header("Word Grip")]
    public List<TextMeshProUGUI[]> letterMatrix = new List<TextMeshProUGUI[]>();
    public TextMeshProUGUI[] row0;
    public TextMeshProUGUI[] row1;
    public TextMeshProUGUI[] row2;
    public TextMeshProUGUI[] row3;
    public TextMeshProUGUI[] row4;
    public TextMeshProUGUI[] row5;
    public int row;
    public int col;

    [Header("Score")]
    public GameObject gameScore;
    public TextMeshProUGUI theWordWas;
    public TextMeshProUGUI finalScore;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        baseWord = RandomBaseWord();
        attemptNumber = 0;
        row = 0;
        col = 0;

        letterMatrix.Add(row0);
        letterMatrix.Add(row1);
        letterMatrix.Add(row2);
        letterMatrix.Add(row3);
        letterMatrix.Add(row4);
        letterMatrix.Add(row5);
    }

    public void KeyboardPressKey(Button pressedButton)
    {
        TextMeshProUGUI buttonText = pressedButton.GetComponentInChildren<TextMeshProUGUI>();
        letterMatrix[row][col].text = buttonText.text;

        col++;
        if (col >= row0.Length)
            col = 0;
    }

    public void EnviarButton()
    {
        // verify if the word is complete
        if (VerifyNumberOfLetters(row))
        {
            if (CompareWord(row))
                GameScore("VITÓRIA");
            else
            {
                row++;
                col = 0;
                attemptNumber++;
                
                if (attemptNumber == maxAttempts)
                    GameScore("DERROTA");
            }
        }
    }

    string RandomBaseWord()
    {
        int randomIndex = Random.Range(0, baseWordPool.Length);
        return baseWordPool[randomIndex];
    }

    public bool VerifyNumberOfLetters(int row)
    {
        for (int i = 0; i < row0.Length; i++)
        {
            if (letterMatrix[row][i].text == "")
            {
                Debug.Log("Linha " + row + ": palavra incompleta");
                return false;
            }
        }
        return true;
    }

    public bool CompareWord(int rowIndex)
    {
        // create a array with the matrix row letters
        TextMeshProUGUI[] row = letterMatrix[rowIndex];

        // convert the array in a string
        attemptWord = string.Join("", System.Array.ConvertAll(row, letter => letter.text)).ToLower();

        // return the match result
        return (attemptWord == baseWord);
    }

    public void GameScore(string result)
    {
        theWordWas.text = baseWord.ToUpper();
        finalScore.text = result;
        gameScore.SetActive(true);
    }

    public void PlayAgain()
    {
        // clear word grid
        foreach (TextMeshProUGUI[] row in letterMatrix)
            foreach (TextMeshProUGUI letter in row)
                letter.text = "";

        // reset variables
        attemptNumber = 0;
        row = 0;
        col = 0;

        // hide score
        gameScore.SetActive(false);

        // generate a new base word
        baseWord = RandomBaseWord();
    }
}
