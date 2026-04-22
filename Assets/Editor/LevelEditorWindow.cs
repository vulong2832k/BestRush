#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

public class RhythmLevelEditorWindow : EditorWindow
{
    [SerializeField] private AudioClip _music;
    [SerializeField] private string _levelName = "level_1";

    private List<NoteData> _notes = new List<NoteData>();

    private bool _isRecording = false;
    private double _startDspTime;

    private float _previewHeight = 300f;
    private float _previewWidth = 200f;
    private float _noteSize = 8f;

    private bool _isPlayingPreview = false;

    private SerializedObject _so;
    private SerializedProperty _musicProp;
    private  SerializedProperty _levelNameProp;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<RhythmLevelEditorWindow>("Level Editor");
    }

    void OnEnable()
    {
        _so = new SerializedObject(this);
        _musicProp = _so.FindProperty("_music");
        _levelNameProp = _so.FindProperty("_levelName");

        SceneView.duringSceneGui += BlockSceneInput;
    }
    void OnDisable()
    {
        SceneView.duringSceneGui -= BlockSceneInput;
    }
    void BlockSceneInput(SceneView view)
    {
        if (!_isRecording) return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
    AudioSource GetAudioSource()
    {
        return GameObject.FindObjectOfType<AudioSource>();
    }
    void OnGUI()
    {
        HandleInput();

        GUILayout.Label("LEVEL SETTINGS", EditorStyles.boldLabel);


        _so.Update();

        EditorGUILayout.PropertyField(_levelNameProp);
        EditorGUILayout.PropertyField(_musicProp);

        _so.ApplyModifiedProperties();

        GUILayout.Space(10);

        GUILayout.Label("AUDIO PREVIEW", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        if (!_isPlayingPreview)
        {
            if (GUILayout.Button("Play Music"))
            {
                var audio = GetAudioSource();

                if (audio != null && _music != null)
                {
                    audio.clip = _music;
                    audio.time = 0;
                    audio.Play();
                    _isPlayingPreview = true;
                }
            }
        }
        else
        {
            if (GUILayout.Button("Stop Music"))
            {
                var audio = GetAudioSource();

                if (audio != null)
                    audio.Stop();

                _isPlayingPreview = false;
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("Load Level")) LoadLevel();
        if (GUILayout.Button("Save Level")) SaveLevel();
        if (GUILayout.Button("Clear Notes")) _notes.Clear();

        GUILayout.Space(10);

        if (!_isRecording)
        {
            if (GUILayout.Button("Start Recording"))
                StartRecording();
        }
        else
        {
            if (GUILayout.Button("Stop Recording"))
                StopRecording();

            GUILayout.Label("Recording... Press A S D F", EditorStyles.helpBox);
        }

        GUILayout.Space(10);
        GUILayout.Label($"Total Notes: {_notes.Count}");

        GUILayout.Space(20);

        if (_isRecording)
        {
            if (Event.current.type == EventType.Repaint)
                Repaint();
        }

        DrawPreview();
    }

    void HandleInput()
    {
        if (!_isRecording) return;

        Event e = Event.current;

        if (e.type == EventType.KeyDown)
        {
            double time = AudioSettings.dspTime - _startDspTime;

            if (e.keyCode == KeyCode.A) AddNote(0, time);
            if (e.keyCode == KeyCode.S) AddNote(1, time);
            if (e.keyCode == KeyCode.D) AddNote(2, time);
            if (e.keyCode == KeyCode.F) AddNote(3, time);

            Repaint();
        }
    }

    void StartRecording()
    {
        var audio = GetAudioSource();

        if (_music == null || audio == null)
        {
            Debug.LogError("Scene cần có AudioSource!");
            return;
        }

        _notes.Clear();

        _startDspTime = AudioSettings.dspTime + 0.1;

        audio.clip = _music;
        audio.PlayScheduled(_startDspTime);

        _isRecording = true;
        _isPlayingPreview = true;

        Focus();
    }

    void StopRecording()
    {
        _isRecording = false;

        _isPlayingPreview = false;
    }

    void AddNote(int lane, double time)
    {
        _notes.Add(new NoteData
        {
            lane = lane,
            time = (float)time,
            duration = 0
        });

        Debug.Log($"Lane {lane} - {time}");
    }

    void SaveLevel()
    {
        LevelData data = new LevelData();
        data.notes = _notes;

        string json = JsonUtility.ToJson(data, true);

        string folder = Application.dataPath + "/Resources/";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = folder + _levelName + ".json";

        File.WriteAllText(path, json);

        AssetDatabase.Refresh();

        Debug.Log("Saved: " + path);
    }

    void LoadLevel()
    {
        string path = Application.dataPath + "/Resources/" + _levelName + ".json";

        if (!File.Exists(path))
        {
            Debug.LogWarning("Level chưa tồn tại!");
            _notes = new List<NoteData>();
            return;
        }

        string json = File.ReadAllText(path);

        LevelData data = JsonUtility.FromJson<LevelData>(json);

        _notes = data.notes ?? new List<NoteData>();

        Debug.Log($"Loaded {_notes.Count} notes");
    }
    void DrawPreview()
    {
        GUILayout.Label("PREVIEW", EditorStyles.boldLabel);

        Rect rect = GUILayoutUtility.GetRect(_previewWidth, _previewHeight);
        EditorGUI.DrawRect(rect, Color.black);

        int laneCount = 4;
        float laneWidth = rect.width / laneCount;

        for (int i = 0; i < laneCount; i++)
        {
            Rect laneRect = new Rect(rect.x + i * laneWidth, rect.y, laneWidth, rect.height);
            EditorGUI.DrawRect(laneRect, new Color(0.15f, 0.15f, 0.15f));
        }

        if (_notes.Count == 0) return;

        float maxTime = 0f;
        foreach (var n in _notes)
            if (n.time > maxTime) maxTime = n.time;

        if (maxTime <= 0) maxTime = 1f;

        foreach (var note in _notes)
        {
            float t = note.time / maxTime;

            float y = Mathf.Lerp(rect.y, rect.yMax, t); 

            float x = rect.x + note.lane * laneWidth + laneWidth / 2;

            Rect noteRect = new Rect(x - _noteSize / 2, y, _noteSize, _noteSize);

            EditorGUI.DrawRect(noteRect, Color.green);
        }
    }
}
#endif