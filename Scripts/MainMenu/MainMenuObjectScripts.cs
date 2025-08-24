using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuObjectScripts : MonoBehaviour
{
    [Header("Left UI")]
    public GameObject leftui; // 왼쪽 UI 오브젝트
    public GameObject leftButton; // 왼쪽 버튼 오브젝트
    public GameObject leftCharEx; // 왼쪽 캐릭터 설명 오브젝트

    [Header("Right UI")]
    public GameObject rightui; // 오른쪽 UI 오브젝트
    public GameObject rightCharList; // 오른쪽 캐릭터 리스트 오브젝트
    public GameObject rightButton; // 오른쪽 버튼 오브젝트
    public GameObject rightSelectCharList; // 오른쪽 캐릭터 리스트 오브젝트

    public void ActiveEnterDungeon()
    {
        leftCharEx.SetActive(true);
        rightui.SetActive(true);
        rightCharList.SetActive(true);
        rightButton.SetActive(true);
        rightSelectCharList.SetActive(true);

        leftButton.SetActive(false);
    }

    public void ActiveReturnMainMenu()
    {
        leftButton.SetActive(true);

        leftCharEx.SetActive(false);
        rightui.SetActive(false);
        rightCharList.SetActive(false);
        rightButton.SetActive(false);
        rightSelectCharList.SetActive(false);

    }


}
