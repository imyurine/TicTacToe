using UnityEditor;

public class MenuView : ViewBase
{
    public GameView gameView;
    public OptionWindow optionWindow;

    public void ClickNewGame()
    {
        // 设置关卡为第1关
        GameData.Instance.SetLevel(1);
        GameData.Instance.SetPlayerScore(0);
        GameData.Instance.SetAIScore(0);
        gameView.Open();
        Close();
    }

    public void ClickContinue()
    {
        gameView.Open();
        Close();
    }

    public void ClickOption()
    {
        optionWindow.Open();
    }

    public void ClickQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }
}