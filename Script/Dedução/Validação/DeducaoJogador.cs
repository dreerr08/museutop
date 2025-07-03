/// <summary>
/// Armazena o estado atual das deduções de um jogador para um único personagem.
/// </summary>
[System.Serializable] // Serializable para que você possa ver a lista de deduções no Inspector
public class DeducaoJogador
{
    /// <summary>
    /// O ID do personagem ao qual esta dedução se refere.
    /// </summary>
    public int idPersonagem;

    /// <summary>
    /// O nome que o jogador selecionou no diário.
    /// </summary>
    public string nomeEscolhido;

    /// <summary>
    /// (ALTERADO) O Sprite do retrato que o jogador associou a este personagem.
    /// </summary>
    public UnityEngine.Sprite retratoEscolhido;

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
    /// </summary>
    public bool estaConfirmado;

    /// <summary>
    /// Construtor para facilitar a criação de uma nova dedução vazia no início do jogo.
    /// </summary>
    public DeducaoJogador(int personagemId)
    {
        this.idPersonagem = personagemId;
        this.nomeEscolhido = null;
        this.retratoEscolhido = null; // Começa como nulo
        this.papelEscolhido = null;
        this.destinoEscolhido = null;
        this.estaConfirmado = false;
    }
}