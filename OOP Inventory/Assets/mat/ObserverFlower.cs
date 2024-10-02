using UnityEngine;

namespace DesignPatterns.Observer
{
    public class SubjectFlower : MonoBehaviour
    {
        // Reference to the SubjectPlant (the object generating the event)
        [SerializeField] private SubjectPlant subjectPlant;

        // Reference to the external object (objflw1) that will be activated/deactivated
        [SerializeField] private GameObject objflw1;

        private void Awake()
        {
            // Initially hide the objflw1 object
            if (objflw1 != null)
            {
                objflw1.SetActive(false);
            }
            else
            {
                Debug.LogWarning("objflw1 is not assigned in the Inspector");
            }
        }

        private void OnEnable()
        {
            // Subscribe to the OnNumberFiveGenerated event from the SubjectPlant
            if (subjectPlant != null)
            {
                subjectPlant.OnNumberFiveGenerated += ShowObjFlw1;
            }
            else
            {
                Debug.LogWarning("subjectPlant is not assigned in the Inspector");
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from the OnNumberFiveGenerated event when this object is disabled
            if (subjectPlant != null)
            {
                subjectPlant.OnNumberFiveGenerated -= ShowObjFlw1;
            }
        }

        // Method to show the objflw1 object when number 5 is generated
        private void ShowObjFlw1()
        {
            if (objflw1 != null)
            {
                objflw1.SetActive(true);
                Debug.Log("objflw1 is now visible.");
            }
        }

        // Optionally, you can add a method to hide the object if needed
        public void HideObjFlw1()
        {
            if (objflw1 != null)
            {
                objflw1.SetActive(false);
                Debug.Log("objflw1 is now hidden.");
            }
        }
    }
}