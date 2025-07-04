// Usar enums � uma pr�tica excelente para evitar erros de digita��o.
// Em vez de escrever "O C�rebro" como uma string (onde voc� pode errar),
// voc� usar� PapelNoRoubo.Cerebro, e o compilador garantir� que est� correto.
// Al�m disso, eles s�o perfeitos para popular menus suspensos (dropdowns) na UI.

/// <summary>
/// Define os pap�is que cada personagem pode ter tido no roubo.
/// </summary>
public enum PapelNoRoubo
{
    // NOVO: Valor padr�o para quando nada foi selecionado.
    Desconhecido,

    // Adicione aqui todos os pap�is poss�veis no seu jogo.
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
/// Define os poss�veis destinos finais de cada personagem.
/// </summary>
public enum DestinoFinal
{
    // NOVO: Valor padr�o para quando nada foi selecionado.
    Desconhecido,

    // Adicione aqui todos os destinos poss�veis.
    Preso,
    FugiuComDinheiro,
    MortoPorCumplice,
    MortoPelaPolicia,
    TraidoEAbandonado,
    Desaparecido,
    AcidenteNaFuga,
    RecebeuUmaFalsificacao
}