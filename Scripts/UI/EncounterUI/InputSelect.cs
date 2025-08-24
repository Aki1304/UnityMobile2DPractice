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

    private bool _canSelectPlayer = false;

    public bool CanSelectPlayer { get {  return _canSelectPlayer; }  set { _canSelectPlayer = value; } }

    

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

            Debug.Log($"Raycast Hit: {unit.name} Bool : {CanSelectPlayer}");

            if (!CanSelectPlayer)
            {
                if (unit.CompareTag("Player")) return;  
            }
            else
            {
                if (unit.CompareTag("Enemy")) return;  // 적 유닛은 선택 불가
            }

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
