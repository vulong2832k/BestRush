using System;
using System.Reflection;
using UnityEngine;

public static class AudioEditorUtil
{
    static Type audioUtilType;
    static MethodInfo playMethod;
    static MethodInfo stopMethod;

    static AudioEditorUtil()
    {
        audioUtilType = Type.GetType("UnityEditor.AudioUtil,UnityEditor");
        playMethod = audioUtilType.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public);
        stopMethod = audioUtilType.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public);
    }

    public static void Play(AudioClip clip)
    {
        playMethod?.Invoke(null, new object[] { clip });
    }

    public static void Stop()
    {
        stopMethod?.Invoke(null, null);
    }
}