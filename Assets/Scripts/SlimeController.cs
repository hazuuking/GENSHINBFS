using UnityEngine;

/// <summary>
/// Componente de controle físico e de estado do Slime.
/// Este script garante que o Slime tenha as propriedades físicas corretas (Rigidbody e Collider)
/// e gerencia o estado de contato com a esteira, essencial para o controle de movimento
/// e para a estabilidade física durante o transporte.
/// </summary>
public class SlimeController : MonoBehaviour
{
    // --- VARIÁVEIS INTERNAS ---

    /// <summary>
    /// Referência ao componente de física (<c>Rigidbody</c>) do Slime.
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Flag booleana que indica se o Slime está atualmente em contato com a esteira transportadora.
    /// </summary>
    private bool onEsteira;

    // --- MÉTODOS DO UNITY ---

    /// <summary>
    /// <c>Start()</c>: Chamado na inicialização.
    /// Configura as propriedades de física do Slime para garantir um comportamento realista.
    /// </summary>
    void Start()
    {
        // Obtém a referência do Rigidbody.
        rb = GetComponent<Rigidbody>();

        // 1. Configurações de Física:
        rb.useGravity = true;           // O Slime deve ser afetado pela gravidade.
        rb.isKinematic = false;         // O Slime é um objeto dinâmico, movido por forças ou manipulação de posição.
        // Interpolação para suavizar o movimento, importante quando a posição é manipulada no FixedUpdate (pelo ConveyorBeltController).
        rb.interpolation = RigidbodyInterpolation.Interpolate; 
        // Detecção de colisão discreta como padrão. A detecção contínua pode ser ativada se o Slime
        // atravessar a esteira em alta velocidade, mas é mais custosa.
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete; 

        // Estabilidade: aumentar amortecimento rotacional e congelar rotações X/Z para evitar tombos
        rb.angularDrag = 2.0f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // Amortecimento linear moderado para reduzir oscilações
        rb.drag = 1.5f;

        // 2. Configuração do Collider:
        // Garante que o Slime tenha um Collider para interagir fisicamente.
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
        }
        col.isTrigger = false; // Deve ser um Collider sólido para detectar colisão.

        // Material físico "grudento" para aumentar aderência na esteira
        if (col.material == null)
        {
            var sticky = new PhysicMaterial("SlimeStickyPhysicMaterial");
            sticky.dynamicFriction = 0.9f;
            sticky.staticFriction = 0.95f;
            sticky.frictionCombine = PhysicMaterialCombine.Maximum;
            sticky.bounciness = 0f;
            sticky.bounceCombine = PhysicMaterialCombine.Minimum;
            col.material = sticky;
        }

        // 3. Força a atualização do sistema de física.
        Physics.SyncTransforms();
        rb.WakeUp();
    }

<<<<<<< HEAD
    /// <summary>
    /// <c>Update()</c>: Chamado a cada frame.
    /// Usado para aplicar correções de estabilidade vertical.
    /// </summary>
    void Update()
    {
        // Correção de Estabilidade: Suaviza a velocidade vertical apenas quando necessário
        // para evitar interferir com o movimento natural da esteira
        if (onEsteira && rb.velocity.y < -0.1f)
        {
            // Suaviza a velocidade vertical negativa em vez de zerar completamente
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.9f, rb.velocity.z);
        }
    }
=======

>>>>>>> 90e94c03446df957929b35bd1ff85042e677564b

    /// <summary>
    /// <c>OnCollisionEnter()</c>: Chamado quando o Slime entra em contato com outro Collider sólido.
    /// </summary>
    /// <param name="collision">Informações sobre a colisão.</param>
    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto colidido é a esteira (identificada pela Tag).
        if (collision.gameObject.CompareTag("Esteira"))
        {
            onEsteira = true;
<<<<<<< HEAD
            // Remove o congelamento de Y para permitir movimento natural da esteira
            // rb.constraints |= RigidbodyConstraints.FreezePositionY;
=======

>>>>>>> 90e94c03446df957929b35bd1ff85042e677564b
            Debug.Log("[SlimeController] Slime pousou na esteira!");
        }
    }

    /// <summary>
    /// <c>OnCollisionExit()</c>: Chamado quando o Slime perde contato com outro Collider sólido.
    /// </summary>
    /// <param name="collision">Informações sobre a colisão.</param>
    void OnCollisionExit(Collision collision)
    {
        // Verifica se o Slime saiu da esteira.
        if (collision.gameObject.CompareTag("Esteira"))
        {
            onEsteira = false;
<<<<<<< HEAD
            // Não precisa mais liberar Y pois não congelamos mais
            // rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
=======

>>>>>>> 90e94c03446df957929b35bd1ff85042e677564b
            Debug.Log("[SlimeController] Slime saiu da esteira!");
        }
    }
}