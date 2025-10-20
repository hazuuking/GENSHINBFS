using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gerencia a lógica principal do jogo, incluindo a aplicação de elementos ao objeto alvo,
/// o cálculo das reações elementais e a orquestração entre os diferentes sistemas.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuração do Alvo")]
    [Tooltip("O objeto 3D que receberá os elementos e exibirá as auras. Deve ter um ElementalAuraManager e SlimeModelManager.")]
    public GameObject targetSlimeObject;

    [Header("Spawn do Slime")]
    public Transform spawnPoint; // arraste aqui o SpawnPoint da primeira máquina
    public GameObject slimePrefab; // arraste o prefab do slime (neutro)

    [Header("Slime Spawner")]
    [Tooltip("Reference to the SlimeSpawner component that handles slime spawning with physics")]
    public SlimeSpawner slimeSpawner;

    private ElementalAuraManager targetAuraManager;
    private SlimeModelManager targetSlimeModelManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Find SlimeSpawner if not assigned
        if (slimeSpawner == null)
        {
            slimeSpawner = FindObjectOfType<SlimeSpawner>();
            if (slimeSpawner == null)
            {
                Debug.LogWarning("SlimeSpawner not found. Falling back to built-in spawn method.");
                SpawnSlime();
            }
            // Don't call SpawnSlime here as SlimeSpawner will handle it
        }
    }

    /// <summary>
    /// Método chamado quando um botão de elemento é clicado (e.g., na primeira máquina).
    /// Aplica o elemento selecionado ao slime alvo.
    /// </summary>
    /// <param name="selectedElement">O ElementType selecionado pelo botão.</param>
    public void OnElementButtonClicked(ElementType selectedElement)
    {
        if (targetAuraManager != null)
        {
            targetAuraManager.ApplyElement(selectedElement);
            Debug.Log($"Elemento {selectedElement} aplicado ao slime.");
        }
        else
        {
            Debug.LogWarning("ElementalAuraManager do alvo não encontrado para aplicar elemento.");
        }
    }

    /// <summary>
    /// Este método é chamado quando o objeto alvo entra na área da SEGUNDA MÁQUINA.
    /// Ele determina o elemento que melhor reage com a aura atual do slime e o aplica.
    /// </summary>
    public void TriggerReactionMachine()
    {
        if (targetAuraManager == null)
        {
            Debug.LogWarning("ElementalAuraManager do alvo não encontrado para trigger de reação.");
            return;
        }

        ElementType currentAura = targetAuraManager.currentAura;
        ElementType currentStatus = targetAuraManager.currentStatus;

        // Se não há aura ou status, não há o que reagir, então não faz nada ou aplica um elemento padrão
        if (currentAura == ElementType.None && currentStatus == ElementType.None)
        {
            Debug.Log("Slime sem aura ou status. Nenhuma reação pode ser formada na segunda máquina.");
            // Opcional: Aplicar um elemento padrão aqui se desejar que a máquina sempre faça algo
            // targetAuraManager.ApplyElement(ElementType.Pyro); 
            return;
        }

        ElementType bestReactingElement = ElementType.None;
        ReactionType bestReactionType = ReactionType.None;

        // Itera sobre todos os elementos base para encontrar o que gera a melhor reação
        // Excluímos ElementType.None, Quicken, Burning, Bloom, Aggravate, Spread pois não são elementos 'incoming'
        ElementType[] possibleIncomingElements = new ElementType[]
        {
            ElementType.Pyro, ElementType.Hydro, ElementType.Electro, ElementType.Cryo, ElementType.Anemo, ElementType.Geo, ElementType.Dendro
        };

        foreach (ElementType potentialElement in possibleIncomingElements)
        {
            ReactionType reaction = ElementalReactionLogic.GetReaction(currentAura, potentialElement, currentStatus);
            
            // Prioriza reações que não sejam 'None'
            if (reaction != ReactionType.None)
            {
                // Lógica simples: qualquer reação é melhor que nenhuma. 
                // Para uma lógica mais complexa (ex: priorizar dano, CC, etc.),
                // você precisaria de um sistema de pontuação para ReactionType.
                bestReactingElement = potentialElement;
                bestReactionType = reaction;
                break; // Encontrou uma reação, pode parar aqui. Ou continuar para encontrar a 'melhor' de acordo com sua regra.
            }
        }

        if (bestReactingElement != ElementType.None)
        {
            Debug.Log($"Slime com aura {currentAura} e status {currentStatus}. Segunda máquina aplicando {bestReactingElement} para causar {bestReactionType}.");
            targetAuraManager.ApplyElement(bestReactingElement);
        }
        else
        {
            Debug.Log("Nenhum elemento encontrou uma reação com a aura/status atual do slime na segunda máquina.");
            // Opcional: Aplicar um elemento padrão ou limpar a aura se nenhuma reação for possível
            // targetAuraManager.ApplyElement(ElementType.Pyro); 
        }
    }

    /// <summary>
    /// Reseta o estado elemental do objeto alvo para 'None' e remove qualquer aura visual.
    /// Este método é útil para testes ou para reiniciar o ciclo de aplicação de elementos.
    /// </summary>
    public void ResetTargetSlime()
    {
        if (targetAuraManager != null)
        {
            targetAuraManager.ClearAuras();
            Debug.Log("Slime alvo resetado.");
        }
        
        // Use SlimeSpawner to respawn the slime if available
        if (slimeSpawner != null)
        {
            slimeSpawner.RespawnSlime();
            Debug.Log("Slime respawned using SlimeSpawner.");
        }
    }

    public void SpawnSlime()
{
    // If SlimeSpawner is available, use it instead
    if (slimeSpawner != null)
    {
        // Check if the slime already exists to prevent double spawning
        if (slimeSpawner.GetSpawnedSlime() == null)
        {
            slimeSpawner.SpawnSlime();
        }
        // Update references after SlimeSpawner has created the slime
        UpdateTargetObject();
        return;
    }
    
    // Fallback to original method if SlimeSpawner is not available
    if (slimePrefab == null || spawnPoint == null)
    {
        Debug.LogError("SpawnPoint ou SlimePrefab não configurado no GameManager!");
        return;
    }

    if (targetSlimeObject != null)
    {
        Destroy(targetSlimeObject); // remove o antigo se existir
    }

    targetSlimeObject = Instantiate(slimePrefab, spawnPoint.position, spawnPoint.rotation);
    UpdateTargetObject();
}

/// <summary>
/// Updates references to the target object's components
/// </summary>
private void UpdateTargetObject()
{
    if (targetSlimeObject == null) return;
    
    targetAuraManager = targetSlimeObject.GetComponent<ElementalAuraManager>();
    if (targetAuraManager == null)
    {
        targetAuraManager = targetSlimeObject.AddComponent<ElementalAuraManager>();
    }
    
    targetSlimeModelManager = targetSlimeObject.GetComponent<SlimeModelManager>();
    if (targetSlimeModelManager == null)
    {
        targetSlimeModelManager = targetSlimeObject.AddComponent<SlimeModelManager>();
    }
}

}


