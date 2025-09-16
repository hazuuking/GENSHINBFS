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
    /// Chamado quando o mouse é clicado sobre este collider.
    /// </summary>
    void OnMouseDown()
    {
        if (GameManager.Instance != null)
        {
            // Chama o método OnElementButtonClicked do GameManager, passando o ElementType.
            GameManager.Instance.OnElementButtonClicked(elementType);
            Debug.Log($"Botão 3D de {elementType} clicado.");
        }
        else
        {
            Debug.LogWarning("GameManager.Instance não encontrado. Certifique-se de que o GameManager está na cena e configurado como Singleton.");
        }
    }
}


