using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainCharacterDeath : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time in seconds before the scene restarts")]
    public float restartDelay = 2f;

    private Rigidbody rb;
    private PlayerMovement movement;
    private bool isDead = false; // Prevent multiple death triggers

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit a trap and aren't already dead
        if (!isDead && collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    // Support for Trigger Traps (e.g., spikes that are triggers)
    void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // 1. Stop player input immediately
        if (movement != null) movement.canControl = false;

        // 2. Disable the Clone Manager so player can't spawn/switch while dead
        CloneManager manager = GetComponent<CloneManager>();
        if (manager != null) manager.enabled = false;

        // 3. Make the character fall over (Physics Death)
        if (rb != null)
        {
            // Unfreeze rotation so he falls down
            rb.constraints = RigidbodyConstraints.None;

            // Add a random spin so he doesn't just fall flat like a board
            rb.AddTorque(Random.onUnitSphere * 2f, ForceMode.Impulse);
        }

        // 4. Start the restart countdown
        StartCoroutine(RestartLevel());
    }

    IEnumerator RestartLevel()
    {
        // Wait for the specified time
        yield return new WaitForSeconds(restartDelay);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}