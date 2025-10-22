using UnityEngine;

public class SlimeController : MonoBehaviour
{
    private Rigidbody rb;
    private bool onEsteira;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Corrige configurações de física no prefab
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Garante que o collider 3D existe e não é trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
        }
        col.isTrigger = false;

        // Força atualização física
        Physics.SyncTransforms();
        rb.WakeUp();
    }

    void Update()
    {
        // Garante que, ao pousar na esteira, ele pare verticalmente
        if (onEsteira && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Esteira"))
        {
            onEsteira = true;
            Debug.Log("Slime pousou na esteira!");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Esteira"))
        {
            onEsteira = false;
            Debug.Log("Slime saiu da esteira!");
        }
    }
}