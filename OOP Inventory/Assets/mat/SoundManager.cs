using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip sprayPickupSound;  // The sound clip to play when a spray is picked up
    private AudioSource audioSource;

    private void Awake()
    {
        // Get the AudioSource component attached to the SoundManager
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            Debug.Log("AudioSource component found.");
        }
        else
        {
            Debug.LogError("AudioSource component is missing on SoundManager!");
        }

        if (sprayPickupSound != null)
        {
            Debug.Log("Spray pickup sound clip assigned.");
        }
        else
        {
            Debug.LogError("Spray pickup sound clip is not assigned in SoundManager!");
        }
    }

    // This method will subscribe to SprayManager's event
    public void Initialize(SprayManager sprayManager)
    {
        if (sprayManager != null)
        {
            sprayManager.OnSprayPickedUp += PlaySprayPickupSound;
            Debug.Log("SoundManager initialized and subscribed to SprayManager's OnSprayPickedUp event.");
        }
        else
        {
            Debug.LogError("SprayManager reference is null in SoundManager!");
        }
    }

    // Method to be called when a spray is picked up
    private void PlaySprayPickupSound()
    {
        Debug.Log("PlaySprayPickupSound method triggered.");

        if (sprayPickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(sprayPickupSound);  // Play the spray pickup sound
            Debug.Log("Spray pickup sound played.");
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is missing in SoundManager, sound not played.");
        }
    }
}