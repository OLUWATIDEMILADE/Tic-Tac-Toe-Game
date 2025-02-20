using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Required for scene management

public class TicTacToe : MonoBehaviour
{
    public Button[] gridButtons;
    public TMP_Text displayText;
    public Button resetButton;
    public Button backToMenuButton;

    private string currentPlayer;
    private string[] board;
    private bool gameOver = false;

    void Start()
    {
        Debug.Log("TicTacToe script started!");

#if UNITY_STANDALONE
        displayText.text = "Playing on PC";
#elif UNITY_ANDROID
        displayText.text = "Playing on Android - Touch Enabled";
#endif


        ResetBoard();

        for (int i = 0; i < gridButtons.Length; i++)
        {
            int index = i;
            gridButtons[i].onClick.RemoveAllListeners();
            gridButtons[i].onClick.AddListener(() => OnGridClick(index));
        }

        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(ResetBoard);
        backToMenuButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.AddListener(BackToMenu);
    }

    void Update()
    {
#if UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Debug.Log("Mouse Click on PC");
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Touch detected on Android");
        }
#endif
    }

    public void ResetBoard()
    {
        gameOver = false;
        currentPlayer = "X"; // Player starts first
        displayText.text = "Next Player: X";
        resetButton.gameObject.SetActive(false);

        board = new string[9];

        for (int i = 0; i < gridButtons.Length; i++)
        {
            board[i] = "";
            TMP_Text buttonText = gridButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.text = "";
            gridButtons[i].GetComponent<Image>().color = Color.white;
            gridButtons[i].interactable = true;
        }
    }

    public void OnGridClick(int index)
    {
        if (gameOver || !string.IsNullOrEmpty(board[index])) return;

        board[index] = currentPlayer;
        gridButtons[index].GetComponentInChildren<TMP_Text>().text = currentPlayer;
        gridButtons[index].interactable = false;

        if (CheckWinner()) return;
        if (IsDraw()) return;

        currentPlayer = "O";
        displayText.text = "AI is Thinking...";

        Invoke("MakeAIMove", 1.0f);
    }

    private void MakeAIMove()
    {
        if (gameOver) return;

        int bestMove = GetBestMove();
        board[bestMove] = "O";
        gridButtons[bestMove].GetComponentInChildren<TMP_Text>().text = "O";
        gridButtons[bestMove].interactable = false;

        if (CheckWinner()) return;
        if (IsDraw()) return;

        currentPlayer = "X";
        displayText.text = "Next Player: X";
    }

    private int GetBestMove()
    {
        List<int> availableMoves = new List<int>();
        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
                availableMoves.Add(i);
        }

        if (availableMoves.Count == 0) return -1;

        int depthLimit = 2;
        int bestScore = int.MinValue;
        int move = availableMoves[Random.Range(0, availableMoves.Count)];

        foreach (int i in availableMoves)
        {
            board[i] = "O";
            int score = Minimax(board, 0, false, depthLimit);
            board[i] = "";

            if (score > bestScore)
            {
                bestScore = score;
                move = i;
            }
        }

        return move;
    }

    private int Minimax(string[] newBoard, int depth, bool isMaximizing, int depthLimit)
    {
        if (depth >= depthLimit) return 0;

        string result = CheckWinState();
        if (result == "O") return 10 - depth;
        if (result == "X") return depth - 10;
        if (result == "Draw") return 0;

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;
        for (int i = 0; i < newBoard.Length; i++)
        {
            if (string.IsNullOrEmpty(newBoard[i]))
            {
                newBoard[i] = isMaximizing ? "O" : "X";
                int score = Minimax(newBoard, depth + 1, !isMaximizing, depthLimit);
                newBoard[i] = "";
                bestScore = isMaximizing ? Mathf.Max(score, bestScore) : Mathf.Min(score, bestScore);
            }
        }
        return bestScore;
    }

    private string CheckWinState()
    {
        int[,] winPatterns = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
            {0, 4, 8}, {2, 4, 6}
        };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            int a = winPatterns[i, 0], b = winPatterns[i, 1], c = winPatterns[i, 2];
            if (!string.IsNullOrEmpty(board[a]) && board[a] == board[b] && board[a] == board[c])
            {
                return board[a];
            }
        }

        foreach (string cell in board)
        {
            if (string.IsNullOrEmpty(cell)) return null;
        }

        return "Draw";
    }

    private bool CheckWinner()
    {
        string winner = CheckWinState();
        if (winner == null) return false;

        if (winner == "X" || winner == "O")
        {
            displayText.text = $"{winner} Wins!";
            gameOver = true;
            resetButton.gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    private void StrikeThrough(int a, int b, int c)
    {
        Image imgA = gridButtons[a].GetComponent<Image>();
        Image imgB = gridButtons[b].GetComponent<Image>();
        Image imgC = gridButtons[c].GetComponent<Image>();

        imgA.color = Color.red;
        imgB.color = Color.red;
        imgC.color = Color.red;

        imgA.SetAllDirty();
        imgB.SetAllDirty();
        imgC.SetAllDirty();
    }

    private bool IsDraw()
    {
        if (CheckWinState() == "Draw")
        {
            displayText.text = "It's a Draw!";
            gameOver = true;
            resetButton.gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("UIScene");
    }
}
