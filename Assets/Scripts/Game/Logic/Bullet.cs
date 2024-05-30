using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float speed = 10f;
	private Enemy target;
	private float damage;
	private bool canSeeCamo;

	public void Initialize(Enemy target, float damage, bool canSeeCamo)
	{
		this.target = target;
		this.damage = damage;
		this.canSeeCamo = canSeeCamo;
	}

	void Update()
	{
		if (target == null)
		{
			Destroy(gameObject);
			return;
		}

		Vector3 direction = (target.transform.position - transform.position).normalized;
		transform.position += direction * speed * Time.deltaTime;

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

		if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
		{
			target.TakeDamage(Mathf.RoundToInt(damage), canSeeCamo);
			Destroy(gameObject);
		}
	}
}