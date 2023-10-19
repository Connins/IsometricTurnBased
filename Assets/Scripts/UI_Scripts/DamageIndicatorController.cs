using System.Collections;
using TMPro;
using UnityEngine;



public class DamageIndicatorController : MonoBehaviour
{
    public float fadeTime = 1f;
    public float moveSpeed = 1f;
    public float moveDistance = 1f;
    public int damageValue = 0;

    private TMP_Text damageText;


    // Use this for initialization
    void Start()
    {
        damageText = GetComponentInChildren<TMP_Text>();
    }

    public void startDamageIndicatorCoroutine(int value)
    {
        damageText.enabled = true;
        damageValue = value;
        damageText.text = damageValue.ToString();
        StartCoroutine(FadeAndMove());
    }
    IEnumerator FadeAndMove()
    {
        float alpha = 1f;
        Vector3 startPos = damageText.rectTransform.position;
        Vector3 endPos = startPos + new Vector3(0, moveDistance, 0);

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeTime;
            damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, alpha);
            damageText.rectTransform.position = Vector3.Lerp(damageText.rectTransform.position, endPos, (moveSpeed * Time.deltaTime));

            yield return null;
        }
        damageText.rectTransform.position = startPos;
        damageText.enabled = false;
    }
}
