using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        // 初始化存档
        GameData.Instance.InitData();
    }

    private void Start()
    {
        // 进入游戏关卡
        SceneManager.LoadScene("Level0");
    }
}