using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Globalization;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Word Database")]
    public TextAsset JSONFile;
    [System.NonSerialized] private List<string> baseWordPool = new List<string>();

    [Header("Game Stats")]
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

    // Singleton ----------------------------------------------------------

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // JSON ---------------------------------------------------------- 

    [System.Serializable]
    public class WordListData
    {
        public List<string> words;
    }

    void LoadWordList()
    {
        WordListData loadedData = JsonUtility.FromJson<WordListData>(JSONFile.text);

        if (loadedData != null && loadedData.words != null)
        {
            baseWordPool = new List<string>(loadedData.words);
            Debug.Log($"JSON carregado com {baseWordPool.Count} palavras.");
        }
        else
        {
            Debug.LogError("Erro: JSON carregado, mas a lista 'words' está nula.");
            baseWordPool = new List<string>();
        }
    }

    // Setup Game ----------------------------------------------------------

    void Start()
    {
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

        LoadWordList();
        baseWord = RandomBaseWord();
        
        LoadGridPersistence();
        ManageCursor(row, col, true);
        
        LoadStats();
    }

    string RandomBaseWord()
    {
        if (PlayerPrefs.GetString("row0") == "")
        {
            int randomIndex = UnityEngine.Random.Range(0, baseWordPool.Count);
            PlayerPrefs.SetString("baseWord", baseWordPool[randomIndex]);
            
            return baseWordPool[randomIndex];
        }
        else
            return PlayerPrefs.GetString("baseWord");
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

    public void LoadGridPersistence()
    {
        // get number of attempts
        attemptNumber = PlayerPrefs.GetInt("attemptNumber");

        // Obtém a baseWord salva
        string baseWord = PlayerPrefs.GetString("baseWord", "");
        if (string.IsNullOrEmpty(baseWord)) return; // Se não houver palavra salva, sai da função

        row = 0; // Linha onde o cursor será ativado

        for (; row < 6; row++)
        {
            string savedWord = PlayerPrefs.GetString("row" + row, "");
            if (string.IsNullOrEmpty(savedWord))
            {
                break; // Define a próxima linha vazia para ativar o cursor
            }

            for (int col = 0; col < savedWord.Length && col < letterMatrix[row].Length; col++)
            {
                char letter = savedWord[col];
                letterMatrix[row][col].text = letter.ToString().ToUpper(); // Escreve a letra
            }

            // Verifica as cores das letras comparando com baseWord
            Dictionary<char, int> baseWordLetterCount = new Dictionary<char, int>();
            foreach (char c in baseWord) // Conta ocorrências das letras na baseWord
            {
                if (baseWordLetterCount.ContainsKey(c)) baseWordLetterCount[c]++;
                else baseWordLetterCount[c] = 1;
            }

            for (int col = 0; col < savedWord.Length && col < letterMatrix[row].Length; col++)
            {
                char letter = savedWord[col];
                Image bgImage = bgMatrix[row][col];

                if (baseWord[col] == letter) // Letra correta na posição correta
                {
                    bgImage.color = ColorManager.instance.rightColor;
                    baseWordLetterCount[letter]--; // Marca como usada
                }
            }

            for (int col = 0; col < savedWord.Length && col < letterMatrix[row].Length; col++)
            {
                char letter = savedWord[col];
                Image bgImage = bgMatrix[row][col];

                if (baseWord[col] == letter) continue; // Já marcada como correta

                if (baseWord.Contains(letter) && baseWordLetterCount.ContainsKey(letter) && baseWordLetterCount[letter] > 0) // Letra certa na posição errada
                {
                    bgImage.color = ColorManager.instance.partialColor;
                    baseWordLetterCount[letter]--; // Marca como usada
                }
                else // Letra errada
                {
                    bgImage.color = ColorManager.instance.wrongColor;
                }
            }
        }
    }

    // Game Interaction ----------------------------------------------------------

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

    public void SendButton()
    {
        // verify if the word is not complete or out of word database
        if (VerifyNumberOfLettersAndWordDatabase(row))
        {
            // correct word on grid
            CorrectWordAccent(row);

            // persist word
            SaveGridPersistence(row);

            // manage cursor
            ManageCursor(row, col, false);

            if (CompareWord(row))
            {
                UpdateStats(1);
                AudioManager.instance.VictorySFX();
                ResetGridPersistence();
                GameScore("VITÓRIA");
            }
            else
            {
                LettersMatchResult(row);

                row++;
                col = 0;
                attemptNumber++;
                PlayerPrefs.SetInt("attemptNumber", attemptNumber);

                if (attemptNumber == maxAttempts)
                {
                    UpdateStats(0);
                    AudioManager.instance.DefeatSFX();
                    ResetGridPersistence();
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

    public void SaveGridPersistence(int rowIndex)
    {
        // Monta a palavra digitada
        string attemptWord = string.Join("", letterMatrix[rowIndex].Select(letter => letter.text)).ToLower();

        // Normaliza a palavra digitada para encontrar a versão correta com acentos
        string normalizedAttemptWord = RemoveDiacritics(attemptWord);

        // Encontra a palavra original com acentos na baseWordPool
        string correctWord = baseWordPool.FirstOrDefault(word => RemoveDiacritics(word) == normalizedAttemptWord);

        // Se encontrou a versão correta com acentos, salva; senão, salva a digitada
        string wordToSave = (correctWord ?? attemptWord).ToLower();

        // Salva no PlayerPrefs
        PlayerPrefs.SetString("row" + rowIndex, wordToSave);
        PlayerPrefs.Save(); // Garante que os dados sejam gravados imediatamente
    }

    public void ResetGridPersistence()
    {
        for (int i = 0; i < 6; i++)
            PlayerPrefs.SetString($"row{i}", "");
        PlayerPrefs.SetInt("attemptNumber", 0);
    }

    // Logic ----------------------------------------------------------

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

        // Normalizar a palavra digitada para evitar problemas com acentos
        string normalizedAttemptWord = RemoveDiacritics(attemptWord);

        // Verificar se a palavra está na lista baseWordPool
        if (!baseWordPool.Any(word => RemoveDiacritics(word) == normalizedAttemptWord))
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
        string normalizedText = new string(text.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray()) // Remove diacríticos
            .Normalize(NormalizationForm.FormC);

        // Substituir "Ç" por "C" e "ç" por "c"
        return normalizedText.Replace("Ç", "C").Replace("ç", "c");
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

    // After Game ----------------------------------------------------------

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

        PlayerPrefs.Save();
    }

    public void PlayAgain()
    {
        // clear word grid
        CleanWordGrid();

        // reset variables
        attemptWord = "";
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
