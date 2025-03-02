using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for UI components like Button
using TMPro; // Importing TextMeshPro namespace
using UnityEngine.SceneManagement; // For scene management

public class TicTacToe : MonoBehaviour
{
    public Button[] buttons; // Array of Tic-Tac-Toe buttons
    public TMP_Text displayText; // TMP_Text component to show the status
    public Button resetButton; // Reset button
    public Button menuButton; // Menu button

    private string[] board; // 3x3 grid of the game
    private string currentPlayer = "X"; // Player starts with X
    private bool gameOver = false;

    void Start()
    {
        board = new string[9]; // Initialize the board
        resetButton.onClick.AddListener(ResetGame);
        menuButton.onClick.AddListener(GoToMenu);
        UpdateDisplayText();

        // Set listeners for each button
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // To capture index correctly inside the loop
            buttons[i].onClick.AddListener(() => MakeMove(index));
        }
    }

    void MakeMove(int index)
    {
        if (gameOver || !string.IsNullOrEmpty(board[index]))
            return;

        board[index] = currentPlayer;
        buttons[index].GetComponentInChildren<TextMeshProUGUI>().text = currentPlayer; // Using TextMeshProUGUI to update the button's text
        buttons[index].interactable = false; // Disable the button after it's clicked

        if (CheckWinner(currentPlayer))
        {
            gameOver = true;
            displayText.text = $"{currentPlayer} Wins!{GetPlatformInfo()}"; // Display winner message
            HighlightWinningCombination(currentPlayer);
        }
        else if (IsBoardFull())
        {
            gameOver = true;
            displayText.text = $"It's a Draw!{GetPlatformInfo()}"; // Display draw message
        }
        else
        {
            currentPlayer = (currentPlayer == "X") ? "O" : "X"; // Switch turns
            UpdateDisplayText();

            if (currentPlayer == "O") // If it's AI's turn
            {
                StartCoroutine(AIMove()); // AI makes its move
            }
        }
    }

    // Update the display text to show whose turn it is
    void UpdateDisplayText()
    {
        displayText.text = $"{currentPlayer}'s Turn {GetPlatformInfo()}"; // Display whose turn it is and platform info
    }

    // Check if the current player has won
    bool CheckWinner(string player)
    {
        int[][] winningCombinations = new int[][]
        {
            new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 }, new int[] { 6, 7, 8 },
            new int[] { 0, 3, 6 }, new int[] { 1, 4, 7 }, new int[] { 2, 5, 8 },
            new int[] { 0, 4, 8 }, new int[] { 2, 4, 6 }
        };

        foreach (var combination in winningCombinations)
        {
            if (board[combination[0]] == player && board[combination[1]] == player && board[combination[2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    // Check if the board is full
    bool IsBoardFull()
    {
        foreach (var cell in board)
        {
            if (string.IsNullOrEmpty(cell))
                return false;
        }
        return true;
    }

    // Highlight the winning combination
    void HighlightWinningCombination(string player)
    {
        int[][] winningCombinations = new int[][]
        {
            new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 }, new int[] { 6, 7, 8 },
            new int[] { 0, 3, 6 }, new int[] { 1, 4, 7 }, new int[] { 2, 5, 8 },
            new int[] { 0, 4, 8 }, new int[] { 2, 4, 6 }
        };

        foreach (var combination in winningCombinations)
        {
            if (board[combination[0]] == player && board[combination[1]] == player && board[combination[2]] == player)
            {
                // Strike-through or change color of the buttons involved in the winning combination
                buttons[combination[0]].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                buttons[combination[1]].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                buttons[combination[2]].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                break;
            }
        }
    }

    // AI Move - Random choice for simplicity
    IEnumerator AIMove()
    {
        yield return new WaitForSeconds(0.5f); // Simulate AI thinking

        int index = GetRandomAvailableMove();
        MakeMove(index);
    }

    // Get a random available move for AI
    int GetRandomAvailableMove()
    {
        System.Random random = new System.Random();
        int[] availableMoves = new int[9];
        int count = 0;

        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                availableMoves[count] = i;
                count++;
            }
        }

        return availableMoves[random.Next(0, count)];
    }

    // Reset the game state
    void ResetGame()
    {
        board = new string[9];
        currentPlayer = "X";
        gameOver = false;
        displayText.text = ""; // Clear display text
        UpdateDisplayText();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = ""; // Clear button text
            buttons[i].interactable = true; // Enable all buttons
        }
    }

    // Go to Menu scene
    void GoToMenu()
    {
        SceneManager.LoadScene("UIManager"); // Replace "Menu" with the actual name of your menu scene
    }

    // Get platform-specific info to display
    string GetPlatformInfo()
    {
#if UNITY_STANDALONE
        return "Playing on PC";
#elif UNITY_ANDROID
            return "Playing on Android - Touch Enabled";
#else
            return "Platform Unknown";
#endif
    }
}
