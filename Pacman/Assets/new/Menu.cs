using UnityEngine;
using UnityEngine.SceneManagement; // For scene management
using TMPro; // For TextMeshPro UI
using UnityEngine.UI; // For UI elements

public class Menu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI howToPlayText; // Reference to the "How to Play" text UI element
    [SerializeField] private Image howToPlayBackground; // Reference to the background image for "How to Play"
    [SerializeField] private Button startButton; // Reference to the Start button
    [SerializeField] private Button howToPlayButton; // Reference to the How to Play button
    [SerializeField] private Button quitButton; // Reference to the Quit button

    private bool isHowToPlayVisible = false; // Track whether the "How to Play" UI is visible

    private void Start()
    {
        // Hide the "How to Play" text and background at the start
        howToPlayText.gameObject.SetActive(false);
        howToPlayBackground.gameObject.SetActive(false);

        // Add listener to Start button to load the Game scene
        startButton.onClick.AddListener(StartGame);

        // Add listener to How to Play button to show/hide the How to Play text and background
        howToPlayButton.onClick.AddListener(ToggleHowToPlay);

        // Add listener to Quit button to quit the application
        quitButton.onClick.AddListener(QuitGame);
    }

    // Function to load the Game scene
    public void StartGame()
    {
        SceneManager.LoadScene("Game"); // Load the scene named "Game"
    }

    // Function to show/hide the "How to Play" UI element and background
    public void ToggleHowToPlay()
    {
        isHowToPlayVisible = !isHowToPlayVisible; // Toggle the visibility

        // Show or hide both the "How to Play" text and the background image
        howToPlayText.gameObject.SetActive(isHowToPlayVisible);
        howToPlayBackground.gameObject.SetActive(isHowToPlayVisible);
    }

    // Function to quit the application
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit(); // End the application (Note: This will only work in a build, not in the editor)
    }
}
