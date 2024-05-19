using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        // Find buttons by name and add listeners to their onClick events
        Button startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(OnStartButtonClick);

        Button howToPlayButton = GameObject.Find("HowToPlayButton").GetComponent<Button>();
        howToPlayButton.onClick.AddListener(OnHowToPlayButtonClick);
    }

    // Method to handle the start button click
    void OnStartButtonClick()
    {
        SceneManager.LoadScene("MainScene");
    }

    // Method to handle the how to play button click
    void OnHowToPlayButtonClick()
    {
        Debug.Log("How to Play button clicked");
    }
}
