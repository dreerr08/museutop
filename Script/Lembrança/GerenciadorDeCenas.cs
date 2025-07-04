using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GerenciadorDeCenas : MonoBehaviour
{
    public static GerenciadorDeCenas Instancia { get; private set; }

    [Header("Configuração do Fade")]
    public float fadeSpeed = 1f;
    public Color fadeColor = Color.black;

    [Header("Base de Dados")]
    [Tooltip("Arraste aqui o asset 'BancoDeDesbloqueios' do seu projeto.")]
    public BancoDeDesbloqueios bancoDeDesbloqueios;

    private Stack<EstadoDeRetorno> historicoDeCenas = new Stack<EstadoDeRetorno>();
    private Image fadeImage;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
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
        var novoEstado = new EstadoDeRetorno
        {
            cenaDeRetorno = SceneManager.GetActiveScene().name,
            idLembrancaAtiva = idLembranca
        };

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            novoEstado.posicaoDeRetorno = player.transform.position;
            novoEstado.rotacaoYJogadorDeRetorno = player.transform.eulerAngles.y;
            FirstPersonController fpController = player.GetComponent<FirstPersonController>();
            if (fpController != null)
            {
                novoEstado.rotacaoXCameraDeRetorno = fpController.GetCurrentPitch();
            }
        }

        historicoDeCenas.Push(novoEstado);
        StartCoroutine(FadeAndLoadScene(nomeCenaLembranca));
    }

    public void RetornarDaLembranca()
    {
        if (historicoDeCenas.Count == 0)
        {
            Debug.LogError("[GerenciadorDeCenas] ERRO: Tentativa de retornar, mas o histórico de cenas está vazio!");
            return;
        }

        EstadoDeRetorno estadoParaRestaurar = historicoDeCenas.Pop();
        string idLembrancaConcluida = estadoParaRestaurar.idLembrancaAtiva;

        if (!string.IsNullOrEmpty(idLembrancaConcluida) && bancoDeDesbloqueios != null)
        {
            List<int> idsParaDesbloquear = bancoDeDesbloqueios.GetPersonagensParaDesbloquear(idLembrancaConcluida);
            PlayerState.Instance.ConcluirLembranca(idLembrancaConcluida, idsParaDesbloquear);
        }

        StartCoroutine(FadeAndLoadScene(estadoParaRestaurar.cenaDeRetorno, estadoParaRestaurar));
    }

    private IEnumerator FadeAndLoadScene(string sceneName, EstadoDeRetorno estadoParaRestaurar = null)
    {
        yield return StartCoroutine(FadeOut());
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;
        if (estadoParaRestaurar != null) yield return StartCoroutine(RestaurarEstadoDoJogador(estadoParaRestaurar));
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator RestaurarEstadoDoJogador(EstadoDeRetorno estado)
    {
        yield return new WaitForEndOfFrame();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            CharacterController charController = player.GetComponent<CharacterController>();
            if (charController != null) charController.enabled = false;
            player.transform.position = estado.posicaoDeRetorno;
            if (charController != null) charController.enabled = true;
            FirstPersonController fpController = player.GetComponent<FirstPersonController>();
            if (fpController != null)
            {
                fpController.InicializarRotacao(estado.rotacaoYJogadorDeRetorno, estado.rotacaoXCameraDeRetorno);
            }
        }
    }

    private IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color c = fadeColor;
        while (elapsedTime < fadeSpeed) { c.a = Mathf.Lerp(1, 0, elapsedTime / fadeSpeed); fadeImage.color = c; elapsedTime += Time.deltaTime; yield return null; }
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color c = fadeColor;
        while (elapsedTime < fadeSpeed) { c.a = Mathf.Lerp(0, 1, elapsedTime / fadeSpeed); fadeImage.color = c; elapsedTime += Time.deltaTime; yield return null; }
    }

    private void SetupFadeCanvas()
    {
        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(this.transform);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform);
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        imageGO.SetActive(false);
    }
}