using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 공통적인 UI의 로직들을 관리한다.
/// 열고 닫는다거나... 그런 목적이 있는 UI들은 이걸 상속받자
/// </summary>
public abstract class SceneLogicUI : MonoBehaviour
{
    public UIManager UM { get { return UIManager.Instance; } }

    public void InitClass()
    {
        
    }

    public void OpenUI()
    { 
        
    }

    public void ClosUI()
    {

    }
}
