// O atributo [System.Serializable] é extremamente importante para o Unity.
// Ele permite que você crie e edite instâncias desta classe diretamente
// no Inspector da Unity, tornando muito fácil preencher os dados do seu jogo
// sem precisar alterar o código.
[System.Serializable]
public class SolucaoPersonagem
{
    // NOTA: Esta classe não herda de MonoBehaviour, pois ela é apenas um
    // contêiner de dados, não um componente que vive em um GameObject.

    /// <summary>
    /// ID único para referenciar este personagem de forma programática (ex: 1, 2, 3...).
    /// </summary>
    public int id;

    /// <summary>
    /// O nome completo e correto do personagem.
    /// </summary>
    public string nomeCompleto;

    /// <summary>
    /// Um identificador para a imagem do personagem. Pode ser o nome do arquivo (ex: "foto_bruno.png")
    /// ou um ID que você usa para buscar a imagem em um gerenciador de recursos.
    /// </summary>
    public string idDaFoto;

    /// <summary>
    /// O verdadeiro envolvimento deste personagem no roubo.
    /// Usa o enum que definimos para garantir consistência.
    /// </summary>
    public PapelNoRoubo papel;

    /// <summary>
    /// O verdadeiro destino deste personagem.
    /// Usa o enum para garantir consistência.
    /// </summary>
    public DestinoFinal destino;
}