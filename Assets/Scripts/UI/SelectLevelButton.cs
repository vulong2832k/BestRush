using UnityEngine;

public class SelectLevelButton : MonoBehaviour
{
    public string levelName;

    public void OnClick()
    {
        LevelLoader loader = FindObjectOfType<LevelLoader>();
        loader.SetLevel(levelName);

        UIManager.Instance.CloseSelectMusic();

        FindObjectOfType<NoteSpawner>().StartGameFromUI();
    }
}