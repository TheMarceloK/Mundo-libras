using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MinigameMemoria : MonoBehaviour
{
    public Image imagemObjeto;    // arraste no Inspector
    public VideoPlayer videoObjeto; // arraste no Inspector

    void Start()
    {
        Fase fase = GameManager.instancia.faseAtual;

        if (fase == null)
        {
            Debug.LogError("Nenhuma fase carregada!");
            return;
        }

        Debug.Log($"Iniciando fase {fase.id} do tipo {fase.tipo}. Palavra: {fase.palavraDebug}");

        // Carregar Imagem
        Sprite sprite = Resources.Load<Sprite>(fase.imagem);
        if (sprite != null && imagemObjeto != null)
            imagemObjeto.sprite = sprite;

        // Carregar Vídeo
        VideoClip video = Resources.Load<VideoClip>(fase.video);
        
        if (video != null && videoObjeto != null)
        {
            videoObjeto.clip = video;
            videoObjeto.Play();
        }
    }
}
