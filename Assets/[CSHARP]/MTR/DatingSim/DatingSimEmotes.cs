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
    private class TrueDatingSimEmotes
    {
        [HideInInspector] public string key;
        public Texture2D value;

        public TrueDatingSimEmotes(string k, Texture2D v)
        {
            key = k;
            value = v;
        }
    }

    [Header("Emote Image")]
    public Texture2D currLupeEmote;
    public Texture2D currMisraEmote;
    public string lupeDefaultEmoteName = "neutral";
    public string misraDefaultEmoteName = "neutral";


    [SerializeField] private List<Texture2D> lupeImages = new List<Texture2D>();
    [SerializeField] private List<Texture2D> misraImages = new List<Texture2D>();

    [SerializeField] private List<TrueDatingSimEmotes> lupeEmotes = new List<TrueDatingSimEmotes>();
    [SerializeField] private List<TrueDatingSimEmotes> misraEmotes = new List<TrueDatingSimEmotes>();

    // private Dictionary<string, Texture2D> old_lupeEmotes = new Dictionary<string, Texture2D>();
    // private Dictionary<string, Texture2D> old_misraEmotes = new Dictionary<string, Texture2D>();

    public void MakeEmotes()
    {
        lupeEmotes.Clear();
        misraEmotes.Clear();

        foreach (Texture2D image in lupeImages)
        {
            string emote = Regex.Match(image.name.ToLower(), @"\((.*)\)").Groups[1].Value;
            emote = string.IsNullOrEmpty(emote) ? "404_INVALID_IMAGE_NAME" : emote;
            lupeEmotes.Add(new TrueDatingSimEmotes(emote, image));
        }
        foreach (Texture2D image in misraImages)
        {
            string emote = Regex.Match(image.name.ToLower(), @"\((.*)\)").Groups[1].Value;
            emote = string.IsNullOrEmpty(emote) ? "404_INVALID_IMAGE_NAME" : emote;
            misraEmotes.Add(new TrueDatingSimEmotes(emote, image));
        }
        SetEmote("lupe", lupeDefaultEmoteName);
        SetEmote("misra", misraDefaultEmoteName);
    }

    public bool SetEmote(string name, string emote)
    {
        if (name == "lupe" && lupeEmotes.Find(x => x.key == emote) != null)
        {
            currLupeEmote = lupeEmotes.Find(x => x.key == emote).value;
            return true;
        }
        else if (name == "misra" && misraEmotes.Find(x => x.key == emote) != null)
        {
            currMisraEmote = misraEmotes.Find(x => x.key == emote).value;
            return true;
        }
        Debug.LogError("DatingSimEmotes: Could not get emote \"" + emote + "\" of character \"" + name + "\"");
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