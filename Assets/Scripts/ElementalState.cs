using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Representa o estado elemental de um objeto, ou seja, quais elementos estão atualmente aplicados a ele.
/// Um objeto pode ter múltiplos elementos aplicados simultaneamente.
/// </summary>
public class ElementalState
{
    /// <summary>
    /// Um conjunto de elementos que estão atualmente aplicados a este estado.
    /// Usamos um HashSet para garantir que cada elemento seja único e para operações eficientes de adição/verificação.
    /// </summary>
    public HashSet<ElementType> AppliedElements { get; private set; }

    /// <summary>
    /// Construtor para criar uma nova instância de ElementalState.
    /// </summary>
    /// <param name="elements">Uma matriz de ElementType a serem aplicados inicialmente a este estado.</param>
    public ElementalState(params ElementType[] elements)
    {
        // Inicializa o HashSet com os elementos fornecidos.
        AppliedElements = new HashSet<ElementType>(elements);
    }

    /// <summary>
    /// Sobrescreve o método Equals para comparar dois objetos ElementalState.
    /// Dois estados são considerados iguais se contiverem o mesmo conjunto de elementos, independentemente da ordem.
    /// </summary>
    /// <param name="obj">O objeto a ser comparado com a instância atual.</param>
    /// <returns>True se os objetos ElementalState forem iguais; caso contrário, False.</returns>
    public override bool Equals(object obj)
    {
        // Verifica se o objeto é nulo ou não é do tipo ElementalState.
        if (obj is ElementalState other)
        {
            // Compara os conjuntos de elementos usando SetEquals, que verifica se os conjuntos contêm os mesmos elementos.
            return AppliedElements.SetEquals(other.AppliedElements);
        }
        return false;
    }

    /// <summary>
    /// Sobrescreve o método GetHashCode para gerar um código hash para o objeto ElementalState.
    /// Isso é crucial para que ElementalState possa ser usado corretamente em coleções baseadas em hash (como Dictionary ou HashSet).
    /// O hash é gerado de forma consistente, independentemente da ordem dos elementos, garantindo que estados iguais tenham o mesmo hash.
    /// </summary>
    /// <returns>Um código hash para a instância atual de ElementalState.</returns>
    public override int GetHashCode()
    {
        int hash = 0;
        // Ordena os elementos para garantir que o hash seja consistente, independentemente da ordem de adição.
        foreach (var element in AppliedElements.OrderBy(e => e.ToString()))
        {
            // Combina os códigos hash dos elementos para formar o hash final do estado.
            hash = hash * 31 + element.GetHashCode();
        }
        return hash;
    }
}

