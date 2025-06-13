=== scene4_3 ===
# Location: Power Up Arcade
-> DONE
= intro
    ~ closeDoor()
    {IsQuestComplete(visited_josh) || IsQuestComplete(visited_jenny) || IsQuestComplete(visited_calvin):
        ~ SetSpeaker(Speaker.Misra)
        Do we really have to come back here...
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
    }
    -> DONE

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
    }
    -> DONE

= jenny
    -> Teens_Dialogue.jenny
= calvin
    -> Teens_Dialogue.calvin
= josh 
    -> Teens_Dialogue.josh

= exit_scene
    {IsClueFound(evidence_jenny) || IsClueFound(evidence_josh) || IsClueFound(evidence_calvin):
        ~ openDoor()
        ~ SetSpeaker(Speaker.Jenny)
            Bye loooosersss.
        ~ ChangeGameScene("scene4_1", 2)
    -else:
        ~ SetSpeaker(Speaker.Misra)
        We haven't really found anything yet...
        Let's keep asking questions before we go.
    }
    -> DONE
    
    
