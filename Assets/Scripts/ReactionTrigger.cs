using UnityEngine;
using System.Collections.Generic;

public class ReactionTrigger : MonoBehaviour
{
    public enum ElementType
    {
        None,
        Pyro,
        Hydro,
        Electro,
        Cryo,
        Anemo,
        Geo,
        Dendro,
        Quicken // Adicionado para representar o estado Quicken
    }

    [Header("Configuração de Elementos")]
    public ElementType currentAura = ElementType.None;
    public ElementType incomingElement = ElementType.None;

    [Header("Prefabs de VFX de Reação")]
    public GameObject overloadVFXPrefab;
    public GameObject vaporizeVFXPrefab;
    public GameObject meltVFXPrefab;
    public GameObject freezeVFXPrefab;
    public GameObject superconductVFXPrefab;
    public GameObject electroChargedVFXPrefab;
    public GameObject swirlVFXPrefab;
    public GameObject crystallizeVFXPrefab;
    public GameObject burningVFXPrefab;
    public GameObject bloomVFXPrefab;
    public GameObject quickenVFXPrefab;
    public GameObject aggravateVFXPrefab;
    public GameObject spreadVFXPrefab;

    // Dicionário para mapear tipos de reação a prefabs de VFX
    private Dictionary<string, GameObject> reactionVFXMap;

    void Start()
    {
        InitializeReactionMap();
    }

    private void InitializeReactionMap()
    {
        reactionVFXMap = new Dictionary<string, GameObject>
        {
            {"Overload", overloadVFXPrefab},
            {"Vaporize", vaporizeVFXPrefab},
            {"Melt", meltVFXPrefab},
            {"Freeze", freezeVFXPrefab},
            {"Superconduct", superconductVFXPrefab},
            {"ElectroCharged", electroChargedVFXPrefab},
            {"Swirl", swirlVFXPrefab},
            {"Crystallize", crystallizeVFXPrefab},
            {"Burning", burningVFXPrefab},
            {"Bloom", bloomVFXPrefab},
            {"Quicken", quickenVFXPrefab},
            {"Aggravate", aggravateVFXPrefab},
            {"Spread", spreadVFXPrefab}
        };
    }

    // Método para simular a aplicação de um elemento
    public void ApplyElement(ElementType element)
    {
        incomingElement = element;
        CheckForReaction();
        // Após a reação (ou não), o elemento que chegou pode se tornar a nova aura
        currentAura = incomingElement;
    }

    private void CheckForReaction()
    {
        string reactionName = GetReactionName(currentAura, incomingElement);

        if (!string.IsNullOrEmpty(reactionName))
        {
            Debug.Log($"Reação detectada: {reactionName}");
            TriggerVFX(reactionName, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log($"Nenhuma reação para {currentAura} + {incomingElement}");
        }
    }

    private string GetReactionName(ElementType aura, ElementType incoming)
    {
        // Lógica simplificada para determinar a reação
        // Em um jogo real, isso seria muito mais complexo e baseado em regras de prioridade
        if ((aura == ElementType.Pyro && incoming == ElementType.Electro) || (aura == ElementType.Electro && incoming == ElementType.Pyro))
            return "Overload";
        if ((aura == ElementType.Pyro && incoming == ElementType.Hydro) || (aura == ElementType.Hydro && incoming == ElementType.Pyro))
            return "Vaporize";
        if ((aura == ElementType.Pyro && incoming == ElementType.Cryo) || (aura == ElementType.Cryo && incoming == ElementType.Pyro))
            return "Melt";
        if ((aura == ElementType.Hydro && incoming == ElementType.Cryo) || (aura == ElementType.Cryo && incoming == ElementType.Hydro))
            return "Freeze";
        if ((aura == ElementType.Electro && incoming == ElementType.Cryo) || (aura == ElementType.Cryo && incoming == ElementType.Electro))
            return "Superconduct";
        if ((aura == ElementType.Electro && incoming == ElementType.Hydro) || (aura == ElementType.Hydro && incoming == ElementType.Electro))
            return "ElectroCharged";
        if (aura == ElementType.Anemo && (incoming == ElementType.Pyro || incoming == ElementType.Hydro || incoming == ElementType.Electro || incoming == ElementType.Cryo))
            return "Swirl";
        if (incoming == ElementType.Anemo && (aura == ElementType.Pyro || aura == ElementType.Hydro || aura == ElementType.Electro || aura == ElementType.Cryo))
            return "Swirl";
        if (aura == ElementType.Geo && (incoming == ElementType.Pyro || incoming == ElementType.Hydro || incoming == ElementType.Electro || incoming == ElementType.Cryo))
            return "Crystallize";
        if (incoming == ElementType.Geo && (aura == ElementType.Pyro || aura == ElementType.Hydro || aura == ElementType.Electro || aura == ElementType.Cryo))
            return "Crystallize";
        if ((aura == ElementType.Dendro && incoming == ElementType.Pyro) || (aura == ElementType.Pyro && incoming == ElementType.Dendro))
            return "Burning";
        if ((aura == ElementType.Dendro && incoming == ElementType.Hydro) || (aura == ElementType.Hydro && incoming == ElementType.Dendro))
            return "Bloom";
        if ((aura == ElementType.Dendro && incoming == ElementType.Electro) || (aura == ElementType.Electro && incoming == ElementType.Dendro))
            return "Quicken";
        // Para Aggravate e Spread, precisamos de um estado de Quicken pré-existente
        // Esta lógica é mais complexa e pode exigir um sistema de aura mais robusto
        // Por simplicidade, vamos assumir que 'Quicken' é um estado que permite Aggravate/Spread
        // Se a aura atual for Quicken e o elemento que chega for Electro, é Aggravate
        if (aura == ElementType.Quicken && incoming == ElementType.Electro)
            return "Aggravate";
        // Se a aura atual for Quicken e o elemento que chega for Dendro, é Spread
        if (aura == ElementType.Quicken && incoming == ElementType.Dendro)
            return "Spread";

        return null;
    }

    private void TriggerVFX(string reactionName, Vector3 position, Quaternion rotation)
    {
        if (reactionVFXMap.TryGetValue(reactionName, out GameObject vfxPrefab))
        {
            if (vfxPrefab != null)
            {
                // Instancia o prefab do VFX
                GameObject vfxInstance = Instantiate(vfxPrefab, position, rotation);
                // Tenta obter o componente ElementalReactionVFX e ativá-lo
                ElementalReactionVFX vfxController = vfxInstance.GetComponent<ElementalReactionVFX>();
                if (vfxController != null)
                {
                    vfxController.Activate(position, rotation);
                }
                else
                {
                    Debug.LogWarning($"Prefab {reactionName} não possui o script ElementalReactionVFX.");
                }
            }
            else
            {
                Debug.LogWarning($"Prefab de VFX para {reactionName} não atribuído no Inspector.");
            }
        }
        else
        {
            Debug.LogWarning($"Reação {reactionName} não encontrada no mapa de VFX.");
        }
    }

    // Exemplo de uso para teste (pode ser removido em um jogo real)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ApplyElement(ElementType.Pyro);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ApplyElement(ElementType.Hydro);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ApplyElement(ElementType.Electro);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ApplyElement(ElementType.Cryo);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ApplyElement(ElementType.Anemo);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ApplyElement(ElementType.Geo);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ApplyElement(ElementType.Dendro);
        }
    }
}


