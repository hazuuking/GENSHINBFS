using UnityEngine;

/// <summary>
/// Componente de configuração inicial da Esteira Transportadora.
/// Este script garante que o objeto da esteira possua as propriedades físicas
/// necessárias para interagir corretamente com o Slime e o sistema de transporte.
/// Ele força a esteira a ser um objeto estático (Kinematic Rigidbody), que é
/// o modelo ideal para superfícies de movimento em simulações físicas.
/// </summary>
public class EsteiraSetup : MonoBehaviour
{
    /// <summary>
    /// <c>Start()</c>: Chamado na inicialização.
    /// Configura os componentes de Tag, Collider e Rigidbody da esteira.
    /// </summary>
    void Start()
    {
        // 1. Tagging: Garante que a esteira tenha a Tag "Esteira".
        // Esta tag é usada pelo SlimeController para detectar o contato.
        gameObject.tag = "Esteira";

        // 2. Configuração do Collider:
        // Adiciona um Collider (se não existir) para permitir a detecção de colisão.
        Collider col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();

        // A esteira deve ter um Collider sólido (não-Trigger) para que o Slime
        // possa pousar sobre ela e disparar eventos de colisão (OnCollisionEnter).
        col.isTrigger = false;

        // 3. Configuração do Rigidbody:
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // A esteira deve ser Estática (Kinematic) e não ser afetada pela gravidade.
        // Isso garante que ela não se mova e atue como uma superfície de referência estável.
        rb.isKinematic = true;
        rb.useGravity = false;
        
        // Nota: O movimento da esteira é simulado pelo ConveyorBeltController,
        // que manipula a posição do Slime, e não pelo movimento da própria esteira.
    }
}