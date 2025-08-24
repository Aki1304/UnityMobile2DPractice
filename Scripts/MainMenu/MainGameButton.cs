using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameButton : MonoBehaviour
{
    public MainMenu _mainMenu;       // 메인 메뉴 스크립트 참조 

    private bool _isPartySet = false; // 파티 설정 여부


    // 메인 메뉴 게임 버튼 
    public void OnEnterDungeonButton()
    {
        _mainMenu._mainObjects.ActiveEnterDungeon();   // 게임 시작 버튼 클릭 시 게임 시작 UI 활성화
    }


    public void OnCollectionButton()
    {

    }

    public void OnGoolgeAdButton()
    {
        // 구글 광고 버튼 클릭 시 광고를 보여주는 기능.
        _mainMenu.mobileAds.ShowRerdedAd(); // 구글 광고 스크립트의 광고 보여주기 메소드 호출
    }

    public void OnGameOptionButton()
    {

    }

    public void OnExitGameButton()
    {
        // 게임 종료 전 설정해야 할 것 이 있다면 부르기

        GameObject msgObject = Helper.PoolManager.GetMessagePool();
        MessageBox box = msgObject.GetComponent<MessageBox>();

        box.OnWriteTextBox("게임이 잠시 뒤 종료 됩니다. \n 감사합니다.");

        Helper.UM._fadeEffect.Fadein();                  // 페이드 인 실행

        Invoke("InvokeExitGame", 2f); // 2초 뒤 게임 종료

        void InvokeExitGame()
        {
            Application.Quit(); // 게임 종료
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 게임 종료
#endif
        }
    }

    public void OnGameStartButton()
    {
        // 이때 정해져있는 파티로 출발
        Helper.PartyManager.UpdateParty(_mainMenu._mainCharList._selectCharList);

        SceneManager.LoadScene(1); // 게임 시작 버튼 클릭 시 게임 씬으로 이동
    }

    public void OnCharSlotClick(int idx)
    {
        Character selectChar = Helper.CharacterManager.GetCharPlayer[idx].GetComponent<Character>(); // 선택된 캐릭터 컴포넌트 가져오기
        // 왼쪽 ui에 캐릭터 정보 표시
        _mainMenu._mainCharEx.OnSelectSkillView(selectChar);

        for(int i = 0; i < _mainMenu._charSlotsUI.Count; i++)
        {
            CharSlotsUI slotsUI = _mainMenu._charSlotsUI[i]; // 캐릭터 슬롯 UI 가져오기

            if (idx == i) slotsUI._borderSprite.color = Color.yellow;   // 선택된 캐릭터 슬롯 테두리 색상 변경
            else slotsUI._borderSprite.color = Color.white;            // 선택되지 않은 캐릭터 슬롯 테두리 색상 원래대로
        }

        // 파티 변경이 true인 경우
        if (_isPartySet)
        {
            _mainMenu._mainCharList.AddSelectCharList(selectChar, out int index, out bool active); // 캐릭터 슬롯 클릭 시 선택 캐릭터 리스트에 추가
            _mainMenu._mainCharList.UpdateSelectCharListUI(_mainMenu._mainObjects.rightSelectCharList); // 캐릭터 리스트 UI 업데이트

            if(index == -1) return; // 선택 캐릭터가 없는 경우
            _mainMenu._charSlotsUI[idx].SetNumIcon(active, index); // 선택된 캐릭터 슬롯 숫자 아이콘 활성화 및 숫자 설정
        }
    }

    public void OnSetPartyButton(Image buttonImage)
    {
        _isPartySet = !_isPartySet; // 파티 설정 여부 토글

        if (_isPartySet) buttonImage.color = Color.blue; // 파티 설정 버튼 색상 변경
        else buttonImage.color = Color.white;            // 파티 설정 해제 시 색상 원래대로
    }

    public void OnReturnMainButton(Image partyButton)
    {
        ResetSlotBorder();     // 캐릭터 슬롯 테두리 색상 원래대로
        _mainMenu.OnUIReset(); // 메인 메뉴 UI 초기화

        if(_isPartySet) OnSetPartyButton(partyButton); // 파티 설정 버튼 색상 원래대로

        void ResetSlotBorder()
        {
            foreach(var slot in _mainMenu._charSlotsUI)
            {
                slot._borderSprite.color = Color.white; // 모든 캐릭터 슬롯 테두리 색상 원래대로
            }
        }
    }

}
