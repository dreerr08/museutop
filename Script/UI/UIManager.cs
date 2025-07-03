using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Referências do Inspector")]
    [SerializeField] private CharacterEntryUI characterEntryPrefab;
    [SerializeField] private TextMeshProUGUI confirmationText;

    private GameObject journalPanel;
    private Transform entriesContainer;
    private bool isJournalOpen = false;

    private void Awake() { if (Instance != null && Instance != this) Destroy(this.gameObject); else { Instance = this; DontDestroyOnLoad(this.gameObject); } }
    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; ValidationSystem.OnDeductionsConfirmed += HandleDeductionsConfirmed; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; ValidationSystem.OnDeductionsConfirmed -= HandleDeductionsConfirmed; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleJournal();
            if (isJournalOpen)
            {
                Debug.Log("[UIManager] Diário aberto. A chamar PopulateJournal...");
                PopulateJournal();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) { FindAndAssignUIReferences(); if (confirmationText != null) confirmationText.gameObject.SetActive(false); }
    private void FindAndAssignUIReferences() { journalPanel = GameObject.FindWithTag("JournalPanel"); if (journalPanel != null) { entriesContainer = journalPanel.transform.Find("Scroll View/Viewport/Content"); journalPanel.SetActive(false); } }
    public void ToggleJournal() { if (journalPanel == null) return; isJournalOpen = !isJournalOpen; journalPanel.SetActive(isJournalOpen); Time.timeScale = isJournalOpen ? 0f : 1f; Cursor.lockState = isJournalOpen ? CursorLockMode.None : CursorLockMode.Locked; Cursor.visible = isJournalOpen; }

    private void PopulateJournal()
    {
        Debug.Log("[UIManager] --- Iniciando PopulateJournal ---");
        if (entriesContainer == null)
        {
            Debug.LogError("[UIManager] ERRO: O 'entriesContainer' (Content do ScrollView) é nulo. Não é possível popular o diário.");
            return;
        }

        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("[UIManager] Entradas antigas do diário foram limpas.");

        var idsPerfisDesbloqueados = PlayerState.Instance.GetPerfisDesbloqueados();
        Debug.Log($"[UIManager] PlayerState retornou {idsPerfisDesbloqueados.Count} perfis desbloqueados.");

        if (idsPerfisDesbloqueados.Count == 0)
        {
            Debug.Log("[UIManager] Nenhum perfil para exibir. O diário permanecerá vazio.");
        }

        foreach (var personagemId in idsPerfisDesbloqueados)
        {
            Debug.Log($"[UIManager] A tentar criar entrada para o personagem com ID: {personagemId}.");
            var solucao = SolutionManager.Instance.GetSolucaoPorId(personagemId);
            if (solucao != null)
            {
                CharacterEntryUI newEntry = Instantiate(characterEntryPrefab, entriesContainer);
                newEntry.Initialize(personagemId);
                Debug.Log($"[UIManager] Prefab de CharacterEntryUI instanciado para ID {personagemId}.");

                var imageComponent = newEntry.transform.Find("CharacterPortraitImage")?.GetComponent<Image>();
                if (imageComponent != null)
                {
                    imageComponent.sprite = solucao.retrato;
                    imageComponent.color = solucao.retrato != null ? Color.white : Color.clear;
                    Debug.Log($"[UIManager] Imagem do retrato para ID {personagemId} foi definida.");
                }
                else
                {
                    Debug.LogError($"[UIManager] ERRO: Não foi encontrado um objeto filho chamado 'CharacterPortraitImage' com um componente 'Image' no prefab CharacterEntryUI para o ID {personagemId}.");
                }
            }
            else
            {
                Debug.LogWarning($"[UIManager] AVISO: Não foi encontrada uma solução no SolutionManager para o ID {personagemId}. Esta entrada não será criada.");
            }
        }
        Debug.Log("[UIManager] --- Fim de PopulateJournal ---");
    }

    private void HandleDeductionsConfirmed(List<DeducaoJogador> confirmedDeductions) { if (confirmationText != null) StartCoroutine(ShowConfirmationMessage($"Você elucidou o destino de {confirmedDeductions.Count} pessoas.")); }
    private IEnumerator ShowConfirmationMessage(string message) { confirmationText.text = message; confirmationText.gameObject.SetActive(true); yield return new WaitForSecondsRealtime(3f); confirmationText.gameObject.SetActive(false); }
}