//using UnityEngine;

//namespace DesignPatterns.Observer
//{
//    public class ObserverBug : MonoBehaviour
//    {
//        // Reference to the SubjectPlant
//        [SerializeField] private SubjectPlant subjectPlant;
//        [SerializeField] private GameObject flwbug1;

//        // The speed at which the ObserverBug will move towards the plant
//        public float moveSpeed = 5f;

//        // The distance at which the ObserverBug will stop moving toward the plant
//        public float stopDistance = 0.5f;



//        private void Awake()
//        {
//            if (flwbug1 != null)
//            {
//                flwbug1.SetActive(false);
//            }
//            else
//            {
//                Debug.LogWarning("flwbug1 is not assigned in the Inspector");
//            }
//        }

//        private void OnEnable()
//        {
//            // Subscribe to the SubjectPlant's event
//            if (subjectPlant != null)
//            {
//                subjectPlant.OnNumberFiveGenerated += OnNumberFiveGenerated;
//            }
//        }
using System;
using UnityEngine;

namespace DesignPatterns.Observer
{
    public class ObserverBug : MonoBehaviour
    {
        public GameObject bugPrefab; // Reference to the bug prefab
        private GameObject bugInstance; // Instance of the spawned bug
        public Transform plantTransform; // Reference to the SubjectPlant's transform
        public float moveSpeed = 0.4f; // Speed at which the bug moves
        public float spawnDistance = 10f; // Distance from the plant where the bug spawns

        private void OnEnable()
        {
            // Find the SubjectPlant component and subscribe to the event
            SubjectPlant subjectPlant = FindObjectOfType<SubjectPlant>();
            if (subjectPlant != null)
            {
                subjectPlant.OnNumberFiveGenerated += OnNumberFiveGenerated;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from the event when the script is disabled
            SubjectPlant subjectPlant = FindObjectOfType<SubjectPlant>();
            if (subjectPlant != null)
            {
                subjectPlant.OnNumberFiveGenerated -= OnNumberFiveGenerated;
            }
        }

        // Method called when number 5 is generated in the SubjectPlant
        private void OnNumberFiveGenerated()
        {
            if (bugInstance == null)
            {
                // Generate a random position that is 10f away from the plant
                Vector3 spawnPosition = GenerateRandomPositionAroundPlant();

                // Instantiate the bug at the random position
                bugInstance = Instantiate(bugPrefab, spawnPosition, Quaternion.identity);
            }

            // Start moving the bug towards the plant
            StartCoroutine(MoveBugTowardsPlant());
        }

        // Method to generate a random position 10f away from the plant
        private Vector3 GenerateRandomPositionAroundPlant()
        {
            // Generate a random angle in radians
            float randomAngle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);

            // Calculate the spawn position based on the random angle and distance from the plant
            float x = plantTransform.position.x + Mathf.Cos(randomAngle) * spawnDistance;
            float y = plantTransform.position.y + Mathf.Sin(randomAngle) * spawnDistance;

            return new Vector3(x, y, plantTransform.position.z); // Same Z as the plant to keep it on the same plane
        }

        // Coroutine to move the bug towards the plant
        private System.Collections.IEnumerator MoveBugTowardsPlant()
        {
            while (bugInstance != null && Vector3.Distance(bugInstance.transform.position, plantTransform.position) > 0.1f)
            {
                // Move the bug towards the plant
                bugInstance.transform.position = Vector3.MoveTowards(bugInstance.transform.position, plantTransform.position, moveSpeed * Time.deltaTime);

                // Wait for the next frame
                yield return null;
            }
        }
    }
}

//        private void OnDisable()
//        {
//            // Unsubscribe to avoid memory leaks
//            if (subjectPlant != null)
//            {
//                subjectPlant.OnNumberFiveGenerated -= OnNumberFiveGenerated;
//            }
//        }

//        private void Update()
//        {
//            if (isActive)
//            {
//                MoveTowardsPlant();
//            }
//        }

//        // Event handler: This method is called when SubjectPlant generates the number 5
//        private void OnNumberFiveGenerated()
//        {
//            // Show the ObserverBug
//            gameObject.SetActive(true);
//            isActive = true;
//        }

//        // Method to hide the ObserverBug (can be called when the number isn't 5)
//        public void HideBug()
//        {
//            gameObject.SetActive(false);
//            isActive = false;
//        }

//        // Move the ObserverBug towards the SubjectPlant
//        private void MoveTowardsPlant()
//        {
//            // Get the direction to the plant
//            Vector3 directionToPlant = (subjectPlant.transform.position - transform.position).normalized;

//            // Check the distance to the plant
//            float distanceToPlant = Vector3.Distance(transform.position, subjectPlant.transform.position);

//            // If the bug is further than the stop distance, move towards the plant
//            if (distanceToPlant > stopDistance)
//            {
//                transform.position += directionToPlant * moveSpeed * Time.deltaTime;
//            }
//            else
//            {
//                // Stop moving once within stop distance
//                isActive = false;
//            }
//        }
//    }
//}