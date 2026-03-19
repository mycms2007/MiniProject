using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public PlayerStats player;
    public CSVParser csvParser;
    public PlayerMovement playerMovement;
    public BattleUIManager uiManager;

    [Header("전투 설정")]
    public float turnDelay = 1.0f;

    private MonsterData currentMonster;
    private int monsterCurrentHp;
    public BattleRippleEffect rippleEffect;

    void Start()
    {
        // 첫 번째 몬스터 미리 준비
        StartCoroutine(PrepareFirstMonster());
    }

    IEnumerator PrepareFirstMonster()
    {
        yield return null; // CSVParser Awake 끝날 때까지 한 프레임 대기
        PrepareMonster();
    }

    public void PrepareMonster()
    {
        if (csvParser.monsters == null || csvParser.monsters.Length == 0) return;

        int randomIndex = Random.Range(0, csvParser.monsters.Length);
        currentMonster = csvParser.monsters[randomIndex];
        monsterCurrentHp = currentMonster.hp;

        uiManager.SetEnemySprite(currentMonster.sprite);
    }

    public void StartBattle()
    {
        if (currentMonster == null) return;
        if (player.currentHp <= 0)
        {
            uiManager.AddLog("=== Player defeated. Game Over ===");
            StartCoroutine(EndBattleSequence());
            return;
        }

        uiManager.ResetBattle(player.currentHp, player.maxHp, currentMonster.hp);
        uiManager.SetEnemySprite(currentMonster.sprite);
        uiManager.UpdatePlayerHP(player.currentHp, player.maxHp);
        uiManager.UpdateEnemyHP(monsterCurrentHp, currentMonster.hp);
        uiManager.AddLog($"=== {currentMonster.monsterName} appeared! ===");
        uiManager.AddLog($"HP:{monsterCurrentHp} ATK:{currentMonster.attack} DEF:{currentMonster.defense}");

        StartCoroutine(TakeTurn());
    }

    public void ClearEnemy()
    {
        uiManager.ResetBattle(player.currentHp, player.maxHp, 0);
    }

    IEnumerator TakeTurn()
    {
        yield return new WaitForSeconds(turnDelay);

        int playerDamage = Mathf.Max(0, player.attack - currentMonster.defense);
        monsterCurrentHp -= playerDamage;
        uiManager.UpdateEnemyHP(monsterCurrentHp, currentMonster.hp);
        uiManager.ShowDamage(playerDamage, false);
        rippleEffect.ShowEnemyHit();
        uiManager.AddLog($"{player.playerName} attacks! {currentMonster.monsterName} takes {playerDamage} damage!");
        uiManager.AddLog($"{currentMonster.monsterName} HP: {monsterCurrentHp}/{currentMonster.hp}");

        if (monsterCurrentHp <= 0)
        {
            uiManager.AddLog($"{currentMonster.monsterName} defeated!");
            CheckDrop();
            yield return new WaitForSeconds(turnDelay);
            StartCoroutine(EndBattleSequence());
            yield break;
        }

        yield return new WaitForSeconds(turnDelay);

        int monsterDamage = Mathf.Max(0, currentMonster.attack - player.defense);
        player.currentHp -= monsterDamage;
        uiManager.UpdatePlayerHP(player.currentHp, player.maxHp);
        uiManager.ShowDamage(monsterDamage, true);
        rippleEffect.ShowPlayerHit();
        uiManager.AddLog($"{currentMonster.monsterName} attacks! {player.playerName} takes {monsterDamage} damage!");
        uiManager.AddLog($"{player.playerName} HP: {player.currentHp}/{player.maxHp}");

        if (player.currentHp <= 0)
        {
            uiManager.AddLog($"{player.playerName} defeated... Game Over");
            StartCoroutine(EndBattleSequence());
            yield break;
        }

        StartCoroutine(TakeTurn());
    }

    void CheckDrop()
    {
        if (Random.value < currentMonster.dropRate)
            uiManager.AddLog($"{currentMonster.dropItem} obtained!");
        else
            uiManager.AddLog("No drop this time.");
    }

    IEnumerator EndBattleSequence()
    {
        yield return new WaitForSeconds(1.5f);
        playerMovement.OnBattleEnd();
    }
}