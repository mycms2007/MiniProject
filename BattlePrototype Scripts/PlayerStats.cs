using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public string playerName = "Hunters";
    public int maxHp = 100;
    public int currentHp;
    public int attack = 15;
    public int defense = 5;

    void Awake()
    {
        currentHp = maxHp;
    }
}
