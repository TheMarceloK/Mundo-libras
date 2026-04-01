using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Novo Par de Cartas", menuName = "Mundo Libras/Jogo da Memória/Par de Cartas")]
public class CardPairData : ScriptableObject
{
    public int pairID; // Um número único para identificar o par (ex: 1 para "Bom dia", 2 para "Obrigado")
    public Sprite librasSprite;
    public Sprite caricatureSprite;
    public VideoClip rewardVideo;
}