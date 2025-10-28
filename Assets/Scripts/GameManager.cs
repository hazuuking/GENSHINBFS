using UnityEngine;


/// <summary>
/// <c>GameManager</c>: O Orquestrador Principal do Sistema de Simulação de Reações Elementais.
/// Este script atua como o controlador de estado (State Machine) do fluxo do jogo,
/// integrando a entrada do usuário (botões), a detecção de máquina (triggers) e a
/// lógica de reação elemental baseada em teoria dos grafos (BFS/DFS simulada).
/// </summary>
public class GameManager : MonoBehaviour
{
    // --- VARIÁVEIS PÚBLICAS (CONFIGURÁVEIS NO INSPECTOR) ---

    /// <summary>
    /// <c>[Header("Configuração do Alvo")]</c> Referência ao <c>GameObject</c> do Slime.
    /// Este é o nó central do sistema, onde os elementos são aplicados e as reações ocorrem.
    /// </summary>
    [Header("Configuração do Alvo")]
    [Tooltip("O GameObject do slime que será controlado. Arraste o slime da sua cena para este campo.")]
    public GameObject targetSlimeObject;

    // --- REFERÊNCIAS INTERNAS (PRIVADAS) ---

    /// <summary>
    /// Referência ao script <c>ElementalAuraManager</c> do Slime.
    /// Responsável por gerenciar o estado elemental e os efeitos visuais (VFX).
    /// </summary>
    private ElementalAuraManager targetAuraManager;

    /// <summary>
    /// Referência ao componente de física (<c>Rigidbody</c>) do Slime.
    /// Usado para controlar o movimento e forçar a parada do objeto na esteira.
    /// </summary>
    private Rigidbody slimeRigidbody;

    // --- CONTROLE DE ESTADO ---

    /// <summary>
    /// Propriedade de controle de movimento.
    /// Esta flag implementa o estado de máquina para o movimento do Slime:
    /// <c>true</c>: O Slime pode ser movido pela esteira (<c>ConveyorBeltController</c>).
    /// <c>false</c>: O Slime está parado, geralmente esperando input (Máquina 1) ou após a conclusão da simulação (Máquina 2).
    /// </summary>
    public bool canSlimeMove { get; private set; } = true;

    // --- MÉTODOS DO UNITY ---

    /// <summary>
    /// <c>Start()</c>: Chamado na inicialização.
    /// Realiza a validação das referências e a inicialização do estado do Slime.
    /// </summary>
    void Start()
    {
        // 1. Validação de Configuração:
        if (targetSlimeObject == null)
        {
            Debug.LogError("[GameManager] O 'Target Slime Object' não foi atribuído no Inspector! O sistema não pode funcionar.");
            this.enabled = false;
            return;
        }

        // 2. Obtenção de Componentes Essenciais:
        // Nota: O nome do script de aura foi corrigido para AuraManager, conforme o arquivo encontrado.
        targetAuraManager = targetSlimeObject.GetComponent<ElementalAuraManager>();
        if (targetAuraManager == null)
        {
            Debug.LogError($"[GameManager] O objeto {targetSlimeObject.name} não possui o componente 'ElementalAuraManager'.");
            this.enabled = false;
            return;
        }

        slimeRigidbody = targetSlimeObject.GetComponent<Rigidbody>();
        if (slimeRigidbody == null)
        {
            Debug.LogError($"[GameManager] O objeto {targetSlimeObject.name} não possui um componente 'Rigidbody'.");
            this.enabled = false;
            return;
        }

        // 3. Estado Inicial:
        targetAuraManager.SetAura(ElementType.None); // Garante que comece sem aura visual.
        ResumeSlimeMovement(); // Permite o movimento inicial na esteira.
    }

    // --- MÉTODOS PÚBLICOS DE MANIPULAÇÃO DE EVENTOS ---

    /// <summary>
    /// Manipulador de Evento: Chamado pelo <c>ElementButton3D</c> após o clique do usuário na Máquina 1.
    /// Este método aplica o elemento selecionado ao Slime e retoma o movimento.
    /// </summary>
    /// <param name="selectedElement">O <c>ElementType</c> selecionado pelo usuário.</param>
    public void OnElementButtonClicked(ElementType selectedElement)
    {
        if (targetAuraManager == null) return;

        // Aplica o elemento. Na Máquina 1, o elemento é aplicado e substitui o anterior.
        targetAuraManager.SetAura(selectedElement);

        // Após a interação do usuário, o Slime é liberado para seguir para a Máquina 2.
        ResumeSlimeMovement();
        Debug.Log($"[GameManager] Elemento {selectedElement} aplicado. Slime liberado para se mover da Máquina 1.");
    }

