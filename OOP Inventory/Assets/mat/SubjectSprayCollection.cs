using System;
using UnityEngine;

public class SubjectSprayCollection : MonoBehaviour
{
    // Event for notifying observers when a spray is collected
    public event Action<string> OnSprayCollected;

    // Call this method when a spray is collected
    public void CollectSpray(string sprayName)
    {
        Debug.Log("Spray collected: " + sprayName);

        // Notify all observers that the spray was collected
        OnSprayCollected?.Invoke(sprayName);
    }

    void Update()
    {
        // Detect mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Perform a raycast from the camera to the clicked point in the world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits a spray object
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object has a Spray tag
                if (hit.collider.CompareTag("Spray"))
                {
                    // Get the spray type from the object (assume sprayName is stored in the object’s script)
                    SprayObject sprayObject = hit.collider.GetComponent<SprayObject>();

                    if (sprayObject != null)
                    {
                        // Notify the observers that this spray was collected
                        CollectSpray(sprayObject.sprayName);

                        // Destroy the spray game object after it's clicked/collected
                        Destroy(hit.collider.gameObject);

                        Debug.Log(sprayObject.sprayName + " has been destroyed after being collected.");
                    }
                }
            }
        }
    }
}