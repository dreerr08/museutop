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

    private void AddListeners()
    {
        nameDropdown.onValueChanged.AddListener(OnNameChanged);
        roleDropdown.onValueChanged.AddListener(OnRoleChanged);
        fateDropdown.onValueChanged.AddListener(OnFateChanged);
    }

    public void OnNameChanged(int index)
    {
        if (isLocked || index == 0) return;
        PlayerState.Instance.GetDeducaoPorId(characterId).nomeEscolhido = nameDropdown.options[index].text;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();

        // LOG ADICIONADO
        Debug.Log("[AUTOSAVE TRIGGER] Mudança de nome. A chamar SaveGame...");
        SaveLoadManager.Instance.SaveGame();
    }

    public void OnRoleChanged(int index)
    {
        if (isLocked) return;
        PlayerState.Instance.GetDeducaoPorId(characterId).papelEscolhido = (PapelNoRoubo)index;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();

        // LOG ADICIONADO
        Debug.Log("[AUTOSAVE TRIGGER] Mudança de papel. A chamar SaveGame...");
        SaveLoadManager.Instance.SaveGame();
    }

    public void OnFateChanged(int index)
    {
        if (isLocked) return;
        PlayerState.Instance.GetDeducaoPorId(characterId).destinoEscolhido = (DestinoFinal)index;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();

        // LOG ADICIONADO
        Debug.Log("[AUTOSAVE TRIGGER] Mudança de destino. A chamar SaveGame...");
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