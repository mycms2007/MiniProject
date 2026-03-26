using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleRippleEffect : MonoBehaviour
{
    [Header("Ripple Settings")]
    [Range(1, 5)]
    public int rippleCount = 3;

    [Range(50f, 500f)]
    public float maxSize = 300f;

    [Range(0.1f, 2f)]
    public float animationDuration = 0.8f;

    [Range(0f, 0.5f)]
    public float rippleDelay = 0.1f;

    [Range(1f, 20f)]
    public float ringThickness = 5f;

    [Header("Color Settings")]
    public Color playerHitColor = Color.red;
    public Color enemyHitColor = Color.yellow;

    [Range(0f, 5f)]
    public float emissionIntensity = 2f;

    [Header("Position")]
    public RectTransform playerPosition;
    public RectTransform enemyPosition;

    private Canvas effectCanvas;
    private RectTransform effectCanvasRect;

    void Start()
    {
        CreateEffectCanvas();
    }

    void CreateEffectCanvas()
    {
        GameObject canvasObj = new GameObject("BattleRipple_Canvas");
        canvasObj.layer = LayerMask.NameToLayer("UI");

        effectCanvas = canvasObj.AddComponent<Canvas>();
        effectCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        effectCanvas.sortingOrder = 32767;

        var raycaster = canvasObj.AddComponent<GraphicRaycaster>();
        raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

        CanvasGroup canvasGroup = canvasObj.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        effectCanvasRect = effectCanvas.GetComponent<RectTransform>();
    }

    public void ShowPlayerHit()
    {
        if (playerPosition == null) return;
        Vector3 screenPos = RectTransformToScreenPoint(playerPosition);
        StartCoroutine(SpawnRipples(screenPos, playerHitColor));
    }

    public void ShowEnemyHit()
    {
        if (enemyPosition == null) return;
        Vector3 screenPos = RectTransformToScreenPoint(enemyPosition);
        StartCoroutine(SpawnRipples(screenPos, enemyHitColor));
    }

    Vector3 RectTransformToScreenPoint(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        Vector3 center = (corners[0] + corners[2]) / 2f;
        return RectTransformUtility.WorldToScreenPoint(null, center);
    }

    IEnumerator SpawnRipples(Vector3 screenPosition, Color color)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            effectCanvasRect,
            screenPosition,
            null,
            out localPoint
        );

        for (int i = 0; i < rippleCount; i++)
        {
            StartCoroutine(CreateRipple(localPoint, i * rippleDelay, color));
        }
        yield return null;
    }

    IEnumerator CreateRipple(Vector2 position, float delay, Color color)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        GameObject rippleObj = new GameObject("Ripple");
        rippleObj.transform.SetParent(effectCanvas.transform, false);

        Image rippleImage = rippleObj.AddComponent<Image>();
        rippleImage.sprite = CreateCircleSprite();
        rippleImage.color = color * emissionIntensity;
        rippleImage.raycastTarget = false;

        RectTransform rectTransform = rippleImage.rectTransform;
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = Vector2.zero;

        Outline outline = rippleObj.AddComponent<Outline>();
        outline.effectColor = color * emissionIntensity;
        outline.effectDistance = new Vector2(ringThickness, ringThickness);
        outline.useGraphicAlpha = true;

        float elapsed = 0f;
        Vector2 startSize = Vector2.zero;
        Vector2 endSize = Vector2.one * maxSize;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            rectTransform.sizeDelta = Vector2.Lerp(startSize, endSize, t);

            Color c = color * emissionIntensity;
            c.a = Mathf.Lerp(1f, 0f, t);
            rippleImage.color = c;
            outline.effectColor = c;

            yield return null;
        }

        Destroy(rippleObj);
    }

    Sprite CreateCircleSprite()
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float dist = Vector2.Distance(pos, center);
                float innerRadius = radius - ringThickness;
                pixels[y * size + x] = (dist > innerRadius && dist < radius) ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
