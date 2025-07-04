// Usar enums é uma prática excelente para evitar erros de digitação.
// Em vez de escrever "O Cérebro" como uma string (onde você pode errar),
// você usará PapelNoRoubo.Cerebro, e o compilador garantirá que está correto.
// Além disso, eles são perfeitos para popular menus suspensos (dropdowns) na UI.

/// <summary>
/// Define os papéis que cada personagem pode ter tido no roubo.
/// </summary>
public enum PapelNoRoubo
{
    // NOVO: Valor padrão para quando nada foi selecionado.
    Desconhecido,

    // Adicione aqui todos os papéis possíveis no seu jogo.
    Cerebro,
    Musculo,
    Infiltrado,
    Falsificadora,
    Motorista,
    Cliente,
    GuardaCorrupto,
    Distracao,
    Gay
}

/// <summary>
/// Define os possíveis destinos finais de cada personagem.
/// </summary>
public enum DestinoFinal
{
    // NOVO: Valor padrão para quando nada foi selecionado.
    Desconhecido,

    // Adicione aqui todos os destinos possíveis.
    Preso,
    FugiuComDinheiro,
    MortoPorCumplice,
    MortoPelaPolicia,
    TraidoEAbandonado,
    Desaparecido,
    AcidenteNaFuga,
    RecebeuUmaFalsificacao
}