using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para o evento sceneLoaded
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private static GameData dataToLoadBuffer;
    private int currentSlot = 0;

    // --- Controlo do evento de carregamento de cena ---
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // ---------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public bool PrepararNovoJogo(int slotToUse = -1)
    {
        int targetSlot = slotToUse;
        if (targetSlot == -1)
        {
            targetSlot = EncontrarSlotVazio();
            if (targetSlot == -1) return false;
        }

        currentSlot = targetSlot;
        dataToLoadBuffer = new GameData();
        ApagarSave(currentSlot);

        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.LoadData(dataToLoadBuffer);
        }

        return true;
    }

    public void AtivarSlot(int slot)
    {
        this.currentSlot = slot;
    }

    public void SaveGame()
    {
        if (currentSlot == 0 || PlayerState.Instance == null) return;
        if (!PlayerState.Instance.GetLembrancasConcluidas().Any() && !PlayerState.Instance.GetPerfisDesbloqueados().Any()) return;

        GameData dataToSave = new GameData();
        dataToSave.listaDeDeducoes = PlayerState.Instance.GetTodasAsDeducoes();
        dataToSave.lembrancasConcluidas = new List<string>(PlayerState.Instance.GetLembrancasConcluidas());
        dataToSave.perfisDesbloqueadosNoDiario = PlayerState.Instance.GetPerfisDesbloqueados();
        dataToSave.ultimaAtualizacao = DateTime.Now.ToBinary();

        string dataToStore = JsonUtility.ToJson(dataToSave, true);
        string filePath = Path.Combine(Application.persistentDataPath, $"save_slot_{currentSlot}.json");

        try
        {
            File.WriteAllText(filePath, dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError($"Falha ao salvar no Slot {currentSlot}: {e.Message}");
        }
    }

    public void LoadGame(int slot)
    {
        currentSlot = slot;
        string filePath = Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
        if (!File.Exists(filePath)) return;

        try
        {
            string dataToLoad = File.ReadAllText(filePath);
            dataToLoadBuffer = JsonUtility.FromJson<GameData>(dataToLoad);
            SceneManager.LoadScene("SampleScene");
        }
        catch (Exception e)
        {
            Debug.LogError($"Falha ao carregar o Slot {slot}: {e.Message}");
        }
    }

    // Este método agora é privado, pois só é chamado de dentro desta classe.
    private void AplicarDadosCarregados()
    {
        if (dataToLoadBuffer != null)
        {
            if (PlayerState.Instance != null)
            {
                PlayerState.Instance.LoadData(dataToLoadBuffer);
            }
            dataToLoadBuffer = null;
        }
    }

    // ---------- CÓDIGO CORRIGIDO ABAIXO ----------
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // A condição foi alterada para "SampleScene".
        if (scene.name == "SampleScene")
        {
            AplicarDadosCarregados();
        }
    }
    // ---------- FIM DA CORREÇÃO ----------

    public GameData GetDataParaSlot(int slot)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
        if (!File.Exists(filePath)) return null;

        string dataToLoad = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(dataToLoad) || dataToLoad.Trim() == "{}") return null;

        GameData tempData = JsonUtility.FromJson<GameData>(dataToLoad);
        if (tempData == null || (!tempData.lembrancasConcluidas.Any() && !tempData.perfisDesbloqueadosNoDiario.Any()))
        {
            return null;
        }
        return tempData;
    }

    public int EncontrarSlotVazio()
    {
        for (int i = 1; i <= 3; i++)
        {
            if (GetDataParaSlot(i) == null) return i;
        }
        return -1;
    }

    public void ApagarSave(int slot)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}