using System.Collections;
using System.Collections.Generic; // Import for Dictionary
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class Character : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField] private float speed = 3.5f; // Normal constant speed set to 3.5f
    [SerializeField] private float boostSpeed = 8f; // The boosted speed
    [SerializeField] private float boostDuration = 2f; // Boost duration set to 2 seconds
    [SerializeField] private float predatorModeDuration = 3.5f; // Predator mode duration set to 5 seconds
    [SerializeField] private float freezeDuration = 2f; // Duration of freezing enemies

    private float originalSpeed; // Store the original speed to reset after boost
    private float boostTimer = 0f; // Timer to track the boost duration
    private bool isBoosted = false; // Track whether the character is boosted

    // Health system variables
    [SerializeField] private int maxHealth = 10; // Player's max health
    private int currentHealth; // Player's current health

    // TextMeshProUGUI variables
    [SerializeField] private TextMeshProUGUI endGameMessage; // End game message
    [SerializeField] private TextMeshProUGUI countdownText;  // Countdown timer
    [SerializeField] private TextMeshProUGUI pointsText;     // Points display during the game
    [SerializeField] private TextMeshProUGUI finalScoreText; // Final score display when game ends
    [SerializeField] private TextMeshProUGUI preyModeText;   // Display for Prey Mode
    [SerializeField] private TextMeshProUGUI predatorModeText; // Display for Predator Mode
    [SerializeField] private TextMeshProUGUI healthText;     // Health display

    // Countdown variables
    [SerializeField] private float gameDuration = 40f; // Game duration set to 10 seconds
    private float countdownTimer; // Countdown timer
    private bool isGameOver = false; // Track if the game is over

    // Points system
    private int points = 0; // Initialize points to 0

    // Mode system
    private bool isPredatorMode = false; // Is the player in Predator mode?
    private float predatorModeTimer = 0f; // Timer to track the duration of Predator mode

    // Cooldown system for taking damage from enemies
    private Dictionary<GameObject, float> enemyDamageTimers = new Dictionary<GameObject, float>(); // Tracks last damage time per enemy
    private float damageCooldown = 1f; // Time before the player can take damage from the same enemy again

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalSpeed = speed; // Store the original speed
        currentHealth = maxHealth; // Initialize health to maxHealth at the start
        endGameMessage.gameObject.SetActive(false); // Hide the end message at the start
        finalScoreText.gameObject.SetActive(false); // Hide the final score at the start
        countdownTimer = gameDuration; // Set the countdown timer to the game duration
        UpdateCountdownText(); // Initialize countdown text
        UpdatePointsText(); // Initialize points text
        UpdateHealthText(); // Initialize health text
        UpdateModeText(); // Initialize mode text
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
            return; // Stop all input and movement if the game is over

        HandleBoost();
        PlayerMovement(); // Call the movement function
        HandleCountdown(); // Handle the countdown
        HandlePredatorMode(); // Handle predator mode timer

        // Check if player health is 0 and handle player death (only in Prey Mode)
        if (currentHealth <= 0 && !isPredatorMode)
        {
            LoseGame("You Lose"); // Player loses due to health reaching 0
        }
    }

    // Function to handle player movement
    void PlayerMovement()
    {
        // Get player input
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Normalize movement to prevent faster diagonal speed
        move = move.normalized;

        // Move the character based on input
        characterController.Move(move * Time.deltaTime * speed);
    }

    // Function to handle the boost timer and reset the speed
    void HandleBoost()
    {
        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0)
            {
                speed = originalSpeed; // Reset speed to original
                isBoosted = false; // End the boost
            }
        }
    }

    // Function to handle the countdown timer
    void HandleCountdown()
    {
        if (countdownTimer > 0)
        {
            countdownTimer -= Time.deltaTime; // Reduce the timer
            UpdateCountdownText(); // Update the displayed timer
        }
        else if (countdownTimer <= 0 && !isGameOver)
        {
            LoseGame("You Lose"); // Player loses if the timer runs out
        }
    }

    // Function to handle Predator mode timer and switching back to Prey mode
    void HandlePredatorMode()
    {
        if (isPredatorMode)
        {
            predatorModeTimer -= Time.deltaTime;
            if (predatorModeTimer <= 0)
            {
                ExitPredatorMode(); // Exit Predator mode when the timer runs out
            }
        }
    }

    // Update the countdown timer text
    void UpdateCountdownText()
    {
        countdownText.text = "Time Left: " + Mathf.Ceil(countdownTimer).ToString(); // Display remaining time
    }

    // Update the points text
    void UpdatePointsText()
    {
        pointsText.text = "Points: " + points.ToString(); // Display the player's points
    }

    // Update the health text
    void UpdateHealthText()
    {
        healthText.text = "Health: " + currentHealth.ToString(); // Display the player's health
    }

    // Update the mode text (show either Prey Mode or Predator Mode)
    void UpdateModeText()
    {
        if (isPredatorMode)
        {
            predatorModeText.gameObject.SetActive(true);  // Show Predator Mode text
            preyModeText.gameObject.SetActive(false);     // Hide Prey Mode text
        }
        else
        {
            preyModeText.gameObject.SetActive(true);      // Show Prey Mode text
            predatorModeText.gameObject.SetActive(false); // Hide Predator Mode text
        }
    }

    // OnTriggerEnter is called when the player character collides with a trigger collider
    void OnTriggerEnter(Collider other)
    {
        if (isGameOver)
            return; // Prevent further actions when the game is over

        if (other.CompareTag("Boost"))
        {
            ActivateBoost();
            Destroy(other.gameObject); // Destroy the boost item on collision
        }
        else if (other.CompareTag("Enemy"))
        {
            if (isPredatorMode)
            {
                ConsumeEnemy(other); // Consume enemy and gain points in Predator mode
            }
            else
            {
                TakeDamageFromEnemy(other.gameObject); // Take damage in Prey mode (with cooldown)
            }
        }
        else if (other.CompareTag("Win"))
        {
            WinGame(); // Player wins when colliding with a "win" item
        }
        else if (other.CompareTag("Freeze"))
        {
            StartCoroutine(FreezeEnemies()); // Freeze enemies for 2 seconds
            Destroy(other.gameObject); // Destroy the freeze item after use
        }
        else if (other.CompareTag("ModeChange"))
        {
            EnterPredatorMode(); // Switch to Predator mode when colliding with a mode change item
            Destroy(other.gameObject); // Destroy the mode change item after collision
        }
    }

    // Function to activate the speed boost
    void ActivateBoost()
    {
        isBoosted = true;
        speed = boostSpeed; // Set the boosted speed
        boostTimer = boostDuration; // Start the boost timer
    }

    // Function to freeze enemy movement for 2 seconds
    IEnumerator FreezeEnemies()
    {
        Enemies[] enemies = FindObjectsOfType<Enemies>(); // Find all enemies in the scene
        foreach (Enemies enemy in enemies)
        {
            enemy.FreezeMovement(); // Freeze each enemy's movement
        }

        yield return new WaitForSeconds(freezeDuration); // Wait for freezeDuration

        foreach (Enemies enemy in enemies)
        {
            enemy.UnfreezeMovement(); // Unfreeze each enemy's movement
        }
    }

    // Function to consume an Enemy and gain points in Predator mode
    void ConsumeEnemy(Collider enemy)
    {
        points += 1; // Add 1 point for consuming an enemy
        UpdatePointsText(); // Update the points display
        Destroy(enemy.gameObject); // Destroy the enemy
    }

    // Function to handle taking damage with a cooldown system
    void TakeDamageFromEnemy(GameObject enemy)
    {
        // Check if the enemy is in the damage cooldown dictionary
        if (enemyDamageTimers.ContainsKey(enemy))
        {
            // If enough time has passed, allow damage again
            if (Time.time - enemyDamageTimers[enemy] >= damageCooldown)
            {
                TakeDamage(1); // Apply damage
                enemyDamageTimers[enemy] = Time.time; // Update the last damage time
            }
        }
        else
        {
            // First-time damage from this enemy
            TakeDamage(1); // Apply damage
            enemyDamageTimers[enemy] = Time.time; // Add enemy to the dictionary with current time
        }
    }

    // Function to decrease player's health when attacked by an Enemy
    void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce health by the damage value
        UpdateHealthText(); // Update the health display
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            LoseGame("You Lose"); // Call the LoseGame function if health drops to or below 0
        }
    }

    // Function to handle player death and losing the game
    void LoseGame(string message)
    {
        isGameOver = true;
        Debug.Log(message);
        characterController.enabled = false; // Disable player movement
        endGameMessage.gameObject.SetActive(true); // Show the game-over message
        endGameMessage.text = message; // Set the message to the passed text (You Lose)
        finalScoreText.gameObject.SetActive(true); // Show the final score message
        finalScoreText.text = "Your Score: " + points.ToString(); // Display the final score
        countdownText.gameObject.SetActive(false); // Hide the countdown when the game ends
        pointsText.gameObject.SetActive(false); // Hide the points text when the game ends
        preyModeText.gameObject.SetActive(false); // Hide Prey Mode text when the game ends
        predatorModeText.gameObject.SetActive(false); // Hide Predator Mode text when the game ends
    }

    // Function to handle player winning the game
    void WinGame()
    {
        isGameOver = true;
        Debug.Log("Player has won!");
        characterController.enabled = false; // Disable player movement
        endGameMessage.gameObject.SetActive(true); // Show the game-over message
        endGameMessage.text = "You Win"; // Set the message to "You Win"
        finalScoreText.gameObject.SetActive(true); // Show the final score message
        finalScoreText.text = "Your Score: " + points.ToString(); // Display the final score
        countdownText.gameObject.SetActive(false); // Hide the countdown when the game ends
        pointsText.gameObject.SetActive(false); // Hide the points text when the game ends
        preyModeText.gameObject.SetActive(false); // Hide Prey Mode text when the game ends
        predatorModeText.gameObject.SetActive(false); // Hide Predator Mode text when the game ends
    }

    // Function to enter Predator mode
    void EnterPredatorMode()
    {
        isPredatorMode = true; // Activate Predator mode
        predatorModeTimer = predatorModeDuration; // Set the Predator mode timer
        UpdateModeText(); // Update the mode display
    }

    // Function to exit Predator mode
    void ExitPredatorMode()
    {
        isPredatorMode = false; // Switch back to Prey mode
        UpdateModeText(); // Update the mode display
    }
}
