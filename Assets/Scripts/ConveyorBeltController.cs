using UnityEngine;

public class ConveyorBeltController : MonoBehaviour
{
    public float scrollSpeed = 0.5f; // Velocidade de rolagem da textura
    public Vector2 scrollDirection = new Vector2(0, 1); // Direção de rolagem (ex: (0,1) para frente no eixo Y da textura)

    private Renderer beltRenderer;

    void Start()
    {
        beltRenderer = GetComponent<Renderer>();
        if (beltRenderer == null)
        {
            Debug.LogError("ConveyorBeltController requer um componente Renderer no mesmo GameObject.");
            enabled = false;
        }
    }

    void Update()
    {
        if (beltRenderer != null)
        {
            // Calcula o offset da textura com base no tempo e velocidade
            float offsetU = Time.time * scrollSpeed * scrollDirection.x;
            float offsetV = Time.time * scrollSpeed * scrollDirection.y;

            // Aplica o offset à textura principal do material
            beltRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetU, offsetV));
        }
    }

    // Opcional: Método para mover objetos sobre a esteira
    void OnTriggerStay(Collider other)
    {
        // Verifica se o objeto tem um Rigidbody para aplicar força
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calcula a força na direção da esteira
            // A direção da esteira é baseada na transformação local do GameObject
            // Nota: Para um movimento mais preciso, você pode querer usar transform.right e transform.forward
            // do próprio objeto da esteira para definir a direção.
            Vector3 moveDirection = transform.forward * scrollDirection.y + transform.right * scrollDirection.x;
            rb.MovePosition(rb.position + moveDirection * scrollSpeed * Time.deltaTime);
        }
    }
}


