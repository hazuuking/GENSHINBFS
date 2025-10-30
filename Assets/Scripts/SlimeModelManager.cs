using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// SlimeModelManager: gerencia a instância visual do slime de acordo com o ElementType/status.
/// Mantém apenas 1 GameObject filho como representação visual.
/// </summary>
public class SlimeModelManager : MonoBehaviour
{
    [System.Serializable]
    public class SlimeModelEntry
    {
        public ElementType elementType;
        public GameObject slimeModelPrefab;
    }

    [Tooltip("Lista de modelos por ElementType (inclua 'None' como fallback).")]
    public List<SlimeModelEntry> slimeModels = new List<SlimeModelEntry>();

    [HideInInspector] public ElementalAuraManager auraManager; // opcional, será buscado se null

    private GameObject currentSlimeModelInstance;
    private ElementType lastElement = ElementType.None;
    private ElementType lastStatus = ElementType.None;

    void Start()
    {
        if (auraManager == null) auraManager = GetComponent<ElementalAuraManager>();
        // Inicializa visual com base no estado atual (se houver)
        UpdateSlimeModel(auraManager != null ? auraManager.currentAura : ElementType.None,
                         auraManager != null ? auraManager.currentStatus : ElementType.None);
    }

    void Update()
    {
        if (auraManager == null) return;
        if (auraManager.currentAura != lastElement || auraManager.currentStatus != lastStatus)
        {
            UpdateSlimeModel(auraManager.currentAura, auraManager.currentStatus);
            lastElement = auraManager.currentAura;
            lastStatus = auraManager.currentStatus;
        }

        // Rotate to face camera (optional aesthetic)
        if (currentSlimeModelInstance != null && Camera.main != null)
        {
            Vector3 lookDir = Camera.main.transform.position - currentSlimeModelInstance.transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                currentSlimeModelInstance.transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    /// <summary>
    /// Troca o modelo do slime de acordo com element/status.
    /// </summary>
    private void UpdateSlimeModel(ElementType element, ElementType status)
    {
        // Destroi o modelo atual (se existir)
        if (currentSlimeModelInstance != null)
        {
            Destroy(currentSlimeModelInstance);
            currentSlimeModelInstance = null;
        }

        // Prioriza status (ex: Burning), se houver
        ElementType toShow = (status != ElementType.None) ? status : element;

        GameObject prefab = FindPrefabForType(toShow);
        if (prefab == null)
        {
            // tenta fallback 'None'
            prefab = FindPrefabForType(ElementType.None);
        }

        if (prefab != null)
        {
            // Instancia como filho direto, com transform zerado
            currentSlimeModelInstance = Instantiate(prefab, transform);
            currentSlimeModelInstance.transform.localPosition = Vector3.zero;
            currentSlimeModelInstance.transform.localRotation = Quaternion.identity;
            currentSlimeModelInstance.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogWarning($"[SlimeModelManager] Nenhum prefab encontrado para {toShow} nem fallback 'None'.");
        }
    }

    private GameObject FindPrefabForType(ElementType type)
    {
        foreach (var e in slimeModels)
        {
            if (e.elementType == type) return e.slimeModelPrefab;
        }
        return null;
    }

    /// <summary>
    /// Força a troca do modelo com um elemento selecionado manualmente.
    /// </summary>
    public void ChangeSlimeModel(ElementType selectedElement)
    {
        UpdateSlimeModel(selectedElement, ElementType.None);
        lastElement = selectedElement;
        lastStatus = ElementType.None;
        Debug.Log($"[SlimeModelManager] Modelo alterado para {selectedElement}");
    }

    
}


