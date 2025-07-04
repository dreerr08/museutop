using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class TelaDeSavesUI : MonoBehaviour
{
    [Header("Configura��o dos Slots")]
    public Button[] slotButtons;
    public TextMeshProUGUI[] slotTexts;
    public Button[] deleteButtons;

    [Header("Configura��o de Cenas")]
    public string nomeCenaPrincipal = "Museu"; // Mantido como "Museu", altere se necess�rio
    public string nomeCenaMenu = "MenuPrincipal";

    private void Start()
    {
        AtualizarTelaDeSaves();
    }

    private void AtualizarTelaDeSaves()
    {
        for (int i = 0; i < 3; i++)
        {
            int slotNumber = i + 1;
            GameData data = SaveLoadManager.Instance.GetDataParaSlot(slotNumber);
            int uiIndex = i;

            // Limpa qualquer evento de clique anterior para evitar ac�mulo
            slotButtons[uiIndex].onClick.RemoveAllListeners();

            if (data != null)
            {
                // --- SLOT COM SAVE ---
                // O bot�o � clic�vel
                slotButtons[uiIndex].interactable = true;

                // Mostra a data e hora do save
                DateTime dateTime = DateTime.FromBinary(data.ultimaAtualizacao);
                slotTexts[uiIndex].text = $"Slot {slotNumber}\n<size=24>{dateTime:dd/MM/yyyy HH:mm:ss}</size>";

                // Adiciona a fun��o para carregar o jogo
                slotButtons[uiIndex].onClick.AddListener(() => CarregarJogo(slotNumber));

                // Ativa e configura o bot�o de apagar
                deleteButtons[uiIndex].gameObject.SetActive(true);
                deleteButtons[uiIndex].onClick.RemoveAllListeners();
                deleteButtons[uiIndex].onClick.AddListener(() => ApagarSaveDoSlot(slotNumber));
            }
            else
            {
                // --- SLOT VAZIO (C�DIGO ALTERADO) ---
                // O bot�o agora � DESATIVADO
                slotButtons[uiIndex].interactable = false;

                // Mostra que o slot est� vazio
                slotTexts[uiIndex].text = $"Slot {slotNumber}\n[ Vazio ]";

                // O bot�o de apagar fica invis�vel pois n�o h� o que apagar
                deleteButtons[uiIndex].gameObject.SetActive(false);

                // A linha que criava um novo jogo foi REMOVIDA
            }
        }
    }

    public void CarregarJogo(int slot)
    {
        // A cena carregada foi corrigida anteriormente para "SampleScene" no SaveLoadManager
        SaveLoadManager.Instance.LoadGame(slot);
    }

    // Esta fun��o n�o � mais chamada a partir de um clique em slot vazio,
    // mas pode ser �til para outras partes do seu c�digo.
    public void IniciarNovoJogoNoSlot(int slot)
    {
        SaveLoadManager.Instance.PrepararNovoJogo(slot);
        SceneManager.LoadScene(nomeCenaPrincipal);
    }

    public void ApagarSaveDoSlot(int slot)
    {
        SaveLoadManager.Instance.ApagarSave(slot);
        // Atualiza a tela para refletir a remo��o do save
        AtualizarTelaDeSaves();
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(nomeCenaMenu);
    }
}