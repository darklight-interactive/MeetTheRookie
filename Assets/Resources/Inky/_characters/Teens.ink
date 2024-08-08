
=== function askedJennysQuestions()
    ~ return IsQuestComplete(jenny_KR_question) && IsQuestComplete(jenny_personal_question) && IsQuestComplete(jenny_winery_question) && IsQuestComplete(jenny_crazies_question)
=== function askedCalvinsQuestions()
    ~ return IsQuestComplete(calvin_KR_questions) && IsQuestComplete(calvin_personal_question) && IsQuestComplete(calvin_winery_question)
=== function askedJoshsQuestions()
    ~ return IsQuestComplete(josh_KR_question) && IsQuestComplete(josh_winery_question) && IsQuestComplete(josh_personal_questions)
    
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
    {
    
    - IsQuestComplete(jenny_suspicion): //after jenny has voiced suspicion, this is her FINAL closing statement in the arcade 
            ~ SetSpeaker(Speaker.Jenny)
            {Ugh! I just lost the level. I've got to focus, leave me alone.| I'm clearly busy. | See ya. } //END OF ARCADE INTERACTIONS
             -> DONE
        
    - askedJennysQuestions():
        -> jenny_suspects
//    TODO FIX - askedJennysQuestions():
    //    ~ SetSpeaker(Speaker.Jenny)
    //    I've already told you what I know.
    //    I've got a highscore to beat.
    //    -> DONE
    - IsQuestComplete(lupe_not_a_cop): //lupe's done intro before questions
        -> jenny_questions
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
= jenny_questions
    * [Tell me about Kettle Rock.] -> KR_Jenny
    * [The Old Winery on the hill...] -> winery_jenny
    * [You're a local?] -> personal_jenny
    * {IsQuestComplete(jenny_crazies_question)} ["Crazies?"] -> hmcrazies
    
= KR_Jenny
    ~ SetSpeaker(Speaker.Jenny)
    Well.
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
    ~ CompleteQuest(jenny_KR_question)
    ~ CompleteQuest(jenny_crazies_question)
        -> DONE
    
= winery_jenny
~ SetSpeaker(Speaker.Jenny)
    Yeah?
     What about it?
    ~ SetSpeaker(Speaker.Lupe)
     I don't know.
     It seemed kinda important, to like, the town.
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
            ~ CompleteQuest(jenny_winery_question)
                -> DONE
                
        - else:
            ~ DiscoverClue(winery_jenny)
            ~ CompleteQuest(jenny_winery_question)
            -> DONE
        }
= personal_jenny
    ~ SetSpeaker(Speaker.Jenny)
    .
    Mhm.
    ~ SetSpeaker(Speaker.Lupe)
    What do you do for fun?
    ~ SetSpeaker(Speaker.Jenny)
     ...
    Public artwork.
    ~ CompleteQuest(jenny_personal_question)
    -> DONE
    
= hmcrazies
    ~ SetSpeaker(Speaker.Jenny)
    ~ CompleteQuest(jenny_crazies_question)
    .
    As in, crazy people.
    ~ SetSpeaker(Speaker.Lupe)
    Yeah, I got that part.
    ~ SetSpeaker(Speaker.Jenny)
    Uh, it's kinda self explanatory. 
    
    -> DONE
    
= jenny_suspects
~ SetSpeaker(Speaker.Lupe)
    Say I wanted to learn more about the Winery...anyone been talking about it lately?
 ~ SetSpeaker(Speaker.Jenny)
     Oh, God. 
     Jenkins won't shut up about it.
     Blah blah this, blah blah that.
     Gah.
     Dumb old Goat.
    // Add to Synthesis: Who broke into the Old Winery?
    ~ CompleteQuest(jenny_suspicion)
    ~CompleteQuest(complete_arcade)
    ~ DiscoverClue(jenny_suspects)
        -> DONE

= calvin 

{IsQuestComplete(what_is_hosi) : // lupe asked about hosi
 //lupe did intro before questions
    {
    - IsQuestComplete(calvin_suspicion): //after calvin has voiced suspicion, this is her FINAL closing statement in the arcade 
            ~ SetSpeaker(Speaker.Calvin)
            {I'vetoldyoueverything.| SorryIcan'ttalkanymore.}
             -> DONE // END OF ARCADE INTERACTIONS
        
    - askedCalvinsQuestions():
        //if asked all teens first round of questions
        -> calvin_sus
//    TODO FIX- askedCalvinsQuestions(): // else if u've asked calvin all of his questions
    //        ~ SetSpeaker(Speaker.Calvin)
    //        SorryI'vegotnothingelseforyou.
    //        -> DONE
    - IsQuestComplete(calvin_questions_intro): // else if u have done the intro to questions already
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
        ~CompleteQuest(calvin_questions_intro)
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
    
* {HOSI_calvin}[Are you from around here?] -> personal_info_calvin

* {HOSI_calvin}[The Old Winery on the hill...] -> winery_calvin

* {HOSI_calvin}[Do you like living in Kettle Rock?] -> KR_calvin



//~ CompleteQuest(calvin_suspects)
-> DONE
= cal_hosi
~ SetSpeaker(Speaker.Calvin)
.
...yes.
~ SetSpeaker(Speaker.Lupe)
Do you play as the, erm, Snake Invaders?
~ SetSpeaker(Speaker.Calvin)
No.
The Snake Invaders are the bad guys.
I don't like being the bad guy.
-> DONE
=personal_info_calvin
~ CompleteQuest(calvin_personal_question)
~ SetSpeaker(Speaker.Calvin)
 .
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

-> DONE
=winery_calvin
~ CompleteQuest(calvin_winery_question)
~ SetSpeaker(Speaker.Calvin)
.
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
// Add to Synthesis: Who broke into the Old Winery?
    {roys_suspicion:
        ~ SetSpeaker(Speaker.Lupe)
        Someone said you maybe were up at the Winery? 
        Last night?
        ~ SetSpeaker(Speaker.Calvin)
        ...
        ~ SetSpeaker(Speaker.Calvin)
        Ihavenoideawhatyou'retalkingaboutIwasathome.
       -> DONE
        - else:
    -> DONE
}
=KR_calvin
~ CompleteQuest(calvin_KR_questions) 
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
 ~ SetSpeaker(Speaker.Lupe)
 ~ CompleteQuest(calvin_suspicion)
 ~CompleteQuest(complete_arcade)
Calvin, you're not in trouble,
but I need you to tell me who should be.
Who do you think would've been at the Winery last night?
 ~ SetSpeaker(Speaker.Calvin)
 Uhhhhh...
 Jenkins Tomm.
He used to work there.
    -> DONE
    
= josh 
{IsQuestComplete(what_is_hosi): // lupe asked about hosi
   {
    - IsQuestComplete(josh_suspicion): //after josh has voiced suspicion, this is her FINAL closing statement in the arcade 
            ~ SetSpeaker(Speaker.Josh)
            {DANCE BATTLE, BRO?| YOU DON'T WANT TO SEE MY SICK MOVES | BON BON VOYAGE.}
             -> DONE //END OF ARCADE INTERACTION
    - askedJoshsQuestions():
        -> josh_sus
//    TODO fix this logic - askedJoshsQuestions():
    //    ~ SetSpeaker(Speaker.Josh)
    //    YOU DIG IT?
    //    -> DONE
    - IsQuestComplete(josh_questions_intro):
        -> josh_questions
    - else:
        ~ SetSpeaker(Speaker.Lupe)
        ~ CompleteQuest(josh_questions_intro)
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
        -> josh_questions
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


=josh_questions
* [So. Who're you?] -> josh_personal_info

* [You a local Kettle Rockian?] -> KettleR_josh

* [The Old Winery on the hill...] -> wine_josh
    
= josh_personal_info
~CompleteQuest(josh_personal_questions)
~ SetSpeaker(Speaker.Josh)
.
Josh.
Josh the Squash.
~ SetSpeaker(Speaker.Lupe)
Sorry, squash?
~ SetSpeaker(Speaker.Josh)
Yuh. 
~ SetSpeaker(Speaker.Josh)
Because it rhymes.
~ SetSpeaker(Speaker.Lupe)
...
~ SetSpeaker(Speaker.Josh)
You don't get it?
~ SetSpeaker(Speaker.Lupe)
I, uh...no?
-> DONE
= KettleR_josh
~CompleteQuest(josh_KR_question) 
 ~ SetSpeaker(Speaker.Josh)
    Yeah.
    Unfortunately.
    This place is SO boring.
 ~ SetSpeaker(Speaker.Misra)
    That's a bit rude.
 ~ SetSpeaker(Speaker.Josh)
    Nothing cool happens.
 ~ SetSpeaker(Speaker.Josh)
    Everything's been closing, not like there was much to begin with.
~ SetSpeaker(Speaker.Misra)
    It's okay, we'll get back on our feet. 
    All we need is time. 
    Time and--
 ~ SetSpeaker(Speaker.Josh)
    Yawwwnnnn. Boring.
    -> DONE
    
= wine_josh
~CompleteQuest(josh_winery_question) 
 ~ SetSpeaker(Speaker.Josh)
    That place?
    It's got good walls. 
~ SetSpeaker(Speaker.Lupe)
What does that have to do with anything?
 ~ SetSpeaker(Speaker.Josh)
 I dunno, bro.
 You asked me.
~ SetSpeaker(Speaker.Lupe)
    You wouldn't have happened to go up there recently?
    Say, last night?
     ~ SetSpeaker(Speaker.Josh)
    I, uh--
    # Josh turns his music back up.
     ~ SetSpeaker(Speaker.Josh)
    SORRY, CAN'T HEAR YOU.
    ~ SetSpeaker(Speaker.Lupe)
    You--
    ~ SetSpeaker(Speaker.Josh)
    SORRY. 
    THIS IS A REALLY GOOD SONG.
-> DONE

//~CompleteQuest(josh_suspects)
= josh_sus
~ CompleteQuest(josh_suspicion)
~CompleteQuest(complete_arcade)
~ SetSpeaker(Speaker.Lupe)
    Josh.
    If you had to pick one person in town that might've been at the Winery last night, who would it be?
~SetSpeaker(Speaker.Josh)
I'D SAY JENKINS, BUT HE'S PROBABLY TOO DRUNK TO TELL YOU ANYTHING.
-> DONE

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
    ~ DiscoverClue(HOSI_highscore)
        -> DONE

= teens 
~ closeDoor()
{IsQuestComplete(jenny_suspicion) || IsQuestComplete(josh_suspicion) || IsQuestComplete(calvin_suspicion):
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
->DONE
}