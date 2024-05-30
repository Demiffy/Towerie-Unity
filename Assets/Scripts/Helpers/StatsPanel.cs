using UnityEngine;
using TMPro;

public class StatsPanel : MonoBehaviour
{
	public GameObject statsPanel;
	public TextMeshProUGUI statsText;

	private TowerPlacementManager towerPlacementManager;

	void Start()
	{
		towerPlacementManager = FindObjectOfType<TowerPlacementManager>();

		// Hide the stats panel initially
		statsPanel.SetActive(false);
	}

	public void ShowStats(int index)
	{
		if (towerPlacementManager.IsPlacingTower()) return;

		Tower tower = towerPlacementManager.towerPrefabs[index].GetComponent<Tower>();
		statsPanel.SetActive(true);
		statsText.text = $"Tower {tower.towerName} Stats:\nDamage: {tower.damage}\nRange: {tower.range}\nRate: {tower.fireRate}\nCan See Camo: {(tower.canSeeCamo ? "Yes" : "No")}";
	}

	public void HideStats()
	{
		statsPanel.SetActive(false);
	}
}
