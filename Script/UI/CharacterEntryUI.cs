using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controla os elementos de UI de uma �nica entrada de personagem no di�rio.
/// </summary>
public class CharacterEntryUI : MonoBehaviour
{
    [Header("Refer�ncias dos Componentes")]
    [SerializeField] private TMP_Dropdown nameDropdown;
    [SerializeField] private TMP_Dropdown photoSelectorDropdown; // Nome alterado para clareza
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
        // Nomes
        nameDropdown.ClearOptions();
        nameDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        nameDropdown.AddOptions(SolutionManager.Instance.GetTodosOsNomes());

        // (ALTERADO) Seletor de Imagens
        photoSelectorDropdown.ClearOptions();
        photoSelectorDropdown.options.Add(new TMP_Dropdown.OptionData("???")); // Op��o padr�o sem imagem
        var allPortraits = SolutionManager.Instance.GetTodosOsRetratos();
        foreach (var portrait in allPortraits)
        {
            // Adiciona uma op��o com o nome do sprite como texto e o sprite como imagem
            photoSelectorDropdown.options.Add(new TMP_Dropdown.OptionData("", portrait));
        }
        photoSelectorDropdown.RefreshShownValue(); // Garante que a UI visual seja atualizada

        // Pap�is
        roleDropdown.ClearOptions();
        roleDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        roleDropdown.AddOptions(System.Enum.GetNames(typeof(PapelNoRoubo)).ToList());

        // Destinos
        fateDropdown.ClearOptions();
        fateDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        fateDropdown.AddOptions(System.Enum.GetNames(typeof(DestinoFinal)).ToList());
    }

    private void AddListeners()
    {
        nameDropdown.onValueChanged.AddListener(OnNameChanged);
        photoSelectorDropdown.onValueChanged.AddListener(OnPhotoChanged); // Nome do m�todo alterado
        roleDropdown.onValueChanged.AddListener(OnRoleChanged);
        fateDropdown.onValueChanged.AddListener(OnFateChanged);
    }

    // --- Fun��es de Callback ---
    public void OnNameChanged(int index)
    {
        if (isLocked || index == 0) return;
        PlayerState.Instance.GetDeducaoPorId(characterId).nomeEscolhido = nameDropdown.options[index].text;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    // (ALTERADO) M�todo para lidar com a sele��o de imagem
    public void OnPhotoChanged(int index)
    {
        if (isLocked) return;

        // Se o �ndice for 0 ("???"), limpa a escolha. Caso contr�rio, atribui o sprite.
        Sprite selectedPortrait = (index == 0) ? null : photoSelectorDropdown.options[index].image;
        PlayerState.Instance.GetDeducaoPorId(characterId).retratoEscolhido = selectedPortrait;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    public void OnRoleChanged(int index)
    {
        if (isLocked || index == 0) return;
        var selectedRole = (PapelNoRoubo)System.Enum.Parse(typeof(PapelNoRoubo), roleDropdown.options[index].text);
        PlayerState.Instance.GetDeducaoPorId(characterId).papelEscolhido = selectedRole;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    public void OnFateChanged(int index)
    {
        if (isLocked || index == 0) return;
        var selectedFate = (DestinoFinal)System.Enum.Parse(typeof(DestinoFinal), fateDropdown.options[index].text);
        PlayerState.Instance.GetDeducaoPorId(characterId).destinoEscolhido = selectedFate;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    // --- L�gica de Confirma��o ---
    private void LockIfConfirmed(List<DeducaoJogador> confirmedDeductions)
    {
        if (confirmedDeductions.Any(d => d.idPersonagem == this.characterId))
        {
            isLocked = true;
            nameDropdown.interactable = false;
            photoSelectorDropdown.interactable = false;
            roleDropdown.interactable = false;
            fateDropdown.interactable = false;
            lockOverlay.SetActive(true);
        }
    }
}