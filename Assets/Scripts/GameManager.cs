using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    public TMP_Text player1HarvestText; 
    public TMP_Text player2HarvestText; 
    public GameObject endGamePanel;     
    public TMP_Text endGameMessageText; 

    public Transform player1StartPos; 
    public Transform player2StartPos;  
    public AudioClip playerWin;
    public AudioClip knockout;
    private int player1Score = 0;
    private int player2Score = 0;
    private int winThreshold = 3;    

    public Harvest player1Harvest;
    public Harvest player2Harvest;

    private void Start()
    {
        winThreshold = PlayerPrefs.GetInt("WinThreshold", 3);
        endGamePanel.SetActive(false); //hide end-game panel at start
        UpdateScoreUI();
        UpdateHarvestUI();
    }

    //updating score UI for both players
    private void UpdateScoreUI()
    {
        player1ScoreText.text = "Player 1 Score: " + player1Score;
        player2ScoreText.text = "Player 2 Score: " + player2Score;
    }

    //Player 1 wins a round
    public void Player1KnockedOutPlayer2()
    {
        player1Score++;
        AudioManager.instance.PlaySound(knockout);
        UpdateScoreUI();
        CheckForGameOver();
    }

    //Player 2 wins a round
    public void Player2KnockedOutPlayer1()
    {
        player2Score++;
        AudioManager.instance.PlaySound(knockout);
        UpdateScoreUI();
        CheckForGameOver();
    }

    //update the harvest UI based on the current press count
    public void UpdateHarvestUI()
    {
        if (player1Harvest != null)
        {
            int remainingPresses1 = player1Harvest.requiredPresses - player1Harvest.currentPressCount;
            player1HarvestText.text = "Next Harvest: " + remainingPresses1;
        }

        if (player2Harvest != null)
        {
            int remainingPresses2 = player2Harvest.requiredPresses - player2Harvest.currentPressCount;
            player2HarvestText.text = "Next Harvest: " + remainingPresses2;
        }
    }

    //check if either player has reached the win threshold
    private void CheckForGameOver()
    {
        if (player1Score >= winThreshold)
        {
            EndGame("Player 1 Wins!");
        }
        else if (player2Score >= winThreshold)
        {
            EndGame("Player 2 Wins!");
        }
    }

    //end-game menu display
    private void EndGame(string winnerMessage)
    {
        endGamePanel.SetActive(true);              
        AudioManager.instance.PlaySound(playerWin);
        endGameMessageText.text = winnerMessage;   
        Time.timeScale = 0;                        
    }

    //restart game
    public void RestartGame()
    {
        Time.timeScale = 1;                       
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    //return to main menu
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;                      
        SceneManager.LoadScene("MainMenu");        
    }

    //set the win threshold from main menu
    public void SetWinThreshold(int threshold)
    {
        winThreshold = threshold;
    }
}


