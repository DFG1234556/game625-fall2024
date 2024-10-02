using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SprayManager : MonoBehaviour
{
    public SoundManager soundManager;  // Reference to the SoundManager
    public SubjectSprayCollection subjectSprayCollection;  // Reference to SubjectSprayCollection for spray collection
    public List<SubjectInventory> subjectInventories;  // List of SubjectInventory references for each clickable area

    // Event to notify listeners (like SoundManager) when a spray is picked up
    public event Action OnSprayPickedUp;

    // TMP_Text UI elements for each spray type
    [SerializeField] private TMP_Text insecticideUI;
    [SerializeField] private TMP_Text fungicideUI;
    [SerializeField] private TMP_Text herbicideUI;
    [SerializeField] private TMP_Text bactericideUI;

    // Encapsulated base class for sprays
    private abstract class Spray
    {
        private string sprayName;
        private int amount;
        private TMP_Text uiText;

        public Spray(string name, TMP_Text uiText)
        {
            this.sprayName = name;
            this.uiText = uiText;
            this.amount = 0;  // Default to 0 sprays on hand
            UpdateUI();  // Initialize the UI at start
        }

        public void AddSpray()
        {
            amount++;
            UpdateUI();
        }

        public void DecreaseSpray()
        {
            if (amount > 0)
            {
                amount--;
                UpdateUI();  // Update UI after decreasing the spray
                Debug.Log($"{sprayName} spray inventory decreased to {amount}.");
            }
            else
            {
                Debug.Log($"{sprayName} spray is out of stock.");
            }
        }

        public void UseSpray(GameObject target)
        {
            if (amount > 0)
            {
                ApplyEffect(target);
                amount--;
                UpdateUI();  // Update UI after using the spray
                Debug.Log($"{sprayName} spray used, remaining: {amount}");
            }
            else
            {
                Debug.Log($"{sprayName} is out of stock.");
            }
        }

        // Method to apply the effect (e.g., destroy target), overridden by specific spray types
        protected abstract void ApplyEffect(GameObject target);

        // Method to update the UI element associated with the spray
        private void UpdateUI()
        {
            if (uiText != null)
            {
                //uiText.text = $"{sprayName}: {amount}";  // Update the UI element with the current amount
                uiText.text = $"{amount}";
            }
        }

        public string GetSprayName() => sprayName;
    }

    private class InsecticideSpray : Spray
    {
        public InsecticideSpray(TMP_Text uiText) : base("Insecticide", uiText) { }

        protected override void ApplyEffect(GameObject target)
        {
            if (target.CompareTag("Insect"))
            {
                Destroy(target);
                Debug.Log("Insecticide spray used on an insect.");
            }
        }
    }

    private class FungicideSpray : Spray
    {
        public FungicideSpray(TMP_Text uiText) : base("Fungicide", uiText) { }

        protected override void ApplyEffect(GameObject target)
        {
            if (target.CompareTag("Fungus"))
            {
                Destroy(target);
                Debug.Log("Fungicide spray used on a fungus.");
            }
        }
    }

    private class HerbicideSpray : Spray
    {
        public HerbicideSpray(TMP_Text uiText) : base("Herbicide", uiText) { }

        protected override void ApplyEffect(GameObject target)
        {
            if (target.CompareTag("Weed"))
            {
                Destroy(target);
                Debug.Log("Herbicide spray used on a weed.");
            }
        }
    }

    private class BactericideSpray : Spray
    {
        public BactericideSpray(TMP_Text uiText) : base("Bactericide", uiText) { }

        protected override void ApplyEffect(GameObject target)
        {
            if (target.CompareTag("Bacteria"))
            {
                Destroy(target);
                Debug.Log("Bactericide spray used on bacteria.");
            }
        }
    }

    // Dictionary to store the spray types and their corresponding classes
    private Dictionary<string, Spray> sprayDictionary = new Dictionary<string, Spray>();

    // Initialize the system in Start()
    private void Start()
    {
        // Initialize the subject and inventory interactions
        Initialize(subjectSprayCollection, subjectInventories);

        // Setup the sprays and their UI elements
        SetupSprays(insecticideUI, fungicideUI, herbicideUI, bactericideUI);

        // Ensure SoundManager is subscribed to the event
        if (soundManager != null)
        {
            soundManager.Initialize(this);  // Pass the SprayManager to the SoundManager
        }
        else
        {
            Debug.LogError("SoundManager is not assigned in the SprayManager!");
        }
    }

    // Initialize subscriptions to SubjectSprayCollection and SubjectInventory
    private void Initialize(SubjectSprayCollection sprayCollection, List<SubjectInventory> inventories)
    {
        subjectSprayCollection = sprayCollection;
        subjectSprayCollection.OnSprayCollected += AddSprayToInventory;

        // Subscribe to the inventory decrease events for each clickable area
        foreach (SubjectInventory inventory in inventories)
        {
            inventory.OnInventoryDecrease += DecreaseSprayInventory;
        }
    }

    // Adds spray to inventory and raises an event
    private void AddSprayToInventory(string sprayName)
    {
        if (sprayDictionary.ContainsKey(sprayName))
        {
            sprayDictionary[sprayName].AddSpray();
            Debug.Log(sprayName + " inventory increased.");

            // Raise the event and log to confirm it's being triggered
            OnSprayPickedUp?.Invoke();
            Debug.Log("OnSprayPickedUp event triggered");
        }
    }

    // Decreases spray inventory when a corresponding area is clicked
    private void DecreaseSprayInventory(string sprayName)
    {
        if (sprayDictionary.ContainsKey(sprayName))
        {
            sprayDictionary[sprayName].DecreaseSpray();
        }
    }

    // Setup the different sprays and their UI elements
    private void SetupSprays(TMP_Text insecticideUI, TMP_Text fungicideUI, TMP_Text herbicideUI, TMP_Text bactericideUI)
    {
        sprayDictionary.Add("Insecticide", new InsecticideSpray(insecticideUI));
        sprayDictionary.Add("Fungicide", new FungicideSpray(fungicideUI));
        sprayDictionary.Add("Herbicide", new HerbicideSpray(herbicideUI));
        sprayDictionary.Add("Bactericide", new BactericideSpray(bactericideUI));
    }

    // Method to use a spray on a target and update the UI accordingly
    public void UseSpray(string sprayName, GameObject target)
    {
        if (sprayDictionary.ContainsKey(sprayName))
        {
            sprayDictionary[sprayName].UseSpray(target);  // Uses spray and updates UI automatically
        }
    }
}