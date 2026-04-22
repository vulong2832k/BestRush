using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public string levelName;

    public void SetLevel(string name)
    {
        levelName = name;
    }

    public LevelData Load()
    {
        TextAsset json = Resources.Load<TextAsset>(levelName);

        if (json == null)
        {
            Debug.LogError("Không tìm thấy file JSON: " + levelName);
            return null;
        }

        return JsonUtility.FromJson<LevelData>(json.text);
    }
}