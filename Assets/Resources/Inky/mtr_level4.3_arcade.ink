
=== scene4_3 ===
// FUNC SCENE CHANGE
# Location: Power Up Arcade
//+ {CompleteQuest(visited_arcade)}[front_door] -> scene4_1.main_street

* [talk_to_misra] -> Misra_Dialogue
* [talk_to_jenny] -> Jenny_Dialogue



= arcade_machines
    {IsQuestComplete(visited_machines):
        [Lupe]{"Mac Pan"... huh | "Donkey King"...that's...okay.. | "GalaxyBattles!" | "Sidewalk Fighter" | So weird...}
    - else:
    [Lupe] I don't recognize any of these games.
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Yeah...
    [Misra] They're all knock offs of knock offs.
    [Misra] They're cheaper that way.
    [Misra] And twice the fun!
-> DONE
}
    
= teens
[Lupe] Is that...
[Lupe] Who you were talking about...?
~ SetSpeaker(Speaker.Misra)
[Misra] Yes.
[Misra] Don't be fooled.
[Misra] They're vicious.
~ SetSpeaker(Speaker.Lupe)
[Lupe] They're....
[Lupe] ...fourteen year olds.
~ SetSpeaker(Speaker.Misra)
[Misra] I know.
~ SetSpeaker(Speaker.Lupe)
[Lupe] They're barely five feet tall.
~ SetSpeaker(Speaker.Misra)
[Misra] <i> I know </i>.
~ SetSpeaker(Speaker.Lupe)
[Lupe] They--
~ SetSpeaker(Speaker.Jenny)
[Jenny] Hey Nimrods. 
[Jenny] We can hear you.
~ SetSpeaker(Speaker.Lupe)
    -> DONE
    
    
= teens_roulette

* [Jenny] -> Jenny_Dialogue
* [Calvin] -> Calvin_Dialogue
* [Josh] -> Josh_Dialogue
* {HOSI_mentioned} [What's HOSI?] -> Jenny_Dialogue


* {IsQuestComplete(visited_jenny)} [Jenny] -> Jenny_Dialogue
* {IsQuestComplete(calvin_first_interact)} [Calvin] -> Calvin_Dialogue
* {IsQuestComplete(josh_first_interact)} [Josh] -> Josh_Dialogue
    
        
* {IsQuestComplete(visited_josh) && IsQuestComplete(visited_calvin) && IsQuestComplete(visited_jenny)} [Say I wanted to learn more about the Winery...anyone been talking about it lately?]
    -> DONE
    
= teens_suspect
    ~ StartQuest(suspects)
    * [Jenny] -> Jenny_Dialogue
    * [Calvin] -> Calvin_Dialogue
    * [Josh] -> Josh_Dialogue
    
    * {IsClueFound(jenny_suspects) && IsClueFound(josh_suspects) && IsClueFound(calvin_suspects)} [So. Anything else you wanna get off your chests?] -> exit_scene
= exit_scene
    ~ SetSpeaker(Speaker.Jenny)
    [Jenny] Unless the Rookie Sheriff wants to conceit their HO:SI score, no.
    [Jenny] We're done talking with you losers.
    ~ SetSpeaker(Speaker.Lupe)
    ~ CompleteQuest(complete_arcade)
    ~ CompleteQuest(suspects)
        -> DONE
