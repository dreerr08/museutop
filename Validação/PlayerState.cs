using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gerenciador Singleton que armazena o estado atual das dedu��es do jogador.
/// Ele � respons�vel por manter a lista de palpites que o jogador fez.
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
    /// Prepara a lista de dedu��es, criando uma entrada vazia para cada personagem do jogo.
    /// Deve ser chamado DEPOIS que o SolutionManager j� estiver pronto.
    /// </summary>
    private void InicializarDeducoes()
    {
        listaDeDeducoes = new List<DeducaoJogador>();

        // Pega o n�mero total de personagens do nosso "gabarito"
        int totalPersonagens = SolutionManager.Instance.GetTotalDePersonagens();

        for (int i = 0; i < totalPersonagens; i++)
        {
            // Pega o ID do personagem real da solu��o para garantir que eles correspondam
            int personagemId = SolutionManager.Instance.GetTodasAsSolucoes()[i].id;
            listaDeDeducoes.Add(new DeducaoJogador(personagemId));
        }

        Debug.Log($"PlayerState inicializado com {listaDeDeducoes.Count} dedu��es vazias.");
    }

    /// <summary>
    /// Retorna a lista completa de dedu��es do jogador.
    /// </summary>
    public List<DeducaoJogador> GetTodasAsDeducoes()
    {
        return listaDeDeducoes;
    }

    /// <summary>
    /// Encontra e retorna a dedu��o espec�fica para um personagem pelo seu ID.
    /// </summary>
    public DeducaoJogador GetDeducaoPorId(int personagemId)
    {
        return listaDeDeducoes.FirstOrDefault(d => d.idPersonagem == personagemId);
    }

    // Voc� adicionar� aqui m�todos para ATUALIZAR as dedu��es conforme o jogador interage com a UI.
    // Ex: public void AtualizarNomeDeducao(int personagemId, string novoNome) { ... }
}