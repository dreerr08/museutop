/// <summary>
/// Armazena o estado atual das dedu��es de um jogador para um �nico personagem.
/// </summary>
[System.Serializable]
public class DeducaoJogador
{
    /// <summary>
    /// O ID do personagem ao qual esta dedu��o se refere.
    /// </summary>
    public int idPersonagem;

    /// <summary>
    /// O nome que o jogador selecionou no di�rio.
    /// </summary>
    public string nomeEscolhido;

    /// <summary>
    /// (ALTERADO) O Sprite do retrato que o jogador associou a este personagem.
    /// </summary>
    public UnityEngine.Sprite retratoEscolhido;

    /// <summary>
    /// O papel que o jogador atribuiu a este personagem.
    /// MUDAN�A: N�o � mais Nullable. O valor padr�o � 'Desconhecido'.
    /// </summary>
    public PapelNoRoubo papelEscolhido;

    /// <summary>
    /// O destino que o jogador atribuiu a este personagem.
    /// MUDAN�A: N�o � mais Nullable. O valor padr�o � 'Desconhecido'.
    /// </summary>
    public DestinoFinal destinoEscolhido;

    /// <summary>
    /// Uma flag para saber se esta dedu��o j� foi validada como correta.
    /// </summary>
    public bool estaConfirmado;

    /// <summary>
    /// Construtor para facilitar a cria��o de uma nova dedu��o vazia no in�cio do jogo.
    /// </summary>
    public DeducaoJogador(int personagemId)
    {
        this.idPersonagem = personagemId;
        this.nomeEscolhido = null;
        this.retratoEscolhido = null;
        // MUDAN�A: Define o estado inicial como Desconhecido em vez de null.
        this.papelEscolhido = PapelNoRoubo.Desconhecido;
        this.destinoEscolhido = DestinoFinal.Desconhecido;
        this.estaConfirmado = false;
    }
}