
=== Misra_Dialogue ===
{IsQuestComplete(visited_misra) == false:
    ~ CompleteQuest(visited_misra)
    ~ SetSpeaker(Speaker.Misra)
    Here we are! 
    Kettle Rock, Main Street. Heart of the Downtown. 
    There's bound to be some locals around - where do you want to start?
    -> DONE
-else: 
    {IsQuestComplete(visited_roy):
        ~ SetSpeaker(Speaker.Misra)
        I'm sorry if Roy seems like a bit of a downer.
        He has no faith.
        ~ SetSpeaker(Speaker.Lupe)
        He seems like he's got a pretty good acceptance of the situation.
        From what I can tell.
        ~ SetSpeaker(Speaker.Misra)
        Well, you've only been here a day...
    -else:
        ~SetSpeaker(Speaker.Misra)
        There's bound to be some locals around - where do you want to start?
    }
    -> DONE
}
{IsQuestComplete(visited_jenny) && IsQuestComplete(complete_arcade):
    ~ SetSpeaker(Speaker.Misra)
     [Misra] Those guys are the worst.
    ~ SetSpeaker(Speaker.Lupe)
     [Lupe] They're definitely hiding <i>something</i>.
    ~ SetSpeaker(Speaker.Misra)
     [Misra] Good luck getting anything out of them.
    ~ SetSpeaker(Speaker.Lupe)
     // Add to Synthesis: Who broke into the Winery?
    -> DONE
}
-> DONE

