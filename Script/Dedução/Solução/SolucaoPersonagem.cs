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
    /// Um identificador para a imagem do personagem. Pode ser o nome do arquivo (ex: "foto_bruno.png")
    /// ou um ID que voc� usa para buscar a imagem em um gerenciador de recursos.
    /// </summary>
    public string idDaFoto;

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
}