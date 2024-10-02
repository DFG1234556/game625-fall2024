using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubjectInventory : MonoBehaviour, IPointerClickHandler
{
    // Event for notifying observers when the inventory should be decreased
    public event Action<string> OnInventoryDecrease;

    // This method is called when the GameObject is clicked
    public void OnPointerClick(PointerEventData eventData)
    {

        

        // Assuming sprayType is defined and passed in properly (e.g., "Insecticide", "Fungicide")
        string sprayType = GetSprayTypeFromObject();
        Debug.Log($"Clicked on {sprayType} area");

        // Notify observers (SprayManager) to decrease the inventory
        if (!string.IsNullOrEmpty(sprayType))
        {
            Debug.Log($"Click detected on {sprayType} GameObject, notifying SprayManager to decrease inventory.");
            OnInventoryDecrease?.Invoke(sprayType);  // Notify observers
        }
    }

    // Helper method to identify which spray type the object represents (this can be set in the inspector)
    private string GetSprayTypeFromObject()
    {
        // This could be set via the Inspector or other logic to determine the type
        // For simplicity, let's assume it's based on the GameObject's name
        return gameObject.name;  // Example: "Insecticide", "Fungicide", etc.
    }
}