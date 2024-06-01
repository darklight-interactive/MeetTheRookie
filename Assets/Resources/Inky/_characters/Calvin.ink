
=== Calvin_Dialogue ===

= 4_3
// first interaction
{IsQuestComplete(calvin_first_interact) == false:
     Excuse me?
    ~ SetSpeaker(Speaker.Calvin)
     ...
    ~ SetSpeaker(Speaker.Lupe)
     Hello?
    ~ SetSpeaker(Speaker.Calvin)
     hisorryifeelbadnottalkingtoyou
     butjennysaidishouldkeepmymouthshut
     sounlessyouwanttotalkabout
     HOSI
     ican'thelpyou. 
    ~ SetSpeaker(Speaker.Lupe)
     ...what??
    ~ CompleteQuest(calvin_first_interact)
    {IsQuestComplete() && IsQuestComplete() && IsQuestComplete():
        -> hosi
    }
    -> DONE
}

// second interaction
{IsQuestComplete(calvin_first_interact):
     Can you, uh, speak a little slower?
    ~ SetSpeaker(Speaker.Calvin)
     I'mnotsupposedtobetalkingtoyouatall.
    ~ SetSpeaker(Speaker.Lupe)
     Dude, you're not in trouble.
    ~ SetSpeaker(Speaker.Calvin)
     ...
    ~ SetSpeaker(Speaker.Lupe)
    -> DONE
}

// Suspects
{IsQuestActive(suspects):
    ~ SetSpeaker(Speaker.Calvin)
    Jenkins Tomm.
    He used to work there.
    ~ SetSpeaker(Speaker.Lupe)
    // Add to Synthesis: Who Broke into the Old Winery?
    ~ DiscoverClue(calvin_suspects)
    -> DONE
}
    
= 4_3_calvin_questions

* So...you like HO:SI?
    ~ SetSpeaker(Speaker.Calvin)
     ...yes.
    ~ SetSpeaker(Speaker.Lupe)
     That's cool. -> DONE
    
* {IsClueFound(HOSI_calvin)}[Are you from around here?] -> DONE

* {IsClueFound(HOSI_calvin)}[The Old Winery on the hill...] -> DONE

* {IsClueFound(HOSI_calvin)}[So, Kettle Rock...] -> DONE

* {IsClueFound(personal_info_calvin) && IsClueFound(KR_calvin) && IsClueFound(winery_calvin)} [Okay, thanks...]
    ~ CompleteQuest(visited_calvin)
    -> DONE

=cal_hosi
    ~ SetSpeaker(Speaker.Lupe)
     Do you...
    * ...play as the hamster?
        -> DONE
    * ...play as the space invader?
        -> DONE
    
= HOSI_questions
    ~ SetSpeaker(Speaker.Calvin)
     Yeah.
     I...
     think it's cool.
     being a hamster, in space.
    ~ SetSpeaker(Speaker.Lupe)
    ~ DiscoverClue(HOSI_calvin)
    -> DONE

= HOSI_questions2
    ~ SetSpeaker(Speaker.Calvin)
     No.
     The invaders are the bad guys.
     I don't like being a bad guy, so I'm the hamster.
    ~ SetSpeaker(Speaker.Lupe)
    ~ DiscoverClue(HOSI_calvin)
    -> DONE

= personal_info_calvin
    ~ SetSpeaker(Speaker.Calvin)
     Yeah.
     I live on Oak Avenue.
     With my parents.
     And my dog.
     And my Grandma.
     My bedrooms on the second floor.
     I used to go to school here, too, until Kettle Rock Junior High shut down.
     I was in Mrs. Peterson's class.
     I got B+'s, usually.
     I tried out for soccer but didn't make the team.
    ~ SetSpeaker(Speaker.Lupe)
     That's ah, more than enough info. 
     But uh, what was your name?
    ~ SetSpeaker(Speaker.Calvin)
     Calvin. Calvin Coorsby.
    ~ SetSpeaker(Lupe)
     Thanks, Calvin.
    ~ DiscoverClue(personal_info_calvin)
    -> DONE

= winery_calvin
    ~ SetSpeaker(Speaker.Calvin)
    That place gives me the heebie-jeebies.
    It's probably haunted or cursed or something.
    ~ SetSpeaker(Speaker.Lupe)
     Why do you say that?
    ~ SetSpeaker(Speaker.Calvin)
    Because it never could stay open.
    It was always closed, open, bankrupt, making money, then losing money, on and on.
    And the stupid Goats were always up there, too.
    ~ SetSpeaker(Speaker.Lupe)
    // Add to Synthesis: Who broke into the Old Winery?
    {IsClueFound(roys_suspicion):
        ~ SetSpeaker(Speaker.Lupe)
         Someone said you maybe were up at the Winery? 
         Last night?
        ~ SetSpeaker(Speaker.Calvin)
         ...
         Ihavenoideawhatyou'retalkingaboutIwasathome.
        ~ SetSpeaker(Speaker.Lupe)
        ~ DiscoverClue(winery_calvin)
        -> DONE
    - else:
        ~ DiscoverClue(winery_calvin)
        -> DONE
}

= KR_calvin
    ~ SetSpeaker(Speaker.Calvin)
     It's fine, I guess.
     Not much to do other than game and--
    ~ SetSpeaker(Speaker.Lupe)
     ...and?
    ~ SetSpeaker(Speaker.Calvin)
     Er, nothing.
    ~ DiscoverClue(KR_calvin)
    -> DONE
