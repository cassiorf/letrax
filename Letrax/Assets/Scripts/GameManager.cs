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
using System.Drawing;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Word Database")]
    public TextAsset AllWordsJSONFile;
    public TextAsset SelectionWordsJSONFile;
    [System.NonSerialized] private List<string> baseWordPool = new List<string>();
    [System.NonSerialized] private List<string> selectionBaseWordPool = new List<string>();

    [Header("Settings")]
    public int colorPalette;

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
    public Animator[] rowAnimator;

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
    public Animator[] rowCursorAnimator;

    [Header("Keyboard")]
    public TextMeshProUGUI[] keyboardKeyText;

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

    // load pool with all words
    void LoadWordList()
    {
        baseWordPool = LoadWordsFromJSON(AllWordsJSONFile);
        Debug.Log($"JSON principal carregado com {baseWordPool.Count} palavras.");
    }

    // load pool with selection words
    void LoadSelectionWordList()
    {
        selectionBaseWordPool = LoadWordsFromJSON(SelectionWordsJSONFile);
        Debug.Log($"JSON secundário carregado com {selectionBaseWordPool.Count} palavras.");
    }

    // Função genérica para carregar um JSON em uma lista de palavras
    List<string> LoadWordsFromJSON(TextAsset jsonFile)
    {
        if (jsonFile == null)
        {
            Debug.LogError("Erro: JSONFile não atribuído.");
            return new List<string>();
        }

        WordListData loadedData = JsonUtility.FromJson<WordListData>(jsonFile.text);

        if (loadedData != null && loadedData.words != null)
        {
            return new List<string>(loadedData.words);
        }
        else
        {
            Debug.LogError("Erro: JSON carregado, mas a lista 'words' está nula.");
            return new List<string>();
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

        // load JSON with all words
        LoadWordList();

        // load JSON with most common words to be selected as base word
        LoadSelectionWordList();

        // randomize base word
        baseWord = RandomBaseWord();

        // load progress from previous section
        LoadGridPersistence();
        ManageCursor(row, col, true);

        // load keyboard colors
        //UpdateKeyboardColors();

        // load settings screen
        LoadSettings();

        // load stats screen
        LoadStats();
    }

    string RandomBaseWord()
    {
        if (PlayerPrefs.GetString("row0") == "")
        {
            int randomIndex = UnityEngine.Random.Range(0, selectionBaseWordPool.Count);
            PlayerPrefs.SetString("baseWord", selectionBaseWordPool[randomIndex]);
            PlayerPrefs.Save();
            return selectionBaseWordPool[randomIndex];
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

    public void LoadSettings()
    {
        AudioManager.instance.enableSFX = (PlayerPrefs.GetInt("enableSFX", 2) != 1);

        ColorManager.instance.UpdateSettingsColorButton(PlayerPrefs.GetInt("colorPalette"), false);
    }

    public void LoadGridPersistence()
    {
        // get number of attempts
        attemptNumber = PlayerPrefs.GetInt("attemptNumber");

        // Obtém a baseWord salva e remove os acentos para a lógica de comparação
        string baseWordRaw = PlayerPrefs.GetString("baseWord", "");
        if (string.IsNullOrEmpty(baseWordRaw)) return; // Se não houver palavra salva, sai da função

        string baseWord = RemoveDiacritics(baseWordRaw); // Versão sem acentos para a lógica

        row = 0; // Linha onde o cursor será ativado

        for (; row < 6; row++)
        {
            string savedWord = PlayerPrefs.GetString("row" + row, "");
            if (string.IsNullOrEmpty(savedWord))
            {
                break; // Define a próxima linha vazia para ativar o cursor
            }

            string normalizedSavedWord = RemoveDiacritics(savedWord); // Remove acentos da palavra salva

            for (int col = 0; col < savedWord.Length && col < letterMatrix[row].Length; col++)
            {
                letterMatrix[row][col].text = savedWord[col].ToString().ToUpper(); // Mantém acento na exibição
            }

            // Criar dicionário com contagem de letras da baseWord sem acentos
            Dictionary<char, int> baseWordLetterCount = new Dictionary<char, int>();
            foreach (char c in baseWord)
            {
                if (baseWordLetterCount.ContainsKey(c)) baseWordLetterCount[c]++;
                else baseWordLetterCount[c] = 1;
            }

            // Verifica posição exata (corretos)
            for (int col = 0; col < normalizedSavedWord.Length && col < letterMatrix[row].Length; col++)
            {
                char letter = normalizedSavedWord[col];
                Image bgImage = bgMatrix[row][col];

                if (baseWord[col] == letter) // Letra correta na posição correta
                {
                    PaintKeyboarLetter(letterMatrix[row][col].text, 2);
                    bgImage.color = ColorManager.instance.rightColor;
                    baseWordLetterCount[letter]--; // Marca como usada
                }
            }

            // Verifica se existe em outra posição (parcial) ou se está errada
            for (int col = 0; col < normalizedSavedWord.Length && col < letterMatrix[row].Length; col++)
            {
                char letter = normalizedSavedWord[col];
                Image bgImage = bgMatrix[row][col];

                if (baseWord[col] == letter) continue; // Já marcada como correta

                if (baseWord.Contains(letter) && baseWordLetterCount.ContainsKey(letter) && baseWordLetterCount[letter] > 0)
                {
                    PaintKeyboarLetter(letterMatrix[row][col].text, 1);
                    bgImage.color = ColorManager.instance.partialColor;
                    baseWordLetterCount[letter]--; // Marca como usada
                }
                else
                {
                    PaintKeyboarLetter(letterMatrix[row][col].text, 0);
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
                ResetGridPersistence();
                AudioManager.instance.VictorySFX();
                ColorManager.instance.UpdateAfterGameScreenColor(true);
                GameScore("VITÓRIA");
            }
            else
            {
                LettersMatchResult(row);

                row++;
                col = 0;
                attemptNumber++;
                PlayerPrefs.SetInt("attemptNumber", attemptNumber);
                PlayerPrefs.Save();

                if (attemptNumber == maxAttempts)
                {
                    UpdateStats(0);
                    ResetGridPersistence();
                    AudioManager.instance.DefeatSFX();
                    ColorManager.instance.UpdateAfterGameScreenColor(false);
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
        PlayerPrefs.Save();
    }

    // Game Logic and Mechanics ----------------------------------------------------------

    public bool VerifyNumberOfLettersAndWordDatabase(int rowIndex)
    {
        // Construir a palavra digitada
        string attemptWord = string.Join("", letterMatrix[rowIndex].Select(letter => letter.text.ToLower()));

        // Verificar se tem menos de 5 letras
        if (attemptWord.Length < 5)
        {
            alertText.text = "A PALAVRA ESTÁ INCOMPLETA";
            VFXError(rowIndex);
            AudioManager.instance.ErrorSFX();
            return false;
        }

        // Normalizar a palavra digitada para evitar problemas com acentos
        string normalizedAttemptWord = RemoveDiacritics(attemptWord);

        // Verificar se a palavra está na lista baseWordPool
        if (!baseWordPool.Any(word => RemoveDiacritics(word) == normalizedAttemptWord))
        {
            alertText.text = "ESSA PALAVRA NÃO É ACEITA";
            VFXError(rowIndex);
            AudioManager.instance.ErrorSFX();
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
                PaintKeyboarLetter(letterMatrix[rowIndex][i].text, 2);
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
                PaintKeyboarLetter(letterMatrix[rowIndex][i].text, 1);
                baseWordLetterCount[attemptLetter]--; // Remove do contador
            }
            else
            {
                bgMatrix[rowIndex][i].color = ColorManager.instance.wrongColor;
                PaintKeyboarLetter(letterMatrix[rowIndex][i].text, 0);
            }
        }
    }

    public void PaintKeyboarLetter(string letter, int result)
    {
        // Garante que a letra seja minúscula para evitar problemas de comparação
        letter = RemoveDiacritics(letter).ToLower();

        // Percorre todas as teclas do teclado
        foreach (TextMeshProUGUI keyText in keyboardKeyText)
        {
            if (keyText.text.ToLower() == letter) // Achou a tecla correspondente
            {
                Image keyImage = keyText.transform.parent.GetComponent<Image>(); // Pega o Image do objeto pai

                switch (result)
                {
                    case 0: // Letra errada > só atualiza se tecla não foi pintada ainda
                        if (keyImage.color == ColorManager.instance.keyBGBasicColor)
                        {
                            keyText.color = ColorManager.instance.keyTextWrongColor;
                            keyImage.color = ColorManager.instance.keyBGWrongColor;
                        }
                        break;
                    case 1: // Letra parcial > só não atualiza se for letra certa 
                        if (keyImage.color != ColorManager.instance.rightColor)
                        {
                            keyText.color = ColorManager.instance.keyTextBasicColor;
                            keyImage.color = ColorManager.instance.partialColor;
                        }
                        break;
                    case 2: // Letra correta > sempre atualiza
                        keyText.color = ColorManager.instance.keyTextBasicColor;
                        keyImage.color = ColorManager.instance.rightColor;
                        break;
                }

                break; // Sai do loop após encontrar a tecla certa
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

        // reset keyboad color
        ClearKeyboardColor();

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

    public void ClearKeyboardColor()
    {
        foreach (TextMeshProUGUI keyText in keyboardKeyText)
        {
            keyText.color = ColorManager.instance.keyTextBasicColor;
            keyText.GetComponentInParent<Image>().color = ColorManager.instance.keyBGBasicColor;
        }
    }

    // VFX ----------------------------------------------------------

    public void VFXError(int rowIndex)
    {
        rowAnimator[rowIndex].SetTrigger("Error");
        rowCursorAnimator[rowIndex].SetTrigger("Error");
    }
}
