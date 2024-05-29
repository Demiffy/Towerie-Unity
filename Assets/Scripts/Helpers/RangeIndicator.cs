using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private Tower tower;

    public void Initialize(Tower tower)
    {
        this.tower = tower;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            tower.AddEnemyInRange(enemy);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            tower.RemoveEnemyInRange(enemy);
        }
    }
}
