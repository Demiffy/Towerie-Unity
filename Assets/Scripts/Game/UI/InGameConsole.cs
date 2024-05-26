using UnityEngine;
using TMPro;

public class InGameConsole : MonoBehaviour
{
    public GameObject consoleUI;
    public TMP_InputField consoleInputField;
    public TextMeshProUGUI consoleOutputText;
    public MapManager mapManager;
    public GameManager gameManager;
    public KeyCode toggleKey = KeyCode.BackQuote;

    private bool isConsoleVisible = false;

    private void Start()
    {
        if (mapManager == null)
        {
            mapManager = FindObjectOfType<MapManager>();
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleConsole();
        }

        if (isConsoleVisible && Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteCommand(consoleInputField.text);
            consoleInputField.text = "";
            consoleInputField.ActivateInputField();
        }
    }

    public void ToggleConsole()
    {
        isConsoleVisible = !isConsoleVisible;
        consoleUI.SetActive(isConsoleVisible);

        if (isConsoleVisible)
        {
            consoleInputField.ActivateInputField();
        }
    }

    private void ExecuteCommand(string command)
    {
        if (mapManager == null)
        {
            AddToConsoleOutput("MapManager not found.");
            return;
        }

        if (gameManager == null)
        {
            AddToConsoleOutput("GameManager not found.");
            return;
        }

        if (command.StartsWith("loadmap "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length == 2 && int.TryParse(parts[1], out int mapIndex))
            {
                int totalMaps = mapManager.GetTotalMaps();
                if (mapIndex >= 0 && mapIndex < totalMaps)
                {
                    mapManager.LoadMap(mapIndex);
                    AddToConsoleOutput($"Loading map with index {mapIndex}");
                }
                else
                {
                    AddToConsoleOutput($"Invalid map index. Valid indices are 0 to {totalMaps - 1}");
                }
            }
            else
            {
                AddToConsoleOutput("Invalid command format. Use: loadmap <index>");
            }
        }
        else if (command.StartsWith("sethealth "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length == 2 && int.TryParse(parts[1], out int health))
            {
                gameManager.PlayerHealth = health;
                FindObjectOfType<GameUIManager>().UpdateUI();
                AddToConsoleOutput($"Player health set to {health}");
            }
            else
            {
                AddToConsoleOutput("Invalid command format. Use: sethealth <value>");
            }
        }
        else if (command.StartsWith("setmoney "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length == 2 && int.TryParse(parts[1], out int money))
            {
                gameManager.PlayerMoney = money;
                FindObjectOfType<GameUIManager>().UpdateUI();
                AddToConsoleOutput($"Player money set to {money}");
            }
            else
            {
                AddToConsoleOutput("Invalid command format. Use: setmoney <value>");
            }
        }
        else if (command.StartsWith("setwave "))
        {
            string[] parts = command.Split(' ');
            if (parts.Length == 2 && int.TryParse(parts[1], out int wave))
            {
                gameManager.CurrentWave = wave;
                FindObjectOfType<GameUIManager>().UpdateUI();
                AddToConsoleOutput($"Current wave set to {wave}");
            }
            else
            {
                AddToConsoleOutput("Invalid command format. Use: setwave <value>");
            }
        }
        else if (command.Equals("clear", System.StringComparison.OrdinalIgnoreCase))
        {
            ClearConsole();
        }
        else if (command.Equals("help", System.StringComparison.OrdinalIgnoreCase))
        {
            ShowHelp();
        }
        else
        {
            AddToConsoleOutput($"Unknown command: {command}");
        }
    }

    public void AddToConsoleOutput(string message)
    {
        consoleOutputText.text += message + "\n";
    }

    private void ClearConsole()
    {
        consoleOutputText.text = "";
    }

    private void ShowHelp()
    {
        string helpMessage = "Available commands:\n" +
                             "loadmap <index> - Load a map with the specified index.\n" +
                             "sethealth <value> - Set the player's health.\n" +
                             "setmoney <value> - Set the player's money.\n" +
                             "setwave <value> - Set the current wave.\n" +
                             "clear - Clear the console output.\n" +
                             "help - Show this help message.";
        AddToConsoleOutput(helpMessage);
    }
}
