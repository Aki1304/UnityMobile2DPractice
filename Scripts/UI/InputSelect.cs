using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSelect : MonoBehaviour
{
    [Header("������ ĳ����")]
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

        // UI �� Ŭ�� ���� Ȯ��
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("UI �� Ŭ�� - Raycast ���� ����");             // UI�� Ŭ�� ���̹Ƿ� Raycast�� �������� ����
            return;
        }

        Ray touchPos = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(touchPos, out hit))
        {
            GameObject unit = hit.collider.gameObject;
            Character unitChar = unit.GetComponent<Character>();

            // ���⿡ ���� ���� �߰��ؼ� �� ���� �ؾ��� ��� �ֱ�.
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
