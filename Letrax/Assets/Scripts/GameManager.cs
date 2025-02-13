using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Globalization;

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
    public TextMeshProUGUI alertText;

    [Header("Score")]
    public GameObject afterGameScreen;
    public TextMeshProUGUI theWordWas;
    public TextMeshProUGUI finalScore;
    public TextMeshProUGUI gamesText;
    public TextMeshProUGUI victoryText;
    public TextMeshProUGUI winningStreakText;
    public TextMeshProUGUI bestStreakText;
    public float games;
    public float victory;
    public int winningStreak;
    public int bestStreak;
    public bool victoryInLastGame;

    [Header("Stats Screens")]
    public TextMeshProUGUI statsGamesText;
    public TextMeshProUGUI statsVictoryText;
    public TextMeshProUGUI statsWinningStreakText;
    public TextMeshProUGUI statsBestStreakText;

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
        LoadStats();
    }

    // Setup Game -----------------------------

    string RandomBaseWord()
    {
        int randomIndex = Random.Range(0, baseWordPool.Length);
        return baseWordPool[randomIndex];
    }

    public void LoadStats()
    {
        // games
        games = PlayerPrefs.GetFloat("games");
        gamesText.text = games.ToString();
        statsGamesText.text = games.ToString();

        // victory
        victory = PlayerPrefs.GetFloat("victory");
        if (victory > 0)
        {
            victoryText.text = ((victory / games) * 100).ToString("F0") + "%";
            statsVictoryText.text = ((victory / games) * 100).ToString("F0") + "%";
        }
        else
        {
            victoryText.text = "0%";
            statsVictoryText.text = "0%";
        }

        // winning streak
        winningStreak = PlayerPrefs.GetInt("winningStreak");
        winningStreakText.text = winningStreak.ToString();
        statsWinningStreakText.text = winningStreak.ToString();

        // best streak
        bestStreak = PlayerPrefs.GetInt("bestStreak");
        bestStreakText.text = bestStreak.ToString();
        statsBestStreakText.text = bestStreak.ToString();
    }

    // Game Interaction -----------------------------

    public void KeyboardPressKey(Button pressedButton)
    {
        TextMeshProUGUI buttonText = pressedButton.GetComponentInChildren<TextMeshProUGUI>();
        letterMatrix[row][col].text = buttonText.text;

        ManageCursor(row, col, false);

        col++;
        if (col >= row0.Length)
            col = 0;

        ManageCursor(row, col, true);

        AudioManager.instance.KeyboardSFX();

        // reset alert text
        if (alertText.text != "")
            alertText.text = "";
    }

    public void BackspacePressKey(Button pressedButton)
    {
        if (letterMatrix[row][col].text != "")
        {
            letterMatrix[row][col].text = "";
        }
        else
        {
            ManageCursor(row, col, false);
            col--;
            if (col < 0)
                col = row0.Length - 1;
            ManageCursor(row, col, true);

            letterMatrix[row][col].text = "";
        }

        AudioManager.instance.KeyboardSFX();
    }

    public void ManageCursor(int row, int col, bool active)
    {
        cursorMatrix[row][col].enabled = active;
    }

    public void EnviarButton()
    {
        // verify if the word is not complete or out of word database
        if (VerifyNumberOfLettersAndWordDatabase(row))
        {
            CorrectWordAccent(row);
            ManageCursor(row, col, false);

            if (CompareWord(row))
            {
                UpdateStats(1);
                AudioManager.instance.VictorySFX();
                GameScore("VITÓRIA");
            }
            else
            {
                LettersMatchResult(row);

                row++;
                col = 0;
                attemptNumber++;

                if (attemptNumber == maxAttempts)
                {
                    UpdateStats(0);
                    AudioManager.instance.DefeatSFX();
                    GameScore("DERROTA");
                }
                else
                {
                    AudioManager.instance.SendButtonSFX();
                    ManageCursor(row, col, true);
                }
            }
        }
    }

    public void CorrectWordAccent(int rowIndex)
    {
        // Construir a palavra digitada pelo jogador
        string attemptWord = string.Join("", letterMatrix[rowIndex].Select(letter => letter.text.ToLower()));

        // Buscar a versão correta da palavra dentro do baseWordPool
        string correctedWord = baseWordPool.FirstOrDefault(word => RemoveDiacritics(word.ToLower()) == attemptWord);

        // Se encontrou a palavra correta, atualizar o grid com os acentos
        if (!string.IsNullOrEmpty(correctedWord))
        {
            for (int i = 0; i < correctedWord.Length; i++)
            {
                letterMatrix[rowIndex][i].text = correctedWord[i].ToString().ToUpper();
            }
        }
    }

    // Logic -----------------------------

    public bool VerifyNumberOfLettersAndWordDatabase(int rowIndex)
    {
        // Construir a palavra digitada
        string attemptWord = string.Join("", letterMatrix[rowIndex].Select(letter => letter.text.ToLower()));

        // Verificar se tem menos de 5 letras
        if (attemptWord.Length < 5)
        {
            AudioManager.instance.ErrorSFX();
            alertText.text = "A PALAVRA ESTÁ INCOMPLETA";
            return false;
        }

        // Verificar se a palavra está na baseWordPool (normalizando para evitar problemas com acentos)
        string normalizedAttemptWord = RemoveDiacritics(attemptWord);
        bool wordExists = baseWordPool.Any(word => RemoveDiacritics(word) == normalizedAttemptWord);

        if (!wordExists)
        {
            AudioManager.instance.ErrorSFX();
            alertText.text = "ESSA PALAVRA NÃO É ACEITA";
            return false;
        }

        return true;
    }

    public bool CompareWord(int rowIndex)
    {
        // create a array with the matrix row letters
        TextMeshProUGUI[] row = letterMatrix[rowIndex];

        // convert the array in a string
        attemptWord = string.Join("", System.Array.ConvertAll(row, letter => letter.text)).ToLower();

        // remove accents from words before comparison
        string normalizedAttempt = RemoveDiacritics(attemptWord);
        string normalizedBaseWord = RemoveDiacritics(baseWord);

        // return the comparison
        return (normalizedAttempt == normalizedBaseWord);
    }

    private string RemoveDiacritics(string text)
    {
        return new string(text.Normalize(NormalizationForm.FormD)
                              .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                              .ToArray()) // Converte a IEnumerable<char> de volta para string
                    .Normalize(NormalizationForm.FormC);
    }

    public void LettersMatchResult(int rowIndex)
    {
        // Criar a palavra digitada sem acentos
        string attemptWord = RemoveDiacritics(string.Join("", letterMatrix[rowIndex].Select(letter => letter.text.ToLower())));

        // Criar a palavra base sem acentos
        string normalizedBaseWord = RemoveDiacritics(baseWord);

        // Array para contar quantas vezes cada letra aparece na baseWord
        Dictionary<char, int> baseWordLetterCount = new Dictionary<char, int>();
        for (int i = 0; i < normalizedBaseWord.Length; i++)
        {
            char letter = normalizedBaseWord[i];
            if (baseWordLetterCount.ContainsKey(letter))
                baseWordLetterCount[letter]++;
            else
                baseWordLetterCount[letter] = 1;
        }

        // Verifica posição exata (corretos)
        bool[] exactMatches = new bool[normalizedBaseWord.Length];

        for (int i = 0; i < normalizedBaseWord.Length; i++)
        {
            if (attemptWord[i] == normalizedBaseWord[i])
            {
                bgMatrix[rowIndex][i].color = ColorManager.instance.rightColor;
                exactMatches[i] = true;
                baseWordLetterCount[attemptWord[i]]--; // Remove do contador
            }
        }

        // Verifica se existe em outra posição
        for (int i = 0; i < normalizedBaseWord.Length; i++)
        {
            if (exactMatches[i]) continue; // Já foi marcado como exato

            char attemptLetter = attemptWord[i];
            if (baseWordLetterCount.ContainsKey(attemptLetter) && baseWordLetterCount[attemptLetter] > 0)
            {
                bgMatrix[rowIndex][i].color = ColorManager.instance.partialColor;
                baseWordLetterCount[attemptLetter]--; // Remove do contador
            }
            else
            {
                bgMatrix[rowIndex][i].color = ColorManager.instance.wrongColor;
            }
        }
    }

    // After Game -----------------------------

    public void GameScore(string result)
    {
        theWordWas.text = baseWord.ToUpper();
        finalScore.text = result;
        afterGameScreen.SetActive(true);
    }

    public void UpdateStats(int victoryScore)
    {
        // games
        games = PlayerPrefs.GetFloat("games") + 1;
        PlayerPrefs.SetFloat("games", games);
        gamesText.text = games.ToString();
        statsGamesText.text = games.ToString();

        // victory
        victory = PlayerPrefs.GetFloat("victory") + victoryScore;
        PlayerPrefs.SetFloat("victory", victory);
        float percentage = (victory / games) * 100;
        if (victory > 0)
        {
            victoryText.text = percentage.ToString("F0") + "%";
            statsVictoryText.text = percentage.ToString("F0") + "%";
        }
        else
        {
            victoryText.text = "0%";
            statsVictoryText.text = "0%";
        }

        // winning streak
        if (victoryScore == 0)
        {
            winningStreak = 0;
            PlayerPrefs.SetInt("winningStreak", 0);
        }
        else
        {
            winningStreak = PlayerPrefs.GetInt("winningStreak") + 1;
            PlayerPrefs.SetInt("winningStreak", winningStreak);
        }
        winningStreakText.text = winningStreak.ToString();
        statsWinningStreakText.text = winningStreak.ToString();

        // best streak
        if (winningStreak > PlayerPrefs.GetInt("bestStreak"))
        {
            bestStreak = winningStreak;
            PlayerPrefs.SetInt("bestStreak", bestStreak);
            bestStreakText.text = bestStreak.ToString();
            statsBestStreakText.text = bestStreak.ToString();
        }
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
        afterGameScreen.SetActive(false);

        // generate a new base word
        baseWord = RandomBaseWord();

        // SFX
        AudioManager.instance.HoverSFX();
    }

    public void CleanWordGrid()
    {
        foreach (TextMeshProUGUI[] row in letterMatrix)
            foreach (TextMeshProUGUI letter in row)
                letter.text = "";

        foreach (Image[] row in bgMatrix)
            foreach (Image letter in row)
                letter.color = ColorManager.instance.emptyColor;
    }

}
