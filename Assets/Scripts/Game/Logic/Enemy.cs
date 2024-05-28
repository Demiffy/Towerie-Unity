using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public float speed = 2f;
    public int maxHealth = 100;
    public int baseDamage = 10;
    public int healingAmount = 20;
    public float resistance = 0f;
    public string enemyName = "Enemy";

    private int currentHealth;
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private bool isSlowed = false;
    private float slowDuration = 0f;
    private GameObject uiPanelInstance;
    private Canvas gameUICanvas;

    public GameObject uiPanelPrefab;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Initialize(List<Transform> pathWaypoints)
    {
        waypoints = pathWaypoints;
        transform.position = waypoints[currentWaypointIndex].position;

        GetComponent<SpriteRenderer>().sortingOrder = 5;

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

        int damageDealt = Mathf.CeilToInt(baseDamage * (currentHealth / (float)maxHealth));
        gameManager.PlayerHealth -= damageDealt;
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

        GameManager gameManager = FindObjectOfType<GameManager>();
        int moneyReward = GetMoneyReward();

        gameManager.IncreaseEnemiesKilled();
        gameManager.AddMoney(moneyReward);

        FindObjectOfType<GameUIManager>().ShowMoneyPopup(moneyReward);

        Destroy(uiPanelInstance);
        Destroy(gameObject);
    }

    public int GetMoneyReward()
    {
        switch (enemyType)
        {
            case EnemyType.Fast:
                return 50;
            case EnemyType.Tank:
                return 100;
            case EnemyType.Healer:
                return 75;
            case EnemyType.Boss:
                return 200;
            default:
                return 25;
        }
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
                speed *= 2;
            }
        }
    }

    public void ApplySlow(float duration)
    {
        isSlowed = true;
        slowDuration = duration;
        speed /= 2;
    }

    public void ApplyPoison(int damageOverTime, float duration)
    {
        StartCoroutine(PoisonCoroutine(damageOverTime, duration));
    }

    private IEnumerator PoisonCoroutine(int damageOverTime, float duration)
    {
        float interval = 1f;
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
