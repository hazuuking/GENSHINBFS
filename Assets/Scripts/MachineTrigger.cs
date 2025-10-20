using UnityEngine;

/// <summary>
/// Script para detectar a entrada e saída do objeto alvo em uma área de máquina.
/// </summary>
public class MachineTrigger : MonoBehaviour
{
    /// <summary>
    /// Indica se esta é a primeira máquina (true) ou a segunda (false).
    /// </summary>
    public bool isFirstMachine = true;
    
    /// <summary>
    /// Tamanho do collider da máquina
    /// </summary>
    public Vector3 colliderSize = new Vector3(5, 5, 5);
    
    /// <summary>
    /// Flag para evitar múltiplas detecções
    /// </summary>
    private bool hasTriggered = false;
    
    void Start()
    {
        // Garante que o collider está configurado corretamente
        ConfigureCollider();
    }
    
    /// <summary>
    /// Configura o collider da máquina para garantir detecção
    /// </summary>
    private void ConfigureCollider()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        // Configura o collider como trigger e com tamanho adequado
        boxCollider.isTrigger = true;
        boxCollider.size = colliderSize;
        boxCollider.center = Vector3.zero;
        
        Debug.Log($"Configurado collider da {(isFirstMachine ? "Primeira" : "Segunda")} Máquina com tamanho {colliderSize}");
    }

    /// <summary>
    /// Chamado quando outro Collider entra no trigger.
    /// </summary>
    /// <param name="other">O Collider que entrou no trigger.</param>
    void OnTriggerEnter(Collider other)
    {
        ProcessTrigger(other);
    }
    
    /// <summary>
    /// Chamado continuamente enquanto outro Collider está no trigger.
    /// </summary>
    /// <param name="other">O Collider que está no trigger.</param>
    void OnTriggerStay(Collider other)
    {
        ProcessTrigger(other);
    }
    
    /// <summary>
    /// Processa a detecção do trigger
    /// </summary>
    private void ProcessTrigger(Collider other)
    {
        // Verifica se o objeto no trigger é o slime alvo do GameManager
        if (GameManager.Instance != null && other.gameObject == GameManager.Instance.targetSlimeObject && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log($"TargetObject detectado na {(isFirstMachine ? "Primeira" : "Segunda")} Máquina.");
            
            if (!isFirstMachine)
            {
                // Se for a segunda máquina, aciona a lógica de reação
                GameManager.Instance.TriggerReactionMachine();
            }
            // Lógica para a primeira máquina (seleção de elemento) é tratada pelos ElementButton3D
        }
    }

    /// <summary>
    /// Chamado quando outro Collider sai do trigger.
    /// </summary>
    /// <param name="other">O Collider que saiu do trigger.</param>
    void OnTriggerExit(Collider other)
    {
        // Verifica se o objeto que saiu do trigger é o slime alvo do GameManager
        if (GameManager.Instance != null && other.gameObject == GameManager.Instance.targetSlimeObject)
        {
            Debug.Log($"TargetObject saiu da {(isFirstMachine ? "Primeira" : "Segunda")} Máquina.");
            // Resetamos a flag para permitir nova detecção
            hasTriggered = false;
            
            // Opcional: Lógica para resetar o slime ou prepará-lo para a próxima fase
            if (!isFirstMachine)
            {
                GameManager.Instance.ResetTargetSlime();
            }
        }
    }
}


