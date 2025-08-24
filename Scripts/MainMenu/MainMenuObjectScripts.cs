using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuObjectScripts : MonoBehaviour
{
    [Header("Left UI")]
    public GameObject leftui; // ���� UI ������Ʈ
    public GameObject leftButton; // ���� ��ư ������Ʈ
    public GameObject leftCharEx; // ���� ĳ���� ���� ������Ʈ

    [Header("Right UI")]
    public GameObject rightui; // ������ UI ������Ʈ
    public GameObject rightCharList; // ������ ĳ���� ����Ʈ ������Ʈ
    public GameObject rightButton; // ������ ��ư ������Ʈ
    public GameObject rightSelectCharList; // ������ ĳ���� ����Ʈ ������Ʈ

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
