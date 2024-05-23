using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public int health = 100;

    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;

    public void Initialize(List<Transform> pathWaypoints)
    {
        waypoints = pathWaypoints;
        transform.position = waypoints[currentWaypointIndex].position;

        GetComponent<SpriteRenderer>().sortingOrder = 5;
    }

    void Update()
    {
        Move();
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
        GameUIManager gameUIManager = FindObjectOfType<GameUIManager>();
        gameUIManager.playerHealth -= 10;
        gameUIManager.UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
