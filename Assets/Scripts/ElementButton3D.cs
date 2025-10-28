using UnityEngine;

/// <summary>
/// Script responsável pela interação do usuário com os botões 3D que aplicam elementos.
/// Este componente atua como a **interface de entrada de dados** para o sistema,
/// permitindo que o usuário selecione o "nó" (elemento) inicial no grafo de reações.
/// </summary>
public class ElementButton3D : MonoBehaviour
{
    // --- VARIÁVEIS PÚBLICAS (CONFIGURÁVEIS NO INSPECTOR) ---

    /// <summary>
    /// <c>[Tooltip]</c> Define qual elemento este botão específico representa.
    /// Deve ser configurado no Inspector do Unity para cada um dos 7 botões.
    /// </summary>
    [Tooltip("O tipo de elemento que este botão representa.")]
    public ElementType elementType;

    /// <summary>
    /// <c>[Tooltip]</c> Referência direta ao script principal <c>GameManager</c>.
    /// O uso de uma referência direta (em vez de um Singleton) aumenta a clareza das dependências
    /// e a facilidade de configuração no ambiente Unity.
    /// </summary>
    [Tooltip("Referência ao GameManager na cena. Arraste o GameManager da hierarquia para este campo.")]
    public GameManager gameManager;

    // --- MÉTODOS DO UNITY ---

    /// <summary>
    /// Método nativo do Unity chamado quando o botão esquerdo do mouse é pressionado
    /// sobre o Collider deste objeto.
    /// Requer um <c>Collider</c> no objeto e um <c>Physics Raycaster</c> na câmera principal.
    /// </summary>
    void OnMouseDown()
    {
        // 1. Verifica se a referência ao GameManager foi corretamente atribuída.
        if (gameManager != null)
        {
            // 2. Chama o método de manipulação de evento no GameManager, passando o elemento selecionado.
            // A conversão para (int) é usada para garantir que o enum seja passado corretamente, embora
            // a sobrecarga de OnElementButtonClicked aceite diretamente ElementType na implementação ideal.
            gameManager.OnElementButtonClicked(elementType);
            
            // Log para fins de depuração e rastreamento de eventos.
            Debug.Log($"[ElementButton3D] Botão {elementType} clicado. Elemento enviado ao GameManager para aplicação no Slime.");
        }
        else
        {
            // Alerta de erro para indicar falha na configuração do Inspector.
            Debug.LogError($"[ElementButton3D] A referência ao GameManager não foi atribuída no Inspector para o botão {this.gameObject.name}! A funcionalidade de clique está desabilitada.");
        }
    }
}