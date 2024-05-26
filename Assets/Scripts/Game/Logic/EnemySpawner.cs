using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public float spawnInterval = 2f;

    private List<Transform> waypoints = new List<Transform>();
    private float spawnTimer;
    private bool spawningEnabled = false;

    void Start()
    {
        enabled = false;
    }

    void Update()
    {
        if (!spawningEnabled) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    public void SetPathParent(Transform pathParent)
    {
        waypoints.Clear();
        foreach (Transform child in pathParent)
        {
            waypoints.Add(child);
        }

        waypoints.Sort((a, b) => CompareWaypointNames(a.name, b.name));
    }

    public void StartSpawning()
    {
        enabled = true;
        spawningEnabled = true;
        SpawnEnemy();
        spawnTimer = spawnInterval;
    }

    public void StopSpawning()
    {
        spawningEnabled = false;
    }

    private void SpawnEnemy()
    {
        if (waypoints.Count == 0) return;

        // Select a random enemy prefab from the list
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        GameObject enemy = Instantiate(randomEnemyPrefab, waypoints[0].position, Quaternion.identity);
        enemy.GetComponent<Enemy>().Initialize(waypoints);
    }

    private int CompareWaypointNames(string nameA, string nameB)
    {
        if (nameA == "Waypoint-start") return -1;
        if (nameB == "Waypoint-start") return 1;
        if (nameA == "Waypoint-end") return 1;
        if (nameB == "Waypoint-end") return -1;

        int numberA = ExtractNumberFromName(nameA);
        int numberB = ExtractNumberFromName(nameB);
        return numberA.CompareTo(numberB);
    }

    private int ExtractNumberFromName(string name)
    {
        string numberString = name.Replace("Waypoint-", "");
        if (int.TryParse(numberString, out int number))
        {
            return number;
        }
        return int.MaxValue;
    }
}
