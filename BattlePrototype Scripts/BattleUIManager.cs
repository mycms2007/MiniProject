using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text;

public class BattleUIManager : MonoBehaviour
{
    [Header("HP 바")]
    public Image playerHPFill;
    public Image enemyHPFill;

    [Header("텍스트")]
    public TextMeshProUGUI logText;
    public ScrollRect logScrollRect;

    [Header("이미지")]
    public Image playerImage;
    public Image enemyImage;

    [Header("데미지 텍스트")]
    public TextMeshProUGUI damagedText;
    public TextMeshProUGUI attackText;

    private StringBuilder log = new StringBuilder();
    private float targetPlayerFill = 1f;
    private float targetEnemyFill = 1f;

    void Update()
    {
        playerHPFill.fillAmount = Mathf.Lerp(playerHPFill.fillAmount, targetPlayerFill, Time.deltaTime * 5f);
        enemyHPFill.fillAmount = Mathf.Lerp(enemyHPFill.fillAmount, targetEnemyFill, Time.deltaTime * 5f);
    }

    public void UpdatePlayerHP(int current, int max)
    {
        targetPlayerFill = (float)current / max;
    }

    public void UpdateEnemyHP(int current, int max)
    {
        targetEnemyFill = (float)current / max;
    }

    public void ShowDamage(int damage, bool isPlayer)
    {
        if (isPlayer)
        {
            StopCoroutine("AnimateDamagedText");
            StartCoroutine(AnimateDamageText(damagedText, damage));
        }
        else
        {
            StopCoroutine("AnimateAttackText");
            StartCoroutine(AnimateDamageText(attackText, damage));
        }
    }

    IEnumerator AnimateDamageText(TextMeshProUGUI text, int damage)
    {
        text.text = $"-{damage}";
        text.color = new Color(1, 0, 0, 1);

        Vector3 startPos = text.rectTransform.anchoredPosition;
        Vector3 targetPos = startPos + new Vector3(0, 50f, 0);
        float elapsed = 0f;
        float duration = 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            text.rectTransform.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);
            text.color = new Color(1, 0, 0, 1 - t);
            yield return null;
        }

        text.text = "";
        text.rectTransform.anchoredPosition = startPos;
        text.color = new Color(1, 0, 0, 1);
    }

    public void AddLog(string message)
    {
        log.AppendLine(message);
        logText.text = log.ToString();
        StopCoroutine("ScrollToBottom");
        StartCoroutine("ScrollToBottom");
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        float start = logScrollRect.verticalNormalizedPosition;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            logScrollRect.verticalNormalizedPosition = Mathf.Lerp(start, 0f, elapsed / duration);
            yield return null;
        }
        logScrollRect.verticalNormalizedPosition = 0f;
    }

    public void ClearLog()
    {
        log.Clear();
        logText.text = "";
    }

    public void ResetBattle(int playerCurrentHp, int playerMaxHp, int enemyMaxHp)
    {
        targetPlayerFill = (float)playerCurrentHp / playerMaxHp;
        targetEnemyFill = 1f;
        playerHPFill.fillAmount = targetPlayerFill;
        enemyHPFill.fillAmount = 1f;
        enemyImage.sprite = null;
        enemyImage.enabled = false;
        if (damagedText != null) damagedText.text = "";
        if (attackText != null) attackText.text = "";
        ClearLog();
    }

    public void SetEnemySprite(Sprite sprite)
    {
        enemyImage.sprite = sprite;
        enemyImage.enabled = true;
        enemyImage.color = new Color(1, 1, 1, 1);
    }
}