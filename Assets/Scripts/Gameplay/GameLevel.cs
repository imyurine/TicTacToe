using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct LevelMode
{
    public int levelMin; // 当关卡处于这个区间，就选择对应的难度
    public int levelMax;
    public int mode; // 1普通，2困难
    public string modeDes; // 模式描述
    public int score; // 关卡分数
}

/// 游戏入口，控制下棋的回合
public class GameLevel : MonoBehaviour
{
    private int nextPlayer; // 下一步落子方，0=玩家，1=AI
    private int curLevel = 1; // 当前关卡
    private int curMode; // 当前关卡难度
    private int levelScore; // 本关分数
    public List<LevelMode> levelModes; // 关卡难度配置

    public Sprite xBg; // 图片x
    public Sprite oBg; // 图片o
    private MapGrid[] grids; // 棋盘格子

    public TextMeshProUGUI levelLabel; // 关卡文本标签
    public TextMeshProUGUI modeLabel;
    public TextMeshProUGUI levelScoreLabel;
    public TextMeshProUGUI playerScoreLabel;
    public TextMeshProUGUI aiScoreLabel;

    public GameObject[] playTipGos; // 对局提示ui
    public string au_putChess; // 棋子放置音效

    private bool playing; // 对局进行中
    private bool aiPlaying; // ai正在落子

    private void OnEnable()
    {
        Init();
        NewGame();
    }

    private void Init()
    {
        // 棋盘格子初始化
        grids = transform.GetComponentsInChildren<MapGrid>();
        foreach (var grid in grids)
        {
            grid.Init();
            grid.onClick = null;
            grid.onClick += Play;
        }

        curLevel = GameData.Instance.data.level;
        levelScore = 0;
        // 默认玩家先手
        nextPlayer = 0;

        // 刷新分数ui
        playerScoreLabel.text = GameData.Instance.data.playerScore.ToString();
        aiScoreLabel.text = GameData.Instance.data.aiScore.ToString();
    }

    public void NewGame()
    {
        GameLogic.Instance.NewGame();
        foreach (var grid in grids)
        {
            grid.SetAsEmpty();
        }

        // 关闭对局提示ui
        foreach (var go in playTipGos)
        {
            go.SetActive(false);
        }

        // 本关序号
        levelLabel.text = curLevel.ToString();

        // 本关分数，难度
        for (int i = 0; i < levelModes.Count; i++)
        {
            if (curLevel >= levelModes[i].levelMin && curLevel < levelModes[i].levelMax)
            {
                levelScore = levelModes[i].score;
                curMode = levelModes[i].mode;
                modeLabel.text = levelModes[i].modeDes;
            }
        }

        levelScoreLabel.text = levelScore.ToString();

        playing = true;
        aiPlaying = false;

        // 先手落子
        // 如果是ai先走，那么第一步总是随机落子
        if (nextPlayer == 1)
        {
            AIPlay();
        }
    }

    /// 下一步落子
    public void NextStep()
    {
        if (!playing)
        {
            return;
        }

        // ai落子
        if (nextPlayer == 1)
        {
            switch (curMode)
            {
                case 1:
                    AIPlay();
                    break;

                case 2:
                    AIPlayHard();
                    break;
            }
        }
    }

    /// 玩家落子
    public void Play(Vector2 coord)
    {
        if (!playing || aiPlaying)
        {
            return;
        }

        if (GameLogic.Instance.PlayOnGrid(coord, 1))
        {
            aiPlaying = true;
            OneSetpEnd();
            AudioSystem.Instance.PlayAudio(au_putChess);
        }
    }

    // ai落子，普通模式
    private void AIPlay()
    {
        GameLogic.Instance.AIPlay();
        OneSetpEnd();
        aiPlaying = false;
    }

    // ai落子，困难模式
    private void AIPlayHard()
    {
        GameLogic.Instance.AIPlayHard();
        OneSetpEnd();
        aiPlaying = false;
    }

    // 一步棋完成后
    private void OneSetpEnd()
    {
        // 下一步轮到谁落子
        int tag = nextPlayer;
        nextPlayer = tag == 1 ? 0 : 1;

        GameStatus status = GameLogic.Instance.CheckMap();
        switch (status)
        {
            case GameStatus.Next:
                NextStep();
                break;

            case GameStatus.PlayerWin:
                OnPlayerWin();
                break;

            case GameStatus.AIWin:
                OnAIWin();
                break;

            case GameStatus.Draw:
                OnDraw();
                break;
        }

        UpdateMapUI();
    }

    private void UpdateMapUI()
    {
        int[] status = GameLogic.Instance.GetMapGridValues();
        if (status.Length != 9 || grids.Length != 9)
        {
            Debug.Log("棋盘格子数量不为9！");
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            if (status[i] == 1)
            {
                grids[i].SetAsOX(oBg);
            }

            if (status[i] == 2)
            {
                grids[i].SetAsOX(xBg);
            }
        }
    }

    private void OnPlayerWin()
    {
        // 玩家赢了，下一关玩家先手
        nextPlayer = 0;
        playing = false;
        // 更新得分
        int score = GameData.Instance.data.playerScore;
        score += levelScore;
        GameData.Instance.SetPlayerScore(score);
        playerScoreLabel.text = score.ToString();
        // 弹出 玩家胜利 的ui
        playTipGos[0].SetActive(true);
    }

    private void OnAIWin()
    {
        nextPlayer = 1;
        playing = false;
        // 更新得分
        int score = GameData.Instance.data.aiScore;
        score += levelScore;
        GameData.Instance.SetAIScore(score);
        aiScoreLabel.text = score.ToString();
        playTipGos[1].SetActive(true);
    }

    private void OnDraw()
    {
        nextPlayer = 0;
        playing = false;
        playTipGos[2].SetActive(true);
    }

    /// 下一关
    public void NextLevel()
    {
        curLevel++;
        GameData.Instance.SetLevel(curLevel);
        NewGame();
    }
}