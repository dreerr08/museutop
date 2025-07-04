using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class TelaDeSavesUI : MonoBehaviour
{
    [Header("Configuração dos Slots")]
    public Button[] slotButtons;
    public TextMeshProUGUI[] slotTexts;
    public Button[] deleteButtons;

    [Header("Configuração de Cenas")]
    public string nomeCenaPrincipal = "Museu"; // Mantido como "Museu", altere se necessário
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

            // Limpa qualquer evento de clique anterior para evitar acúmulo
            slotButtons[uiIndex].onClick.RemoveAllListeners();

            if (data != null)
            {
                // --- SLOT COM SAVE ---
                // O botão é clicável
                slotButtons[uiIndex].interactable = true;

                // Mostra a data e hora do save
                DateTime dateTime = DateTime.FromBinary(data.ultimaAtualizacao);
                slotTexts[uiIndex].text = $"Slot {slotNumber}\n<size=24>{dateTime:dd/MM/yyyy HH:mm:ss}</size>";

                // Adiciona a função para carregar o jogo
                slotButtons[uiIndex].onClick.AddListener(() => CarregarJogo(slotNumber));

                // Ativa e configura o botão de apagar
                deleteButtons[uiIndex].gameObject.SetActive(true);
                deleteButtons[uiIndex].onClick.RemoveAllListeners();
                deleteButtons[uiIndex].onClick.AddListener(() => ApagarSaveDoSlot(slotNumber));
            }
            else
            {
                // --- SLOT VAZIO (CÓDIGO ALTERADO) ---
                // O botão agora é DESATIVADO
                slotButtons[uiIndex].interactable = false;

                // Mostra que o slot está vazio
                slotTexts[uiIndex].text = $"Slot {slotNumber}\n[ Vazio ]";

                // O botão de apagar fica invisível pois não há o que apagar
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

    // Esta função não é mais chamada a partir de um clique em slot vazio,
    // mas pode ser útil para outras partes do seu código.
    public void IniciarNovoJogoNoSlot(int slot)
    {
        SaveLoadManager.Instance.PrepararNovoJogo(slot);
        SceneManager.LoadScene(nomeCenaPrincipal);
    }

    public void ApagarSaveDoSlot(int slot)
    {
        SaveLoadManager.Instance.ApagarSave(slot);
        // Atualiza a tela para refletir a remoção do save
        AtualizarTelaDeSaves();
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(nomeCenaMenu);
    }
}