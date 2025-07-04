using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public long ultimaAtualizacao;
    public List<DeducaoJogador> listaDeDeducoes;
    public List<string> lembrancasConcluidas;
    public List<int> perfisDesbloqueadosNoDiario;

    public GameData()
    {
        this.listaDeDeducoes = new List<DeducaoJogador>();
        this.lembrancasConcluidas = new List<string>();
        this.perfisDesbloqueadosNoDiario = new List<int>();
    }
}