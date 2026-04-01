using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para trocar de cena

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager Instance;

    [Header("Configuração do Nível")]
    public List<CardPairData> levelPairs;
    public Sprite cardBackSprite;
    public string nomeCenaSelecaoNiveis = "Menu_Selecao"; // Mude para o nome exato da sua cena

    [Header("Referências")]
    public GameObject cardPrefab;
    public Transform gridLayoutParent;

    // CORREÇÃO: Mudamos de VideoController para MemoryVideoController
    public MemoryVideoController videoController;

    // NOVO: Referência para a tela de Parabéns
    public GameObject gameOverPanel;

    public bool IsInputBlocked { get; private set; }

    private MemoryCard firstSelectedCard;
    private MemoryCard secondSelectedCard;
    private int matchesFound = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Garante que o painel de fim de jogo comece desligado
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        List<MemoryCard> generatedCards = new List<MemoryCard>();

        foreach (CardPairData pair in levelPairs)
        {
            GameObject card1Obj = Instantiate(cardPrefab, gridLayoutParent);
            MemoryCard card1 = card1Obj.GetComponent<MemoryCard>();
            card1.SetupCard(pair.pairID, pair.librasSprite, cardBackSprite, pair.rewardVideo);
            generatedCards.Add(card1);

            GameObject card2Obj = Instantiate(cardPrefab, gridLayoutParent);
            MemoryCard card2 = card2Obj.GetComponent<MemoryCard>();
            card2.SetupCard(pair.pairID, pair.caricatureSprite, cardBackSprite, pair.rewardVideo);
            generatedCards.Add(card2);
        }

        ShuffleCards(generatedCards);
    }

    private void ShuffleCards(List<MemoryCard> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);
            cards[i].transform.SetSiblingIndex(randomIndex);
        }
    }

    public void CardClicked(MemoryCard clickedCard)
    {
        if (firstSelectedCard == null)
        {
            firstSelectedCard = clickedCard;
        }
        else
        {
            secondSelectedCard = clickedCard;
            StartCoroutine(CheckMatchCoroutine());
        }
    }

    private IEnumerator CheckMatchCoroutine()
    {
        IsInputBlocked = true;

        yield return new WaitForSeconds(0.5f);

        if (firstSelectedCard.myPairID == secondSelectedCard.myPairID)
        {
            matchesFound++;

            // NOVO: As cartas desaparecem do Grid
            firstSelectedCard.gameObject.SetActive(false);
            secondSelectedCard.gameObject.SetActive(false);

            // Mostra o vídeo
            videoController.ShowVideo(firstSelectedCard.myVideo);

            firstSelectedCard = null;
            secondSelectedCard = null;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(firstSelectedCard.FlipAnimation(false));
            StartCoroutine(secondSelectedCard.FlipAnimation(false));
            firstSelectedCard = null;
            secondSelectedCard = null;
        }

        IsInputBlocked = false;
    }

    // NOVO: Método para verificar se o jogo acabou
    public void CheckGameOver()
    {
        // Compara os acertos com a quantidade total de pares (agora escalável para 4 pares ou mais)
        if (matchesFound >= levelPairs.Count)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
    }

    // NOVO: Método para o botão da tela de Parabéns
    public void VoltarParaSelecao()
    {
        SceneManager.LoadScene(nomeCenaSelecaoNiveis);
    }
}