using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Scan the story text 
/// </summary>
public class InkyDecryptor
{
    /// <summary>
    /// Look for [SpeakerName:] at the beginning of story text when finding a speaker.
    /// </summary>
    Regex scanner = new Regex(@"^(\[(?<speaker>.+):\]){0,1}(?<dialog>.*)");
    public string speakerName = "[ Unknown ]";
    public string textBody = " default text body";

    public InkyDecryptor(string storyText)
    {
        // Get the token values from the dialogueReader
        Match dialogueTokens = scanner.Match(storyText);
        if (dialogueTokens.Success)
        {
            // if speaker is found, set the speaker text
            if (dialogueTokens.Groups["speaker"].Success)
            {
                this.speakerName = dialogueTokens.Groups["speaker"].Value;
            }

            this.textBody = dialogueTokens.Groups["dialog"].Value;
        }
        else
        {
            Debug.LogError("Regex match for dialog not found.");
            this.textBody = storyText;
        }
    }
}
