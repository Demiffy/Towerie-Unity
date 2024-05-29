using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TowerPlacementManager : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    public Camera mainCamera;
    public float minTowerDistance = 1.0f;
    public float minPathDistance = 0.1f;
    public float minObstacleDistance = 0.1f;

    private GameObject currentTower;
    private bool isPlacingTower = false;
    private List<GameObject> placedTowers = new List<GameObject>();
    private Tilemap pathTilemap;
    private Tilemap backgroundTilemap;
    private Tilemap obstaclesTilemap;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (isPlacingTower)
        {
            HandleTowerPlacement();
        }

        if (Input.GetMouseButtonDown(0) && isPlacingTower)
        {
            PlaceTower();
        }

        if (Input.GetMouseButtonDown(2) && isPlacingTower) // Middle mouse button
        {
            CancelPlacingTower();
        }
    }

    public void StartPlacingTower(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Length) return;

        Tower tower = towerPrefabs[towerIndex].GetComponent<Tower>();

        if (gameManager.PlayerMoney < tower.cost)
        {
            Debug.Log("Not enough money to place this tower!");
            return;
        }

        if (currentTower != null)
        {
            Destroy(currentTower);
        }

        currentTower = Instantiate(towerPrefabs[towerIndex]);
        isPlacingTower = true;
    }

    private void HandleTowerPlacement()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        currentTower.transform.position = mouseWorldPos;

        // Check if the current position is valid for placement
        if (IsValidPlacement(mouseWorldPos))
        {
            currentTower.GetComponent<SpriteRenderer>().color = Color.white; // Valid placement
        }
        else
        {
            currentTower.GetComponent<SpriteRenderer>().color = Color.red; // Invalid placement
        }
    }

    private bool IsValidPlacement(Vector3 position)
    {
        // Check distance from other towers
        foreach (GameObject tower in placedTowers)
        {
            if (Vector3.Distance(position, tower.transform.position) < minTowerDistance)
            {
                return false;
            }
        }

        // Check distance from path tiles
        if (pathTilemap != null)
        {
            Vector3Int cellPosition = pathTilemap.WorldToCell(position);
            foreach (Vector3Int adjacentCell in GetAdjacentCells(cellPosition))
            {
                if (pathTilemap.HasTile(adjacentCell))
                {
                    if (Vector3.Distance(position, pathTilemap.GetCellCenterWorld(adjacentCell)) < minPathDistance)
                    {
                        return false;
                    }
                }
            }
        }

        // Check distance from obstacle tiles
        if (obstaclesTilemap != null)
        {
            Vector3Int cellPosition = obstaclesTilemap.WorldToCell(position);
            foreach (Vector3Int adjacentCell in GetAdjacentCells(cellPosition))
            {
                if (obstaclesTilemap.HasTile(adjacentCell))
                {
                    if (Vector3.Distance(position, obstaclesTilemap.GetCellCenterWorld(adjacentCell)) < minObstacleDistance)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private List<Vector3Int> GetAdjacentCells(Vector3Int cellPosition)
    {
        List<Vector3Int> adjacentCells = new List<Vector3Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                adjacentCells.Add(new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z));
            }
        }

        return adjacentCells;
    }

    private void PlaceTower()
    {
        Vector3 towerPosition = currentTower.transform.position;

        if (IsValidPlacement(towerPosition))
        {
            Tower tower = currentTower.GetComponent<Tower>();

            if (gameManager.PlayerMoney >= tower.cost)
            {
                gameManager.PlayerMoney -= tower.cost;
                placedTowers.Add(currentTower);
                currentTower = null;
                isPlacingTower = false;

                FindObjectOfType<GameUIManager>().HideStatsPanel();
            }
            else
            {
                Debug.Log("Not enough money to place this tower!");
            }
        }
    }

    public void CancelPlacingTower()
    {
        if (currentTower != null)
        {
            Destroy(currentTower);
            currentTower = null;
            isPlacingTower = false;
        }
    }

    public void SetPathTilemap(Tilemap tilemap)
    {
        pathTilemap = tilemap;
    }

    public void SetBackgroundTilemap(Tilemap tilemap)
    {
        backgroundTilemap = tilemap;
    }

    public void SetObstaclesTilemap(Tilemap tilemap)
    {
        obstaclesTilemap = tilemap;
    }
}
