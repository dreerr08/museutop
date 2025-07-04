using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class CharacterEntryUI : MonoBehaviour
{
    [Header("Referências dos Componentes")]
    [SerializeField] private TMP_Dropdown nameDropdown;
    [SerializeField] private TMP_Dropdown roleDropdown;
    [SerializeField] private TMP_Dropdown fateDropdown;
    [SerializeField] private GameObject lockOverlay;

    private int characterId;
    private bool isLocked = false;

    private void OnEnable()
    {
        ValidationSystem.OnDeductionsConfirmed += LockIfConfirmed;
    }

    private void OnDisable()
    {
        ValidationSystem.OnDeductionsConfirmed -= LockIfConfirmed;
    }

    public void Initialize(int charId)
    {
        this.characterId = charId;
        lockOverlay.SetActive(false);
        PopulateDropdowns();

        // --- ADIÇÃO PRINCIPAL ---
        // Carrega o estado salvo e atualiza os dropdowns
        LoadState();

        AddListeners();
    }

    private void PopulateDropdowns()
    {
        nameDropdown.ClearOptions();
        nameDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        nameDropdown.AddOptions(SolutionManager.Instance.GetTodosOsNomes());

        roleDropdown.ClearOptions();
        roleDropdown.AddOptions(System.Enum.GetNames(typeof(PapelNoRoubo)).ToList());

        fateDropdown.ClearOptions();
        fateDropdown.AddOptions(System.Enum.GetNames(typeof(DestinoFinal)).ToList());
    }

    // --- NOVO MÉTODO ---
    /// <summary>
    /// Carrega os dados do PlayerState e atualiza a UI para refletir as deduções salvas.
    /// </summary>
    private void LoadState()
    {
        // Busca a dedução salva para este personagem
        DeducaoJogador deducaoSalva = PlayerState.Instance.GetDeducaoPorId(this.characterId);
        if (deducaoSalva == null) return;

        // Atualiza o dropdown de nomes
        if (!string.IsNullOrEmpty(deducaoSalva.nomeEscolhido))
        {
            int nameIndex = nameDropdown.options.FindIndex(option => option.text == deducaoSalva.nomeEscolhido);
            if (nameIndex > 0) // Garante que não é "???"
            {
                nameDropdown.value = nameIndex;
            }
        }

        // Atualiza os dropdowns de papel e destino
        // A ordem dos enums corresponde diretamente aos índices dos dropdowns
        roleDropdown.value = (int)deducaoSalva.papelEscolhido;
        fateDropdown.value = (int)deducaoSalva.destinoEscolhido;
    }

    private void AddListeners()
    {
        // Remove listeners antigos para evitar chamadas duplicadas
        nameDropdown.onValueChanged.RemoveAllListeners();
        roleDropdown.onValueChanged.RemoveAllListeners();
        fateDropdown.onValueChanged.RemoveAllListeners();

        // Adiciona os listeners para salvar as alterações
        nameDropdown.onValueChanged.AddListener(OnNameChanged);
        roleDropdown.onValueChanged.AddListener(OnRoleChanged);
        fateDropdown.onValueChanged.AddListener(OnFateChanged);
    }

    public void OnNameChanged(int index)
    {
        if (isLocked) return;

        // Atualiza a dedução no PlayerState
        string nome = (index == 0) ? null : nameDropdown.options[index].text;
        PlayerState.Instance.GetDeducaoPorId(characterId).nomeEscolhido = nome;

        ValidationSystem.Instance.ValidarTodasAsDeducoes();
        SaveLoadManager.Instance.SaveGame();
    }

    public void OnRoleChanged(int index)
    {
        if (isLocked) return;

        // Atualiza a dedução no PlayerState
        PlayerState.Instance.GetDeducaoPorId(characterId).papelEscolhido = (PapelNoRoubo)index;

        ValidationSystem.Instance.ValidarTodasAsDeducoes();
        SaveLoadManager.Instance.SaveGame();
    }

    public void OnFateChanged(int index)
    {
        if (isLocked) return;

        // Atualiza a dedução no PlayerState
        PlayerState.Instance.GetDeducaoPorId(characterId).destinoEscolhido = (DestinoFinal)index;

        ValidationSystem.Instance.ValidarTodasAsDeducoes();
        SaveLoadManager.Instance.SaveGame();
    }

    private void LockIfConfirmed(List<DeducaoJogador> confirmedDeductions)
    {
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