public class UnitData
{
    public string unitName;
    public int maxHp;
    public int attack;
    public int defense;

    public UnitData(string name, int hp, int atk, int def)
    {
        unitName = name;
        maxHp = hp;
        attack = atk;
        defense = def;
    }

    public void GetStatus(out int hp, out int atk, out int def)
    {
        hp = maxHp;
        atk = attack;
        def = defense;
    }
}