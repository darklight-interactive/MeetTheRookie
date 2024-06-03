using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Name Collection", menuName = "Addressables/Character Name Collection")]
public class CharacterColors : ScriptableObject
{
    [SerializeField] List<string> characterNames = new List<string> { "Unknown", "Misra", "Lupe", "Chief_Thelton", "Marlowe", "Beth", "Mel", "Roy_Rodgerson", "Jenny", "Calvin", "Josh", "Irene", "Jenkins" };
    [SerializeField] List<CharColor> characterColors = new List<CharColor>();

    public Color this[string key]
    {
        get => GetCharacterColor(key);
        set => SetCharacterColor(key, value);
    }

    [Serializable]
    private class CharColor
    {
        public string name;
        public Color color;

        public CharColor(string n, Color c)
        {
            name = n;
            color = c;
        }
    }

    Color GetCharacterColor(string character)
    {
        CharColor chara = characterColors.Find(x => x.name == character);
        if(chara == null){
            Debug.LogError("CharacterColors: "+character+" does not have a color");
            return Color.black;
        }
        return chara.color;
    }

    void SetCharacterColor(string character, Color color)
    {
        CharColor chara = characterColors.Find(x => x.name == character);
        if(chara != null){
            chara.color = color;
        }
        else{
            Debug.LogError("CharcterColors: Could not find "+character+" to assign a color to");
        }
    }

    public void AddCharacters()
    {
        foreach (string cha in characterNames)
        {
            if(characterColors.Find(x => x.name == cha) == null){
                characterColors.Add(new CharColor(cha, Color.black));
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CharacterColors))]
public class CharacterNamesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add Characters"))
        {
            CharacterColors characterNames = (CharacterColors)target;
            characterNames.AddCharacters();
        }
    }
}
#endif