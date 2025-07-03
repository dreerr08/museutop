using UnityEngine;
using UnityEngine.SceneManagement; // Para gerenciar eventos de cena
using System.Collections;         // Para usar Coroutines (como o timer da mensagem)
using System.Collections.Generic; // Para usar List
using TMPro;                      // Para controlar o componente TextMeshPro - Text

/// <summary>
/// Gerencia toda a interface do usuário, incluindo a exibição do diário,
/// a população dinâmica das entradas e o feedback visual de confirmação.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance { get; private set; }
    #endregion

    #region Variáveis de Referência

    [Header("Referências do Inspector")]
    [Tooltip("O prefab da entrada de um personagem para a lista.")]
    [SerializeField] private CharacterEntryUI characterEntryPrefab;

    [Tooltip("O componente de texto que mostrará a mensagem de confirmação (Ex: 'Você elucidou 3 destinos').")]
    [SerializeField] private TextMeshProUGUI confirmationText;

    // As referências abaixo são encontradas dinamicamente a cada cena
    private GameObject journalPanel;
    private Transform entriesContainer;

    #endregion

    #region Variáveis de Estado
    private bool isJournalOpen = false;
    #endregion

    #region Métodos do Ciclo de Vida da Unity

    private void Awake()
    {
        // Configuração do padrão Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // OnEnable é chamado quando o objeto é ativado.
    // É o lugar perfeito para se inscrever (ouvir) eventos.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Se inscreve para ouvir o evento de sucesso do ValidationSystem
        ValidationSystem.OnDeductionsConfirmed += HandleDeductionsConfirmed;
    }

    // OnDisable é chamado quando o objeto é desativado.
    // É crucial se desinscrever para evitar erros e vazamentos de memória.
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // Se desinscreve do evento
        ValidationSystem.OnDeductionsConfirmed -= HandleDeductionsConfirmed;
    }

    private void Update()
    {
        // Verifica se a tecla TAB foi pressionada para abrir/fechar o diário
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleJournal();
        }
    }

    #endregion

    #region Lógica da UI

    /// <summary>
    /// Este método é chamado automaticamente toda vez que uma nova cena termina de carregar.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("UIManager: Cena carregada. Procurando referências da UI...");
        FindAndAssignUIReferences();
        // Garante que a mensagem de confirmação comece desativada
        if (confirmationText != null)
        {
            confirmationText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Procura pelos objetos da UI na cena atual e atribui as variáveis.
    /// </summary>
    private void FindAndAssignUIReferences()
    {
        // Limpa referências antigas para garantir que não estamos usando objetos de cenas passadas
        journalPanel = null;
        entriesContainer = null;

        GameObject panelObject = GameObject.FindWithTag("JournalPanel");
        if (panelObject != null)
        {
            journalPanel = panelObject;

            // --- MUDANÇA PRINCIPAL AQUI ---
            // O caminho para encontrar o container agora é mais específico.
            entriesContainer = journalPanel.transform.Find("Scroll View/Viewport/Content");

            if (entriesContainer == null)
            {
                Debug.LogError("UIManager: O objeto 'Content' do Scroll View não foi encontrado! Verifique a hierarquia e os nomes.");
                return;
            }

            journalPanel.SetActive(false); // Garante que comece fechado
            PopulateJournal();
        }
        else
        {
            Debug.LogWarning("UIManager: JournalPanel não encontrado na cena atual. A funcionalidade do diário estará desativada.");
        }
    }

    /// <summary>
    /// Abre ou fecha o painel do diário.
    /// </summary>
    public void ToggleJournal()
    {
        if (journalPanel == null)
        {
            Debug.LogWarning("Tentou abrir o diário, mas ele não existe nesta cena.");
            return;
        }

        isJournalOpen = !isJournalOpen;
        journalPanel.SetActive(isJournalOpen);

        // Pausa o jogo e libera o cursor quando o diário está aberto
        Time.timeScale = isJournalOpen ? 0f : 1f;
        Cursor.lockState = isJournalOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isJournalOpen;
    }

    /// <summary>
    /// Instancia uma entrada de UI para cada personagem definido no SolutionManager.
    /// </summary>
    private void PopulateJournal()
    {
        if (entriesContainer == null) return;

        // Limpa entradas antigas antes de popular novamente
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }

        var solucoes = SolutionManager.Instance.GetTodasAsSolucoes();
        foreach (var solucao in solucoes)
        {
            CharacterEntryUI newEntry = Instantiate(characterEntryPrefab, entriesContainer);
            newEntry.Initialize(solucao.id);
        }
    }

    #endregion

    #region Lógica de Feedback (Validação)

    /// <summary>
    /// Este método é o "ouvinte". Ele é executado AUTOMATICAMENTE quando o ValidationSystem anuncia um sucesso.
    /// </summary>
    private void HandleDeductionsConfirmed(List<DeducaoJogador> confirmedDeductions)
    {
        if (confirmationText != null)
        {
            // Inicia a rotina para mostrar a mensagem de sucesso
            StartCoroutine(ShowConfirmationMessage($"Você elucidou o destino de {confirmedDeductions.Count} pessoas."));
        }
    }

    /// <summary>
    /// Corrotina para mostrar uma mensagem por um tempo e depois escondê-la.
    /// </summary>
    private IEnumerator ShowConfirmationMessage(string message)
    {
        confirmationText.text = message;
        confirmationText.gameObject.SetActive(true);

        // Espera por 3 segundos de tempo real (funciona mesmo com o jogo pausado)
        yield return new WaitForSecondsRealtime(3f);

        confirmationText.gameObject.SetActive(false);
    }

    #endregion
}