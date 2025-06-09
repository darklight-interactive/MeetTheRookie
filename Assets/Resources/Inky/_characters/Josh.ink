
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
    ~ DiscoverClue(Mystery2.evidence_josh)
    -> DONE
}

= josh_questions
    * [So. Who're you?] -> josh_personal_info_question
    * [You a local Kettle Rockian?] -> josh_KR_question
    * [The Old Winery on the hill...] -> josh_winery_question

= josh_personal_info_question
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
    ~ CompleteQuest(josh_personal_info)
    -> DONE

= josh_KR_question
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
    ~ CompleteQuest(josh_KR)
    -> DONE

= josh_winery_question
    ~ SetSpeaker(Speaker.Josh)
    It's got good walls. 
    ~ SetSpeaker(Speaker.Lupe)
     What does that have to do with anything?
    ~ SetSpeaker(Speaker.Josh)
    I dunno, bro.
    You asked me.
    ~ SetSpeaker(Speaker.Lupe)
    {IsClueFound(evidence_roy):
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
        ~ CompleteQuest(josh_winery)
        -> DONE
    -else:
        ~ CompleteQuest(josh_winery)
        -> DONE
    }

= josh_suspicion_question
    ~ SetSpeaker(Speaker.Lupe)
        Josh.
        If you had to pick one person in town that might've been at the Winery last night, who would it be?
    ~SetSpeaker(Speaker.Josh)
    I'D SAY JENKINS, BUT HE'S PROBABLY TOO DRUNK TO TELL YOU ANYTHING.
    ~ DiscoverClue(Mystery2.evidence_josh)
    -> DONE