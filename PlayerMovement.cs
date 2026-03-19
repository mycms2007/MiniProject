using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveDelay = 0.2f;
    public int encounterStep = 3;
    public BattleManager battleManager;
    public GameObject battlePanel;
    public FadeManager fadeManager;

    private int stepCount = 0;
    private bool isMoving = false;
    private bool inBattle = false;

    void Update()
    {
        if (isMoving || inBattle) return;

        Vector2 dir = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            dir = Vector2.right;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            dir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            dir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            dir = Vector2.down;

        if (dir != Vector2.zero)
            StartCoroutine(Move(dir));
    }

    IEnumerator Move(Vector2 dir)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(dir.x, dir.y, 0);
        float elapsed = 0f;

        while (elapsed < moveDelay)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDelay);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
        stepCount++;

        if (stepCount % encounterStep == 0)
        {
            StartCoroutine(StartEncounter());
        }
    }

    IEnumerator StartEncounter()
    {
        inBattle = true;
        yield return StartCoroutine(fadeManager.FadeOut());
        battlePanel.SetActive(true);
        yield return StartCoroutine(fadeManager.FadeIn());
        battleManager.StartBattle();
    }

    public void OnBattleEnd()
    {
        StartCoroutine(EndEncounter());
    }

    IEnumerator EndEncounter()
    {
        yield return StartCoroutine(fadeManager.FadeOut());
        battleManager.ClearEnemy();
        battleManager.PrepareMonster(); // 다음 전투 몬스터 미리 준비
        battlePanel.SetActive(false);
        yield return StartCoroutine(fadeManager.FadeIn());
        inBattle = false;
    }
}