using System.Collections.Generic;
using UnityEngine;

/// 对局状态
public enum GameStatus
{
    Next = 0, // 下一步
    PlayerWin = 1, // 玩家胜利
    AIWin = 2, // ai胜利
    Draw = 3, // 平局
}

/// 棋盘，ai下棋逻辑，胜利条件
public class GameLogic : Singleton<GameLogic>
{
    /// 值=0表示空格子, 1表示O, 2表示X
    private int[,] map = new int[3, 3];

    /// 开始一局新游戏
    public void NewGame()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                map[i, j] = 0;
            }
        }
    }

    /// 玩家落子
    public bool PlayOnGrid(Vector2 coord, int value)
    {
        int x = (int)coord.x;
        int y = (int)coord.y;
        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
        {
            return false;
        }

        if (map[x, y] == 0)
        {
            map[x, y] = value;
            return true;
        }

        return false;
    }

    /// ai落子，普通模式。随机选一个空格落子
    public void AIPlay()
    {
        List<int> coord = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (map[i, j] == 0)
                {
                    // 收集全部空格子
                    coord.Add(i * 3 + j);
                }
            }
        }

        if (coord.Count <= 1)
        {
            int c = coord[0];
            map[c / 3, c % 3] = 2;
        }
        else
        {
            // 随机一个
            int k = UnityEngine.Random.Range(0, coord.Count);
            int c = coord[k];
            map[c / 3, c % 3] = 2;
        }
    }

    /// ai落子，困难模式。选一个最可能赢的空格落子
    public void AIPlayHard()
    {
        // 不考虑玩家后续的落子，在剩下的空格中选一个最可能赢的格子
        int x = -1, y = -1; // 目标格子的坐标
        int value = 9999;
        List<int> coord = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // 如果是空的格子，那么，假设ai把棋子下在这个格子里，计算这次落子的结果的估值
                if (map[i, j] == 0)
                {
                    // 假设ai下在这个格子里
                    map[i, j] = 2;
                    int v = CalculateMapValue();

                    // 存在对ai更有优势的格子
                    if (v < value)
                    {
                        value = v;
                        // 清除待选列表
                        coord.Clear();
                        // 新加入待选列表
                        x = i;
                        y = j;
                        coord.Add(i * 3 + j);
                    }
                    // 找到了一个优势相等的格子
                    else if (v == value)
                    {
                        // 加入待选列表，之后随机一个
                        coord.Add(i * 3 + j);
                    }

                    // 重置为空格子
                    map[i, j] = 0;
                }
            }
        }

        if (x != -1 && y != -1)
        {
            // 只有一个目标格子
            if (coord.Count <= 1)
            {
                map[x, y] = 2;
            }
            // 多个目标格子，随机一个
            else
            {
                int k = UnityEngine.Random.Range(0, coord.Count);
                int c = coord[k];
                map[c / 3, c % 3] = 2;
            }
        }
    }

    /// 检查棋盘，返回对局状态
    public GameStatus CheckMap()
    {
        int status = CheckMapStatus();
        return (GameStatus)status;
    }

    /// 检查棋盘，返回0表示轮到对方落子，1表示O胜利，2表示X胜利，3表示平局
    private int CheckMapStatus()
    {
        // 3行，3列，2对角线，共8种胜利情况
        // 3行
        for (int i = 0; i < 3; i++)
        {
            if (map[i, 0] != 0 && map[i, 0] == map[i, 1] && map[i, 1] == map[i, 2])
            {
                return map[i, 0];
            }
        }

        // 3列 
        for (int j = 0; j < 3; j++)
        {
            if (map[0, j] != 0 && map[0, j] == map[1, j] && map[1, j] == map[2, j])
            {
                return map[0, j];
            }
        }

        // 2对角线
        if (map[1, 1] != 0 &&
            (map[1, 1] == map[0, 0] && map[1, 1] == map[2, 2] || map[1, 1] == map[0, 2] && map[1, 1] == map[2, 0]))
        {
            return map[1, 1];
        }

        // 如果存在空格子
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (map[i, j] == 0)
                {
                    // 轮到对方落子
                    return 0;
                }
            }
        }

        // 默认结果为平局
        return 3;
    }

    /// 返回每个格子的值，用于判断是否已落子
    public int[] GetMapGridValues()
    {
        int[] values = new int[9];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                values[i * 3 + j] = map[i, j];
            }
        }

        return values;
    }


    #region 最大最小估值

    // 估值。xyz是一条线上的3个格子的值
    private int Valuation(int x, int y, int z)
    {
        int cnt1 = 0; // 玩家棋子计数
        int cnt2 = 0; // ai棋子计数
        int[] a = new int[3] { x, y, z }; // 只是用于计算

        // 统计棋子的个数
        for (int i = 0; i < 3; i++)
        {
            if (a[i] == 1)
            {
                cnt1++;
            }

            if (a[i] == 2)
            {
                cnt2++;
            }
        }

        // 连线上不存在ai的棋子
        if (cnt2 == 0)
        {
            // 1玩家棋子和2空格的估值
            if (cnt1 == 1)
            {
                return 1;
            }

            // 2玩家棋子和1空格 
            if (cnt1 == 2)
            {
                return 5;
            }

            // 3玩家棋子
            if (cnt1 == 3)
            {
                return 1000;
            }
        }

        // 连线上不存在玩家的棋子
        else if (cnt1 == 0)
        {
            // 1ai棋子和2空格
            if (cnt2 == 1)
            {
                return -1;
            }

            // 2ai棋子和1空格
            if (cnt2 == 2)
            {
                return -5;
            }

            // 3ai棋子
            if (cnt2 == 3)
            {
                return -1000;
            }
        }

        // 其他情况的估值
        return 0;
    }

    // 对棋盘局面做估值，得分越低对ai越有利，越高对玩家越有利
    private int CalculateMapValue()
    {
        // 行、列、对角线分别估值 
        int value = 0;
        for (int i = 0; i < 3; i++)
        {
            // 3行
            value += Valuation(map[i, 0], map[i, 1], map[i, 2]);
            // 3列
            value += Valuation(map[0, i], map[1, i], map[2, i]);
        }

        // 2对角线
        value += Valuation(map[0, 0], map[1, 1], map[2, 2]);
        value += Valuation(map[2, 0], map[1, 1], map[0, 2]);
        return value;
    }

    #endregion
}