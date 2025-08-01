using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class WaveData
{
    // [���� �ε���][3���̺��� �� ��]
    private static readonly int[][] _enemyCountsByRound = new int[][]
    {
        new int[] { 5, 2, 0 }, // ���� 1 
        new int[] { 2, 3, 1 }, // ���� 2 
        new int[] { 3, 3, 1 }, // ���� 3 
        new int[] { 3, 3, 2 }, // ���� 4
        new int[] { 5, 5, 1 }, // ���� 5 ���� ����

        new int[] { 3, 3, 1 }, // ���� 6
        new int[] { 4, 4, 1 }, // ���� 7
        new int[] { 5, 4, 2 }, // ���� 8 
        new int[] { 5, 5, 2 }, // ���� 9
        new int[] { 5, 5, 3 }, // ���� 10 ���� ����

        new int[] { 3, 3, 1 }, // ���� 11
        new int[] { 4, 4, 2 }, // ���� 12 
        new int[] { 5, 5, 3 }, // ���� 13
        new int[] { 5, 5, 3 }, // ���� 14
        new int[] { 5, 5, 4 }, // ���� 15 ���� ����

        new int[] { 4, 4, 1 }, // ���� 16
        new int[] { 5, 5, 3 }, // ���� 17
        new int[] { 5, 5, 3 }, // ���� 18
        new int[] { 5, 5, 4 }, // ���� 19
        new int[] { 5, 5, 5 }, // ���� 20 ���� ����

        new int[] { 4, 4, 2 }, // ���� 21 
        new int[] { 5, 5, 3 }, // ���� 22
        new int[] { 5, 5, 4 }, // ���� 23
        new int[] { 5, 5, 4 }, // ���� 24
        new int[] { 5, 5, 5 }, // ���� 25 ���� ����
    };

    public static int[] GetEnemyCount(int round)
    {
        if (round < 1 || round > _enemyCountsByRound.Length)                                    // ���尡 ���� ������ ���������ٸ�
            throw new ArgumentOutOfRangeException(nameof(round), "1~25 ���̿��� �մϴ�.");

        return _enemyCountsByRound[round - 1];
    }
}
