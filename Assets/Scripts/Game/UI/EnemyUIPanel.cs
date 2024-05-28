using UnityEngine;
using TMPro;

public class EnemyUIPanel : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI camouflageText;
    public RectTransform panelBackground;

    private Transform targetEnemy;
    private int maxHealth;

    public void Initialize(Transform enemy, string name, string type, int currentHealth, int maxHealth)
    {
        targetEnemy = enemy;
        nameText.text = name;
        typeText.text = type;
        this.maxHealth = maxHealth;
        UpdateHealth(currentHealth);

        // Update camouflage status
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            camouflageText.text = enemyScript.isCamouflaged ? "Camouflaged" : "Visible";
        }
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetEnemy.position + Vector3.up);
            transform.position = screenPosition;

            Enemy enemy = targetEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                UpdateHealth(enemy.GetCurrentHealth());
                camouflageText.text = enemy.isCamouflaged ? "Camouflaged" : "Visible";
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHealth(int currentHealth)
    {
        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}
