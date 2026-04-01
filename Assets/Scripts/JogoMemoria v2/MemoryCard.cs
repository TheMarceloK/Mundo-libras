using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MemoryCard : MonoBehaviour
{
    public Image cardImage;
    public Button cardButton;

    private Sprite frontSprite;
    private Sprite backSprite;
    public int myPairID { get; private set; }
    public VideoClip myVideo { get; private set; }

    // NOVO: Essa função faz a atribuição automática que você queria!
    private void Awake()
    {
        // Pega os componentes de Imagem e Botão que estão no mesmo objeto do Prefab
        if (cardImage == null) cardImage = GetComponent<Image>();
        if (cardButton == null) cardButton = GetComponent<Button>();

        // Aproveita e já configura o clique do botão automaticamente também
        cardButton.onClick.AddListener(OnClickCard);
    }

    public void SetupCard(int id, Sprite front, Sprite back, VideoClip video)
    {
        myPairID = id;
        frontSprite = front;
        backSprite = back;
        myVideo = video;

        // Agora o cardImage não será mais nulo aqui!
        cardImage.sprite = backSprite;
    }

    public void OnClickCard()
    {
        if (MemoryGameManager.Instance.IsInputBlocked) return;
        StartCoroutine(FlipAnimation(true));
        MemoryGameManager.Instance.CardClicked(this);
    }

    public IEnumerator FlipAnimation(bool showFront)
    {
        float duration = 0.2f;
        float time = 0;

        while (time < duration)
        {
            transform.localScale = new Vector3(Mathf.Lerp(1, 0, time / duration), 1, 1);
            time += Time.deltaTime;
            yield return null;
        }

        cardImage.sprite = showFront ? frontSprite : backSprite;
        cardButton.interactable = !showFront;

        time = 0;
        while (time < duration)
        {
            transform.localScale = new Vector3(Mathf.Lerp(0, 1, time / duration), 1, 1);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }
}