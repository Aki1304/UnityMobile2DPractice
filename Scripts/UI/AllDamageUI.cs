using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllDamageUI : MonoBehaviour
{
    public int totalDamage = 0;

    [SerializeField] private TextMeshProUGUI _damgeTMP;

    [Header("상위 오브젝트")]
    [SerializeField] private GameObject _damageObject;
    [SerializeField] private RectTransform rect;

    private bool isAct = false;

    private float _normalSize = 80f;              // 일반 글자 크기 설정
    public float _focusSize = 120f;              // 글자 크기 설정

    private void Start()
    {
        Helper.UM.GetAllDMGUI = this;
    }

    public void OnAllDamgeUI()
    {
        isAct = true;

        rect.anchoredPosition = new Vector2(0f, -200f);

        _damageObject.SetActive(true);
        StartCoroutine(ActionFont());
    }

    public IEnumerator ActionFont()
    {
        string stringText = totalDamage.ToString();
        string tempText = "";

        for(int i = 0; i < stringText.Length; i++)
        {
            tempText += stringText[i];
            _damgeTMP.text = tempText;
            yield return new WaitForSeconds(0.05f);
        }

        _damgeTMP.fontSize = _focusSize;              // 글자 크기 설정
        StartCoroutine(RoutineCheck());               // 코루틴 시작
    }

    private IEnumerator RoutineCheck()
    {
        float duraiton = 0f;
        float targetTime = 2.5f;

        float targetPosX = 400f;

        while(duraiton < targetTime)
        {
            // if문으로 조건 생기면 그때 바로 나가게
            if (!isAct) break;

            duraiton += Time.deltaTime;          // 시간 증가 
            yield return null;
        }

        while(true)
        {
            // if문으로 조건 생기면 그때 바로 나가게
            if (!isAct) break;

            float velocity = 0f; // 클래스 멤버 변수로 선언 필요
            float update = Mathf.SmoothDamp(rect.anchoredPosition.x, targetPosX, ref velocity, 0.1f);

            rect.anchoredPosition = new Vector2(update, rect.anchoredPosition.y);

            if(rect.anchoredPosition.x == targetPosX) { break; }
            yield return null;
        }

        // 혹시 바로 나올경우를 대비
        rect.anchoredPosition = new Vector2(targetPosX, rect.anchoredPosition.y);
    }


    public void ResetDamage()
    {
        isAct = false;                        // 코루틴 즉시 종료
        _damageObject.SetActive(false);

        totalDamage = 0;
        _damgeTMP.text = totalDamage.ToString();

        _damgeTMP.fontSize = _normalSize;      // 글자 크기 초기화
    }
}
