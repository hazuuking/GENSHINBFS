using UnityEngine;

/// <summary>
/// Script para gerenciar a interação com botões 3D de elementos.
/// Deve ser anexado a cada GameObject que representa um botão elemental no modelo 3D da máquina.
/// </summary>
public class ElementButton3D : MonoBehaviour
{
    /// <summary>
    /// O tipo de elemento que este botão representa.
    /// Atribua no Inspector da Unity para cada botão.
    /// </summary>
    public ElementType elementType; 

    /// <summary>
    /// Referência ao GameManager na cena.
    /// Deve ser atribuído no Inspector da Unity.
    /// </summary>
    public GameManager gameManager; 

    /// <summary>
    /// Chamado quando o mouse é clicado sobre este collider.
    /// </summary>
    void OnMouseDown()
    {
        if (gameManager != null)
        {
            // Chama o método OnElementButtonClicked do GameManager, passando o índice do elemento.
            gameManager.OnElementButtonClicked((int)elementType);
            Debug.Log($"Botão 3D de {elementType} clicado.");
        }
        else
        {
            Debug.LogWarning("GameManager não atribuído ao ElementButton3D.");
        }
    }
}

