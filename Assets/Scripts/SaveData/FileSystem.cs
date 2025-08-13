using LitJson;
using System.IO;

// 读写文件
public class FileSystem
{
    /// 解析为json
    public static string ToJson(object data)
    {
        return JsonMapper.ToJson(data);
    }

    /// 从json还原
    public static T FromJson<T>(string json)
    {
        return JsonMapper.ToObject<T>(json);
    }

    /// 把json写入文件
    public static void ToFile<T>(T _object, string filePath)
    {
        string content = ToJson(_object);
        File.WriteAllText(filePath, content);
    }

    /// 从文件读取json
    public static T FromFile<T>(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            T target = FromJson<T>(json);
            return target;
        }

        return default(T);
    }

    /// 文件是否存在
    public static bool IsFileExists(string filePath)
    {
        return File.Exists(filePath);
    }
}