using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IPassiveExtraAttack
{
    void ActAttack(GameObject[] objects);
    void OnExtraAttack(List<GameObject> @object);
}
