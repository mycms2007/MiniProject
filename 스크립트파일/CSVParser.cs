using UnityEngine;
using System.Collections.Generic;

public class CSVParser : MonoBehaviour
{
    public MonsterData[] monsters;

    void Awake()
    {
        monsters = ParseCSV("monsters");
    }

    public MonsterData[] ParseCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        string[] lines = csvFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        List<MonsterData> result = new List<MonsterData>();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 7) continue;

            MonsterData data = ScriptableObject.CreateInstance<MonsterData>();
            data.monsterName = values[0].Trim();
            data.hp = int.Parse(values[1].Trim());
            data.attack = int.Parse(values[2].Trim());
            data.defense = int.Parse(values[3].Trim());
            data.dropItem = values[4].Trim();
            data.dropRate = float.Parse(values[5].Trim());
            data.spriteName = values[6].Trim();
            data.sprite = Resources.Load<Sprite>($"Sprites/{data.spriteName}");

            result.Add(data);
        }

        return result.ToArray();
    }
}