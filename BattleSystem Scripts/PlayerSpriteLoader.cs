using UnityEngine;
using UnityEngine.UI;

public class PlayerSpriteLoader : MonoBehaviour
{
    [Header("Character Sprites")]
    public Sprite warriorSprite;
    public Sprite mageSprite;
    public Sprite healerSprite;

    [Header("UI")]
    public Image playerCharImage;

    void Awake()
    {
        Sprite selected = GameDataManager.selectedType switch
        {
            CharacterType.Warrior => warriorSprite,
            CharacterType.Mage => mageSprite,
            CharacterType.Healer => healerSprite,
            _ => null
        };

        if (selected == null) return;

        // Sprite Renderer (필드 캐릭터)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = selected;

        // UI Image (전투 패널 캐릭터)
        if (playerCharImage != null) playerCharImage.sprite = selected;
    }
}