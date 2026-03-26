using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public PlayerStats player;
    public CSVParser csvParser;
    public PlayerMovement playerMovement;
    public BattleUIManager uiManager;
    public BattleRippleEffect rippleEffect;

    [Header("Battle Settings")]
    public float turnDelay = 1.0f;

    private MonsterData currentMonster;
    private int monsterCurrentHp;
    private BattleState currentState = BattleState.Start;

    void Start()
    {
        StartCoroutine(PrepareFirstMonster());
    }

    IEnumerator PrepareFirstMonster()
    {
        yield return null;
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
            uiManager.AddLog("=== Game Over ===");
            StartCoroutine(EndBattleSequence());
            return;
        }

        uiManager.ResetBattle(player.currentHp, player.maxHp, currentMonster.hp);
        uiManager.SetEnemySprite(currentMonster.sprite);
        uiManager.UpdatePlayerHP(player.currentHp, player.maxHp);
        uiManager.UpdateEnemyHP(monsterCurrentHp, currentMonster.hp);
        uiManager.AddLog($"=== {currentMonster.monsterName} appeared! ===");
        uiManager.AddLog($"HP:{monsterCurrentHp} ATK:{currentMonster.attack} DEF:{currentMonster.defense}");

        SetState(BattleState.PlayerTurn);
    }

    void SetState(BattleState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case BattleState.PlayerTurn:
                uiManager.AddLog("--- Your Turn ---");
                uiManager.SetButtonsInteractable(true);
                break;

            case BattleState.EnemyTurn:
                uiManager.SetButtonsInteractable(false);
                StartCoroutine(EnemyTurn());
                break;

            case BattleState.Win:
                uiManager.SetButtonsInteractable(false);
                CheckDrop();
                uiManager.AddLog("=== Victory! ===");
                StartCoroutine(EndBattleSequence());
                break;

            case BattleState.Lose:
                uiManager.SetButtonsInteractable(false);
                uiManager.AddLog("=== Defeated... Game Over ===");
                StartCoroutine(EndBattleSequence());
                break;
        }
    }

    public void OnAttackButton()
    {
        if (currentState != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerAttack());
    }

    public void OnPotionButton()
    {
        if (currentState != BattleState.PlayerTurn) return;
        player.UsePotion(uiManager);
        uiManager.UpdatePlayerHP(player.currentHp, player.maxHp);
        SetState(BattleState.EnemyTurn);
    }

    IEnumerator PlayerAttack()
    {
        uiManager.SetButtonsInteractable(false);

        int playerDamage = Mathf.Max(0, player.attack - currentMonster.defense);
        monsterCurrentHp -= playerDamage;
        uiManager.UpdateEnemyHP(monsterCurrentHp, currentMonster.hp);
        uiManager.ShowDamage(playerDamage, false);
        rippleEffect.ShowEnemyHit();
        uiManager.AddLog($"{player.playerName} attacks! {currentMonster.monsterName} takes {playerDamage} damage!");
        uiManager.AddLog($"{currentMonster.monsterName} HP: {monsterCurrentHp}/{currentMonster.hp}");

        if (monsterCurrentHp <= 0)
        {
            SetState(BattleState.Win);
            yield break;
        }

        yield return new WaitForSeconds(turnDelay);
        SetState(BattleState.EnemyTurn);
    }

    IEnumerator EnemyTurn()
    {
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
            SetState(BattleState.Lose);
            yield break;
        }

        SetState(BattleState.PlayerTurn);
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

    public void ClearEnemy()
    {
        uiManager.ResetBattle(player.currentHp, player.maxHp, 0);
    }
}