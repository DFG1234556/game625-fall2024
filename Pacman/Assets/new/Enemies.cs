using UnityEngine;
using UnityEngine.AI; // Import NavMesh components
using UnityEngine.UI; // Import for UI
using System.Collections;

public class Enemies : MonoBehaviour
{
    private NavMeshAgent agent; // Reference to the NavMeshAgent component
    private Transform player;   // Reference to the player character
    private Vector3 wanderTarget; // Random point for wandering

    [SerializeField] private float chaseDistance = 4f; // Distance to start chasing the player
    [SerializeField] private float stopChaseDistance = 5f; // Distance to stop chasing the player
    [SerializeField] private float chaseSpeed = 4.1f; // Speed while chasing the player
    [SerializeField] private float wanderSpeed = 1.5f; // Speed while wandering
    [SerializeField] private float minWanderDistance = 2f; // Minimum distance for wandering
    [SerializeField] private float maxWanderDistance = 4f; // Maximum distance for wandering

    [SerializeField] private Image sourceExclamationMark; // Source exclamation mark UI element (World Space Canvas)
    private Image exclamationMarkInstance; // Instance of the exclamation mark for this enemy
    [SerializeField] private Canvas canvas; // Reference to the World Space Canvas where the exclamation mark is located

    private bool isExclamationShown = false; // Track if the exclamation mark is visible
    private bool hasBlinked = false; // Track if the exclamation mark has blinked twice

    private bool isFrozen = false; // Track if the enemy is frozen
    private bool isChasing = false; // Track if the enemy is currently chasing the player

    private float exclamationZOffset = 1.0f; // Adjusted Z offset for orthographic camera

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag

        // Instantiate a new exclamation mark from the source
        exclamationMarkInstance = Instantiate(sourceExclamationMark, canvas.transform);
        exclamationMarkInstance.gameObject.SetActive(false); // Hide the exclamation mark at the start

        SetRandomWanderTarget(); // Set the initial random wander target
    }

    private void Update()
    {
        if (isFrozen) return; // If frozen, do nothing

        float distanceToPlayer = Vector3.Distance(transform.position, player.position); // Calculate distance to the player

        if (distanceToPlayer < chaseDistance && !isChasing) // Start chasing when within the chase distance
        {
            StartChasingPlayer();
        }
        else if (distanceToPlayer > stopChaseDistance && isChasing) // Stop chasing when outside stop chase distance
        {
            StopChasingPlayer();
        }

        // If the enemy is chasing the player
        if (isChasing)
        {
            agent.SetDestination(player.position); // Set the player's position as the destination
        }
        else
        {
            // If the enemy has reached the wander target, choose a new random target
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SetRandomWanderTarget(); // Set a new random wander target
            }

            agent.SetDestination(wanderTarget); // Set the wander target as the destination
        }

        // Update the exclamation mark's position above the enemy
        UpdateExclamationMarkPosition();
    }

    // Function to start chasing the player
    private void StartChasingPlayer()
    {
        isChasing = true;
        agent.speed = chaseSpeed; // Set speed to chase speed

        if (!hasBlinked)
        {
            StartCoroutine(BlinkExclamationMark()); // Start blinking if not already done
        }
        else
        {
            exclamationMarkInstance.gameObject.SetActive(true); // Show the exclamation mark while chasing
        }
    }

    // Function to stop chasing the player and resume wandering
    private void StopChasingPlayer()
    {
        isChasing = false;
        agent.speed = wanderSpeed; // Set speed to wander speed
        exclamationMarkInstance.gameObject.SetActive(false); // Hide the exclamation mark
        SetRandomWanderTarget(); // Pick a new random wander target
    }

    // Function to freeze the enemy
    public void FreezeMovement()
    {
        isFrozen = true;
        agent.isStopped = true; // Stop the NavMeshAgent
    }

    // Function to unfreeze the enemy
    public void UnfreezeMovement()
    {
        isFrozen = false;
        agent.isStopped = false; // Resume the NavMeshAgent
    }

    // Function to set a new random wander target within 2-4 units
    private void SetRandomWanderTarget()
    {
        float wanderDistance = Random.Range(minWanderDistance, maxWanderDistance); // Pick a random distance between 2 and 4 units
        Vector3 randomDirection = Random.insideUnitSphere * wanderDistance; // Generate a random direction

        randomDirection += transform.position; // Offset by the enemy's current position

        NavMeshHit navHit;
        // Find a valid point on the NavMesh within the random direction
        if (NavMesh.SamplePosition(randomDirection, out navHit, wanderDistance, NavMesh.AllAreas))
        {
            wanderTarget = navHit.position; // Set the wander target to the found valid NavMesh position
        }
    }

    // Coroutine to blink the exclamation mark twice with a speed of 0.15f
    private IEnumerator BlinkExclamationMark()
    {
        hasBlinked = true; // Mark that blinking has occurred
        exclamationMarkInstance.gameObject.SetActive(true); // Show the exclamation mark

        for (int i = 0; i < 2; i++)
        {
            exclamationMarkInstance.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.15f); // Blink speed of 0.15 seconds
            exclamationMarkInstance.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.15f); // Blink speed of 0.15 seconds
        }

        exclamationMarkInstance.gameObject.SetActive(true); // Show permanently after blinking
    }

    // Function to update the position of the exclamation mark in world space (using Z offset)
    private void UpdateExclamationMarkPosition()
    {
        if (exclamationMarkInstance.gameObject.activeSelf) // If the exclamation mark is active
        {
            // Directly set the position of the exclamation mark slightly above the enemy in world space along the Z-axis
            exclamationMarkInstance.rectTransform.position = transform.position + Vector3.forward * exclamationZOffset; // Z offset for orthographic view
        }
    }

    // When the enemy is destroyed, destroy the exclamation mark instance as well
    private void OnDestroy()
    {
        if (exclamationMarkInstance != null)
        {
            Destroy(exclamationMarkInstance.gameObject); // Destroy the exclamation mark when the enemy is destroyed
        }
    }
}
