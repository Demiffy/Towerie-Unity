using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject howToPlayPanel;

    void Start()
    {
        // Find buttons by name and add listeners to their onClick events
        Button startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(OnStartButtonClick);

        Button howToPlayButton = GameObject.Find("HowToPlayButton").GetComponent<Button>();
        howToPlayButton.onClick.AddListener(OnHowToPlayButtonClick);

        Button closeButton = GameObject.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseButtonClick);

        howToPlayPanel.SetActive(false);
    }

    // Method to handle the start button click
    void OnStartButtonClick()
    {
        SceneManager.LoadScene("MainScene");
    }

    // Method to handle the how to play button click
    void OnHowToPlayButtonClick()
    {
        howToPlayPanel.SetActive(true);
    }

    // Method to handle the close button click on the how to play panel
    void OnCloseButtonClick()
    {
        howToPlayPanel.SetActive(false);
    }
}
