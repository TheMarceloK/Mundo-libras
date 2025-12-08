using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    // Campos para arrastar os componentes da Unity no Inspector
    [Header("Componentes do Vídeo")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Configuração de Cena")]
    // Nome da cena do quebra-cabeça para onde você voltará
    [SerializeField] private string puzzleSceneName = "NomeDaSuaCenaDoQuebraCabeca";

    void Start()
    {
        // Garante que o vídeo seja reproduzido automaticamente
        // (Isso também pode ser definido no Inspector)
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }

    /// <summary>
    /// Chamado pelo botão "Recomeçar Vídeo".
    /// </summary>
    public void RestartVideo()
    {
        if (videoPlayer != null)
        {
            // Para, define o tempo para 0 e começa a reproduzir
            videoPlayer.Stop();
            videoPlayer.time = 0;
            videoPlayer.Play();
            Debug.Log("Vídeo reiniciado.");
        }
    }

    /// <summary>
    /// Chamado pelo botão "Finalizar Fase" (Voltar para o jogo ou ir para a próxima fase).
    /// </summary>
    public void FinishPhase()
    {
        // Neste exemplo, ele volta para a cena do quebra-cabeça.
        // Você pode alterá-lo para SceneManager.LoadScene("NomeDaProximaCena");
        SceneManager.LoadScene(puzzleSceneName);
        Debug.Log("Fase concluída e voltando para a cena do jogo.");
    }

    // Dentro da classe VideoController

    void OnEnable()
    {
        // Adiciona um listener para quando a reprodução do vídeo terminar
        videoPlayer.loopPointReached += EndReached;
    }

    void OnDisable()
    {
        // Remove o listener para evitar erros quando o objeto for desativado/destruído
        videoPlayer.loopPointReached -= EndReached;
    }

    void EndReached(VideoPlayer vp)
    {
        // Esta função é chamada quando o vídeo atinge o final do loop/clip (se Loop estiver desmarcado)
        vp.Stop();
        // Você pode adicionar qualquer lógica aqui, como escurecer o Raw Image, etc.
        Debug.Log("O vídeo de conclusão terminou a reprodução.");
    }

}