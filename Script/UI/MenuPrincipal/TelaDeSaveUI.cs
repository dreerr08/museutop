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
    public string nomeCenaPrincipal = "Museu";
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

            if (data != null)
            {
                DateTime dateTime = DateTime.FromBinary(data.ultimaAtualizacao);
                slotTexts[uiIndex].text = $"Slot {slotNumber}\n<size=24>{dateTime:dd/MM/yyyy HH:mm:ss}</size>";

                slotButtons[uiIndex].onClick.RemoveAllListeners();
                slotButtons[uiIndex].onClick.AddListener(() => CarregarJogo(slotNumber));

                deleteButtons[uiIndex].gameObject.SetActive(true);
                deleteButtons[uiIndex].onClick.RemoveAllListeners();
                deleteButtons[uiIndex].onClick.AddListener(() => ApagarSaveDoSlot(slotNumber));
            }
            else
            {
                slotTexts[uiIndex].text = $"Slot {slotNumber}\n[ Vazio ]";

                slotButtons[uiIndex].onClick.RemoveAllListeners();
                slotButtons[uiIndex].onClick.AddListener(() => IniciarNovoJogoNoSlot(slotNumber));

                deleteButtons[uiIndex].gameObject.SetActive(false);
            }
        }
    }

    public void CarregarJogo(int slot)
    {
        SaveLoadManager.Instance.LoadGame(slot);
    }

    public void IniciarNovoJogoNoSlot(int slot)
    {
        SaveLoadManager.Instance.PrepararNovoJogo(slot);
        SceneManager.LoadScene(nomeCenaPrincipal);
    }

    public void ApagarSaveDoSlot(int slot)
    {
        SaveLoadManager.Instance.ApagarSave(slot);
        AtualizarTelaDeSaves();
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(nomeCenaMenu);
    }
}