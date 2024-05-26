using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int currentWave = 1;
    public float preparationTime = 10f;
    public float attackTime = 20f;
    public int playerMoney = 1000;
    public int playerHealth = 100;

    private string currentPhase = "Preparation";
    private float timeRemaining;
    private MapManager mapManager;
    private EnemySpawner enemySpawner;

    void Start()
    {
        timeRemaining = preparationTime;
        enemySpawner = FindObjectOfType<EnemySpawner>();
        mapManager = FindObjectOfType<MapManager>();
        enemySpawner.StopSpawning();
    }

    void Update()
    {
        // Update the time remaining
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            // Handle phase transition
            if (currentPhase == "Preparation")
            {
                StartAttackPhase();
            }
            else if (currentPhase == "Attack")
            {
                StartPreparationPhase();
                currentWave++;
            }
        }
    }

    public void SkipPhase()
    {
        if (currentPhase == "Preparation")
        {
            StartAttackPhase();
        }
        else if (currentPhase == "Attack")
        {
            StartPreparationPhase();
            currentWave++;
        }
    }

    public int CurrentWave
    {
        get => currentWave;
        set => currentWave = value;
    }
    public string CurrentPhase => currentPhase;
    public float TimeRemaining => timeRemaining;
    public int PlayerMoney
    {
        get => playerMoney;
        set => playerMoney = value;
    }
    public int PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = value;
            if (playerHealth <= 0)
            {
                EndGame();
            }
        }
    }

    private void StartPreparationPhase()
    {
        currentPhase = "Preparation";
        timeRemaining = preparationTime;
        enemySpawner.StopSpawning();
        Debug.Log("Preparation Phase Started");
    }

    private void StartAttackPhase()
    {
        currentPhase = "Attack";
        timeRemaining = attackTime;
        enemySpawner.StartSpawning();
        Debug.Log("Attack Phase Started");
    }

    public void EndGame()
    {
        Debug.Log($"Game Over! Highest wave reached: {currentWave}");
        StartCoroutine(EndGameSequence());
    }

    private IEnumerator EndGameSequence()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Perform cleanup actions after the main menu is loaded
        DestroyAllEnemies();
        mapManager.UnloadCurrentMap();
    }

    private void DestroyAllEnemies()
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            Destroy(enemy.gameObject);
        }
    }
}
