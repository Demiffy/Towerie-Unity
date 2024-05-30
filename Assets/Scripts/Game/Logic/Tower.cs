using UnityEngine;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
	public string towerName;
	public int cost;
	public float damage;
	public float range;
	public float fireRate;
	public bool canSeeCamo;
	public GameObject bulletPrefab;
	public Transform firePoint;

	public GameObject rangeIndicatorPrefab;

	private GameObject rangeIndicatorInstance;
	private CircleCollider2D rangeCollider;
	private List<Enemy> enemiesInRange = new List<Enemy>();
	private Enemy targetEnemy;
	private bool isPlaced = false;
	private float rotationSpeed = 5f;
	private float fireCooldown = 0f;

	void Awake()
	{
		if (rangeIndicatorPrefab != null)
		{
			rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity, transform);
			rangeIndicatorInstance.transform.localScale = new Vector3(range * 2 / 2.56f, range * 2 / 2.56f, 1);
			rangeCollider = rangeIndicatorInstance.AddComponent<CircleCollider2D>();
			rangeCollider.isTrigger = true;
			rangeCollider.radius = 2.56f;

			RangeIndicator rangeIndicator = rangeIndicatorInstance.AddComponent<RangeIndicator>();
			rangeIndicator.Initialize(this);

			rangeIndicatorInstance.GetComponent<SpriteRenderer>().enabled = false;
			rangeCollider.enabled = false;
		}
	}

	void Update()
	{
		if (!isPlaced) return;

		SelectTarget();

		if (targetEnemy != null)
		{
			LookAtTarget();

			fireCooldown -= Time.deltaTime;
			if (fireCooldown <= 0f)
			{
				Shoot();
				fireCooldown = 1f / fireRate;
			}
		}
	}

	public void ShowRangeIndicator()
	{
		if (rangeIndicatorInstance != null)
		{
			rangeIndicatorInstance.GetComponent<SpriteRenderer>().enabled = true;
		}
	}

	public void HideRangeIndicator()
	{
		if (rangeIndicatorInstance != null)
		{
			rangeIndicatorInstance.GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	public void EnableRangeCollider()
	{
		if (rangeCollider != null)
		{
			rangeCollider.enabled = true;
		}
	}

	public void DisableRangeCollider()
	{
		if (rangeCollider != null)
		{
			rangeCollider.enabled = false;
		}
	}

	public void SetPlaced(bool placed)
	{
		isPlaced = placed;
		if (placed)
		{
			EnableRangeCollider();
		}
	}

	private void SelectTarget()
	{
		targetEnemy = null;
		float maxDistance = float.MinValue;

		foreach (Enemy enemy in enemiesInRange)
		{
			float distance = enemy.GetDistanceAlongPath();
			if (distance > maxDistance)
			{
				maxDistance = distance;
				targetEnemy = enemy;
			}
		}
	}

	private void LookAtTarget()
	{
		if (targetEnemy != null)
		{
			Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
		}
	}

	private void Shoot()
	{
		if (bulletPrefab != null && firePoint != null && targetEnemy != null)
		{
			GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
			Bullet bullet = bulletInstance.GetComponent<Bullet>();
			if (bullet != null)
			{
				bullet.Initialize(targetEnemy, damage, canSeeCamo);
			}
		}
	}

	public void AddEnemyInRange(Enemy enemy)
	{
		if (isPlaced && enemy != null && (!enemy.isCamouflaged || canSeeCamo))
		{
			enemiesInRange.Add(enemy);
		}
	}

	public void RemoveEnemyInRange(Enemy enemy)
	{
		if (enemy != null)
		{
			enemiesInRange.Remove(enemy);
		}
	}
}
