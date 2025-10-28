using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controla as auras e reações elementais aplicadas ao slime.
/// </summary>
public class ElementalAuraManager : MonoBehaviour
{
    public ElementType currentAura = ElementType.None;
    public ElementType currentStatus = ElementType.None;

    [Header("Prefabs de VFX de Aura")]
    public GameObject pyroAuraVFXPrefab;
    public GameObject hydroAuraVFXPrefab;
    public GameObject electroAuraVFXPrefab;
    public GameObject cryoAuraVFXPrefab;
    public GameObject anemoAuraVFXPrefab;
    public GameObject geoAuraVFXPrefab;
    public GameObject dendroAuraVFXPrefab;
    public GameObject quickenAuraVFXPrefab;
    public GameObject burningAuraVFXPrefab;
    public GameObject bloomAuraVFXPrefab;

    private readonly Dictionary<ElementType, GameObject> auraVFXInstances = new();
    private readonly Dictionary<ElementType, GameObject> auraVFXPrefabs = new();

    void Awake()
    {
        // Inicializa o mapa de prefabs
        auraVFXPrefabs.Add(ElementType.Pyro, pyroAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Hydro, hydroAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Electro, electroAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Cryo, cryoAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Anemo, anemoAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Geo, geoAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Dendro, dendroAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Quicken, quickenAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Burning, burningAuraVFXPrefab);
        auraVFXPrefabs.Add(ElementType.Bloom, bloomAuraVFXPrefab);
    }

    /// <summary>
    /// Aplica um novo elemento ao slime e verifica se há reação.
    /// </summary>
    public void ApplyElement(ElementType incomingElement)
    {
        ElementType previousAura = currentAura;
        ElementType previousStatus = currentStatus;

        // Determina reação via lógica centralizada
        ReactionType reaction = ElementalReactionLogic.GetReaction(currentAura, incomingElement, currentStatus);

        // Atualiza estado conforme a reação
        switch (reaction)
        {
            case ReactionType.Overload:
            case ReactionType.Vaporize:
            case ReactionType.Melt:
            case ReactionType.Freeze:
            case ReactionType.Superconduct:
            case ReactionType.ElectroCharged:
            case ReactionType.Swirl:
            case ReactionType.Crystallize:
                currentAura = ElementType.None;
                currentStatus = ElementType.None;
                break;

            case ReactionType.Burning:
                currentAura = incomingElement;
                currentStatus = ElementType.Burning;
                break;

            case ReactionType.Bloom:
                currentAura = incomingElement;
                currentStatus = ElementType.Bloom;
                break;

            case ReactionType.Quicken:
                currentAura = incomingElement;
                currentStatus = ElementType.Quicken;
                break;

            default:
                currentAura = incomingElement;
                currentStatus = ElementType.None;
                break;
        }

        UpdateAuraVFX(previousAura, previousStatus);

        if (ReactionTrigger.Instance != null && reaction != ReactionType.None)
            ReactionTrigger.Instance.TriggerReactionVFX(reaction, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// Atualiza o VFX ativo com base na aura e status atuais.
    /// </summary>
    private void UpdateAuraVFX(ElementType previousAura, ElementType previousStatus)
    {
        DisableVFX(previousAura);
        DisableVFX(previousStatus);

        EnableVFX(currentAura);
        EnableVFX(currentStatus);
    }

    private void EnableVFX(ElementType type)
    {
        if (type == ElementType.None || !auraVFXPrefabs.ContainsKey(type) || auraVFXPrefabs[type] == null)
            return;

        if (!auraVFXInstances.ContainsKey(type) || auraVFXInstances[type] == null)
        {
            auraVFXInstances[type] = Instantiate(auraVFXPrefabs[type], transform);
            auraVFXInstances[type].transform.localPosition = Vector3.zero;
        }

        auraVFXInstances[type].SetActive(true);
    }

    private void DisableVFX(ElementType type)
    {
        if (auraVFXInstances.ContainsKey(type) && auraVFXInstances[type] != null)
            auraVFXInstances[type].SetActive(false);
    }

    /// <summary>
    /// Remove todas as auras e destrói os VFX ativos.
    /// </summary>
    public void ClearAuras()
    {
        foreach (var vfx in auraVFXInstances.Values)
        {
            if (vfx != null)
                Destroy(vfx);
        }

        auraVFXInstances.Clear();
        currentAura = ElementType.None;
        currentStatus = ElementType.None;
    }
}