    /// <summary>
    /// Manipulador de Evento: Chamado pelo <c>MachineTrigger</c> quando o Slime entra em uma das máquinas.
    /// </summary>
    /// <param name="machineID">O ID da máquina (1 ou 2) em que o Slime entrou.</param>
    public void OnMachineEnter(int machineID)
    {
        if (machineID == 1)
        {
            // Máquina 1: Parada para Espera de Input.
            Debug.Log("[GameManager] Slime entrou na Máquina 1. Movimento parado, aguardando seleção de elemento.");
            StopSlimeMovement();
        }
        else if (machineID == 2)
        {
            // Máquina 2: Parada Permanente e Execução da Lógica Ótima.
            Debug.Log("[GameManager] Slime entrou na Máquina 2. Movimento parado permanentemente. Acionando lógica de reação ótima.");
            StopSlimeMovement();
            TriggerOptimalReaction();
        }
    }

    // --- LÓGICA DE GRAFO E BUSCA ÓTIMA (BFS/DFS SIMULADA) ---

    /// <summary>
    /// Executa a lógica de busca para encontrar a **Reação Elemental Ótima** (Best-First Search simulada).
    /// O objetivo é encontrar o <c>incoming</c> elemento que, ao reagir com o <c>current</c> elemento
    /// do Slime, resulta na <c>ReactionType</c> com a maior pontuação (<c>ReactionEvaluator</c>).
    /// </summary>
    private void TriggerOptimalReaction()
    {
        if (targetAuraManager == null) return;

        // O elemento atual do Slime é o "nó de partida" no grafo.
        ElementType currentElement = targetAuraManager.currentAura;

        if (currentElement != ElementType.None)
        {
            ElementType bestIncomingElement = ElementType.None;
            ReactionEvaluator.ReactionType bestReaction = ReactionEvaluator.ReactionType.None;
            float maxEvaluation = -1.0f; // Inicializa com um valor baixo para garantir a primeira atualização.

            // Simulação da Busca: Itera sobre todos os 7 elementos possíveis (nós de destino).
            // Isso simula uma busca em largura (BFS) de profundidade 1 no grafo de reações.
            // O laço vai de 1 (Pyro) até 7 (Geo), ignorando o ElementType.None (0).
            for (int i = 1; i <= (int)ElementType.Geo; i++)
            {
                ElementType potentialIncomingElement = (ElementType)i;
                
                // 1. Determinação da Aresta: Usa a lógica do grafo para encontrar a reação (aresta).
                ReactionEvaluator.ReactionType potentialReaction = ElementalReactionLogic.GetReaction(
                    currentElement, 
                    potentialIncomingElement, 
                    targetAuraManager.currentStatus // Passa o status para reações de segundo nível (Aggravate/Spread)
                );
                
                // 2. Avaliação do Custo/Heurística: Usa a função de avaliação para obter o score (custo/peso da aresta).
                float currentEvaluation = ReactionEvaluator.EvaluateReaction(potentialReaction);

                // 3. Seleção Ótima: Atualiza o "melhor caminho" (melhor reação) encontrado até agora.
                if (currentEvaluation > maxEvaluation)
                {
                    maxEvaluation = currentEvaluation;
                    bestReaction = potentialReaction;
                    bestIncomingElement = potentialIncomingElement;
                }
            }

            Debug.Log($"[GameManager] Lógica de Busca Ótima Concluída. Slime com {currentElement}. Melhor elemento para reagir: {bestIncomingElement}. Reação: {bestReaction} (Avaliação: {maxEvaluation})");

            // Execução da Ação Ótima: Aplica o elemento que resultou na melhor reação.
            targetAuraManager.SetAura(bestIncomingElement);
        }
        else
        {
            Debug.Log("[GameManager] Slime chegou na Máquina 2 sem elemento. Nenhuma reação para acionar.");
        }
    }

    // --- MÉTODOS DE CONTROLE DE MOVIMENTO ---

    /// <summary>
    /// <c>StopSlimeMovement()</c>: Interrompe o movimento do Slime.
    /// </summary>
    private void StopSlimeMovement()
    {
        canSlimeMove = false;
        if (slimeRigidbody != null)
        {
            // Zera as velocidades para garantir que o Slime pare imediatamente.
            slimeRigidbody.velocity = Vector3.zero;
            slimeRigidbody.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// <c>ResumeSlimeMovement()</c>: Libera o Slime para ser movido pela esteira.
    /// </summary>
    private void ResumeSlimeMovement()
    {
        canSlimeMove = true;
    }
}