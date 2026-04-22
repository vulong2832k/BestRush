using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelRecorder : MonoBehaviour
{
    public AudioSource music;

    private List<NoteData> recordedNotes = new List<NoteData>();

    private double startDspTime;
    private bool isRecording;

    public void StartRecording()
    {
        recordedNotes.Clear();

        startDspTime = AudioSettings.dspTime + 0.1;
        music.PlayScheduled(startDspTime);

        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    void Update()
    {
        if (!isRecording) return;

        double songTime = AudioSettings.dspTime - startDspTime;

        if (Input.GetKeyDown(KeyCode.A)) AddNote(0, songTime);
        if (Input.GetKeyDown(KeyCode.S)) AddNote(1, songTime);
        if (Input.GetKeyDown(KeyCode.D)) AddNote(2, songTime);
        if (Input.GetKeyDown(KeyCode.F)) AddNote(3, songTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveToJson();
        }
    }

    void AddNote(int lane, double time)
    {
        recordedNotes.Add(new NoteData
        {
            lane = lane,
            time = (float)time,
            duration = 0
        });

        Debug.Log($"Note: lane {lane} - time {time}");
    }

    void SaveToJson()
    {
        LevelData data = new LevelData();
        data.notes = recordedNotes;

        string json = JsonUtility.ToJson(data, true);

        string path = Application.dataPath + "/level_1.json";
        File.WriteAllText(path, json);

        Debug.Log("Saved: " + path);
    }
}