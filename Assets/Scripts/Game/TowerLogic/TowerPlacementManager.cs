using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacementManager : MonoBehaviour
{
    public GameObject[] towerPrefabs; // Array of tower prefabs
    public LayerMask pathMask; // Layer mask for path
    public LayerMask obstacleMask; // Layer mask for obstacles
    public LayerMask towerMask; // Layer mask for placed towers
    public Sprite rangeSprite; // Pre-made circle sprite for range indicator
    public string sortingLayerName = "UI"; // Sorting layer name for tower and range indicator
    public int sortingOrder = 5; // Sorting order for tower and range indicator
    public float minTowerDistance = 2f; // Minimum distance between towers
    public float minPathDistance = 1f; // Minimum distance from paths
    public float minObstacleDistance = 1f; // Minimum distance from obstacles

    private GameObject currentTower;
    private SpriteRenderer towerSpriteRenderer;
    private GameObject rangeIndicator;
    private bool canPlace = false;

    void Update()
    {
        if (currentTower != null)
        {
            MoveCurrentTowerToMouse();

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceTower();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    public void StartPlacingTower(int towerIndex)
    {
        if (currentTower != null)
        {
            Destroy(currentTower);
        }

        currentTower = Instantiate(towerPrefabs[towerIndex]);
        towerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();
        towerSpriteRenderer.sortingLayerName = sortingLayerName;
        towerSpriteRenderer.sortingOrder = sortingOrder;

        CreateRangeIndicator();
    }

    private void MoveCurrentTowerToMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentTower.transform.position = mousePosition;

        canPlace = CheckPlacementValidity(mousePosition);

        towerSpriteRenderer.color = canPlace ? Color.white : Color.red;
    }

    private bool CheckPlacementValidity(Vector2 position)
    {
        // Check distance from other towers
        Collider2D[] towerColliders = Physics2D.OverlapCircleAll(position, minTowerDistance);
        foreach (Collider2D collider in towerColliders)
        {
            if (collider.gameObject != currentTower && collider.gameObject.layer == LayerMask.NameToLayer("Tower"))
            {
                return false;
            }
        }

        // Check distance from paths
        Collider2D[] pathColliders = Physics2D.OverlapCircleAll(position, minPathDistance, pathMask);
        if (pathColliders.Length > 0)
        {
            return false;
        }

        // Check distance from obstacles
        Collider2D[] obstacleColliders = Physics2D.OverlapCircleAll(position, minObstacleDistance, obstacleMask);
        if (obstacleColliders.Length > 0)
        {
            return false;
        }

        return true;
    }

    private void CreateRangeIndicator()
    {
        rangeIndicator = new GameObject("RangeIndicator");
        SpriteRenderer rangeSpriteRenderer = rangeIndicator.AddComponent<SpriteRenderer>();
        rangeSpriteRenderer.sprite = rangeSprite;
        rangeSpriteRenderer.color = new Color(1, 1, 1, 0.3f); // Semi-transparent white
        rangeSpriteRenderer.sortingLayerName = sortingLayerName;
        rangeSpriteRenderer.sortingOrder = sortingOrder - 1; // Slightly behind the tower sprite

        Tower tower = currentTower.GetComponent<Tower>();
        rangeIndicator.transform.localScale = new Vector3(tower.range * 2, tower.range * 2, 1);

        rangeIndicator.transform.SetParent(currentTower.transform, false);
    }

    private void PlaceTower()
    {
        currentTower.layer = LayerMask.NameToLayer("Tower");
        Destroy(rangeIndicator);
        currentTower = null;
        GameUIManager.Instance.DeselectTower(); // Ensure to hide stats and deselect tower
    }

    private void CancelPlacement()
    {
        Destroy(currentTower);
        currentTower = null;
    }

    public void DeselectTower()
    {
        CancelPlacement();
    }
}
