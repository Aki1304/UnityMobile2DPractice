using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;

public class CharSlotsUI : MonoBehaviour
{
    public Character GetCharacter { get; private set; } // ĳ���� ������Ʈ

    public Image _playersprite;          // �÷��̾� ������
    public Image _borderSprite;          // �÷��̾� ������ �׵θ�
    public GameObject _numIcon;          // �÷��̾� ������ ����
    public TextMeshProUGUI _numText;     // �÷��̾� ������ ���� �ؽ�Ʈ
    public TextMeshProUGUI _charName;    // ĳ���� �̸�

    // �⺻ ĳ���� ����Ʈ �ʱ�ȭ �� ����
    public void InitSlot(Character unit)
    {
        GetCharacter = unit;            // ĳ���� ������Ʈ ����

        // �÷��̾� ������
        _playersprite.sprite = GetCharacter.ReturnPlayerSprite(0); // �÷��̾� ��������Ʈ ����
        // �÷��̾� ������ ����
        SetNumIcon(false, 0);
        // ĳ���� �̸�
        _charName.text = GetCharacter.GetCharTable.dataName;
    }

    public void SetNumIcon(bool active,int idxNum)
    {
        if(active)                      // ų ���
        {
            idxNum += 1;                // ���� 1 ����
            _numText.text = idxNum.ToString();
            _numIcon.SetActive(true);
        }
        else _numIcon.SetActive(false); // ��Ȱ��ȭ
    }

    // ���õ� ��Ƽ ����Ʈ UI ������Ʈ
    public void SelectListUI(Character unit)
    {
        if(unit is null)
        {
            GetCharacter = null;        // ĳ���� ���̺��� null�� ���
            _playersprite.sprite = null; // �÷��̾� ������ ��Ȱ��ȭ
            _playersprite.color = new Color(1, 1, 1, 0); // �����ϰ� ����
            return;
        }

        GetCharacter = unit;                                         // ���õ� ĳ���� ���̺��� ����
        _playersprite.sprite = GetCharacter.ReturnPlayerSprite(0);   // ĳ���� ���� UI ������Ʈ
        _playersprite.color = new Color(1, 1, 1, 1);                 // �������ϰ� ����
    }
}
