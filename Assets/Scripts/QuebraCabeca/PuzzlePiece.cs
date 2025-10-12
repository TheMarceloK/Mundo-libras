using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// PuzzlePiece (com sistema de snap por proximidade de slot)
/// Detecta o centro da peça ou o ponteiro passando pelo ponto alvo (slot)
/// e fixa imediatamente, mesmo durante o arraste.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class PuzzlePiece : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [HideInInspector] public PuzzleManager manager;
    [HideInInspector] public RectTransform targetSlot;   // definido pelo PuzzleManager
    [HideInInspector] public float snapDistance = 60f;   // distância em pixels de tela para encaixar

    private RectTransform rt;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private bool isPlaced = false;
    private bool isDragging = false;
    private Vector2 dragOffset;

    public bool GetIsPlaced() => isPlaced;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // Corrige pivô e anchors para comportamento consistente
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isPlaced) return;
        isDragging = true;

        rt.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.95f;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        dragOffset = rt.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced || !isDragging) return;

        RectTransform parentRT = rt.parent as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            rt.anchoredPosition = localPoint + dragOffset;
        }

        // Verifica encaixe em tempo real
        Vector2 pieceCenterScreen = RectTransformUtility.WorldToScreenPoint(canvas != null ? canvas.worldCamera : null, rt.TransformPoint(Vector3.zero));
        Vector2 slotCenterScreen = RectTransformUtility.WorldToScreenPoint(canvas != null ? canvas.worldCamera : null, manager.puzzleArea.TransformPoint(targetSlot.anchoredPosition));

        float distCenter = Vector2.Distance(pieceCenterScreen, slotCenterScreen);
        if (distCenter <= snapDistance)
        {
            SnapToSlot();
            return;
        }

        float pointerDist = Vector2.Distance(eventData.position, slotCenterScreen);
        if (pointerDist <= snapDistance)
        {
            SnapToSlot();
            return;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPlaced)
        {
            isDragging = false;
            return;
        }

        // Fallback: checar se soltou próximo ao slot
        float localDist = Vector2.Distance(rt.anchoredPosition, targetSlot.anchoredPosition);
        if (localDist <= (snapDistance / 2f))
        {
            SnapToSlot();
        }
        else
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        isDragging = false;
    }

    private void SnapToSlot()
    {
        if (isPlaced) return;

        isPlaced = true;
        isDragging = false;
        canvasGroup.blocksRaycasts = false;

        // Coloca a peça atrás das outras (para não cobrir as soltas)
        rt.SetAsFirstSibling();

        // ?? Toca som de encaixe
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySnapSound();

        // Vibra no mobile
        #if UNITY_ANDROID
            Handheld.Vibrate();
        #endif

        // Anima suavemente o encaixe
        StartCoroutine(SmoothSnapCoroutine(targetSlot.anchoredPosition, 0.15f));
    }


    private System.Collections.IEnumerator SmoothSnapCoroutine(Vector2 targetAnchored, float duration)
    {
        Vector2 start = rt.anchoredPosition;
        float t = 0f;

        rt.localScale = Vector3.one * 1.08f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float f = Mathf.SmoothStep(0f, 1f, t / duration);
            rt.anchoredPosition = Vector2.Lerp(start, targetAnchored, f);
            rt.localScale = Vector3.Lerp(Vector3.one * 1.08f, Vector3.one, f);
            yield return null;
        }

        rt.anchoredPosition = targetAnchored;
        rt.localScale = Vector3.one;

        manager.NotifyPiecePlaced(this);
    }
}
