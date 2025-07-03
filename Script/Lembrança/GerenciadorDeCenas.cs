using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Necessário para List

public class GerenciadorDeCenas : MonoBehaviour
{
    public static GerenciadorDeCenas Instancia { get; private set; }

    [Header("Configuração do Fade")]
    public float fadeSpeed = 1f;
    public Color fadeColor = Color.black;

    [Header("Base de Dados")]
    [Tooltip("Arraste aqui o asset 'BancoDeDesbloqueios' do seu projeto.")]
    public BancoDeDesbloqueios bancoDeDesbloqueios;

    private Image fadeImage;
    private string cenaDeRetorno;
    private Vector3 posicaoDeRetorno;
    private float rotacaoXCameraDeRetorno;
    private float rotacaoYJogadorDeRetorno;
    private string idLembrancaAtiva;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SetupFadeCanvas();

            if (bancoDeDesbloqueios == null)
            {
                Debug.LogError("[GerenciadorDeCenas] ERRO CRÍTICO: O 'Banco De Desbloqueios' não foi atribuído no Inspector!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IrParaLembranca(string nomeCenaLembranca, string idLembranca)
    {
        this.idLembrancaAtiva = idLembranca;
        Debug.Log($"[GerenciadorDeCenas] Iniciando transição. Lembrança ativa foi definida como: '{idLembrancaAtiva}'.");

        this.cenaDeRetorno = SceneManager.GetActiveScene().name;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            this.posicaoDeRetorno = player.transform.position;
            this.rotacaoYJogadorDeRetorno = player.transform.eulerAngles.y;
            FirstPersonController fpController = player.GetComponent<FirstPersonController>();
            if (fpController != null) this.rotacaoXCameraDeRetorno = fpController.GetCurrentPitch();
        }
        StartCoroutine(FadeAndLoadScene(nomeCenaLembranca));
    }

    public void RetornarDaLembranca()
    {
        Debug.Log("[GerenciadorDeCenas] Método RetornarDaLembranca foi chamado.");

        if (!string.IsNullOrEmpty(idLembrancaAtiva) && bancoDeDesbloqueios != null)
        {
            Debug.Log($"[GerenciadorDeCenas] A processar desbloqueio para a lembrança: '{idLembrancaAtiva}'.");

            List<int> idsParaDesbloquear = bancoDeDesbloqueios.GetPersonagensParaDesbloquear(idLembrancaAtiva);

            if (idsParaDesbloquear.Count > 0)
            {
                Debug.Log($"[GerenciadorDeCenas] Banco de Desbloqueios retornou {idsParaDesbloquear.Count} IDs de personagem para desbloquear. A enviar para o PlayerState.");
            }
            else
            {
                Debug.LogWarning($"[GerenciadorDeCenas] AVISO: O Banco de Desbloqueios não encontrou nenhuma entrada ou a entrada está vazia para o ID '{idLembrancaAtiva}'.");
            }

            PlayerState.Instance.ConcluirLembranca(idLembrancaAtiva, idsParaDesbloquear);
            idLembrancaAtiva = null;
        }
        else
        {
            Debug.LogError($"[GerenciadorDeCenas] ERRO: Não foi possível processar o desbloqueio. ID da Lembrança Ativa: '{idLembrancaAtiva}', Banco de Desbloqueios atribuído: {bancoDeDesbloqueios != null}");
        }

        if (!string.IsNullOrEmpty(cenaDeRetorno))
        {
            StartCoroutine(FadeAndLoadScene(cenaDeRetorno));
        }
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == cenaDeRetorno)
        {
            StartCoroutine(RestaurarEstadoDoJogador());
        }
        StartCoroutine(FadeIn());
    }

    private IEnumerator RestaurarEstadoDoJogador()
    {
        yield return new WaitForEndOfFrame();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            CharacterController charController = player.GetComponent<CharacterController>();
            if (charController != null) charController.enabled = false;
            player.transform.position = posicaoDeRetorno;
            if (charController != null) charController.enabled = true;

            FirstPersonController fpController = player.GetComponent<FirstPersonController>();
            if (fpController != null)
            {
                fpController.InicializarRotacao(rotacaoYJogadorDeRetorno, rotacaoXCameraDeRetorno);
            }
        }
    }

    private IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color c = fadeColor;
        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, elapsedTime / fadeSpeed);
            fadeImage.color = c;
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color c = fadeColor;
        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, elapsedTime / fadeSpeed);
            fadeImage.color = c;
            yield return null;
        }
    }

    private void SetupFadeCanvas()
    {
        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(this.transform);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform);
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        imageGO.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}