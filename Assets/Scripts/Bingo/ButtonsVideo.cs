using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ButtonsVideo : MonoBehaviour
{
    // Campos para arrastar os componentes da Unity no Inspector
    [Header("Componentes do Vídeo")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Configuração de Cena")]
    // Nome da cena do quebra-cabeça para onde você voltará
    [SerializeField] private string puzzleSceneName = "Menu";

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

    public void FinishPhase()
    {
        SceneManager.LoadScene(puzzleSceneName);
        Debug.Log("Fase concluída e voltando para a cena do jogo.");
    }

}

