using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameButton : MonoBehaviour
{
    public MainMenu _mainMenu;       // ���� �޴� ��ũ��Ʈ ���� 

    private bool _isPartySet = false; // ��Ƽ ���� ����


    // ���� �޴� ���� ��ư 
    public void OnEnterDungeonButton()
    {
        _mainMenu._mainObjects.ActiveEnterDungeon();   // ���� ���� ��ư Ŭ�� �� ���� ���� UI Ȱ��ȭ
    }


    public void OnCollectionButton()
    {

    }

    public void OnGoolgeAdButton()
    {
        // ���� ���� ��ư Ŭ�� �� ���� �����ִ� ���.
        _mainMenu.mobileAds.ShowRerdedAd(); // ���� ���� ��ũ��Ʈ�� ���� �����ֱ� �޼ҵ� ȣ��
    }

    public void OnGameOptionButton()
    {

    }

    public void OnExitGameButton()
    {
        // ���� ���� �� �����ؾ� �� �� �� �ִٸ� �θ���

        GameObject msgObject = Helper.PoolManager.GetMessagePool();
        MessageBox box = msgObject.GetComponent<MessageBox>();

        box.OnWriteTextBox("������ ��� �� ���� �˴ϴ�. \n �����մϴ�.");

        Helper.UM._fadeEffect.Fadein();                  // ���̵� �� ����

        Invoke("InvokeExitGame", 2f); // 2�� �� ���� ����

        void InvokeExitGame()
        {
            Application.Quit(); // ���� ����
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� ���� ����
#endif
        }
    }

    public void OnGameStartButton()
    {
        // �̶� �������ִ� ��Ƽ�� ���
        Helper.PartyManager.UpdateParty(_mainMenu._mainCharList._selectCharList);

        SceneManager.LoadScene(1); // ���� ���� ��ư Ŭ�� �� ���� ������ �̵�
    }

    public void OnCharSlotClick(int idx)
    {
        Character selectChar = Helper.CharacterManager.GetCharPlayer[idx].GetComponent<Character>(); // ���õ� ĳ���� ������Ʈ ��������
        // ���� ui�� ĳ���� ���� ǥ��
        _mainMenu._mainCharEx.OnSelectSkillView(selectChar);

        for(int i = 0; i < _mainMenu._charSlotsUI.Count; i++)
        {
            CharSlotsUI slotsUI = _mainMenu._charSlotsUI[i]; // ĳ���� ���� UI ��������

            if (idx == i) slotsUI._borderSprite.color = Color.yellow;   // ���õ� ĳ���� ���� �׵θ� ���� ����
            else slotsUI._borderSprite.color = Color.white;            // ���õ��� ���� ĳ���� ���� �׵θ� ���� �������
        }

        // ��Ƽ ������ true�� ���
        if (_isPartySet)
        {
            _mainMenu._mainCharList.AddSelectCharList(selectChar, out int index, out bool active); // ĳ���� ���� Ŭ�� �� ���� ĳ���� ����Ʈ�� �߰�
            _mainMenu._mainCharList.UpdateSelectCharListUI(_mainMenu._mainObjects.rightSelectCharList); // ĳ���� ����Ʈ UI ������Ʈ

            if(index == -1) return; // ���� ĳ���Ͱ� ���� ���
            _mainMenu._charSlotsUI[idx].SetNumIcon(active, index); // ���õ� ĳ���� ���� ���� ������ Ȱ��ȭ �� ���� ����
        }
    }

    public void OnSetPartyButton(Image buttonImage)
    {
        _isPartySet = !_isPartySet; // ��Ƽ ���� ���� ���

        if (_isPartySet) buttonImage.color = Color.blue; // ��Ƽ ���� ��ư ���� ����
        else buttonImage.color = Color.white;            // ��Ƽ ���� ���� �� ���� �������
    }

    public void OnReturnMainButton(Image partyButton)
    {
        ResetSlotBorder();     // ĳ���� ���� �׵θ� ���� �������
        _mainMenu.OnUIReset(); // ���� �޴� UI �ʱ�ȭ

        if(_isPartySet) OnSetPartyButton(partyButton); // ��Ƽ ���� ��ư ���� �������

        void ResetSlotBorder()
        {
            foreach(var slot in _mainMenu._charSlotsUI)
            {
                slot._borderSprite.color = Color.white; // ��� ĳ���� ���� �׵θ� ���� �������
            }
        }
    }

}
