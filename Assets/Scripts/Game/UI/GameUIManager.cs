using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameUIManager : MonoBehaviour
{
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI phaseText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI healthText;

    public Button[] towerButtons;
    public TextMeshProUGUI[] towerPrices;
    public TextMeshProUGUI[] towerNames;
    public Image[] towerImages;
    public Sprite[] towerSprites;
    public Button skipButton;
    public TextMeshProUGUI notEnoughMoneyText;

    public GameObject endGamePanel;
    public TextMeshProUGUI endGameWaveText;
    public TextMeshProUGUI endGameEnemiesKilledText;
    public Button endGameMainMenuButton;
    public GameObject moneyPopupPrefab;

    private GameManager gameManager;
    private TowerPlacementManager towerPlacementManager;
    private StatsPanel statsPanel;
    private int selectedTowerIndex = -1;
    private List<GameObject> moneyPopups = new List<GameObject>();

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        towerPlacementManager = FindObjectOfType<TowerPlacementManager>();
        statsPanel = FindObjectOfType<StatsPanel>();

        // Hide the end game panel and not enough money text initially
        endGamePanel.SetActive(false);
        notEnoughMoneyText.gameObject.SetActive(false);

        // Assign onClick listeners
        for (int i = 0; i < towerButtons.Length; i++)
        {
            int index = i;
            towerButtons[i].onClick.AddListener(() => OnTowerButtonClicked(index));
            AddEventTrigger(towerButtons[i].gameObject, EventTriggerType.PointerEnter, () => OnTowerButtonHover(index));
            AddEventTrigger(towerButtons[i].gameObject, EventTriggerType.PointerExit, OnTowerButtonExit);
        }

        skipButton.onClick.AddListener(OnSkipButtonPressed);
        endGameMainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2)) // Middle mouse button pressed
        {
            DeselectTower();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        waveText.text = $"Wave: {gameManager.CurrentWave}";
        phaseText.text = $"Phase: {gameManager.CurrentPhase}";
        moneyText.text = $"Money: {gameManager.PlayerMoney}";
        timeText.text = $"Time: {Mathf.CeilToInt(gameManager.TimeRemaining)}s";
        healthText.text = $"Health: {gameManager.PlayerHealth}";

        // Update tower buttons
        for (int i = 0; i < towerButtons.Length; i++)
        {
            Tower tower = towerPlacementManager.towerPrefabs[i].GetComponent<Tower>();
            towerNames[i].text = tower.towerName;
            towerPrices[i].text = $"${tower.cost}";
            if (i < towerSprites.Length)
            {
                towerImages[i].sprite = towerSprites[i];
            }
        }
    }

    public void ShowNotEnoughMoneyText()
    {
        notEnoughMoneyText.gameObject.SetActive(true);
        Invoke("HideNotEnoughMoneyText", 2f);
    }

    private void HideNotEnoughMoneyText()
    {
        notEnoughMoneyText.gameObject.SetActive(false);
    }

    public void OnSkipButtonPressed()
    {
        gameManager.SkipPhase();
        Debug.Log("Skip Button Pressed");
    }

    public void OnTowerButtonClicked(int index)
    {
        Tower tower = towerPlacementManager.towerPrefabs[index].GetComponent<Tower>();
        if (gameManager.PlayerMoney < tower.cost)
        {
            ShowNotEnoughMoneyText();
            Debug.Log("Not enough money to place this tower!");
            return;
        }

        selectedTowerIndex = index;
        statsPanel.ShowStats(index);
        towerPlacementManager.StartPlacingTower(index);
        Debug.Log($"Tower Button {index + 1} Pressed");
    }

    public void OnTowerButtonHover(int index)
    {
        statsPanel.ShowStats(index);
    }

    public void OnTowerButtonExit()
    {
        if (selectedTowerIndex == -1)
        {
            statsPanel.HideStats();
        }
    }

    public void DeselectTower()
    {
        selectedTowerIndex = -1;
        statsPanel.HideStats();
        towerPlacementManager.CancelPlacingTower();
        Debug.Log("Tower Deselected");
    }

    public void ShowWaveComplete()
    {
        StartCoroutine(WaveCompleteCoroutine());
    }

    private IEnumerator WaveCompleteCoroutine()
    {
        waveText.color = Color.green;
        yield return new WaitForSeconds(1f); // Wait for 1 second
        waveText.color = Color.white;
    }

    public void ShowEndGamePanel(int wavesSurvived, int enemiesKilled)
    {
        Time.timeScale = 0f;
        endGamePanel.SetActive(true);
        endGameWaveText.text = $"Waves Survived: {wavesSurvived}";
        endGameEnemiesKilledText.text = $"Enemies Killed: {enemiesKilled}";
    }

    private void OnMainMenuButtonPressed()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ShowMoneyPopup(int amount)
    {
        Vector3 moneyTextPosition = moneyText.transform.position;
        Vector3 spawnPosition = new Vector3(moneyTextPosition.x, moneyTextPosition.y - (moneyPopups.Count * 20) - 20, moneyTextPosition.z); // Adjust Y-offset to make them closer
        GameObject popup = Instantiate(moneyPopupPrefab, spawnPosition, Quaternion.identity, moneyText.transform.parent);
        popup.transform.localScale = new Vector3(1, 25, 1);
        popup.GetComponent<TextMeshProUGUI>().text = $"+{amount}";
        moneyPopups.Add(popup);
        StartCoroutine(FadeAndDestroyPopup(popup));
    }

    private IEnumerator FadeAndDestroyPopup(GameObject popup)
    {
        TextMeshProUGUI text = popup.GetComponent<TextMeshProUGUI>();
        Color originalColor = text.color;
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(originalColor, Color.clear, elapsedTime / duration);
            yield return null;
        }

        moneyPopups.Remove(popup);
        Destroy(popup);
    }

    private void AddEventTrigger(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }
}
