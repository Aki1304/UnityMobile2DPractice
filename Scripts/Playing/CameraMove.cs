using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private  Camera _cam;

    private GameObject[] GetPartyInfo { get { return Helper.DualManager._instanceParty; } }

    public void SetCameraPosition(int idx)
    {
        if(_cam == null) _cam = Camera.main;

        _cam.transform.position = PosSelect(idx);
        _cam.transform.rotation = RotSelect(idx);
    }

    public void SetPartyCameraPosition()
    {
        if (_cam == null) _cam = Camera.main;

        _cam.transform.position = new Vector3(0, -0.5f, 2.67f);
        _cam.transform.rotation = Quaternion.Euler(5, 180, 0);
    }


    public void SetEnemyCamera()
    {
        if (_cam == null) _cam = Camera.main;

        _cam.transform.position = new Vector3(0, 2.0999999f, -11);
        _cam.transform.rotation = Quaternion.Euler(13f, 0, 0);
    }

    Vector3 PosSelect(int idx)
    {
        return idx switch
        {
            0 => new Vector3(-3.18f, -0.81f, -6.91f),
            1 => new Vector3(1.07f, -0.41f, -6.63f),
            2 => new Vector3(-0.88f, -1.094f, -6.39f),
            3 => new Vector3(2.83f, -0.64f, -7.22f),
            _ => throw new System.Exception()
        };
    }

    Quaternion RotSelect(int idx)
    {
        return idx switch
        {
            0 => Quaternion.Euler(new Vector3(3.3f, 1.94f, 0)),   // 예시
            1 => Quaternion.Euler(new Vector3(6.73f, -12.31f, 0f)),
            2 => Quaternion.Euler(new Vector3(0f, 6.98f, 0f)),
            3 => Quaternion.Euler(new Vector3(3.264f, -6.52f, 0f)),
            _ => throw new System.Exception()
        };
    }

    public void CurrentCameraTurn(Character currentUnit)
    {
        int unitcode = currentUnit.GetStats.GetUnitCode;

        if (unitcode == 0)
        {
            int idxPos = Array.IndexOf(GetPartyInfo, currentUnit.gameObject);

            if (idxPos < 0)
            {
                Debug.LogWarning("현재 유닛이 파티 정보에 없습니다.");
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                if (GetPartyInfo[i] is null) { continue; }

                if (i == idxPos) GetPartyInfo[i].SetActive(true);
                else GetPartyInfo[i].SetActive(false);
            }

            SetCameraPosition(idxPos);
        }

        if (unitcode != 0)
        {
            foreach (var unit in GetPartyInfo)
            {
                if (unit == null) continue;
                unit.SetActive(true);
            }
            SetEnemyCamera();
        }
    }

    public void OnHealAndBuffCamera()
    {
        foreach(var instance in GetPartyInfo)
        {
            if (instance == null) continue;
            instance.SetActive(true);
        }

        SetPartyCameraPosition();
    }

}
