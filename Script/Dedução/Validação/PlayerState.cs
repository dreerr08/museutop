using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gerenciador Singleton que armazena o estado atual das deduções do jogador.
/// Ele é responsável por manter a lista de palpites que o jogador fez.
/// </summary>
public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    private List<DeducaoJogador> listaDeDeducoes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            InicializarDeducoes();
        }
    }

    /// <summary>
    /// Prepara a lista de deduções, criando uma entrada vazia para cada personagem do jogo.
    /// Deve ser chamado DEPOIS que o SolutionManager já estiver pronto.
    /// </summary>
    private void InicializarDeducoes()
    {
        listaDeDeducoes = new List<DeducaoJogador>();

        // Pega o número total de personagens do nosso "gabarito"
        int totalPersonagens = SolutionManager.Instance.GetTotalDePersonagens();

        for (int i = 0; i < totalPersonagens; i++)
        {
            // Pega o ID do personagem real da solução para garantir que eles correspondam
            int personagemId = SolutionManager.Instance.GetTodasAsSolucoes()[i].id;
            listaDeDeducoes.Add(new DeducaoJogador(personagemId));
        }

        Debug.Log($"PlayerState inicializado com {listaDeDeducoes.Count} deduções vazias.");
    }

    /// <summary>
    /// Retorna a lista completa de deduções do jogador.
    /// </summary>
    public List<DeducaoJogador> GetTodasAsDeducoes()
    {
        return listaDeDeducoes;
    }

    /// <summary>
    /// Encontra e retorna a dedução específica para um personagem pelo seu ID.
    /// </summary>
    public DeducaoJogador GetDeducaoPorId(int personagemId)
    {
        return listaDeDeducoes.FirstOrDefault(d => d.idPersonagem == personagemId);
    }

    // Você adicionará aqui métodos para ATUALIZAR as deduções conforme o jogador interage com a UI.
    // Ex: public void AtualizarNomeDeducao(int personagemId, string novoNome) { ... }
}