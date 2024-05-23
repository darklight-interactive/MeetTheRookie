
=== Josh_Dialogue ===

= 4_3

// first interact
{IsQuestComplete(josh_first_interact) == false:
    
    ~ SetSpeaker(Speaker.Lupe)
     Can I have a minute?
    ~ SetSpeaker(Speaker.Josh)
     I DUNNO, CAN YOU?
    ~ SetSpeaker(Speaker.Lupe)
     ...
    ~ CompleteQuest(josh_first_interact)
    -> DONE
}

// second interact
{IsQuestComplete(josh_first_interact):
     Hey man, can you turn that down a bit?
    ~ SetSpeaker(Speaker.Josh)
     YOU DON'T LIKE THE BEAT?
    ~ SetSpeaker(Speaker.Lupe)
     It's just, really loud
    ~ SetSpeaker(Speaker.Josh)
     THAT SOUNDS LIKE A YOU PROBLEM.
    ~ SetSpeaker(Speaker.Lupe)
     ...
     Okay.
     You must not have anything important to say, then.
    ~ SetSpeaker(Speaker.Josh)
     WHAT?
    ~ SetSpeaker(Speaker.Lupe)
     I can just talk to your friends, then.
     You probably don't matter as much, anyway.
    ~ SetSpeaker(Speaker.Josh)
     HEY.
     I MATTER.
    # Josh turns down the music a bit.
     What do you wanna know?
    ~ SetSpeaker(Speaker.Lupe)
    -> DONE
}
    
// Suspects
{IsQuestActive(suspects):
    ~ SetSpeaker(Speaker.Josh)
    I'D SAY JENKINS, BUT HE'S PROBABLY TOO DRUNK TO TELL YOU ANYTHING.
    ~ DiscoverClue(josh_suspects)
    // Add to Synthesis: Who broke into the Old Winery?
    -> DONE
}

= 4_3_josh_questions

* So. Who're you?
    ~ StartQuest(personal_info_josh_quest)
    -> DONE

* You a local Kettle Rockian?
    ~ StartQuest(KR_josh_quest)
    -> DONE

* The Old Winery on the hill...
    ~ StartQuest(winery_josh_quest)
    -> DONE

* {IsClueFound(personal_info_josh) && IsClueFound(winery_josh) && IsClueFound(KR_josh)} [Well...] 
    ~ CompleteQuest(visited_josh)
    -> DONE

= 4_3_josh_personal_info
    ~ SetSpeaker(Speaker.Josh)
    Josh.
    Josh the Squash.
    ~ SetSpeaker(Speaker.Lupe)
     Sorry, squash?
    ~ SetSpeaker(Speaker.Josh)
    Yuh. 
    Because it rhymes.
    ~ SetSpeaker(Speaker.Lupe)
     ...
    ~ SetSpeaker(Speaker.Josh)
    You don't get it?
    ~ SetSpeaker(Speaker.Lupe)
     I, uh...
    ~ DiscoverClue(personal_info_josh)
    -> DONE

= 4_3_KR_josh
    ~ SetSpeaker(Speaker.Josh)
     This place is SO boring.
    ~ SetSpeaker(Speaker.Misra)
     That's a bit rude.
    ~ SetSpeaker(Speaker.Josh)
     Nothing cool happens.
     Everything's been closing, not like there was much to begin with.
    ~ SetSpeaker(Speaker.Misra)
     It's okay, we'll get back on our feet. 
     All we need is time. 
     Time and--
    ~ SetSpeaker(Speaker.Josh)
     Yawwwnnnn. Boring.
    ~ SetSpeaker(Speaker.Lupe)
    ~ DiscoverClue(KR_josh)
    -> DONE

= 4_3_winery_josh
    ~ SetSpeaker(Speaker.Josh)
    It's got good walls. 
    ~ SetSpeaker(Speaker.Lupe)
     What does that have to do with anything?
    ~ SetSpeaker(Speaker.Josh)
    I dunno, bro.
    You asked me.
    ~ SetSpeaker(Speaker.Lupe)
    {IsClueFound(roys_suspicion):
        ~ SetSpeaker(Speaker.Lupe)
         You wouldn't have happened to go up there recently?
         Say, last night?
        ~ SetSpeaker(Speaker.Josh)
        I, uh--
        # Josh turns his music back up.
         SORRY, CAN'T HEAR YOU.
         You--
         SORRY. 
         THIS IS A REALLY GOOD SONG.
        ~ DiscoverClue(winery_josh)
        ~ CompleteQuest(winery_josh_quest)
        -> DONE
    -else:
        ~ DiscoverClue(winery_josh)
        ~ CompleteQuest(winery_josh_quest)
        -> DONE
    }

