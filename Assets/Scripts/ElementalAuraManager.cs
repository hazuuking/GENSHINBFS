using UnityEngine;
using System.Collections.Generic;

public class ElementalAuraManager : MonoBehaviour
{
    public ElementType currentAura = ElementType.None;
    public ElementType currentStatus = ElementType.None; // Para estados como Quicken, Burning, Bloom

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

    private Dictionary<ElementType, GameObject> auraVFXInstances = new Dictionary<ElementType, GameObject>();
    private Dictionary<ElementType, GameObject> auraVFXPrefabs = new Dictionary<ElementType, GameObject>();

    void Awake()
    {
        // Inicializa o mapa de prefabs de aura
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

    public void ApplyElement(ElementType incomingElement)
    {
        ElementType previousAura = currentAura;
        ElementType previousStatus = currentStatus;

        // Determina a reação usando a lógica centralizada
        ReactionType reaction = ElementalReactionLogic.GetReaction(currentAura, incomingElement, currentStatus);

        // Lógica para atualizar auras e status com base na reação
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
                // Reações que consomem auras e não deixam um status persistente no alvo
                currentAura = ElementType.None;
                currentStatus = ElementType.None;
                break;
            case ReactionType.Burning:
                currentAura = incomingElement; // O elemento que causou Burning pode permanecer como aura
                currentStatus = ElementType.Burning;
                break;
            case ReactionType.Bloom:
                currentAura = incomingElement; // O elemento que causou Bloom pode permanecer como aura
                currentStatus = ElementType.Bloom;
                break;
            case ReactionType.Quicken:
                currentAura = incomingElement; // O elemento que causou Quicken pode permanecer como aura
                currentStatus = ElementType.Quicken;
                break;
            case ReactionType.Aggravate:
            case ReactionType.Spread:
                // Aggravate/Spread não mudam a aura base nem o status Quicken, apenas consomem o incomingElement
                // O status Quicken permanece
                break;
            case ReactionType.None:
                // Se não houve reação, o incomingElement se torna a nova aura, limpando o status
                currentAura = incomingElement;
                currentStatus = ElementType.None;
                break;
        }

        UpdateAuraVFX(previousAura, previousStatus);

        // Notifica o ReactionTrigger para disparar o VFX da reação, se houver
        if (ReactionTrigger.Instance != null && reaction != ReactionType.None)
        {
            ReactionTrigger.Instance.TriggerReactionVFX(reaction, transform.position, Quaternion.identity);
        }
    }

    private void UpdateAuraVFX(ElementType previousAura, ElementType previousStatus)
    {
        // Desativa VFX da aura anterior
        if (auraVFXInstances.ContainsKey(previousAura) && auraVFXInstances[previousAura] != null)
        {
            auraVFXInstances[previousAura].SetActive(false);
        }
        // Desativa VFX do status anterior
        if (auraVFXInstances.ContainsKey(previousStatus) && auraVFXInstances[previousStatus] != null)
        {
            auraVFXInstances[previousStatus].SetActive(false);
        }

        // Ativa VFX da nova aura
        if (currentAura != ElementType.None && auraVFXPrefabs.ContainsKey(currentAura) && auraVFXPrefabs[currentAura] != null)
        {
            if (!auraVFXInstances.ContainsKey(currentAura) || auraVFXInstances[currentAura] == null)
            {
                auraVFXInstances[currentAura] = Instantiate(auraVFXPrefabs[currentAura], transform);
                auraVFXInstances[currentAura].transform.localPosition = Vector3.zero;
            }
            auraVFXInstances[currentAura].SetActive(true);
        }

        // Ativa VFX do novo status (se houver)
        if (currentStatus != ElementType.None && auraVFXPrefabs.ContainsKey(currentStatus) && auraVFXPrefabs[currentStatus] != null)
        {
            if (!auraVFXInstances.ContainsKey(currentStatus) || auraVFXInstances[currentStatus] == null)
            {
                auraVFXInstances[currentStatus] = Instantiate(auraVFXPrefabs[currentStatus], transform);
                auraVFXInstances[currentStatus].transform.localPosition = Vector3.zero;
            }
            auraVFXInstances[currentStatus].SetActive(true);
        }
    }

    public void ClearAuras()
    {
        foreach (var vfxInstance in auraVFXInstances.Values)
        {
            if (vfxInstance != null) Destroy(vfxInstance); // Destruir em vez de apenas desativar para limpar completamente
        }
        auraVFXInstances.Clear();
        currentAura = ElementType.None;
        currentStatus = ElementType.None;
    }
}


