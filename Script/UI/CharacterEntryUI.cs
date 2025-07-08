// Arquivo: dreerr08/museutop/museutop-6ea31f3c45b1c0f813e03be5a1425dc73cd4b2a0/Script/UI/CharacterEntryUI.cs

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
    [SerializeField] private TMP_Dropdown killerDropdown; // NOVO CAMPO
    [SerializeField] private GameObject lockOverlay;

    private int characterId;
    private bool isLocked = false;
    private List<SolucaoPersonagem> todosOsPersonagens; // NOVA LISTA PARA REFERÊNCIA

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

        // Armazena a lista de todos os personagens para usar no dropdown de assassinos
        todosOsPersonagens = SolutionManager.Instance.GetTodasAsSolucoes();

        PopulateDropdowns();
        LoadState();
        AddListeners();
    }

    private void PopulateDropdowns()
    {
        // Popula Nomes
        nameDropdown.ClearOptions();
        nameDropdown.options.Add(new TMP_Dropdown.OptionData("???"));
        nameDropdown.AddOptions(SolutionManager.Instance.GetTodosOsNomes());

        // Popula Papéis
        roleDropdown.ClearOptions();
        roleDropdown.AddOptions(System.Enum.GetNames(typeof(PapelNoRoubo)).ToList());

        // Popula Destinos
        fateDropdown.ClearOptions();
        fateDropdown.AddOptions(System.Enum.GetNames(typeof(DestinoFinal)).ToList());

        // Popula o novo Dropdown de Assassinos
        killerDropdown.ClearOptions();
        killerDropdown.options.Add(new TMP_Dropdown.OptionData("Desconhecido"));
        foreach (var p in todosOsPersonagens)
        {
            if (p.id != this.characterId) // Um personagem não pode matar a si mesmo
            {
                killerDropdown.options.Add(new TMP_Dropdown.OptionData(p.nomeCompleto));
            }
        }
    }

    private void LoadState()
    {
        DeducaoJogador deducaoSalva = PlayerState.Instance.GetDeducaoPorId(this.characterId);
        if (deducaoSalva == null) return;

        // Carrega Nomes
        if (!string.IsNullOrEmpty(deducaoSalva.nomeEscolhido))
        {
            int nameIndex = nameDropdown.options.FindIndex(option => option.text == deducaoSalva.nomeEscolhido);
            if (nameIndex > 0) nameDropdown.value = nameIndex;
        }

        // Carrega Papel e Destino
        roleDropdown.value = (int)deducaoSalva.papelEscolhido;
        fateDropdown.value = (int)deducaoSalva.destinoEscolhido;

        // Carrega Assassino
        int killerId = deducaoSalva.idAssassinoEscolhido;
        if (killerId > 0)
        {
            var assassino = todosOsPersonagens.FirstOrDefault(p => p.id == killerId);
            if (assassino != null)
            {
                int killerIndex = killerDropdown.options.FindIndex(opt => opt.text == assassino.nomeCompleto);
                if (killerIndex > -1) killerDropdown.value = killerIndex;
            }
        }
        else
        {
            killerDropdown.value = 0; // "Desconhecido"
        }

        // Aplica a regra de exceção ao carregar os dados
        CheckFateAndToggleKillerDropdown((DestinoFinal)fateDropdown.value);
    }

    private void AddListeners()
    {
        nameDropdown.onValueChanged.RemoveAllListeners();
        roleDropdown.onValueChanged.RemoveAllListeners();
        fateDropdown.onValueChanged.RemoveAllListeners();
        killerDropdown.onValueChanged.RemoveAllListeners(); // NOVO

        nameDropdown.onValueChanged.AddListener(OnNameChanged);
        roleDropdown.onValueChanged.AddListener(OnRoleChanged);
        fateDropdown.onValueChanged.AddListener(OnFateChanged);
        killerDropdown.onValueChanged.AddListener(OnKillerChanged); // NOVO
    }

    public void OnNameChanged(int index)
    {
        if (isLocked) return;
        string nome = (index == 0) ? null : nameDropdown.options[index].text;
        PlayerState.Instance.GetDeducaoPorId(characterId).nomeEscolhido = nome;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
        SaveLoadManager.Instance.SaveGame();
    }

    public void OnRoleChanged(int index)
    {
        if (isLocked) return;
        PlayerState.Instance.GetDeducaoPorId(characterId).papelEscolhido = (PapelNoRoubo)index;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
        SaveLoadManager.Instance.SaveGame();
    }

    public void OnFateChanged(int index)
    {
        if (isLocked) return;
        DestinoFinal destinoEscolhido = (DestinoFinal)index;
        PlayerState.Instance.GetDeducaoPorId(characterId).destinoEscolhido = destinoEscolhido;

        // Aplica a regra de exceção aqui
        CheckFateAndToggleKillerDropdown(destinoEscolhido);

        ValidationSystem.Instance.ValidarTodasAsDeducoes();
        SaveLoadManager.Instance.SaveGame();
    }

    public void OnKillerChanged(int index)
    {
        if (isLocked) return;

        int assassinoId = 0;
        if (index > 0)
        {
            string nomeEscolhido = killerDropdown.options[index].text;
            var assassino = todosOsPersonagens.FirstOrDefault(p => p.nomeCompleto == nomeEscolhido);
            if (assassino != null) assassinoId = assassino.id;
        }

        PlayerState.Instance.GetDeducaoPorId(characterId).idAssassinoEscolhido = assassinoId;
        ValidationSystem.Instance.ValidarTodasAsDeducoes();
        SaveLoadManager.Instance.SaveGame();
    }

    private void CheckFateAndToggleKillerDropdown(DestinoFinal destino)
    {
        bool isFugiuComDinheiro = (destino == DestinoFinal.FugiuComDinheiro);

        killerDropdown.interactable = !isFugiuComDinheiro;

        if (isFugiuComDinheiro)
        {
            killerDropdown.value = 0; // Reseta para "Desconhecido"
            OnKillerChanged(0); // Garante que o estado seja salvo como "ninguém"
        }
    }

    private void LockIfConfirmed(List<DeducaoJogador> confirmedDeductions)
    {
        if (confirmedDeductions.Any(d => d.idPersonagem == this.characterId))
        {
            isLocked = true;
            nameDropdown.interactable = false;
            roleDropdown.interactable = false;
            fateDropdown.interactable = false;
            killerDropdown.interactable = false; // NOVO
            lockOverlay.SetActive(true);
        }
    }
}