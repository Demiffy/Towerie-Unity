using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

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
    public GameObject statsPanel;
    public TextMeshProUGUI statsText;

    public GameObject endGamePanel; // Assign this in the inspector
    public TextMeshProUGUI endGameWaveText; // Assign this in the inspector
    public TextMeshProUGUI endGameEnemiesKilledText; // Assign this in the inspector
    public Button endGameMainMenuButton; // Assign this in the inspector

    private GameManager gameManager;
    private int selectedTowerIndex = -1;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Hide the stats panel and end game panel initially
        statsPanel.SetActive(false);
        endGamePanel.SetActive(false);

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
            // Tower data will come from a script class in the future
            towerNames[i].text = $"Tower {i + 1}";
            towerPrices[i].text = $"$100";
            // Assign images to tower buttons
            if (i < towerSprites.Length)
            {
                towerImages[i].sprite = towerSprites[i];
            }
        }
    }

    public void OnSkipButtonPressed()
    {
        gameManager.SkipPhase();
        Debug.Log("Skip Button Pressed");
    }

    public void OnTowerButtonClicked(int index)
    {
        // Handle tower button press
        selectedTowerIndex = index;
        ShowTowerStats(index);
        Debug.Log($"Tower Button {index + 1} Pressed");
    }

    public void OnTowerButtonHover(int index)
    {
        // Show tower stats on hover only if no tower is selected
        if (selectedTowerIndex == -1)
        {
            ShowTowerStats(index);
        }
    }

    public void OnTowerButtonExit()
    {
        // Hide tower stats if no tower is selected
        if (selectedTowerIndex == -1)
        {
            statsPanel.SetActive(false);
        }
    }

    private void ShowTowerStats(int index)
    {
        statsPanel.SetActive(true);
        statsText.text = $"Tower {index + 1} Stats:\nDamage: 10\nRange: 5\nRate: 1.0s";
    }

    private void DeselectTower()
    {
        selectedTowerIndex = -1;
        statsPanel.SetActive(false);
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
        // Pause the game
        Time.timeScale = 0f;

        // Display the end game panel
        endGamePanel.SetActive(true);
        endGameWaveText.text = $"Waves Survived: {wavesSurvived}";
        endGameEnemiesKilledText.text = $"Enemies Killed: {enemiesKilled}";
    }

    private void OnMainMenuButtonPressed()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    // Helper method to add EventTrigger to a GameObject
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
