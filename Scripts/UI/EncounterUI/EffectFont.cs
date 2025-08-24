using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EffectFont : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void OnActiveFontEffect(int damage, bool isCritical)
    {
        text.fontStyle = isCritical
          ? FontStyles.Bold | FontStyles.Underline
          : FontStyles.Bold;

        text.text = damage.ToString();

        float randX = UnityEngine.Random.Range(-20f, 20f);
        float randY = UnityEngine.Random.Range(0, 40f);

        RectTransform rect = this.gameObject.GetComponent<RectTransform>();
        rect.position += new Vector3(randX, randY, 0f);

        StartCoroutine(AlphaRoutine());                                     // ������ ǥ��
        StartCoroutine(SizeRoutine());                                      // ���� ũ�� ����
    }

    public void ResetFont()
    {
        text.text = "";                                 // ���� �ʱ�ȭ
        text.fontSize = 250f;                           // ���� ũ�� �ʱ�ȭ
        text.color = Color.red;                         // ���� �ʱ�ȭ
        text.alpha = 0f;                                // ���İ� �ʱ�ȭ
        text.fontStyle = FontStyles.Bold;             // ���� ��Ÿ�� �ʱ�ȭ

        Helper.PoolManager.ReturnFontPool(this.gameObject); // ��Ʈ Ǯ�� ��ȯ
    }

    public IEnumerator AlphaRoutine()
    {
        float duration = 0f;                // ���� ǥ�� ���� �ð�   
        float targetTime = 0.25f;              // ���� �ð�

        while (duration < targetTime)
        {
            duration += Time.deltaTime;      // �ð� ����
            Color color = text.color;
            color.a = Mathf.Lerp(0f, 1f, duration / targetTime); // ���İ��� ������ ����
            text.color = color;             // �� ���� ������ ������� ����

            yield return null;              // ���� �����ӱ��� ���
        }

    }

    public IEnumerator SizeRoutine()
    {
        float duration = 0f;                // ���� ǥ�� ���� �ð�   
        float targetTime = 0.5f;              // ���� �ð�

        float targetFontSize = 120f;          // ���� ũ��

        while (duration < targetTime)
        {
            duration += Time.deltaTime;      // �ð� ����
            text.fontSize = Mathf.Lerp(200, targetFontSize, duration / targetTime); // ���� ũ�� ������ ����

            yield return null;              // ���� �����ӱ��� ���
        }

        StartCoroutine(RectRoutine());  // ũ�Ⱑ ���� �� �پ��� RectTransform �̵� ����
    }

    public IEnumerator RectRoutine()
    {
        float speed = 15f; // �̵� �ӵ� (���� ����)
        RectTransform rect = this.gameObject.GetComponent<RectTransform>();
        Vector3 targetPos = rect.position + new Vector3(0f, 7.5f, 0f); // Z ����

        while (Vector3.Distance(rect.position, targetPos) > 0.1f)
        {
            rect.position = Vector3.MoveTowards(rect.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        ResetFont();
    }
}
