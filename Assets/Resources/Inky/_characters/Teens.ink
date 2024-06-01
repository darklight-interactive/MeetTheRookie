=== Teens_Dialogue ===
//  lupe hasn't mentioned hosi, Lupe has mentioned hosi, 
// haven't done intro before questions, have done intro before questions
// "asked all questions," -> blowoff statment
// "haven't asked Q1" ... "crazies mentioned"


// ~ SetSpeaker(Speaker.[Speaker])
// IsQuestComplete("boolean")      //add to global
// CompleteQuest("boolean")     // set bool as true, 
// ^^ make sure u add to global list

= jenny 
 {IsQuestComplete(what_is_hosi) : // lupe asked about hosi
        {IsQuestComplete(lupe_not_a_cop): //lupe did intro before questions
            {IsQuestComplete(jenny_KR_question) && IsQuestComplete(jenny_local_question) && IsQuestComplete(jenny_winery_question) && IsQuestComplete(jenny_crazies_question):
                        ~ SetSpeaker(Speaker.Jenny)
                         I've already told you what I know.
                         I've got a highscore to beat.
                    - else:
                      -> jenny_questions
         
         
         
         
         
            }
    
        - else:
                {IsQuestComplete(lupe_not_a_cop): //lupe hasn't done intro before questions
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
                    ~ CompleteQuest(lupe_not_a_cop)
                      -> DONE 
                    }
                 
        
    - else: // lupe hasn't asked about hosi yet
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
            {IsQuestComplete(calvin_first_interact) && IsQuestComplete(josh_first_interact) && IsQuestComplete(jenny_first_interact):
                -> hosi
           -  else: 
             ~ SetSpeaker(Speaker.Jenny)
             Do you mind?
             -> DONE 
                 }
 

}
}

=jenny_questions
    * Tell me about Kettle Rock. -> KR_Jenny
    
    
    
    
    
= calvin 

{IsQuestComplete(what_is_hosi) : // lupe asked about hosi
  ~ SetSpeaker(Speaker.Lupe)
    Can you, uh, speak a little slower?
  ~ SetSpeaker(Speaker.Calvin)
    I'mnotsupposedtobetalkingtoyouatall.
  ~ SetSpeaker(Speaker.Lupe)
    Dude, you're not in trouble.
  ~ SetSpeaker(Speaker.Calvin)
    I'm not??
        -> DONE
        
- else: //Lupe hasn't asked about hosi

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
    
    {IsQuestComplete(calvin_first_interact) && IsQuestComplete(jenny_first_interact) && IsQuestComplete(josh_first_interact):
        -> hosi
    }
    
    }
    -> DONE

= josh 
{IsQuestComplete(what_is_hosi) : // lupe asked about hosi
 ~ SetSpeaker(Speaker.Lupe)
    Hey man, can you turn that down a bit?
     ~ SetSpeaker(Speaker.Josh)
     YOU DON'T LIKE THE BEAT?
     ~ SetSpeaker(Speaker.Lupe)
    It's just, really loud.
     ~ SetSpeaker(Speaker.Josh)
        THAT SOUNDS LIKE A YOU PROBLEM.
    ~ SetSpeaker(Speaker.Lupe)
    ...
     ~ SetSpeaker(Speaker.Lupe)
        Okay.
        You must not have anything important to say, then.
     ~ SetSpeaker(Speaker.Josh)
        WHAT?
    ~ SetSpeaker(Speaker.Lupe)
    I can just talk to your friends, then.
     ~ SetSpeaker(Speaker.Lupe)
    You probably don't matter as much, anyway.
     ~ SetSpeaker(Speaker.Josh)
        HEY.
        I MATTER.
    # Josh turns down the music a bit.
        What do you wanna know?
        -> DONE
        
- else: //Lupe hasn't asked about hosi
    ~ SetSpeaker(Speaker.Lupe)
     Can I have a minute?
    ~ SetSpeaker(Speaker.Josh)
     I DUNNO, CAN YOU?
    ~ SetSpeaker(Speaker.Lupe)
     ...
    ~ CompleteQuest(josh_first_interact)
    {IsQuestComplete(calvin_first_interact) && IsQuestComplete(jenny_first_interact) && IsQuestComplete(josh_first_interact):
        -> hosi
}
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
    ~ CompleteQuest(what_is_hosi)
    ~ DiscoverClue(HOSI_highscore)
        -> DONE

= teens 
~ SetSpeaker(Speaker.Lupe)
Is that...
 ~ SetSpeaker(Speaker.Jenny)
 DIE SNAKES, DIE!
  ~ SetSpeaker(Speaker.Lupe)
 Who you were talking about...?
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