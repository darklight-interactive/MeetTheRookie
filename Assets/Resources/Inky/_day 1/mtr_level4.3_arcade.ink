

=== scene4_3 ===
# Location: Power Up Arcade
= intro
    ~ closeDoor()
    {IsQuestComplete(visited_josh) && IsQuestComplete(visited_jenny) && IsQuestComplete(visited_calvin):
        ~ SetSpeaker(Speaker.Misra)
        Do we really have to come back here...
        -> DONE
    - else:
        ~ SetSpeaker(Speaker.Misra)
        Maybe if we don't move they won't see us.
        ~ SetSpeaker(Speaker.Jenny)
        DIE SNAKES, DIE!
        ~ SetSpeaker(Speaker.Lupe)
        Is that...
        ~ SetSpeaker(Speaker.Lupe)
        Who you're talking about...?
        ~ SetSpeaker(Speaker.Misra)
        Yes.
        Don't be fooled.
        They're vicious.
        ~ SetSpeaker(Speaker.Lupe)
        They're....
        ...fourteen year olds.
        ~ SetSpeaker(Speaker.Misra)
        I know.
        ~ SetSpeaker(Speaker.Lupe)
        They're barely five feet tall.
        ~ SetSpeaker(Speaker.Misra)
        <i> I know </i>.
        ~ SetSpeaker(Speaker.Lupe)
        They--
        ~ SetSpeaker(Speaker.Jenny)
        Hey Nimrods. 
        We can hear you.
        ~ SetSpeaker(Speaker.Lupe)
        ->DONE
    }

= arcade_machines
    {IsQuestComplete(visited_machines):
        {"Mac Pan"... huh | "Donkey King"...that's...okay.. | "GalaxyBattles!" | "Sidewalk Fighter" | So weird...}
    - else:
     ~ CompleteQuest(visited_machines)
    ~ SetSpeaker(Speaker.Lupe)
     I don't recognize any of these games.
    ~ SetSpeaker(Speaker.Misra)
     Yeah...
     They're all knock offs of knock offs.
     They're cheaper that way.
     And twice the fun!
    -> DONE
    }

= jenny
    -> Teens_Dialogue.jenny
= calvin
    -> Teens_Dialogue.calvin
= josh 
    -> Teens_Dialogue.josh
    
= teens_suspect
    ~ StartQuest(suspects)
    *  -> Jenny_Dialogue
    * [Calvin] -> Calvin_Dialogue
    * [Josh] -> Josh_Dialogue
    
//    * {IsClueFound(jenny_suspects) && IsClueFound(josh_suspects) && IsClueFound(calvin_suspects)} [So. Anything else you wanna get off your chests?] -> exit_scene

= exit_scene
    ~ openDoor()
    ~ SetSpeaker(Speaker.Jenny)
        Bye loooosersss.
    ~ ChangeGameScene("scene4_1")    
    -> DONE
    
    
