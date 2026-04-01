using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections; // Necessário para a Coroutine

public class MemoryVideoController : MonoBehaviour
{
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;
    public Button btnPlayAgain;
    public Button btnContinue;

    void Awake()
    {
        btnPlayAgain.onClick.AddListener(PlayVideoAgain);
        btnContinue.onClick.AddListener(ClosePanel);
        // Garante que o painel comece desligado
        videoPanel.SetActive(false);
    }

    public void ShowVideo(VideoClip clip)
    {
        // 1. Ativa o painel e os componentes
        videoPanel.SetActive(true);
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.enabled = true;

        // 2. Carrega o vídeo
        videoPlayer.clip = clip;

        // 3. Chama a rotina para esperar a Unity "acordar" o player antes do Play
        StartCoroutine(WaitAndPlay());
    }

    private IEnumerator WaitAndPlay()
    {
        // Espera o final do frame atual (resolve o erro de VideoPlayer desabilitado)
        yield return new WaitForEndOfFrame();
        videoPlayer.Play();
    }

    public void PlayVideoAgain()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
    }

    public void ClosePanel()
    {
        videoPlayer.Stop();
        videoPanel.SetActive(false);
        MemoryGameManager.Instance.CheckGameOver();
    }
}