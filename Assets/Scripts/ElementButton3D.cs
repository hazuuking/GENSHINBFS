using UnityEngine;

/// <summary>
/// Script para botões 3D que interagem com o GameManager para aplicar elementos.
/// </summary>
public class ElementButton3D : MonoBehaviour
{
    [Tooltip("O tipo de elemento que este botão representa.")]
    public ElementType elementType;

    [Tooltip("Referência ao GameManager na cena. Arraste o GameManager da hierarquia para este campo.")]
    public GameManager gameManager;

    void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.OnElementButtonClicked((int)elementType);
            Debug.Log($"Botão {elementType} clicado e chamada enviada ao GameManager.");
        }
        else
        {
            Debug.LogError($"[ElementButton3D] A referência ao GameManager não foi atribuída no Inspector para o botão {this.gameObject.name}!");
        }
    }
}