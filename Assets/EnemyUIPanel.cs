using UnityEngine;
using TMPro;

public class EnemyUIPanel : MonoBehaviour
{
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI typeText;
	public TextMeshProUGUI healthText;
	public RectTransform panelBackground; // Reference to the panel's background RectTransform

	private Transform targetEnemy;

	public void Initialize(Transform enemy, string name, string type)
	{
		targetEnemy = enemy;
		nameText.text = name;
		typeText.text = type;
	}

	void Update()
	{
		if (targetEnemy != null)
		{
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetEnemy.position + Vector3.up); // Adjust offset as needed
			transform.position = screenPosition;

			Enemy enemy = targetEnemy.GetComponent<Enemy>();
			if (enemy != null)
			{
				healthText.text = $"Health: {enemy.health}";
			}
		}
		else
		{
			Destroy(gameObject); // Destroy the panel if the enemy is destroyed
		}
	}
}