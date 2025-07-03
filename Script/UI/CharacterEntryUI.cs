using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necess�rio para TextMeshPro Dropdowns
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Controla os elementos de UI de uma �nica entrada de personagem no di�rio.
/// </summary>
public class CharacterEntryUI : MonoBehaviour
{
    [Header("Refer�ncias dos Componentes")]
    [SerializeField] private TMP_Dropdown nameDropdown;
    [SerializeField] private TMP_Dropdown photoIdDropdown;
    [SerializeField] private TMP_Dropdown roleDropdown;
    [SerializeField] private TMP_Dropdown fateDropdown;
    [SerializeField] private GameObject lockOverlay;

    private int characterId;
    private bool isLocked = false;

    // ALTERADO: A inscri��o do evento foi movida para OnEnable.
    // OnEnable � chamado sempre que o objeto se torna ativo na cena.
    private void OnEnable()
    {
        ValidationSystem.OnDeductionsConfirmed += LockIfConfirmed;
    }

    // ALTERADO: O cancelamento da inscri��o foi movido para OnDisable.
    // OnDisable � chamado quando o objeto � desativado, destru�do ou quando a cena muda.
    // Isto garante que a refer�ncia � removida antes que o objeto seja destru�do.
    private void OnDisable()
    {
        ValidationSystem.OnDeductionsConfirmed -= LockIfConfirmed;
    }

    /// <summary>
    /// Configura a entrada da UI com os dados iniciais e popula os dropdowns.
    /// </summary>
    public void Initialize(int charId)
    {
        this.characterId = charId;
        lockOverlay.SetActive(false);

        PopulateDropdowns();
        AddListeners();
        // A inscri��o do evento foi removida daqui.
    }

    // O m�todo OnDestroy j� n�o � necess�rio para esta tarefa,
    // uma vez que OnDisable � mais seguro para a gest�o de eventos.

    private void PopulateDropdowns()
    {
        // --- Nomes ---
        List<string> allNames = SolutionManager.Instance.GetTodosOsNomes();
        nameDropdown.ClearOptions();
        nameDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        nameDropdown.AddOptions(allNames);

        // --- IDs das Fotos ---
        List<string> allPhotoIds = SolutionManager.Instance.GetTodosOsIdsDeFoto();
        photoIdDropdown.ClearOptions();
        photoIdDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        photoIdDropdown.AddOptions(allPhotoIds);

        // --- Pap�is ---
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
        nameDropdown.onValueChanged.AddListener(OnNameChanged);
        photoIdDropdown.onValueChanged.AddListener(OnPhotoIdChanged);
        roleDropdown.onValueChanged.AddListener(OnRoleChanged);
        fateDropdown.onValueChanged.AddListener(OnFateChanged);
    }

    // --- Fun��es de Callback ---

    public void OnNameChanged(int index)
    {
        if (isLocked || index == 0) return;
        string selectedName = nameDropdown.options[index].text;
        PlayerState.Instance.GetDeducaoPorId(characterId).nomeEscolhido = selectedName;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    public void OnPhotoIdChanged(int index)
    {
        if (isLocked || index == 0) return;
        string selectedPhotoId = photoIdDropdown.options[index].text;
        PlayerState.Instance.GetDeducaoPorId(characterId).fotoEscolhida = selectedPhotoId;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
    }

    public void OnRoleChanged(int index)
    {
        if (isLocked || index == 0) return;
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

    // --- L�gica de Confirma��o ---

    private void LockIfConfirmed(List<DeducaoJogador> confirmedDeductions)
    {
        if (confirmedDeductions.Any(d => d.idPersonagem == this.characterId))
        {
            isLocked = true;
            nameDropdown.interactable = false;
            photoIdDropdown.interactable = false;
            roleDropdown.interactable = false;
            fateDropdown.interactable = false;
            lockOverlay.SetActive(true);
        }
    }
}