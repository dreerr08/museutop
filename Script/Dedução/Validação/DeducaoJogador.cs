/// <summary>
/// Armazena o estado atual das dedu��es de um jogador para um �nico personagem.
/// </summary>
[System.Serializable] // Serializable para que voc� possa ver a lista de dedu��es no Inspector
public class DeducaoJogador
{
    /// <summary>
    /// O ID do personagem ao qual esta dedu��o se refere.
    /// � assim que ligamos o palpite � solu��o correta.
    /// </summary>
    public int idPersonagem;

    /// <summary>
    /// O nome que o jogador selecionou no di�rio.
    /// </summary>
    public string nomeEscolhido;

    /// <summary>
    /// O ID da foto que o jogador associou a este personagem.
    /// </summary>
    public string fotoEscolhida;

    // A interroga��o (?) depois do tipo do enum o torna "nullable" (anul�vel).
    // Isso � crucial porque, no in�cio, o jogador ainda n�o fez uma escolha.
    // Um enum normal n�o pode ser "vazio" ou "nulo", mas um nullable pode.
    // Assim, podemos saber se o jogador j� selecionou um papel ou n�o.

    /// <summary>
    /// O papel que o jogador atribuiu a este personagem.
    /// </summary>
    public PapelNoRoubo? papelEscolhido;

    /// <summary>
    /// O destino que o jogador atribuiu a este personagem.
    /// </summary>
    public DestinoFinal? destinoEscolhido;

    /// <summary>
    /// Uma flag para saber se esta dedu��o j� foi validada como correta.
    /// Quando for true, o jogador n�o poder� mais alterar os campos no di�rio.
    /// </summary>
    public bool estaConfirmado;

    /// <summary>
    /// Construtor para facilitar a cria��o de uma nova dedu��o vazia no in�cio do jogo.
    /// </summary>
    /// <param name="personagemId">O ID do personagem para o qual esta dedu��o ser� feita.</param>
    public DeducaoJogador(int personagemId)
    {
        this.idPersonagem = personagemId;
        this.nomeEscolhido = null;
        this.fotoEscolhida = null;
        this.papelEscolhido = null; // Come�a como nulo (sem escolha)
        this.destinoEscolhido = null; // Come�a como nulo (sem escolha)
        this.estaConfirmado = false;
    }
}