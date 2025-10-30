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
    private Transform visualAnchor;
    private ElementType lastElement = ElementType.None;
    private ElementType lastStatus = ElementType.None;

    void Start()
    {
        if (auraManager == null) auraManager = GetComponent<ElementalAuraManager>();
        // Cria uma âncora visual para centralizar o modelo instanciado
        if (visualAnchor == null)
        {
            GameObject anchorObj = new GameObject("SlimeVisualAnchor");
            anchorObj.transform.SetParent(transform, false);
            anchorObj.transform.localPosition = Vector3.zero;
            anchorObj.transform.localRotation = Quaternion.identity;
            anchorObj.transform.localScale = Vector3.one;
            visualAnchor = anchorObj.transform;
        }
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
        if (visualAnchor != null && Camera.main != null)
        {
            Vector3 lookDir = Camera.main.transform.position - visualAnchor.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                visualAnchor.rotation = Quaternion.LookRotation(lookDir);
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
        // Reseta âncora
        if (visualAnchor != null)
        {
            visualAnchor.localPosition = Vector3.zero;
            visualAnchor.localRotation = Quaternion.identity;
            visualAnchor.localScale = Vector3.one;
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
            // Instancia como filho da âncora visual, com transform zerado
            currentSlimeModelInstance = Instantiate(prefab, visualAnchor);
            currentSlimeModelInstance.transform.localPosition = Vector3.zero;
            currentSlimeModelInstance.transform.localRotation = Quaternion.identity;
            currentSlimeModelInstance.transform.localScale = Vector3.one;

            // Centraliza o visual para que a "massa" do modelo fique no (0,0,0) do slime
            CenterModelAtAnchor();
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

    /// <summary>
    /// Centraliza o modelo recém-instanciado em relação à âncora utilizando os bounds dos Renderers.
    /// Isso corrige pivôs deslocados em prefabs, mantendo o slime sempre na mesma posição.
    /// </summary>
    private void CenterModelAtAnchor()
    {
        if (visualAnchor == null || currentSlimeModelInstance == null) return;

        var renderers = currentSlimeModelInstance.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) return;

        Bounds combined = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            combined.Encapsulate(renderers[i].bounds);
        }

        // Converte centro dos bounds (world) para espaço local da âncora
        Vector3 worldCenter = combined.center;
        Vector3 localCenter = visualAnchor.InverseTransformPoint(worldCenter);

        // Move a âncora para compensar o deslocamento do centro
        visualAnchor.localPosition -= localCenter;
    }
}


