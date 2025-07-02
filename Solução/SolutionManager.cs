using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Essencial para usar funções como .FirstOrDefault() e .Select()

/// <summary>
/// Gerenciador Singleton que detém a "Matriz de Soluções" - a verdade absoluta do jogo.
/// Ele armazena as informações corretas para cada personagem e fornece métodos
/// para que outros sistemas possam consultar esses dados de forma segura.
/// </summary>
public class SolutionManager : MonoBehaviour
{
    #region Singleton Pattern

    // A propriedade estática 'Instance' é o coração do padrão Singleton.
    // 'public get' permite que qualquer script leia esta instância.
    // 'private set' garante que apenas este script possa definir quem é a instância.
    public static SolutionManager Instance { get; private set; }

    // O método Awake() é chamado pela Unity antes de qualquer método Start().
    // É o lugar ideal para configurar o Singleton.
    private void Awake()
    {
        // VERIFICAÇÃO 1: Já existe uma instância?
        if (Instance != null && Instance != this)
        {
            // Se sim, e não for esta, significa que outra instância foi criada por engano.
            // Devemos destruir este objeto duplicado para evitar conflitos.
            Debug.LogWarning("Já existe um SolutionManager na cena. Destruindo a nova instância.");
            Destroy(this.gameObject);
        }
        else
        {
            // VERIFICAÇÃO 2: A instância ainda não foi definida.
            // Se não, esta se torna a instância única.
            Instance = this;

            // (Opcional, mas recomendado) Mantém este objeto vivo ao carregar novas cenas.
            // Essencial se o seu jogo tiver múltiplas cenas (ex: menu, jogo, créditos).
            DontDestroyOnLoad(this.gameObject);
        }
    }

    #endregion

    #region Solution Data

    // Usamos [SerializeField] em um campo privado.
    // Isso expõe a variável ao Inspector da Unity para que possamos preenchê-la
    // arrastando e soltando, mas impede que outros scripts a modifiquem diretamente,
    // forçando-os a usar os métodos públicos que criamos (boa prática de encapsulamento).
    [SerializeField]
    private List<SolucaoPersonagem> matrizDeSolucoes;

    #endregion

    #region Public Access Methods

    /// <summary>
    /// Busca e retorna a solução completa para um personagem específico pelo seu ID.
    /// Este é o método de consulta mais importante.
    /// </summary>
    /// <param name="id">O ID do personagem a ser procurado.</param>
    /// <returns>O objeto SolucaoPersonagem se encontrado; caso contrário, retorna null.</returns>
    public SolucaoPersonagem GetSolucaoPorId(int id)
    {
        // .FirstOrDefault() é um método do LINQ que é perfeito para isso.
        // Ele varre a lista e retorna o primeiro elemento que corresponde à condição.
        // Se nenhum elemento for encontrado, ele retorna o valor padrão (null para classes).
        return matrizDeSolucoes.FirstOrDefault(p => p.id == id);
    }

    /// <summary>
    /// Retorna uma lista com todas as soluções de personagens.
    /// </summary>
    /// <returns>A lista completa de soluções.</returns>
    public List<SolucaoPersonagem> GetTodasAsSolucoes()
    {
        return matrizDeSolucoes;
    }

    /// <summary>
    /// Retorna uma lista contendo apenas os nomes de todos os personagens.
    /// Extremamente útil para popular a UI do diário (menus suspensos de nomes).
    /// </summary>
    /// <returns>Uma lista de strings com os nomes completos.</returns>
    public List<string> GetTodosOsNomes()
    {
        // .Select() é um método do LINQ que projeta cada elemento de uma sequência em um novo formato.
        // Aqui, estamos transformando uma List<SolucaoPersonagem> em uma List<string>.
        return matrizDeSolucoes.Select(p => p.nomeCompleto).ToList();
    }

    /// <summary>
    /// Retorna o número total de personagens no jogo.
    /// </summary>
    public int GetTotalDePersonagens()
    {
        return matrizDeSolucoes.Count;
    }

    #endregion
}