using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necessário para TextMeshPro Dropdowns
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controla os elementos de UI de uma única entrada de personagem no diário.
/// </summary>
public class CharacterEntryUI : MonoBehaviour
{
    [Header("Referências dos Componentes")]
    [SerializeField] private TMP_Dropdown nameDropdown;
    [SerializeField] private TMP_Dropdown roleDropdown;
    [SerializeField] private TMP_Dropdown fateDropdown;
    // [SerializeField] private Image portraitImage; // Para o retrato
    [SerializeField] private GameObject lockOverlay; // Para o "selo" de confirmação

    private int characterId;
    private bool isLocked = false;

    /// <summary>
    /// Configura a entrada da UI com os dados iniciais e popula os dropdowns.
    /// </summary>
    public void Initialize(int charId)
    {
        this.characterId = charId;
        lockOverlay.SetActive(false);

        PopulateDropdowns();
        AddListeners();

        // Se inscrever no evento de confirmação para poder se trancar
        ValidationSystem.OnDeductionsConfirmed += LockIfConfirmed;
    }

    private void OnDestroy()
    {
        // Sempre se desinscreva de eventos quando o objeto for destruído
        ValidationSystem.OnDeductionsConfirmed -= LockIfConfirmed;
    }

    private void PopulateDropdowns()
    {
        // --- Nomes ---
        List<string> allNames = SolutionManager.Instance.GetTodosOsNomes();
        nameDropdown.ClearOptions();
        nameDropdown.options.Add(new TMP_Dropdown.OptionData("???")); // Opção padrão
        nameDropdown.AddOptions(allNames);

        // --- Papéis ---
        List<string> allRoles = System.Enum.GetNames(typeof(PapelNoRoubo)).ToList();
        roleDropdown.ClearOptions();
        roleDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        roleDropdown.AddOptions(allRoles);

        // --- Destinos ---
        List<string> allFates = System.Enum.GetNames(typeof(DestinoFinal)).ToList();
        fateDropdown.ClearOptions();
        fateDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        fateDropdown.AddOptions(allFates);
    }

    private void AddListeners()
    {
        // Adiciona funções para serem chamadas quando o valor de um dropdown mudar
        nameDropdown.onValueChanged.AddListener(OnNameChanged);
        roleDropdown.onValueChanged.AddListener(OnRoleChanged);
        fateDropdown.onValueChanged.AddListener(OnFateChanged);
    }

    // --- Funções de Callback ---

    public void OnNameChanged(int index)
    {
        if (isLocked || index == 0) return;
        string selectedName = nameDropdown.options[index].text;
        PlayerState.Instance.GetDeducaoPorId(characterId).nomeEscolhido = selectedName;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    public void OnRoleChanged(int index)
    {
        if (isLocked || index == 0) return;
        // Converte a string de volta para o enum
        PapelNoRoubo selectedRole = (PapelNoRoubo)System.Enum.Parse(typeof(PapelNoRoubo), roleDropdown.options[index].text);
        PlayerState.Instance.GetDeducaoPorId(characterId).papelEscolhido = selectedRole;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    public void OnFateChanged(int index)
    {
        if (isLocked || index == 0) return;
        DestinoFinal selectedFate = (DestinoFinal)System.Enum.Parse(typeof(DestinoFinal), fateDropdown.options[index].text);
        PlayerState.Instance.GetDeducaoPorId(characterId).destinoEscolhido = selectedFate;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    // --- Lógica de Confirmação ---

    private void LockIfConfirmed(List<DeducaoJogador> confirmedDeductions)
    {
        // Verifica se a *sua própria* ID está na lista de deduções recém-confirmadas
        if (confirmedDeductions.Any(d => d.idPersonagem == this.characterId))
        {
            isLocked = true;
            nameDropdown.interactable = false;
            roleDropdown.interactable = false;
            fateDropdown.interactable = false;
            lockOverlay.SetActive(true);
        }
    }
}