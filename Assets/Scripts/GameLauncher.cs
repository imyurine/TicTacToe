using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        // 加载存档
        GameData.Instance.LoadData();
    }

    private void Start()
    {
        // 进入游戏关卡
        SceneManager.LoadScene("Level0");
    }
}