using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Stat Card")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public GameObject statCard;
    public Image characterImage;

    [Header("Character Sprites")]
    public Sprite[] characterSprites;

    private UnitData[] characters;
    private UnitData selectedUnit;
    private int selectedIndex = -1;

    void Start()
    {
        characters = new UnitData[]
        {
            new UnitData("Warrior", 150, 20, 10),
            new UnitData("Mage",    450,  40,  5),
            new UnitData("Healer",  120, 10,  8)
        };

        characterImage.color = new Color(1, 1, 1, 0);
        statCard.SetActive(false);
    }

    public void OnCharacterClicked(int index)
    {
        selectedIndex = index;
        selectedUnit = characters[index];

        selectedUnit.GetStatus(out int hp, out int atk, out int def);

        nameText.text = selectedUnit.unitName;
        hpText.text = $"HP: {hp}";
        atkText.text = $"ATK: {atk}";
        defText.text = $"DEF: {def}";

        if (characterSprites != null && index < characterSprites.Length)
        {
            characterImage.sprite = characterSprites[index];
            characterImage.color = new Color(1, 1, 1, 1);
        }

        statCard.SetActive(true);
    }

    public void OnConfirmClicked()
    {
        if (selectedUnit == null || selectedIndex < 0) return;

        GameDataManager.selectedUnit = selectedUnit;
        GameDataManager.selectedType = (CharacterType)selectedIndex;

        SceneManager.LoadScene("MainScene");
    }
}