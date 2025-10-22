using UnityEngine;

public class EsteiraSetup : MonoBehaviour
{
    void Start()
    {
        // Garante que tenha a tag "Esteira"
        gameObject.tag = "Esteira";

        // Adiciona collider se não existir
        Collider col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();

        col.isTrigger = false;

        // Garante que tenha Rigidbody estático (kinematic)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;
        rb.useGravity = false;
    }
}