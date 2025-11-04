/// <summary>
/// Define os tipos de reações elementais que podem ocorrer no Genshin Impact.
/// </summary>
public enum ReactionType
{
    /// <summary>
    /// Nenhuma reação ocorreu.
    /// </summary>
    None,
    /// <summary>
    /// Reação de Sobrecarga (Pyro + Electro).
    /// </summary>
    Overload,
    /// <summary>
    /// Reação de Estilhaçar (Cryo + Dano Físico).
    /// </summary>
    Shatter,
    /// <summary>
    /// Reação de Redemoinho (Anemo + Pyro/Hydro/Electro/Cryo).
    /// </summary>
    Swirl,
    /// <summary>
    /// Reação de Supercondutor (Cryo + Electro).
    /// </summary>
    Superconduct,
    /// <summary>
    /// Reação de Eletricamente Carregado (Hydro + Electro).
    /// </summary>
    ElectroCharged,
    /// <summary>
    /// Reação de Fusão (Pyro + Cryo ou Cryo + Pyro).
    /// </summary>
    Melt,
    /// <summary>
    /// Reação de Vaporização (Hydro + Pyro ou Pyro + Hydro).
    /// </summary>
    Vaporize,
    /// <summary>
    /// Reação de Congelar (Hydro + Cryo).
    /// </summary>
    Freeze,
    /// <summary>
    /// Reação de Cristalização (Geo + Pyro/Hydro/Electro/Cryo).
    /// </summary>
    Crystallize,
    /// <summary>
    /// Reação de Queimadura (Dendro + Pyro).
    /// </summary>
    Burning,
    /// <summary>
    /// Reação de Florescimento (Dendro + Hydro).
    /// </summary>
    Bloom,
    /// <summary>
    /// Reação de Aceleração (Dendro + Electro).
    /// </summary>
    Quicken,
    /// <summary>
    /// Reação de Intensificação (Quicken + Electro).
    /// </summary>
    Aggravate,
    /// <summary>
    /// Reação de Propagação (Quicken + Dendro).
    /// </summary>
    Spread
}