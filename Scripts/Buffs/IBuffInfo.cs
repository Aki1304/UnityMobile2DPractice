using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffInfo
{
    string InfoID { get; }          //   ���� �ĺ���
    string InfoViewId { get; }      //   �ѱ��� ǥ��
    Sprite buffSprite { get; }      //   ���� ��������Ʈ
    BuffDurationType durationType { get; }  // ���� Ÿ�� ����
    SkillActionType actionType { get; }

    /// <summary>
    /// ���� �� ���� ���� ��� �� ������ ����ٰ� ����
    /// </summary>
    /// <returns></returns>
    CharStats.buffStats GetBuffStats();

    Sprite ReturnSprite();              // ��������Ʈ �����ֱ�

    void BuffRoutine();
}
