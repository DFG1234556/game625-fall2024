using UnityEngine;
using TMPro;

public class DayRecorder : MonoBehaviour
{
    public TMP_Text uiText;  // Reference to the TMP UI Text element
    private int dayCounter = 0;  // Renamed from counter to dayCounter

    void Update()
    {
        // Check if the 'R' key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            IncrementCounter();
        }
    }

    // Increments the dayCounter and updates the UI with the format "Day X"
    void IncrementCounter()
    {
        dayCounter++;
        uiText.text = "Day " + dayCounter.ToString();
    }
}