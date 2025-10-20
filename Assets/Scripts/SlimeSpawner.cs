using UnityEngine;

/// <summary>
/// Handles spawning and managing the target slime object.
/// </summary>
public class SlimeSpawner : MonoBehaviour
{
    /// <summary>
    /// Reference to the slime prefab to spawn.
    /// </summary>
    public GameObject slimePrefab;

    /// <summary>
    /// Reference to the GameManager.
    /// </summary>
    public GameManager gameManager;

    /// <summary>
    /// The spawn position for the slime.
    /// </summary>
    public Transform spawnPoint;

    /// <summary>
    /// The currently spawned slime instance.
    /// </summary>
    private GameObject currentSlime;

    void Start()
    {
        SpawnSlime();
    }

    /// <summary>
    /// Spawns a slime at the designated spawn point.
    /// </summary>
    public void SpawnSlime()
    {
        // If there's already a slime, destroy it
        if (currentSlime != null)
        {
            Destroy(currentSlime);
        }

        // Spawn the new slime
        currentSlime = Instantiate(slimePrefab, spawnPoint.position, Quaternion.identity);
        
        // Add a rigidbody for physics interactions
        Rigidbody rb = currentSlime.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = currentSlime.AddComponent<Rigidbody>();
        }
        
        // Configure the rigidbody
        rb.mass = 1f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.05f;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Add a collider if it doesn't exist
        SphereCollider collider = currentSlime.GetComponent<SphereCollider>();
        if (collider == null)
        {
            collider = currentSlime.AddComponent<SphereCollider>();
            collider.radius = 0.5f; // Adjust based on your slime model
            
            // Create a physics material for better collision behavior
            PhysicMaterial slimePhysicsMaterial = new PhysicMaterial("SlimePhysics");
            slimePhysicsMaterial.dynamicFriction = 0.6f;
            slimePhysicsMaterial.staticFriction = 0.6f;
            slimePhysicsMaterial.bounciness = 0.2f;
            slimePhysicsMaterial.frictionCombine = PhysicMaterialCombine.Average;
            slimePhysicsMaterial.bounceCombine = PhysicMaterialCombine.Average;
            
            collider.material = slimePhysicsMaterial;
        }
        
        // Ensure the slime has the correct layer for collision
        currentSlime.layer = LayerMask.NameToLayer("Default");
        
        // Set this as the target object in the GameManager
        if (gameManager != null)
        {
            gameManager.targetObject = currentSlime;
        }
        else
        {
            Debug.LogError("GameManager reference not set in SlimeSpawner!");
        }
    }

    /// <summary>
    /// Respawns the slime at the spawn point.
    /// </summary>
    public void RespawnSlime()
    {
        SpawnSlime();
    }
}