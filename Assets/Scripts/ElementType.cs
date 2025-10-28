/// <summary>
/// Define os tipos de elementos fundamentais (Element Type) que compõem o sistema de reações elementais,
/// em conformidade com o modelo do jogo Genshin Impact.
/// Este enum serve como a base para todas as operações de lógica elemental e de grafo.
/// </summary>
public enum ElementType
{
    /// <summary>
    /// Representa a ausência de um elemento. É o estado inicial de qualquer objeto ou
    /// o elemento de "vazio" necessário para certas verificações lógicas.
    /// </summary>
    None = 0,
    /// <summary>
    /// Elemento Pyro (Fogo). Base para reações como Sobrecarga e Fusão.
    /// </summary>
    Pyro = 1,
    /// <summary>
    /// Elemento Hydro (Água). Base para reações como Eletrificação e Vaporização.
    /// </summary>
    Hydro = 2,
    /// <summary>
    /// Elemento Anemo (Vento). Elemento de suporte, primariamente envolvido na reação de Redemoinho.
    /// </summary>
    Anemo = 3,
    /// <summary>
    /// Elemento Electro (Eletricidade). Base para reações como Sobrecarga e Eletrificação.
    /// </summary>
    Electro = 4,
    /// <summary>
    /// Elemento Dendro (Natureza). Elemento central para as reações de Catalisação.
    /// </summary>
    Dendro = 5,
    /// <summary>
    /// Elemento Cryo (Gelo). Base para reações como Fusão e Supercondução.
    /// </summary>
    Cryo = 6,
    /// <summary>
    /// Elemento Geo (Terra). Elemento de suporte, primariamente envolvido na reação de Cristalização.
    /// </summary>
    Geo = 7,
    /// <summary>
    /// Estado Quicken (Catalisação). Não é um elemento, mas um estado intermediário
    /// necessário para a lógica de reações de segundo nível (Aggravate/Spread).
    /// </summary>
    Quicken = 8
}