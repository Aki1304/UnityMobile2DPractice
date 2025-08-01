using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSelect : MonoBehaviour
{
    [Header("선택한 캐릭터")]
    public Character _selectChar;

    public Action OnChangeCursor;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            EncounterTouch();
        }
    }

    public void EncounterTouch()
    {
        if (!Helper.KeySet.GetEncounterTouch) return;

        // UI 위 클릭 여부 확인
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("UI 위 클릭 - Raycast 수행 안함");             // UI를 클릭 중이므로 Raycast는 수행하지 않음
            return;
        }

        Ray touchPos = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(touchPos, out hit))
        {
            GameObject unit = hit.collider.gameObject;
            Character unitChar = unit.GetComponent<Character>();

            // 여기에 이후 조건 추가해서 팀 선택 해야할 경우 넣기.
            if (unit.CompareTag("Player")) return;

            _selectChar = unitChar;
            OnChangeCursor?.Invoke();
        }
    }

    public void SetFirstChar(Character unitChar)
    {
        StartCoroutine(DelayCode());

        IEnumerator DelayCode()
        {
            _selectChar = unitChar;
            yield return null;
            OnChangeCursor?.Invoke();

        }
    }



    public void NullSelectChar() => _selectChar = null;

}
