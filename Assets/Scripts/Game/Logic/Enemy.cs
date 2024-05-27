using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public EnemyType enemyType;
	public float speed = 2f;
	public int maxHealth = 100;  // Max health
	public int damage = 10;
	public int healingAmount = 20;
	public float resistance = 0f;
	public string enemyName = "Enemy";

	private int currentHealth; // Current health
	private List<Transform> waypoints;
	private int currentWaypointIndex = 0;
	private bool isSlowed = false;
	private float slowDuration = 0f;
	private GameObject uiPanelInstance;

	public GameObject uiPanelPrefab; // Assign this in the inspector
	private Canvas gameUICanvas; // Reference to the GameUICanvas

	void Awake()
	{
		currentHealth = maxHealth; // Initialize current health
	}

	public void Initialize(List<Transform> pathWaypoints)
	{
		waypoints = pathWaypoints;
		transform.position = waypoints[currentWaypointIndex].position;

		GetComponent<SpriteRenderer>().sortingOrder = 5;

		// Find the GameUICanvas at runtime
		gameUICanvas = FindObjectOfType<Canvas>();
	}

	void Update()
	{
		Move();
		HandleStatusEffects();
	}

	private void Move()
	{
		if (currentWaypointIndex < waypoints.Count)
		{
			Vector3 targetPosition = waypoints[currentWaypointIndex].position;
			Vector3 movementDirection = (targetPosition - transform.position).normalized;
			transform.position += movementDirection * speed * Time.deltaTime;

			if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
			{
				currentWaypointIndex++;

				if (currentWaypointIndex >= waypoints.Count)
				{
					ReachEndOfPath();
				}
			}
		}
	}

	private void ReachEndOfPath()
	{
		Destroy(gameObject);
		GameManager gameManager = FindObjectOfType<GameManager>();
		gameManager.PlayerHealth -= damage;
		FindObjectOfType<GameUIManager>().UpdateUI();

		if (gameManager.PlayerHealth <= 0)
		{
			gameManager.EndGame();
		}
	}

	public void TakeDamage(int damage)
	{
		int actualDamage = Mathf.RoundToInt(damage * (1f - resistance));
		currentHealth -= actualDamage;

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		if (enemyType == EnemyType.Healer)
		{
			HealNearbyEnemies();
		}

		Destroy(uiPanelInstance); // Destroy the UI panel if the enemy dies
		Destroy(gameObject);
	}

	private void HealNearbyEnemies()
	{
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 5f);
		foreach (var hitCollider in hitColliders)
		{
			Enemy enemy = hitCollider.GetComponent<Enemy>();
			if (enemy != null && enemy != this)
			{
				enemy.currentHealth += healingAmount;
				if (enemy.currentHealth > enemy.maxHealth)
				{
					enemy.currentHealth = enemy.maxHealth;
				}
			}
		}
	}

	private void HandleStatusEffects()
	{
		if (isSlowed)
		{
			slowDuration -= Time.deltaTime;
			if (slowDuration <= 0)
			{
				isSlowed = false;
				speed *= 2; // Reset speed to normal
			}
		}
	}

	public void ApplySlow(float duration)
	{
		isSlowed = true;
		slowDuration = duration;
		speed /= 2; // Reduce speed
	}

	public void ApplyPoison(int damageOverTime, float duration)
	{
		StartCoroutine(PoisonCoroutine(damageOverTime, duration));
	}

	private IEnumerator PoisonCoroutine(int damageOverTime, float duration)
	{
		float interval = 1f; // Damage every second
		while (duration > 0)
		{
			TakeDamage(damageOverTime);
			yield return new WaitForSeconds(interval);
			duration -= interval;
		}
	}

	void OnMouseDown()
	{
		if (uiPanelInstance == null && gameUICanvas != null)
		{
			uiPanelInstance = Instantiate(uiPanelPrefab, gameUICanvas.transform);
			uiPanelInstance.GetComponent<EnemyUIPanel>().Initialize(transform, enemyName, enemyType.ToString(), currentHealth, maxHealth);
		}
	}

	public int GetCurrentHealth()
	{
		return currentHealth;
	}

	public int GetMaxHealth()
	{
		return maxHealth;
	}
}
