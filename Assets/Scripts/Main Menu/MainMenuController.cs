using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject howToPlayPanel;
    public int numberOfMaps;

    void Start()
    {
        Button startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(OnStartButtonClick);

        Button howToPlayButton = GameObject.Find("HowToPlayButton").GetComponent<Button>();
        howToPlayButton.onClick.AddListener(OnHowToPlayButtonClick);

        Button closeButton = GameObject.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseButtonClick);

        howToPlayPanel.SetActive(false);
    }

    void OnStartButtonClick()
    {
        int randomMapIndex = Random.Range(0, numberOfMaps);
        PlayerPrefs.SetInt("SelectedMapIndex", randomMapIndex);
        SceneManager.LoadScene("MainScene");
    }

    void OnHowToPlayButtonClick()
    {
        howToPlayPanel.SetActive(true);
    }

    void OnCloseButtonClick()
    {
        howToPlayPanel.SetActive(false);
    }

    void OnExitButtonClick()
    {
        Application.Quit();
    }
}
