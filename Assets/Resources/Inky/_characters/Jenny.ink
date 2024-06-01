=== Jenny_Dialogue ===
{IsQuestComplete(visited_jenny):
        ~ SetSpeaker(Speaker.Jenny)
        {Look, unless you want to talk about HOSI, we're done. | Get lost.}
        -> DONE
    -else:
        ~ CompleteQuest(visited_jenny)
        ~ SetSpeaker(Speaker.Lupe)
         Hey--
        ~ SetSpeaker(Speaker.Jenny)
         Do you mind?
         We're kinda busy.
        ~ SetSpeaker(Speaker.Lupe)
         I just want to ask you a few questions.
        ~ SetSpeaker(Speaker.Jenny)
         I don't talk to cops.
         Especially <i>you</i>, Rookie.
        ~ SetSpeaker(Speaker.Misra)
         Come on, Jenny.
         You're still mad about the HO:SI thing?
        ~ SetSpeaker(Speaker.Lupe)
         HO:SI...?
        ~ DiscoverClue(HOSI_mentioned)
        {IsQuestComplete() && IsQuestComplete() && IsQuestComplete():
            -> hosi
        }
        -> DONE
}

= hosi
    ~ SetSpeaker(Speaker.Lupe)
    What's "HO:SI?"
    ~ SetSpeaker(Speaker.Jenny)
     Hamster Origins: Space Invaders.
    ~ SetSpeaker(Speaker.Misra)
     It's the game they're playing.
     They've been trying to beat it for a while now.
    ~ SetSpeaker(Speaker.Lupe)
     What does that have to do with anything? 
    ~ SetSpeaker(Speaker.Misra)
     It's the only game in here they <i>don't</i> have the high score on.
    ~ SetSpeaker(Speaker.Lupe)
     Why does that matter?
    ~ SetSpeaker(Speaker.Misra)
     ...
     Because <i>I</i> have the high score.
    ~ DiscoverClue(HOSI_highscore)
        -> DONE

// Secondary Dialogue

{IsClueFound(HOSI_highscore):
     Well, <i>I'm</i> not a cop.
     And I'm not the one blocking you from your high score.
    ~ SetSpeaker(Speaker.Jenny)
     ...
     What do you want, then?
    ~ SetSpeaker(Speaker.Lupe)
     I'm...just passing through. 
     I wanted to learn a little more about the town.
    ~ SetSpeaker(Speaker.Jenny)
     I can tell when someone's lying, you know.
    ~ SetSpeaker(Speaker.Lupe)
     Am I lying?
    ~ SetSpeaker(Speaker.Jenny)
     ...
    ~ SetSpeaker(Speaker.Lupe)
        -> DONE
}

// Kettle Rock
{IsQuestActive(KR_jenny_quest):
    ~ SetSpeaker(Speaker.Jenny)
     Small ass town in the middle of nowhere.
     Not much to see here.
    ~ SetSpeaker(Speaker.Lupe)
     Yeah.
     I noticed that there's not too many people around.
    ~ SetSpeaker(Speaker.Jenny)
     Yeah, all the sane people have moved away in the last few years.
     My mom wants to leave, too,
     but she isn't sure where we would go.
    ~ SetSpeaker(Speaker.Lupe)
     Do you like it here?
    ~ SetSpeaker(Speaker.Jenny)
     Eh. This town is full of crazies and people stuck in denial.
    ~ SetSpeaker(Speaker.Misra)
     That's not very nice, Jenny.
    ~ SetSpeaker(Speaker.Jenny)
     Case in point.
    ~ SetSpeaker(Speaker.Lupe)
    ~ DiscoverClue(jenny_crazies)
    ~ DiscoverClue(KR_Jenny)
    ~ CompleteQuest(KR_jenny_quest)
        -> DONE
}

{IsQuestActive(personal_info_jenny_quest):
    ~ SetSpeaker(Speaker.Jenny)
     Mhm.
    ~ SetSpeaker(Speaker.Lupe)
     What do you do for fun?
    ~ SetSpeaker(Speaker.Jenny)
     ...
     Public artwork.
    ~ CompleteQuest(personal_info_jenny_quest)
    ~ DiscoverClue(personal_info_jenny)
        -> DONE
}

{IsQuestActive(winery_jenny_quest):
    ~ SetSpeaker(Speaker.Jenny)
     What about it?
    ~ SetSpeaker(Speaker.Lupe)
     I don't know.
     It seemed kinda important, to like, the town
    ~ SetSpeaker(Speaker.Jenny)
     Oh, sure.
     I mean, the stupid Goats sure liked it up there.
    ~ SetSpeaker(Speaker.Lupe)
    // Add to Synthesis: WHo broke into the Old Winery?
        {IsClueFound(roys_suspicion):
            ~ SetSpeaker(Speaker.Lupe)
             Someone told us that you guys like to hang around up there.
            ~ SetSpeaker(Speaker.Jenny)
             Psh. Whatever.
            ~ SetSpeaker(Speaker.Lupe)
             You know, I have knack for when someone's not telling me the whole truth, too.
            ~ SetSpeaker(Speaker.Jenny)
            ...
             I've never set foot inside that place.
             That's the truth.
            ~ SetSpeaker(Speaker.Lupe)
             Hmm.
            ~ DiscoverClue(winery_jenny)
                -> DONE
        }
        - else:
            ~ DiscoverClue(winery_jenny)
            -> DONE
}

{IsClueFound(jenny_crazies):
    ~ SetSpeaker(Speaker.Jenny)
     As in, crazy people.
    ~ SetSpeaker(Speaker.Lupe)
     Yeah, I got that part.
    ~ SetSpeaker(Speaker.Jenny)
     Uh, it's kinda self explanatory.
        {IsClueFound(tragedy):
            ~ SetSpeaker(Speaker.Lupe)
             Are you talking about the people who disappeared in 1940?
            ~ SetSpeaker(Speaker.Jenny)
             Maybe.
             Mom tells me to not speak ill of the dead.
            ~ SetSpeaker(Speaker.Lupe)
                -> DONE
        - else:
            -> DONE
        }
}

// Suspects
{IsQuestActive(suspects):
    ~ SetSpeaker(Speaker.Jenny)
     Oh, God. 
     Jenkins won't shut up about it.
     Blah blah this, blah blah that.
     Gah.
     Dumb old Goat.
    // Add to Synthesis: Who broke into the Old Winery?
    ~ DiscoverClue(jenny_suspects)
        -> DONE
}


= 4_3_jenny_questions
* Tell me about Kettle Rock.
    ~ StartQuest(KR_jenny_quest)
    -> DONE

* The Old Winery on the hill... 
    ~ StartQuest(winery_jenny_quest)
    -> DONE

* You're a local? 
    ~ StartQuest(personal_info_jenny_quest)
    -> DONE

* {jenny_crazies} "Crazies"?
    -> DONE

* {IsClueFound(personal_info_jenny) && IsClueFound(winery_jenny) && IsClueFound(KR_Jenny)} [Right then.]
    ~ CompleteQuest(visited_jenny)
    -> DONE
