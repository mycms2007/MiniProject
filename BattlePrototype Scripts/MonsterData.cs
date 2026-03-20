using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "BattlePrototype/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public int hp;
    public int attack;
    public int defense;
    public string dropItem;
    public float dropRate;
    public Sprite sprite;
    public string spriteName;
}
