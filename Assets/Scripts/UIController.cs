using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public TextMeshProUGUI manaText;
    public TextMeshProUGUI enemyManaText;

    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;

    public GameObject lowManaWarning;
    public float manaShowTimer = 2f;
    public float manaShowCounter;

    public GameObject endPlayerTurnButton;

    public GameObject endScreen;
    public TextMeshProUGUI winnerText;

    public GameObject pauseScreen;
    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (manaShowCounter > 0)
        {
            manaShowCounter -= Time.deltaTime;
        }
        else
            lowManaWarning.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnPause();
        }
    }
    public void SetManaText(int manaAmount)
    {
        manaText.text = "Mana: " + manaAmount;
    }
    public void SetEnemyManaText(int manaAmount)
    {
        enemyManaText.text = "Enemy Mana: " + manaAmount;
    }
    public void SetPlayerHealthText(int playerHealthAmount)
    {
        playerHealthText.text = "Player's Health: " + playerHealthAmount;
    }
    public void SetEnemyHealthText(int enemyHealthAmount)
    {
        enemyHealthText.text = "Enemy's Health: " + enemyHealthAmount;
    }
    public void ShowLowManaWarning()
    {
        lowManaWarning.SetActive(true);
        manaShowCounter = manaShowTimer;
    }

    public void EndPlayerTurn()
    {
        BattleController.instance.EndPlayerTurn();
    }

    public void MainMenu()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("Main Menu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Battle");
    }

    public void SelectNewLevel()
    {
        //TODO THIS PART WILL BE UPDATED WHEN I DECIDE DESIGN NEW LEVELS
        //It will be open a new screen that we can choose which level we prefer to use
        //Every level will has new visuality and new difficulty
    }

    public void SetWinnerText(bool isWin)
    {
        if (isWin)
        {
            winnerText.text = "YOU WIN!";
        }
        else
        {
            winnerText.text = "YOU LOST";
        }
    }

    public void PauseUnPause()
    {
        if (pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
