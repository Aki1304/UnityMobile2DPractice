using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCharList
{
    public GameObject[] _selectCharList;     // ĳ���� ����Ʈ ������Ʈ��

    public void InitCharList()
    {
        _selectCharList = new GameObject[4]; // ���� ĳ���� ����Ʈ �ʱ�ȭ
    }

    public void UpdateUI(GameObject left, GameObject right)
    {
        for (int i = 0; i < _selectCharList.Length; i++) _selectCharList[i] = null; // ���� ĳ���� ����Ʈ �ʱ�ȭ

        UpdateCharListUI(left);         // ĳ���� ����Ʈ UI ������Ʈ
        UpdateSelectCharListUI(right);  // ���� ĳ���� ����Ʈ UI ������Ʈ
    }

    public void UpdateCharListUI(GameObject objectList)
    {
        int size = Helper.CharacterManager.GetCharPlayer.Length; // �÷��̾� ĳ������ ����

        // �ݺ� ���� �� ĳ���� ���� �� �־��ֱ�
        for (int i = 0; i < 8; i++)
        {
            GameObject ui = objectList.transform.GetChild(i).gameObject; // ĳ���� ����Ʈ UI ������Ʈ ��������
            CharSlotsUI charSlotsUI = ui.GetComponent<CharSlotsUI>();    // CharSlotsUI ������Ʈ ��������
            Character unit = Helper.CharacterManager.GetCharPlayer[i].GetComponent<Character>(); // ĳ���� ������Ʈ ��������

            charSlotsUI.InitSlot(unit); // ĳ���� ���̺� �ʱ�ȭ
        }
    }

    public void AddSelectCharList(Character character, out int index, out bool active)
    {
        int size = _selectCharList.Length; // ���� ĳ���� ����Ʈ�� ũ��
        index = -1;     // �ʱ�ȭ
        active = false; // ���� ����

        // 1. ������ ĳ���Ͱ� �̹� ���õ����� Ȯ��
        for (int i = 0; i < size; i++)
        {
            if (_selectCharList[i] == character.gameObject)
            {
                _selectCharList[i] = null; // �̹� ���õ� ĳ���Ͷ�� ���� ����
                index = i; // ���� ������ ĳ������ �ε��� ��ȯ
                active = false; // ���� ���� ���·� ����
                return;
            }
        }

        // 2.. ���� ĳ���� ����Ʈ�� �� ������ �ִ��� Ȯ��
        for (int i = 0; i < size; i++)
        {
            if(_selectCharList[i] is null) break;   // null�� ��� �� ������ �ִ� ���̴� for�� break

            if (i == size - 1) return; // �� ������ ���ٸ� ����
        }

        // 3. �� ������ ĳ���� �߰�
        for (int i = 0; i < size; i++)
        {
            if (_selectCharList[i] is null) // �� ������ �ִٸ�
            {
                _selectCharList[i] = character.gameObject; // ���� ĳ���� ����Ʈ�� �߰�
                index = i; // �߰��� ĳ������ �ε��� ��ȯ
                active = true; // ���� ���·� ����
                return;
            }
        }
    }


    public void UpdateSelectCharListUI(GameObject objectList)
    {
        int size = _selectCharList.Length; // ���� ĳ���� ����Ʈ�� ũ��

        // �ݺ� ���� �� ĳ���� ���� �� �־��ֱ�
        for (int i = 0; i < size; i++)
        {
            GameObject ui = objectList.transform.GetChild(i).gameObject; // ĳ���� ����Ʈ UI ������Ʈ ��������
            CharSlotsUI charSlotsUI = ui.GetComponent<CharSlotsUI>();    // CharSlotsUI ������Ʈ ��������

            if (_selectCharList[i] is null) // ���õ� ĳ���Ͱ� ���� ���
            {
                charSlotsUI.SelectListUI(null);
                continue;
            }

            Character character = _selectCharList[i].GetComponent<Character>(); // ���õ� ĳ���� ������Ʈ ��������
            charSlotsUI.SelectListUI(character); // ĳ���� ���̺� ������Ʈ
        }
    }
}
