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

        StartCoroutine(AlphaRoutine());                                     // 데미지 표시
        StartCoroutine(SizeRoutine());                                      // 글자 크기 변경
    }

    public void ResetFont()
    {
        text.text = "";                                 // 글자 초기화
        text.fontSize = 250f;                           // 글자 크기 초기화
        text.color = Color.red;                         // 색상 초기화
        text.alpha = 0f;                                // 알파값 초기화
        text.fontStyle = FontStyles.Bold;             // 글자 스타일 초기화

        Helper.PoolManager.ReturnFontPool(this.gameObject); // 폰트 풀로 반환
    }

    public IEnumerator AlphaRoutine()
    {
        float duration = 0f;                // 글자 표시 지속 시간   
        float targetTime = 0.25f;              // 현재 시간

        while (duration < targetTime)
        {
            duration += Time.deltaTime;      // 시간 증가
            Color color = text.color;
            color.a = Mathf.Lerp(0f, 1f, duration / targetTime); // 알파값을 서서히 줄임
            text.color = color;             // 이 줄이 빠지면 적용되지 않음

            yield return null;              // 다음 프레임까지 대기
        }

    }

    public IEnumerator SizeRoutine()
    {
        float duration = 0f;                // 글자 표시 지속 시간   
        float targetTime = 0.5f;              // 현재 시간

        float targetFontSize = 120f;          // 최종 크기

        while (duration < targetTime)
        {
            duration += Time.deltaTime;      // 시간 증가
            text.fontSize = Mathf.Lerp(200, targetFontSize, duration / targetTime); // 글자 크기 서서히 줄임

            yield return null;              // 다음 프레임까지 대기
        }

        StartCoroutine(RectRoutine());  // 크기가 거의 다 줄어들면 RectTransform 이동 시작
    }

    public IEnumerator RectRoutine()
    {
        float speed = 15f; // 이동 속도 (조절 가능)
        RectTransform rect = this.gameObject.GetComponent<RectTransform>();
        Vector3 targetPos = rect.position + new Vector3(0f, 7.5f, 0f); // Z 유지

        while (Vector3.Distance(rect.position, targetPos) > 0.1f)
        {
            rect.position = Vector3.MoveTowards(rect.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        ResetFont();
    }
}
