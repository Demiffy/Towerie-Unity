using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public int currentWave = 1;
    public float preparationTime = 10f;
    public float attackTime = 20f;
    public int playerMoney = 1000;
    public int playerHealth = 100;

    private string currentPhase = "Preparation";
    private float timeRemaining;
    private int selectedTowerIndex = -1;

    void Start()
    {
        timeRemaining = preparationTime;

        // Hide the stats panel initially
        statsPanel.SetActive(false);

        // Assign onClick listeners
        for (int i = 0; i < towerButtons.Length; i++)
        {
            int index = i;
            towerButtons[i].onClick.AddListener(() => OnTowerButtonClicked(index));
            AddEventTrigger(towerButtons[i].gameObject, EventTriggerType.PointerEnter, () => OnTowerButtonHover(index));
            AddEventTrigger(towerButtons[i].gameObject, EventTriggerType.PointerExit, OnTowerButtonExit);
        }

        skipButton.onClick.AddListener(OnSkipButtonPressed);

        UpdateUI();
    }

    void Update()
    {
        // Update the time remaining
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            // Handle phase transition
            if (currentPhase == "Preparation")
            {
                StartAttackPhase();
            }
            else if (currentPhase == "Attack")
            {
                StartPreparationPhase();
                currentWave++;
            }
        }

        if (Input.GetMouseButtonDown(2)) // Middle mouse button pressed
        {
            DeselectTower();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        waveText.text = $"Wave: {currentWave}";
        phaseText.text = $"Phase: {currentPhase}";
        moneyText.text = $"Money: {playerMoney}";
        timeText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s";
        healthText.text = $"Health: {playerHealth}";

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
        // Handle skip button press
        if (currentPhase == "Preparation")
        {
            StartAttackPhase();
        }
        else if (currentPhase == "Attack")
        {
            StartPreparationPhase();
            currentWave++;
        }

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

    private void StartPreparationPhase()
    {
        currentPhase = "Preparation";
        timeRemaining = preparationTime;
        FindObjectOfType<EnemySpawner>().enabled = false;
        Debug.Log("Preparation Phase Started");
    }


    private void StartAttackPhase()
    {
        currentPhase = "Attack";
        timeRemaining = attackTime;
        FindObjectOfType<EnemySpawner>().enabled = true;
        Debug.Log("Attack Phase Started");
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
