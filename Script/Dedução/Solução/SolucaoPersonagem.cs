// Arquivo: dreerr08/museutop/museutop-6ea31f3c45b1c0f813e03be5a1425dc73cd4b2a0/Script/Dedu��o/Solu��o/SolucaoPersonagem.cs

// O atributo [System.Serializable] � extremamente importante para o Unity.
// Ele permite que voc� crie e edite inst�ncias desta classe diretamente
// no Inspector da Unity, tornando muito f�cil preencher os dados do seu jogo
// sem precisar alterar o c�digo.
[System.Serializable]
public class SolucaoPersonagem
{
    // NOTA: Esta classe n�o herda de MonoBehaviour, pois ela � apenas um
    // cont�iner de dados, n�o um componente que vive em um GameObject.

    /// <summary>
    /// ID �nico para referenciar este personagem de forma program�tica (ex: 1, 2, 3...).
    /// </summary>
    public int id;

    /// <summary>
    /// O nome completo e correto do personagem.
    /// </summary>
    public string nomeCompleto;

    /// <summary>
    /// (ALTERADO) A imagem (Sprite) do retrato do personagem.
    /// Este campo substitui o antigo 'idDaFoto'.
    /// </summary>
    public UnityEngine.Sprite retrato;

    /// <summary>
    /// O verdadeiro envolvimento deste personagem no roubo.
    /// Usa o enum que definimos para garantir consist�ncia.
    /// </summary>
    public PapelNoRoubo papel;

    /// <summary>
    /// O verdadeiro destino deste personagem.
    /// Usa o enum para garantir consist�ncia.
    /// </summary>
    public DestinoFinal destino;

    /// <summary>
    /// (NOVO) O ID do personagem que assassinou este.
    /// Use 0 se n�o foi assassinado (ex: acidente, fugiu, etc.).
    /// </summary>
    public int idAssassino;
}