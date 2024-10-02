using System;
using UnityEngine;

namespace DesignPatterns.Observer
{
    public class SubjectPlant : MonoBehaviour
    {
        // Event to notify observers when the number 5 is generated
        public event Action OnNumberFiveGenerated;

        void Update()
        {
            // Check if the 'R' key is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                GenerateRandomNumber();
            }
        }

        // Method to generate a random number and trigger the event if the number is 5
        private void GenerateRandomNumber()
        {
            int randomNumber = UnityEngine.Random.Range(1, 11); // Random number between 1 and 10
            Debug.Log("Generated Number: " + randomNumber);

            // Trigger the event if the generated number equals 5
            if (randomNumber == 5)
            {
                OnNumberFiveGenerated?.Invoke();
            }
        }
    }
}