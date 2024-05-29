using UnityEngine;

public class Tower : MonoBehaviour
{
    public string towerName;
    public int cost;
    public float damage;
    public float range;
    public float fireRate;
    public bool canSeeCamo;

    public GameObject rangeIndicatorPrefab;

    private GameObject rangeIndicatorInstance;

    void Awake()
    {
        if (rangeIndicatorPrefab != null)
        {
            rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity, transform);
            rangeIndicatorInstance.transform.localScale = new Vector3(range * 2, range * 2, 1);
            rangeIndicatorInstance.SetActive(false);
        }
    }

    public void ShowRangeIndicator()
    {
        if (rangeIndicatorInstance != null)
        {
            rangeIndicatorInstance.SetActive(true);
        }
    }

    public void HideRangeIndicator()
    {
        if (rangeIndicatorInstance != null)
        {
            rangeIndicatorInstance.SetActive(false);
        }
    }
}
