using UnityEngine;

public enum PotionType { Health, Mana }

public class PlayerStats : MonoBehaviour
{
    public string playerName = "MS";
    public int maxHp = 100;
    public int currentHp;
    public int attack = 15;
    public int defense = 5;

    [Header("Potion")]
    public int potionCount = 3;
    public int potionHealAmount = 50;

    void Awake()
    {
        if (GameDataManager.selectedUnit != null)
        {
            playerName = GameDataManager.selectedUnit.unitName;
            maxHp = GameDataManager.selectedUnit.maxHp;
            attack = GameDataManager.selectedUnit.attack;
            defense = GameDataManager.selectedUnit.defense;
        }
        currentHp = maxHp;
    }

    public void UsePotion(BattleUIManager uiManager)
    {
        UsePotion(PotionType.Health, ref potionCount, ref currentHp, uiManager);
    }

    private void UsePotion(PotionType type, ref int count, ref int hp, BattleUIManager uiManager)
    {
        if (count <= 0)
        {
            uiManager.AddLog("No potions left!");
            return;
        }

        string result = type switch
        {
            PotionType.Health => HealPlayer(ref hp),
            PotionType.Mana => "Mana is already full.",
            _ => "Unknown potion."
        };

        count--;
        uiManager.AddLog(result);
        uiManager.AddLog($"Potions remaining: {count}");

        string hpStatus = hp switch
        {
            var h when h >= maxHp => $"{playerName} HP is full!",
            var h when h >= maxHp * 0.7f => $"{playerName} is in good condition.",
            var h when h >= maxHp * 0.3f => $"{playerName} needs caution.",
            _ => $"{playerName} is in danger! HP is very low!"
        };

        uiManager.AddLog(hpStatus);
    }

    private string HealPlayer(ref int hp)
    {
        int healAmount = Mathf.Min(potionHealAmount, maxHp - hp);
        hp += healAmount;
        return $"Recovered {healAmount} HP! (Current HP: {hp}/{maxHp})";
    }
}