using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Essencial para usar fun��es como .FirstOrDefault() e .Select()

/// <summary>
/// Gerenciador Singleton que det�m a "Matriz de Solu��es" - a verdade absoluta do jogo.
/// Ele armazena as informa��es corretas para cada personagem e fornece m�todos
/// para que outros sistemas possam consultar esses dados de forma segura.
/// </summary>
public class SolutionManager : MonoBehaviour
{
    #region Singleton Pattern
    public static SolutionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("J� existe um SolutionManager na cena. Destruindo a nova inst�ncia.");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    #region Solution Data
    [SerializeField]
    private List<SolucaoPersonagem> matrizDeSolucoes;
    #endregion

    #region Public Access Methods

    /// <summary>
    /// Busca e retorna a solu��o completa para um personagem espec�fico pelo seu ID.
    /// </summary>
    public SolucaoPersonagem GetSolucaoPorId(int id)
    {
        return matrizDeSolucoes.FirstOrDefault(p => p.id == id);
    }

    /// <summary>
    /// Retorna uma lista com todas as solu��es de personagens.
    /// </summary>
    public List<SolucaoPersonagem> GetTodasAsSolucoes()
    {
        return matrizDeSolucoes;
    }

    /// <summary>
    /// Retorna uma lista contendo apenas os nomes de todos os personagens.
    /// </summary>
    public List<string> GetTodosOsNomes()
    {
        return matrizDeSolucoes.Select(p => p.nomeCompleto).ToList();
    }

    /// <summary>
    /// (NOVO) Retorna uma lista contendo apenas os IDs de foto de todos os personagens.
    /// �til para popular a UI do di�rio (menus suspensos de fotos).
    /// </summary>
    /// <returns>Uma lista de strings com os IDs das fotos.</returns>
    public List<string> GetTodosOsIdsDeFoto()
    {
        // .Select() transforma a nossa lista de Solucoes em uma nova lista contendo apenas os IDs das fotos.
        return matrizDeSolucoes.Select(p => p.idDaFoto).ToList();
    }

    /// <summary>
    /// Retorna o n�mero total de personagens no jogo.
    /// </summary>
    public int GetTotalDePersonagens()
    {
        return matrizDeSolucoes.Count;
    }
    #endregion
}