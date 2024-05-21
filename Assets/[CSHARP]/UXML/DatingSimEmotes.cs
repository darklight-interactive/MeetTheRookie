using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(fileName = "New Emote Collection", menuName = "Addressables/Emote Collection")]
public class DatingSimEmotes : ScriptableObject
{
    [Serializable]
    private struct ExposedDictionary
    {
        [HideInInspector] public string key;
        public Texture2D value;

        public ExposedDictionary(string k, Texture2D v)
        {
            key = k;
            value = v;
        }
    }

    public Texture2D currLupeEmote;
    public Texture2D currMisraEmote;
    public string lupeDefaultEmoteName = "neutral";
    public string misraDefaultEmoteName = "neutral";

    [SerializeField] private List<Texture2D> lupeImages = new List<Texture2D>();
    [SerializeField] private List<Texture2D> misraImages = new List<Texture2D>();

    [SerializeField] private List<ExposedDictionary> lupeEmotesReadOnly = new List<ExposedDictionary>();
    [SerializeField] private List<ExposedDictionary> misraEmotesReadOnly = new List<ExposedDictionary>();


    private Dictionary<string, Texture2D> lupeEmotes = new Dictionary<string, Texture2D>();
    private Dictionary<string, Texture2D> misraEmotes = new Dictionary<string, Texture2D>();

    public void MakeEmotes()
    {
        lupeEmotes.Clear();
        misraEmotes.Clear();
        lupeEmotesReadOnly.Clear();
        misraEmotesReadOnly.Clear();

        foreach (Texture2D image in lupeImages)
        {
            string emote = Regex.Match(image.name.ToLower(), @"\((.*)\)").Groups[1].Value;
            emote = string.IsNullOrEmpty(emote) ? "404_INVALID_IMAGE_NAME" : emote;
            lupeEmotes.Add(emote, image);
            lupeEmotesReadOnly.Add(new ExposedDictionary(emote, image));
        }
        foreach (Texture2D image in misraImages)
        {
            string emote = Regex.Match(image.name.ToLower(), @"\((.*)\)").Groups[1].Value;
            emote = string.IsNullOrEmpty(emote) ? "404_INVALID_IMAGE_NAME" : emote;
            misraEmotes.Add(emote, image);
            misraEmotesReadOnly.Add(new ExposedDictionary(emote, image));
        }
        SetEmote("lupe", lupeDefaultEmoteName);
        SetEmote("misra", misraDefaultEmoteName);
    }

    public bool SetEmote(string name, string emote)
    {
        if (name == "lupe" && lupeEmotes.TryGetValue(emote, out currLupeEmote)) { return true; }
        else if (name == "misra" && misraEmotes.TryGetValue(emote, out currMisraEmote)) { return true; }
        return false;
    }

    private void OnValidate()
    {
        //MakeEmotes();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DatingSimEmotes))]
public class DatingSimEmotesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update Dating Sim Emotes"))
        {
            DatingSimEmotes datingSimEmotes = (DatingSimEmotes)target;
            datingSimEmotes.MakeEmotes();
        }
    }
}
#endif