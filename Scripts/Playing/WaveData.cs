using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class WaveData
{
    // [라운드 인덱스][3웨이브의 적 수]
    private static readonly int[][] _enemyCountsByRound = new int[][]
    {
        new int[] { 5, 2, 0 }, // 라운드 1 
        new int[] { 2, 3, 1 }, // 라운드 2 
        new int[] { 3, 3, 1 }, // 라운드 3 
        new int[] { 3, 3, 2 }, // 라운드 4
        new int[] { 5, 5, 1 }, // 라운드 5 보스 라운드

        new int[] { 3, 3, 1 }, // 라운드 6
        new int[] { 4, 4, 1 }, // 라운드 7
        new int[] { 5, 4, 2 }, // 라운드 8 
        new int[] { 5, 5, 2 }, // 라운드 9
        new int[] { 5, 5, 3 }, // 라운드 10 보스 라운드

        new int[] { 3, 3, 1 }, // 라운드 11
        new int[] { 4, 4, 2 }, // 라운드 12 
        new int[] { 5, 5, 3 }, // 라운드 13
        new int[] { 5, 5, 3 }, // 라운드 14
        new int[] { 5, 5, 4 }, // 라운드 15 보스 라운드

        new int[] { 4, 4, 1 }, // 라운드 16
        new int[] { 5, 5, 3 }, // 라운드 17
        new int[] { 5, 5, 3 }, // 라운드 18
        new int[] { 5, 5, 4 }, // 라운드 19
        new int[] { 5, 5, 5 }, // 라운드 20 보스 라운드

        new int[] { 4, 4, 2 }, // 라운드 21 
        new int[] { 5, 5, 3 }, // 라운드 22
        new int[] { 5, 5, 4 }, // 라운드 23
        new int[] { 5, 5, 4 }, // 라운드 24
        new int[] { 5, 5, 5 }, // 라운드 25 보스 라운드
    };

    public static int[] GetEnemyCount(int round)
    {
        if (round < 1 || round > _enemyCountsByRound.Length)                                    // 라운드가 범위 밖으로 나가버린다면
            throw new ArgumentOutOfRangeException(nameof(round), "1~25 사이여야 합니다.");

        return _enemyCountsByRound[round - 1];
    }
}
