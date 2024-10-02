using UnityEngine;

public class InventoryMenuToggler : MonoBehaviour
{
    public GameObject inventoryMenu;  // The GameObject representing the inventory menu

    void Start()
    {
        // Initially hide the inventory menu when the game starts
        if (inventoryMenu != null)
        {
            inventoryMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("No GameObject assigned for the inventory menu.");
        }
    }

    // This method toggles the visibility of the inventory menu
    public void ToggleInventoryMenu()
    {
        if (inventoryMenu != null)
        {
            // Toggle the active state (show/hide)
            bool isActive = inventoryMenu.activeSelf;
            inventoryMenu.SetActive(!isActive);
        }
        else
        {
            Debug.LogError("No GameObject assigned for the inventory menu.");
        }
    }
}