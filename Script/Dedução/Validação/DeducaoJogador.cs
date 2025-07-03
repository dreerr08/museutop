/// <summary>
/// Armazena o estado atual das deduções de um jogador para um único personagem.
/// </summary>
[System.Serializable] // Serializable para que você possa ver a lista de deduções no Inspector
public class DeducaoJogador
{
    /// <summary>
    /// O ID do personagem ao qual esta dedução se refere.
    /// É assim que ligamos o palpite à solução correta.
    /// </summary>
    public int idPersonagem;

    /// <summary>
    /// O nome que o jogador selecionou no diário.
    /// </summary>
    public string nomeEscolhido;

    /// <summary>
    /// O ID da foto que o jogador associou a este personagem.
    /// </summary>
    public string fotoEscolhida;

    // A interrogação (?) depois do tipo do enum o torna "nullable" (anulável).
    // Isso é crucial porque, no início, o jogador ainda não fez uma escolha.
    // Um enum normal não pode ser "vazio" ou "nulo", mas um nullable pode.
    // Assim, podemos saber se o jogador já selecionou um papel ou não.

    /// <summary>
    /// O papel que o jogador atribuiu a este personagem.
    /// </summary>
    public PapelNoRoubo? papelEscolhido;

    /// <summary>
    /// O destino que o jogador atribuiu a este personagem.
    /// </summary>
    public DestinoFinal? destinoEscolhido;

    /// <summary>
    /// Uma flag para saber se esta dedução já foi validada como correta.
    /// Quando for true, o jogador não poderá mais alterar os campos no diário.
    /// </summary>
    public bool estaConfirmado;

    /// <summary>
    /// Construtor para facilitar a criação de uma nova dedução vazia no início do jogo.
    /// </summary>
    /// <param name="personagemId">O ID do personagem para o qual esta dedução será feita.</param>
    public DeducaoJogador(int personagemId)
    {
        this.idPersonagem = personagemId;
        this.nomeEscolhido = null;
        this.fotoEscolhida = null;
        this.papelEscolhido = null; // Começa como nulo (sem escolha)
        this.destinoEscolhido = null; // Começa como nulo (sem escolha)
        this.estaConfirmado = false;
    }
}