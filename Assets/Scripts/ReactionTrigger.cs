using UnityEngine;
using System.Collections.Generic;

public class ReactionTrigger : MonoBehaviour
{
    public static ReactionTrigger Instance { get; private set; }

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
    private Dictionary<ReactionType, GameObject> reactionVFXMap;

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
        InitializeReactionMap();
    }

    private void InitializeReactionMap()
    {
        reactionVFXMap = new Dictionary<ReactionType, GameObject>
        {
            {ReactionType.Overload, overloadVFXPrefab},
            {ReactionType.Vaporize, vaporizeVFXPrefab},
            {ReactionType.Melt, meltVFXPrefab},
            {ReactionType.Freeze, freezeVFXPrefab},
            {ReactionType.Superconduct, superconductVFXPrefab},
            {ReactionType.ElectroCharged, electroChargedVFXPrefab},
            {ReactionType.Swirl, swirlVFXPrefab},
            {ReactionType.Crystallize, crystallizeVFXPrefab},
            {ReactionType.Burning, burningVFXPrefab},
            {ReactionType.Bloom, bloomVFXPrefab},
            {ReactionType.Quicken, quickenVFXPrefab},
            {ReactionType.Aggravate, aggravateVFXPrefab},
            {ReactionType.Spread, spreadVFXPrefab}
        };
    }

    // Este método será chamado pelo ElementalAuraManager quando uma reação for determinada
    public void TriggerReactionVFX(ReactionType reaction, Vector3 position, Quaternion rotation)
    {
        if (reactionVFXMap.TryGetValue(reaction, out GameObject vfxPrefab))
        {
            if (vfxPrefab != null)
            {
                // Usar ObjectPoolManager se estiver configurado
                GameObject vfxInstance = null;
                if (ObjectPoolManager.Instance != null)
                {
                    vfxInstance = ObjectPoolManager.Instance.SpawnFromPool(reaction.ToString(), position, rotation);
                }
                else
                {
                    vfxInstance = Instantiate(vfxPrefab, position, rotation);
                }

                if (vfxInstance != null)
                {
                    ElementalReactionVFX vfxController = vfxInstance.GetComponent<ElementalReactionVFX>();
                    if (vfxController != null)
                    {
                        // O Activate já é chamado pelo SpawnFromPool que ativa o GameObject
                        // Se o VFX for contínuo e precisar ser desativado manualmente, não defina effectDuration
                        // e chame vfxController.Deactivate() quando apropriado.
                        // Para efeitos de curta duração, o ElementalReactionVFX já gerencia a desativação.
                    }
                    else
                    {
                        Debug.LogWarning($"Prefab {reaction.ToString()} não possui o script ElementalReactionVFX.");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Prefab de VFX para {reaction.ToString()} não atribuído no Inspector.");
            }
        }
        else
        {
            Debug.LogWarning($"Reação {reaction.ToString()} não encontrada no mapa de VFX.");
        }
    }
}


