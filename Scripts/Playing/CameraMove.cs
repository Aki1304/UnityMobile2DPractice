using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private  Camera _cam;

    public void SetCameraPosition(int idx)
    {
        if(_cam == null) _cam = Camera.main;

        _cam.transform.position = PosSelect(idx);
        _cam.transform.rotation = RotSelect(idx);
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
            0 => Quaternion.Euler(new Vector3(3.3f, 1.94f, 0)),   // ¿¹½Ã
            1 => Quaternion.Euler(new Vector3(6.73f, -12.31f, 0f)),
            2 => Quaternion.Euler(new Vector3(0f, 6.98f, 0f)),
            3 => Quaternion.Euler(new Vector3(3.264f, -6.52f, 0f)),
            _ => throw new System.Exception()
        };
    }
}
