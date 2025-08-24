using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCharList
{
    public GameObject[] _selectCharList;     // 캐릭터 리스트 오브젝트들

    public void InitCharList()
    {
        _selectCharList = new GameObject[4]; // 선택 캐릭터 리스트 초기화
    }

    public void UpdateUI(GameObject left, GameObject right)
    {
        for (int i = 0; i < _selectCharList.Length; i++) _selectCharList[i] = null; // 선택 캐릭터 리스트 초기화

        UpdateCharListUI(left);         // 캐릭터 리스트 UI 업데이트
        UpdateSelectCharListUI(right);  // 선택 캐릭터 리스트 UI 업데이트
    }

    public void UpdateCharListUI(GameObject objectList)
    {
        int size = Helper.CharacterManager.GetCharPlayer.Length; // 플레이어 캐릭터의 개수

        // 반복 수로 각 캐릭터 정보 값 넣어주기
        for (int i = 0; i < 8; i++)
        {
            GameObject ui = objectList.transform.GetChild(i).gameObject; // 캐릭터 리스트 UI 오브젝트 가져오기
            CharSlotsUI charSlotsUI = ui.GetComponent<CharSlotsUI>();    // CharSlotsUI 컴포넌트 가져오기
            Character unit = Helper.CharacterManager.GetCharPlayer[i].GetComponent<Character>(); // 캐릭터 컴포넌트 가져오기

            charSlotsUI.InitSlot(unit); // 캐릭터 테이블 초기화
        }
    }

    public void AddSelectCharList(Character character, out int index, out bool active)
    {
        int size = _selectCharList.Length; // 선택 캐릭터 리스트의 크기
        index = -1;     // 초기화
        active = false; // 꺼진 상태

        // 1. 동일한 캐릭터가 이미 선택된지를 확인
        for (int i = 0; i < size; i++)
        {
            if (_selectCharList[i] == character.gameObject)
            {
                _selectCharList[i] = null; // 이미 선택된 캐릭터라면 선택 해제
                index = i; // 선택 해제된 캐릭터의 인덱스 반환
                active = false; // 선택 해제 상태로 변경
                return;
            }
        }

        // 2.. 선택 캐릭터 리스트에 빈 공간이 있는지 확인
        for (int i = 0; i < size; i++)
        {
            if(_selectCharList[i] is null) break;   // null인 경우 빈 공간이 있는 것이니 for문 break

            if (i == size - 1) return; // 빈 공간이 없다면 리턴
        }

        // 3. 빈 공간에 캐릭터 추가
        for (int i = 0; i < size; i++)
        {
            if (_selectCharList[i] is null) // 빈 공간이 있다면
            {
                _selectCharList[i] = character.gameObject; // 선택 캐릭터 리스트에 추가
                index = i; // 추가된 캐릭터의 인덱스 반환
                active = true; // 선택 상태로 변경
                return;
            }
        }
    }


    public void UpdateSelectCharListUI(GameObject objectList)
    {
        int size = _selectCharList.Length; // 선택 캐릭터 리스트의 크기

        // 반복 수로 각 캐릭터 정보 값 넣어주기
        for (int i = 0; i < size; i++)
        {
            GameObject ui = objectList.transform.GetChild(i).gameObject; // 캐릭터 리스트 UI 오브젝트 가져오기
            CharSlotsUI charSlotsUI = ui.GetComponent<CharSlotsUI>();    // CharSlotsUI 컴포넌트 가져오기

            if (_selectCharList[i] is null) // 선택된 캐릭터가 없는 경우
            {
                charSlotsUI.SelectListUI(null);
                continue;
            }

            Character character = _selectCharList[i].GetComponent<Character>(); // 선택된 캐릭터 컴포넌트 가져오기
            charSlotsUI.SelectListUI(character); // 캐릭터 테이블 업데이트
        }
    }
}
