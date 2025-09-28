using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class teste : MonoBehaviour
{
    public VideoPlayer videoObjeto; // arraste no Inspector

    void Start()
    {

       // Carregar Vídeo
        VideoClip video = Resources.Load<VideoClip>("teste_video1");
        if (video != null && videoObjeto != null)
        {
            videoObjeto.clip = video;
            videoObjeto.Play();
        }
    }
}
