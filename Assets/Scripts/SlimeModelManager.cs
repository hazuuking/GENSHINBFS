using UnityEngine;
using System.Collections.Generic;

public class SlimeModelManager : MonoBehaviour
{
    public ElementalAuraManager auraManager; // Referência ao ElementalAuraManager do slime

    [System.Serializable]
    public class SlimeModelEntry
    {
        public ElementType elementType;
        public GameObject slimeModelPrefab; // Prefab do modelo 3D do slime para este elemento
    }

    public List<SlimeModelEntry> slimeModels;

    private GameObject currentSlimeModelInstance;
    private ElementType lastKnownElementType = ElementType.None;
    private ElementType lastKnownStatusType = ElementType.None;

    void Start()
    {
        if (auraManager == null)
        {
            auraManager = GetComponent<ElementalAuraManager>();
            if (auraManager == null)
            {
                Debug.LogError("SlimeModelManager requer um ElementalAuraManager no mesmo GameObject ou atribuído.");
                enabled = false; // Desativa o script se não encontrar o AuraManager
                return;
            }
        }
        // Inicializa com o modelo neutro ou o modelo da aura inicial, se houver
        UpdateSlimeModel(auraManager.currentAura, auraManager.currentStatus);
    }

    void Update()
    {
        // Verifica se a aura principal ou o status mudou e atualiza o modelo
        if (auraManager.currentAura != lastKnownElementType || auraManager.currentStatus != lastKnownStatusType)
        {
            UpdateSlimeModel(auraManager.currentAura, auraManager.currentStatus);
            lastKnownElementType = auraManager.currentAura;
            lastKnownStatusType = auraManager.currentStatus;
        }

        // Opcional: Manter o slime sempre olhando para a câmera após a inicialização
        if (currentSlimeModelInstance != null && Camera.main != null)
        {
            Vector3 lookDirection = Camera.main.transform.position - currentSlimeModelInstance.transform.position;
            lookDirection.y = 0; // Ignora a rotação no eixo Y para não inclinar o slime
            if (lookDirection != Vector3.zero)
            {
                currentSlimeModelInstance.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    private void UpdateSlimeModel(ElementType newElementType, ElementType newStatusType)
    {
        // Destrói o modelo atual, se houver
        if (currentSlimeModelInstance != null)
        {
            Destroy(currentSlimeModelInstance);
            currentSlimeModelInstance = null;
        }

        // Prioriza o status para a troca de modelo (ex: Quicken Slime, Burning Slime)
        ElementType modelToDisplay = newStatusType != ElementType.None ? newStatusType : newElementType;

        // Encontra o prefab do modelo correspondente ao novo elemento/status
        GameObject modelPrefabToInstantiate = null;
        foreach (var entry in slimeModels)
        {
            if (entry.elementType == modelToDisplay)
            {
                modelPrefabToInstantiate = entry.slimeModelPrefab;
                break;
            }
        }

        // Se encontrou um prefab, instancia-o
        if (modelPrefabToInstantiate != null)
        {
            currentSlimeModelInstance = Instantiate(modelPrefabToInstantiate, transform);
            currentSlimeModelInstance.transform.localPosition = Vector3.zero;
            currentSlimeModelInstance.transform.localScale = Vector3.one; // Garante escala padrão

            // Ajusta a rotação para que o slime olhe para a câmera principal
            if (Camera.main != null)
            {
                Vector3 lookDirection = Camera.main.transform.position - currentSlimeModelInstance.transform.position;
                lookDirection.y = 0; // Ignora a rotação no eixo Y para não inclinar o slime
                if (lookDirection != Vector3.zero)
                {
                    currentSlimeModelInstance.transform.rotation = Quaternion.LookRotation(lookDirection);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Nenhum modelo de slime encontrado para o ElementType/Status: {modelToDisplay}. Usando modelo padrão (None) se disponível.");
            // Tenta instanciar o modelo 'None' se nenhum específico for encontrado
            foreach (var entry in slimeModels)
            {
                if (entry.elementType == ElementType.None)
                {
                    modelPrefabToInstantiate = entry.slimeModelPrefab;
                    break;
                }
            }
            if (modelPrefabToInstantiate != null)
            {
                currentSlimeModelInstance = Instantiate(modelPrefabToInstantiate, transform);
                currentSlimeModelInstance.transform.localPosition = Vector3.zero;
                currentSlimeModelInstance.transform.localScale = Vector3.one;

                // Ajusta a rotação para que o slime olhe para a câmera principal
                if (Camera.main != null)
                {
                    Vector3 lookDirection = Camera.main.transform.position - currentSlimeModelInstance.transform.position;
                    lookDirection.y = 0; // Ignora a rotação no eixo Y para não inclinar o slime
                    if (lookDirection != Vector3.zero)
                    {
                        currentSlimeModelInstance.transform.rotation = Quaternion.LookRotation(lookDirection);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Nenhum modelo padrão (None) encontrado para o slime.");
            }
        }
    }

    /// <summary>
    /// Método público para mudar o modelo do slime baseado no elemento selecionado
    /// </summary>
    /// <param name="selectedElement">O elemento selecionado para aplicar ao slime</param>
    public void ChangeSlimeModel(ElementType selectedElement)
    {
        // Força a atualização do modelo para o elemento selecionado
        UpdateSlimeModel(selectedElement, ElementType.None);
        lastKnownElementType = selectedElement;
        lastKnownStatusType = ElementType.None;
        
        Debug.Log($"Modelo do slime alterado para: {selectedElement}");
    }
}


