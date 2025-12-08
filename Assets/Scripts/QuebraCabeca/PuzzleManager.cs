using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// PuzzleManager (versão ajustada)
/// Gera peças a partir da fullImage, posiciona dentro do puzzleArea (UI)
/// e mostra endPanel quando todas as peças são colocadas.
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    [Header("Referências")]
    public Texture2D fullImage;          // imagem original da fase
    public GameObject piecePrefab;       // prefab da peça (UI > RawImage + PuzzlePiece)
    public RectTransform puzzleArea;     // painel onde as peças serão colocadas

    [Header("Configuração do Quebra-Cabeça")]
    public int rows = 2;
    public int columns = 4;
    public float pieceSpacing = 5f;

    [Header("Snap")]
    public float snapDistance = 60f;

    [Header("Identificação da Fase")]
    public string faseNome;

    [Header("UI de Fim de Fase")]
    public GameObject endPanel;

    private List<RectTransform> pieces = new List<RectTransform>();

    void Start()
    {

        if (fullImage != null && piecePrefab != null && puzzleArea != null)
            GeneratePuzzle();
        else
            Debug.LogWarning("PuzzleManager: fullImage / piecePrefab / puzzleArea não configurados no Inspector.");
    }

    public void GeneratePuzzle()
    {
        ClearPieces();

        float pieceWidth = (puzzleArea.rect.width - (columns - 1) * pieceSpacing) / columns;
        float pieceHeight = (puzzleArea.rect.height - (rows - 1) * pieceSpacing) / rows;

        // remover Slots antigos se existirem
        Transform existingSlots = puzzleArea.Find("Slots");
        if (existingSlots != null) DestroyImmediate(existingSlots.gameObject);
        GameObject slotParent = new GameObject("Slots");
        slotParent.transform.SetParent(puzzleArea, false);
        RectTransform slotParentRT = slotParent.AddComponent<RectTransform>();
        slotParentRT.anchorMin = slotParentRT.anchorMax = new Vector2(0.5f, 0.5f);
        slotParentRT.pivot = new Vector2(0.5f, 0.5f);
        slotParentRT.anchoredPosition = Vector2.zero;
        slotParentRT.sizeDelta = puzzleArea.sizeDelta;

        // ponto inicial centralizado
        float startX = -((columns - 1) * (pieceWidth + pieceSpacing)) / 2f;
        float startY = ((rows - 1) * (pieceHeight + pieceSpacing)) / 2f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int texWidth = fullImage.width / columns;
                int texHeight = fullImage.height / rows;


                int yPixel = fullImage.height - (row + 1) * texHeight;

                Texture2D pieceTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
                Color[] px = fullImage.GetPixels(col * texWidth, yPixel, texWidth, texHeight);
                pieceTexture.SetPixels(px);
                pieceTexture.Apply();

                GameObject pieceObj = Instantiate(piecePrefab, puzzleArea);
                pieceObj.name = $"Piece_{row}_{col}";

                RectTransform rtPiece = pieceObj.GetComponent<RectTransform>();
                rtPiece.localScale = Vector3.one;
                rtPiece.pivot = new Vector2(0.5f, 0.5f);
                rtPiece.anchorMin = rtPiece.anchorMax = new Vector2(0.5f, 0.5f);
                rtPiece.sizeDelta = new Vector2(pieceWidth, pieceHeight);

                RawImage ri = pieceObj.GetComponent<RawImage>();
                if (ri != null) ri.texture = pieceTexture;

                GameObject slot = new GameObject($"Slot_{row}_{col}", typeof(RectTransform));
                slot.transform.SetParent(slotParent.transform, false);
                RectTransform slotRT = slot.GetComponent<RectTransform>();
                slotRT.pivot = new Vector2(0.5f, 0.5f);
                slotRT.anchorMin = slotRT.anchorMax = new Vector2(0.5f, 0.5f);
                slotRT.sizeDelta = rtPiece.sizeDelta;

                Vector2 correctPos = new Vector2(
                    startX + col * (pieceWidth + pieceSpacing),
                    startY - row * (pieceHeight + pieceSpacing)
                );
                slotRT.anchoredPosition = correctPos;

                PuzzlePiece pp = pieceObj.GetComponent<PuzzlePiece>();
                pp.manager = this;
                pp.targetSlot = slotRT;
                pp.snapDistance = snapDistance;

                // posição inicial aleatória
                rtPiece.anchoredPosition = new Vector2(
                    Random.Range(-puzzleArea.rect.width / 2f, puzzleArea.rect.width / 2f),
                    Random.Range(-puzzleArea.rect.height / 2f, puzzleArea.rect.height / 2f)
                );
            }
        }

        if (endPanel != null) endPanel.SetActive(false);
    }



    private Vector2 GetRandomStartPosition(RectTransform rt, Vector2 correctPos)
    {
        float halfW = puzzleArea.rect.width / 2f - rt.sizeDelta.x / 2f;
        float halfH = puzzleArea.rect.height / 2f - rt.sizeDelta.y / 2f;

        Vector2 pos;
        int tries = 0;
        do
        {
            float rx = Random.Range(-halfW, halfW);
            float ry = Random.Range(-halfH, halfH);
            pos = new Vector2(rx, ry);
            tries++;
            if (tries > 100) break;
        } while (Vector2.Distance(pos, correctPos) < snapDistance * 1.5f);

        return pos;
    }

    private void ClearPieces()
    {
        foreach (Transform t in puzzleArea)
        {
            Destroy(t.gameObject);
        }
        pieces.Clear();
    }

    public void NotifyPiecePlaced(PuzzlePiece piece)
    {
        foreach (Transform t in puzzleArea)
        {
            PuzzlePiece p = t.GetComponent<PuzzlePiece>();
            if (p != null && !p.GetIsPlaced()) return;
        }

        OnAllPiecesPlaced();
    }

    private void OnAllPiecesPlaced()
    {
        Debug.Log("Puzzle completo!");

        if (endPanel != null)
        {
            endPanel.SetActive(true);

            //VideoUI videoUI = endPanel.GetComponent<VideoUI>();
            //if (videoUI != null)
            //{
            //    videoUI.LoadAndPlayVideo(faseNome);
            //}
        }
    }
}

