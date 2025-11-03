using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;   // for TMP text + input

public class Main : MonoBehaviour
{
    public static Main S;   // singleton

    [Header("Inscribed")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2f;

    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;

    // this is from the book: which powerups are more common
    public eWeaponType[] powerUpFrequency = new eWeaponType[]
    {
        eWeaponType.blaster, eWeaponType.blaster,
        eWeaponType.spread, eWeaponType.shield
    };

    [Header("UI Refs")]
    public GameObject startPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    public TMP_InputField playerNameInput;
    public TMP_Text scoreText;
    public TMP_Text playerNameText;
    public TMP_Text highScoreText;

    [Header("Dynamic")]
    public int score = 0;

    // internal state
    private bool gameRunning = false;
    public bool GameIsRunning => gameRunning;   // <--- this is what Hero wants

    private string currentPlayerName = "Player";
    private int highScore = 0;

    void Awake()
    {
        S = this;

        // UI initial state
        if (startPanel != null) startPanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // don't start spawning yet – user presses Start
        // InvokeRepeating("SpawnEnemy", 1f, 1f / enemySpawnPerSecond);
    }

    // =================== UI ENTRY POINTS ===================

    // called by Start button
    public void StartGameFromUI()
    {
        // read player name
        if (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            currentPlayerName = playerNameInput.text;
        }

        // show on HUD
        if (playerNameText != null)
        {
            playerNameText.text = currentPlayerName;
        }

        // panels
        if (startPanel != null) startPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // start game
        score = 0;
        UpdateScoreUI();

        gameRunning = true;
        // now start spawning
        InvokeRepeating(nameof(SpawnEnemy), 0.5f, 1f / enemySpawnPerSecond);
    }

    // called by Game Over "Play Again" button
    public void RestartGame()
    {
        // easiest: reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =================== GAME LOGIC ===================

    void SpawnEnemy()
    {
        if (!gameRunning) return;
        if (prefabEnemies == null || prefabEnemies.Length == 0) return;

        // pick enemy (array may contain same enemy multiple times for weighting)
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // place it just above the top of the screen
        float enemyInset = enemyInsetDefault;
        float xMin = -BoundsCheck.S.camWidth + enemyInset;
        float xMax = BoundsCheck.S.camWidth - enemyInset;
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(xMin, xMax);
        pos.y = BoundsCheck.S.camHeight + enemyInset;
        go.transform.position = pos;
    }

    // this is what Enemy calls when it dies (from the book)
    public static void SHIP_DESTROYED(Enemy e)
    {
        // add score
        if (S != null && S.gameRunning)
        {
            S.score += e.score;
            S.UpdateScoreUI();
        }

        // maybe drop powerup
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            pUp.SetType(pUpType);
            pUp.transform.position = e.transform.position;
        }
    }

    // =================== HERO DEATH ===================

    // this is what Hero.cs wants to call
    public void HeroDied()
    {
        if (!gameRunning) return;

        gameRunning = false;

        // stop enemy spawning
        CancelInvoke(nameof(SpawnEnemy));

        // check high score
        if (score > highScore)
        {
            highScore = score;
        }

        // show game over
        if (hudPanel != null) hudPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // show high score somewhere (start panel has one)
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    // =================== WEAPONS (from book) ===================

    // quick lookup
    public static WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        foreach (WeaponDefinition def in S.weaponDefinitions)
        {
            if (def.type == wt) return def;
        }
        // if none, return a default one
        return new WeaponDefinition() { type = eWeaponType.none };
    }

    // =================== UI HELPERS ===================
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
