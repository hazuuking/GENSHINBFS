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

    private Dictionary<ReactionType, GameObject> reactionVFXMap;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

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

    public void TriggerReactionVFX(ReactionType reaction, Vector3 position, Quaternion rotation)
    {
        if (reactionVFXMap.TryGetValue(reaction, out GameObject vfxPrefab) && vfxPrefab != null)
        {
            GameObject vfxInstance = null;

            if (ObjectPoolManager.Instance != null)
                vfxInstance = ObjectPoolManager.Instance.SpawnFromPool(reaction.ToString(), position, rotation);
            else
                vfxInstance = Instantiate(vfxPrefab, position, rotation);

            if (vfxInstance != null)
            {
                var vfxController = vfxInstance.GetComponent<ElementalReactionVFX>();
                if (vfxController == null)
                    Debug.LogWarning($"Prefab {reaction} não possui o script ElementalReactionVFX.");
            }
        }
        else
        {
            Debug.LogWarning($"Prefab de VFX para {reaction} não encontrado ou não atribuído.");
        }
    }
}