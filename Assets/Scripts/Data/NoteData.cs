using System;
using System.Collections.Generic;

[Serializable]
public class NoteData
{
    public float time;
    public int lane;
    public float duration;
}

[Serializable]
public class LevelData
{
    public List<NoteData> notes;
}