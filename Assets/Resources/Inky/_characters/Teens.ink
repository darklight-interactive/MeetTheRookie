

=== Teens_Dialogue ===
//  lupe hasn't mentioned hosi, Lupe has mentioned hosi, 
// haven't done intro before questions, have done intro before questions
// "asked all questions," -> blowoff statment
// "haven't asked Q1" ... "crazies mentioned"
//

// ~ SetSpeaker(Speaker.[Speaker])
// IsQuestComplete("boolean")      //add to global
// CompleteQuest("boolean")     // set bool as true, 
// ^^ make sure u add to global list

= jenny 
{IsQuestComplete(what_is_hosi) : // lupe asked about hosi

    // after jenny has voiced suspicion, this is her FINAL closing statement in the arcade 
    {IsClueFound(Mystery2.evidence_jenny): 
        ~ SetSpeaker(Speaker.Jenny)
        {Ugh! I just lost the level. I've got to focus, leave me alone.| I'm clearly busy. | See ya. } 
        -> DONE
    }
        
    //- askedJennysQuestions():
        //-> jenny_suspects
//    TODO FIX - askedJennysQuestions():
    //    ~ SetSpeaker(Speaker.Jenny)
    //    I've already told you what I know.
    //    I've got a highscore to beat.
    //    -> DONE
    
    //lupe's done intro before questions
    {IsQuestComplete(lupe_not_a_cop): 
        -> Jenny_Dialogue.jenny_questions
        - else:
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
        }
- else: // lupe hasn't asked about hosi yet
    {IsQuestComplete(jenny_first_interact):
        Die, stupid snakes! Feel my wrath! //if you've already talked to jenny once 
        -> DONE
        -else:
            ~ CompleteQuest(jenny_first_interact)
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


= calvin 

{IsQuestComplete(what_is_hosi) : // lupe asked about hosi
 //lupe did intro before questions
    {
    - IsClueFound(evidence_calvin):
        ~ SetSpeaker(Speaker.Calvin)
        {I'vetoldyoueverything.| SorryIcan'ttalkanymore.}
        -> DONE
    - IsQuestComplete(visited_calvin): // else if u have done the intro to questions already
        -> calvin_questions
    - else: //else, (haven't done the intro to questions yet)
        ~ SetSpeaker(Speaker.Lupe)
        Can you, uh, speak a little slower?
        ~ SetSpeaker(Speaker.Calvin)
        I'mnotsupposedtobetalkingtoyouatall.
        ~ SetSpeaker(Speaker.Lupe)
        Dude, you're not in trouble.
        ~ SetSpeaker(Speaker.Calvin)
        I'm not??
        ~CompleteQuest(visited_calvin)
        -> calvin_questions
    }

    
- else: //Lupe hasn't asked about hosi
    Excuse me?
    ~ CompleteQuest(calvin_first_interact)
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
    {IsQuestComplete(calvin_first_interact) && IsQuestComplete(jenny_first_interact) && IsQuestComplete(josh_first_interact):
        -> hosi
    -else:
        ->DONE
    }
  }
 

= calvin_questions

* [So...you like HO:SI?] -> cal_hosi
    
* {IsQuestComplete(calvin_hosi)}[Are you from around here?] -> personal_info_calvin

* {IsQuestComplete(calvin_hosi)}[The Old Winery on the hill...] -> winery_calvin

* {IsQuestComplete(calvin_hosi)}[Do you like living in Kettle Rock?] -> KR_calvin



//~ CompleteQuest(calvin_suspects)
-> DONE
= cal_hosi
    ~ SetSpeaker(Speaker.Calvin)
    ...yes.
    ~ SetSpeaker(Speaker.Lupe)
    Do you play as the, erm, Snake Invaders?
    ~ SetSpeaker(Speaker.Calvin)
    No.
    The Snake Invaders are the bad guys.
    I don't like being the bad guy.
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
    ~ SetSpeaker(Speaker.Lupe)
    Thanks, Calvin.
    ~ CompleteQuest(calvin_personal_info)
    -> DONE

= winery_calvin
    ~ SetSpeaker(Speaker.Calvin)
    That place gives me the heebie-jeebies.
    ~ SetSpeaker(Speaker.Calvin)
    It's probably haunted or cursed or something.
    ~ SetSpeaker(Speaker.Lupe)
    Why do you say that?
    ~ SetSpeaker(Speaker.Calvin)
    Because it never could stay open.
    ~ SetSpeaker(Speaker.Calvin)
    It was always closed, open, bankrupt, making money, then losing money, on and on.
    ~ SetSpeaker(Speaker.Calvin)
    And the stupid Goats were always up there, too.
    {IsClueFound(evidence_roy):
        ~ SetSpeaker(Speaker.Lupe)
        Someone said you maybe were up at the Winery? 
        Last night?
        ~ SetSpeaker(Speaker.Calvin)
        ...
        ~ SetSpeaker(Speaker.Calvin)
        Ihavenoideawhatyou'retalkingaboutIwasathome.
        ~ DiscoverClue(evidence_calvin)
    }
    ~ CompleteQuest(calvin_winery)
    -> DONE

=KR_calvin
    ~ CompleteQuest(calvin_KR) 
    ~ SetSpeaker(Speaker.Calvin)
 .
It's fine, I guess.
Not much to do other than game and--
 ~ SetSpeaker(Speaker.Lupe)
...and?
 ~ SetSpeaker(Speaker.Calvin)
Er, nothing.
    -> DONE
    
= calvin_sus //TODO LUPE NEEDS TO ASK CALVIN A QUESSTION BEFORE HE RESPONDS
    Calvin, you're not in trouble,
    but I need you to tell me who should be.
    Who do you think would've been at the Winery last night?
    ~ SetSpeaker(Speaker.Calvin)
    Uhhhhh...
    Jenkins Tomm.
    He used to work there.
    ~ DiscoverClue(evidence_calvin)
    -> DONE
    
= josh 
{IsQuestComplete(what_is_hosi): // lupe asked about hosi
   {
    - IsClueFound(evidence_josh): //after josh has voiced suspicion, this is his FINAL closing statement in the arcade 
            ~ SetSpeaker(Speaker.Josh)
            {DANCE BATTLE, BRO?| YOU DON'T WANT TO SEE MY SICK MOVES | BON BON VOYAGE.}
             -> DONE //END OF ARCADE INTERACTION
    - IsQuestComplete(visited_josh):
        -> Josh_Dialogue.josh_questions
    - else:
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
       // TODO ryan got sad
        ~ SetSpeaker(Speaker.Lupe)
        You probably don't matter as much, anyway.
        ~ SetSpeaker(Speaker.Josh)
        HEY.
        I MATTER.
        # Josh turns down the music a bit.
        What do you wanna know?
        ~ CompleteQuest(visited_josh)
        -> Josh_Dialogue.josh_questions
    }
- else: //Lupe hasn't asked about hosi
    ~ CompleteQuest(josh_first_interact)
    ~ SetSpeaker(Speaker.Lupe)
    Can I have a minute?
    ~ SetSpeaker(Speaker.Josh)
    I DUNNO, CAN YOU?
    ~ SetSpeaker(Speaker.Lupe)
    ...
    {IsQuestComplete(calvin_first_interact) && IsQuestComplete(jenny_first_interact) && IsQuestComplete(josh_first_interact): //if last first round of interactions, lupe asks about hosi
        -> hosi
    -else:
        -> DONE
    }
}



= hosi
    ~ SetSpeaker(Speaker.Lupe)
    What's "HO:SI?"
    ~ SetSpeaker(Speaker.Jenny)
    Hamster Origins: Snake Invaders.
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
    -> DONE
