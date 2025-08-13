using UnityEngine;

/// 玩家的游戏数据存档
public class GameData : Singleton<GameData>
{
    private string filePath = Application.persistentDataPath + "/gamedata.json";

    public class JsonData
    {
        /// 关卡进度
        public int level = 1;

        /// 玩家累计得分
        public int playerScore = 0;

        /// ai累计得分
        public int aiScore = 0;

        /// 音乐音量
        public float musicVolume = 1;

        /// 音效音量
        public float soundVolume = 1;
    }

    public JsonData data = new JsonData();

    public void SetLevel(int value)
    {
        data.level = value;
        SaveData();
    }

    public void SetPlayerScore(int value)
    {
        data.playerScore = value;
        SaveData();
    }

    public void SetAIScore(int value)
    {
        data.aiScore = value;
        SaveData();
    }

    public void SetMusicVolume(float value)
    {
        data.musicVolume = value;
        SaveData();
    }

    public void SetSoundVolume(float value)
    {
        data.soundVolume = value;
        SaveData();
    }

    /// 初始化存档
    public void InitData()
    {
        // 如果文件不存在，新建
        if (!FileSystem.IsFileExists(filePath))
        {
            SaveData();
        }

        LoadData();
    }

    /// 存档
    public void SaveData()
    {
        FileSystem.ToFile(data, filePath);
    }

    /// 读档
    public void LoadData()
    {
        data = FileSystem.FromFile<JsonData>(filePath);
    }
}