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
	private int enemiesKilled = 0;

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
		timeRemaining -= Time.deltaTime;
		if (timeRemaining <= 0)
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
		FindObjectOfType<GameUIManager>().ShowWaveComplete();
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
		FindObjectOfType<GameUIManager>().ShowEndGamePanel(currentWave, enemiesKilled);
	}

	private IEnumerator EndGameSequence()
	{
		FindObjectOfType<GameUIManager>().ShowEndGamePanel(currentWave, enemiesKilled);
		Time.timeScale = 0f;
		yield return null;
	}

	private void DestroyAllEnemies()
	{
		foreach (Enemy enemy in FindObjectsOfType<Enemy>())
		{
			Destroy(enemy.gameObject);
		}
	}

	public void DamageAllEnemies(int damage)
	{
		foreach (Enemy enemy in FindObjectsOfType<Enemy>())
		{
			enemy.TakeDamage(damage, false);
		}
	}

	public void KillAllEnemies()
	{
		foreach (Enemy enemy in FindObjectsOfType<Enemy>())
		{
			int moneyReward = enemy.GetMoneyReward();
			IncreaseEnemiesKilled();
			AddMoney(moneyReward);
			FindObjectOfType<GameUIManager>().ShowMoneyPopup(moneyReward);
			Destroy(enemy.gameObject);
		}
	}

	public void IncreaseEnemiesKilled()
	{
		enemiesKilled++;
	}

	public void AddMoney(int amount)
	{
		playerMoney += amount;
		FindObjectOfType<GameUIManager>().UpdateUI();
	}
}
