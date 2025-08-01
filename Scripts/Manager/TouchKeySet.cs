using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchKeySet : MonoBehaviour
{
    private bool _encounterTouch = false;
    public bool GetEncounterTouch { get { return _encounterTouch; } set { _encounterTouch = value; } }


}
