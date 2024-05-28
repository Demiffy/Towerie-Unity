using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public GameObject[] maps;
    public EnemySpawner enemySpawner;

    private GameObject currentMap;
    private TowerPlacementManager towerPlacementManager;

    void Start()
    {
        towerPlacementManager = FindObjectOfType<TowerPlacementManager>();
        int mapIndex = PlayerPrefs.GetInt("SelectedMapIndex", 0);
        LoadMap(mapIndex);
        Debug.Log($"Map with index {mapIndex} loaded!");
    }

    public void LoadMap(int index)
    {
        if (currentMap != null)
        {
            Destroy(currentMap);
        }

        if (index >= 0 && index < maps.Length)
        {
            currentMap = Instantiate(maps[index], Vector3.zero, Quaternion.identity);

            // Find the path parent in the newly instantiated map
            Transform pathParent = currentMap.transform.Find("Grid/PathTilemap/Path");
            if (pathParent != null)
            {
                enemySpawner.SetPathParent(pathParent);
            }
            else
            {
                string errorMessage = "Path parent not found in the loaded map!";
                Debug.LogError(errorMessage);
            }

            // Find the BackgroundTilemap in the newly instantiated map
            Tilemap backgroundTilemap = currentMap.transform.Find("Grid/BackgroundTilemap").GetComponent<Tilemap>();
            if (backgroundTilemap != null)
            {
                towerPlacementManager.SetBackgroundTilemap(backgroundTilemap);
            }
            else
            {
                string errorMessage = "BackgroundTilemap not found in the loaded map!";
                Debug.LogError(errorMessage);
            }

            // Find the PathTilemap in the newly instantiated map
            Tilemap pathTilemap = currentMap.transform.Find("Grid/PathTilemap").GetComponent<Tilemap>();
            if (pathTilemap != null)
            {
                towerPlacementManager.SetPathTilemap(pathTilemap);
            }
            else
            {
                string errorMessage = "PathTilemap not found in the loaded map!";
                Debug.LogError(errorMessage);
            }

            // Find the ObstaclesTilemap in the newly instantiated map
            Tilemap obstaclesTilemap = currentMap.transform.Find("Grid/ObstaclesTilemap").GetComponent<Tilemap>();
            if (obstaclesTilemap != null)
            {
                towerPlacementManager.SetObstaclesTilemap(obstaclesTilemap);
            }
            else
            {
                string errorMessage = "ObstaclesTilemap not found in the loaded map!";
                Debug.LogError(errorMessage);
            }
        }
        else
        {
            string errorMessage = "Map index out of range!";
            Debug.LogError(errorMessage);
        }
    }

    public int GetTotalMaps()
    {
        return maps.Length;
    }
}
