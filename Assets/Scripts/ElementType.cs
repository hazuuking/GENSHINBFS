/// <summary>
/// Define os tipos de elementos disponíveis no Genshin Impact.
/// Cada membro representa um elemento elemental que pode ser aplicado a um objeto ou personagem.
/// </summary>
public enum ElementType
{
    /// <summary>
    /// Representa a ausência de um elemento. Usado para o estado inicial de um objeto ou para indicar que nenhum elemento está aplicado.
    /// </summary>
    None = 0,
    /// <summary>
    /// Elemento Pyro (Fogo).
    /// </summary>
    Pyro = 1,
    /// <summary>
    /// Elemento Hydro (Água).
    /// </summary>
    Hydro = 2,
    /// <summary>
    /// Elemento Anemo (Vento).
    /// </summary>
    Anemo = 3,
    /// <summary>
    /// Elemento Electro (Eletricidade).
    /// </summary>
    Electro = 4,
    /// <summary>
    /// Elemento Dendro (Natureza).
    /// </summary>
    Dendro = 5,
    /// <summary>
    /// Elemento Cryo (Gelo).
    /// </summary>
    Cryo = 6,
    /// <summary>
    /// Elemento Geo (Terra).
    /// </summary>
    Geo = 7,
    /// <summary>
    /// Representa o estado Quicken, que é um status elemental e não um elemento base. 
    /// Usado para lógica de reações como Aggravate e Spread.
    /// </summary>
    Quicken = 8,
    /// <summary>
    /// Representa o estado Burning, um status elemental de dano contínuo.
    /// </summary>
    Burning = 9,
    /// <summary>
    /// Representa o estado Bloom, que gera Dendro Cores.
    /// </summary>
    Bloom = 10
}


