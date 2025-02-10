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

    [Header("Color")]
    public Color emptyColor;
    public Color wrongColor;
    public Color partialColor;
    public Color rightColor;

    [Header("Score")]
    public GameObject gameScore;
    public TextMeshProUGUI theWordWas;
    public TextMeshProUGUI finalScore;
    
    [Header("Word Grid Text")]
    public int row;
    public int col;
    public TextMeshProUGUI[] row0;
    public TextMeshProUGUI[] row1;
    public TextMeshProUGUI[] row2;
    public TextMeshProUGUI[] row3;
    public TextMeshProUGUI[] row4;
    public TextMeshProUGUI[] row5;
    public List<TextMeshProUGUI[]> letterMatrix = new List<TextMeshProUGUI[]>();

    [Header("Letter BG")]
    public Image[] bgRow0;
    public Image[] bgRow1;
    public Image[] bgRow2;
    public Image[] bgRow3;
    public Image[] bgRow4;
    public Image[] bgRow5;
    public List<Image[]> bgMatrix = new List<Image[]>();

    [Header("Letter Cursor")]
    public Image[] cursorRow0;
    public Image[] cursorRow1;
    public Image[] cursorRow2;
    public Image[] cursorRow3;
    public Image[] cursorRow4;
    public Image[] cursorRow5;
    public List<Image[]> cursorMatrix = new List<Image[]>();

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

        cursorMatrix.Add(cursorRow0);
        cursorMatrix.Add(cursorRow1);
        cursorMatrix.Add(cursorRow2);
        cursorMatrix.Add(cursorRow3);
        cursorMatrix.Add(cursorRow4);
        cursorMatrix.Add(cursorRow5);

        bgMatrix.Add(bgRow0);
        bgMatrix.Add(bgRow1);
        bgMatrix.Add(bgRow2);
        bgMatrix.Add(bgRow3);
        bgMatrix.Add(bgRow4);
        bgMatrix.Add(bgRow5);

        CleanWordGrid();
        ManageCursor(row, col, true);
    }

    string RandomBaseWord()
    {
        int randomIndex = Random.Range(0, baseWordPool.Length);
        return baseWordPool[randomIndex];
    }

    public void KeyboardPressKey(Button pressedButton)
    {
        TextMeshProUGUI buttonText = pressedButton.GetComponentInChildren<TextMeshProUGUI>();
        letterMatrix[row][col].text = buttonText.text;

        ManageCursor(row, col, false);

        col++;
        if (col >= row0.Length)
            col = 0;

        ManageCursor(row, col, true);
    }

    public void ManageCursor(int row, int col, bool active)
    {
        cursorMatrix[row][col].enabled = active;
    }

    public void EnviarButton()
    {
        // verify if the word is complete
        if (VerifyNumberOfLetters(row))
        {
            ManageCursor(row, col, false);

            if (CompareWord(row))
            {
                GameScore("VITÓRIA");
            }
            else
            {
                LettersMatchResult(row);

                row++;
                col = 0;
                attemptNumber++;

                if (attemptNumber == maxAttempts)
                    GameScore("DERROTA");
                else
                    ManageCursor(row, col, true);
            }
        }
    }

    public bool VerifyNumberOfLetters(int row)
    {
        for (int i = 0; i < row0.Length; i++)
        {
            if (letterMatrix[row][i].text == "")
            {
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

    public void LettersMatchResult(int rowIndex)
    {
        string attemptWord = "";
        foreach (var letter in letterMatrix[rowIndex])
        {
            attemptWord += letter.text.ToLower();
        }

        // Array para contar quantas vezes cada letra aparece na baseWord
        Dictionary<char, int> baseWordLetterCount = new Dictionary<char, int>();
        for (int i = 0; i < baseWord.Length; i++)
        {
            char letter = baseWord[i];
            if (baseWordLetterCount.ContainsKey(letter))
                baseWordLetterCount[letter]++;
            else
                baseWordLetterCount[letter] = 1;
        }

        // Verifica posição exata (corretos)
        bool[] exactMatches = new bool[baseWord.Length];

        for (int i = 0; i < baseWord.Length; i++)
        {
            if (attemptWord[i] == baseWord[i])
            {
                bgMatrix[rowIndex][i].color = rightColor;
                exactMatches[i] = true;
                baseWordLetterCount[attemptWord[i]]--; // Remove do contador
            }
        }

        // Verifica se existe em outra posição
        for (int i = 0; i < baseWord.Length; i++)
        {
            if (exactMatches[i]) continue; // Já foi marcado como exato

            char attemptLetter = attemptWord[i];
            if (baseWordLetterCount.ContainsKey(attemptLetter) && baseWordLetterCount[attemptLetter] > 0)
            {
                bgMatrix[rowIndex][i].color = partialColor;
                baseWordLetterCount[attemptLetter]--; // Remove do contador
            }
            else
            {
                bgMatrix[rowIndex][i].color = wrongColor;
            }
        }
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
        CleanWordGrid();

        // reset variables
        attemptNumber = 0;
        row = 0;
        col = 0;

        // setup cursor
        ManageCursor(row, col, true);

        // hide score
        gameScore.SetActive(false);

        // generate a new base word
        baseWord = RandomBaseWord();
    }

    public void CleanWordGrid()
    {
        foreach (TextMeshProUGUI[] row in letterMatrix)
            foreach (TextMeshProUGUI letter in row)
                letter.text = "";

        foreach (Image[] row in bgMatrix)
            foreach (Image letter in row)
                letter.color = emptyColor;
    }
}
