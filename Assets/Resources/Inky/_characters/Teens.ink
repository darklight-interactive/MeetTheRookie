

=== Teens_Dialogue ===
//  lupe hasn't mentioned hosi, Lupe has mentioned hosi, 
// haven't done intro before questions, have done intro before questions
// "asked all questions," -> blowoff statment
// "haven't asked Q1" ... "crazies mentioned"

= calvin
    {
    -!IsQuestComplete(calvin_first_interact):
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
        ~ SetSpeaker(Speaker.Lupe)
        Can you, uh, speak a little slower?
        ~ SetSpeaker(Speaker.Calvin)
        I'mnotsupposedtobetalkingtoyouatall.
        ~ SetSpeaker(Speaker.Lupe)
        Dude, you're not in trouble.
        ~ SetSpeaker(Speaker.Calvin)
        I'm not??
        ~ CompleteQuest(visited_calvin)
        ~ CompleteQuest(calvin_first_interact)
        -> calvin_questions
    -else:
        ...
        -> calvin_questions
    }
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
    -> DONE

= calvin_questions
    * {!IsQuestComplete(what_is_hosi)}[What is HO:SI?] -> hosi
    * {IsQuestComplete(what_is_hosi)}[So...you like HO:SI?] -> calvin_hosi_question
    * {IsQuestComplete(calvin_hosi)}[Are you from around here?] -> calvin_personal_info_question
    * {IsQuestComplete(calvin_hosi)}[The Old Winery on the hill...] -> calvin_winery_question
    * {IsQuestComplete(calvin_hosi)}[Do you like living in Kettle Rock?] -> calvin_KR_question
    + {IsQuestComplete(calvin_winery)}[Any suspects?] -> calvin_sus_question

= calvin_hosi_question
    ~ SetSpeaker(Speaker.Calvin)
    ...yes.
    ~ SetSpeaker(Speaker.Lupe)
    Do you play as the, erm, Snake Invaders?
    ~ SetSpeaker(Speaker.Calvin)
    No.
    The Snake Invaders are the bad guys.
    I don't like being the bad guy.
    ~ CompleteQuest(calvin_hosi)
    -> calvin_questions
    
= calvin_personal_info_question
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
    -> calvin_questions

= calvin_winery_question
    ~ SetSpeaker(Speaker.Calvin)
    That place gives me the heebie-jeebies.
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
    }
    ~ CompleteQuest(calvin_winery)
    -> calvin_questions

= calvin_KR_question
    ~ SetSpeaker(Speaker.Calvin)
    It's fine, I guess.
    Not much to do other than game and--
    ~ SetSpeaker(Speaker.Lupe)
    ...and?
    ~ SetSpeaker(Speaker.Calvin)
    Er, nothing.
    ~ CompleteQuest(calvin_KR) 
    -> calvin_questions
    
= calvin_sus_question
    Calvin, you're not in trouble,
    but I need you to tell me who should be.
    Who do you think would've been at the Winery last night?
    ~ SetSpeaker(Speaker.Calvin)
    Uhhhhh...
    Jenkins Tomm.
    He used to work there.
    ~ SetSpeaker(Speaker.Lupe)
    Thanks, Calvin.
    ~ DiscoverClue(evidence_calvin)
    -> DONE
    
= josh 
    {
    -!IsQuestComplete(josh_first_interact):
        ~ SetSpeaker(Speaker.Lupe)
         Can I have a minute?
        ~ SetSpeaker(Speaker.Josh)
         I DUNNO, CAN YOU?
        ~ SetSpeaker(Speaker.Lupe)
         ...
        ~ CompleteQuest(josh_first_interact)
        -> DONE
    -!IsQuestComplete(visited_josh):
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
        // Josh turns down the music a bit.
         What do you wanna know?
        ~ CompleteQuest(visited_josh)
        -> josh_questions
    -IsQuestComplete(visited_josh):
        What do you wanna know?
        -> josh_questions
    }
    -> DONE

= josh_questions
    * {!IsQuestComplete(josh_personal_info)}[So. Who're you?] -> josh_personal_info_question
    * {IsQuestComplete(josh_personal_info)}[You a local Kettle Rockian?] -> josh_KR_question
    * {IsQuestComplete(josh_personal_info)}[The Old Winery on the hill...] -> josh_winery_question
    + {IsQuestComplete(josh_winery)} [Any suspects?] -> josh_suspicion_question

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
    -> josh_questions

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
    -> josh_questions

= josh_winery_question
    ~ SetSpeaker(Speaker.Josh)
    ...
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
    }
    ~ CompleteQuest(josh_winery)
    -> josh_questions


= josh_suspicion_question
    ~ SetSpeaker(Speaker.Lupe)
        Josh.
        If you had to pick one person in town that might've been at the Winery last night, who would it be?
    ~SetSpeaker(Speaker.Josh)
    I'd say Jenkins, but he's probably too drunk to tell you anything.
    ~ DiscoverClue(Mystery2.evidence_josh)
    -> DONE

= jenny
    {
    - IsQuestComplete(visited_jenny) || IsQuestComplete(jenny_personal_info) || IsQuestComplete(jenny_KR) || IsQuestComplete(jenny_winery):
        What.
        -> jenny_questions
    - else:
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
        {
        - !IsQuestComplete(what_is_hosi):
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
            ~ SetSpeaker(Speaker.Lupe)
            ~ CompleteQuest(what_is_hosi)
        - else:
            ~ SetSpeaker(Speaker.Jenny)
            No...
            You obviously hacked this thing with a skeleton key or something.
            <i>Classic</i> cop move.
        }
        ~ SetSpeaker(Speaker.Lupe)
        ...
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
        ~ CompleteQuest(visited_jenny)
        -> jenny_questions
    }
    -> DONE

= jenny_questions
    * [So you're a local?] -> jenny_personal_question
    * {IsQuestComplete(jenny_personal_info)}[Tell me about Kettle Rock.] -> jenny_KR_question
    * {IsQuestComplete(jenny_personal_info)}[The Old Winery on the hill...] -> jenny_winery_question
    * {IsQuestActive(jenny_crazies)} ["Crazies?"] -> jenny_crazies_question
    + {IsQuestComplete(jenny_winery)}[Any suspects?] -> jenny_suspicion_question

= jenny_personal_question
    ~ SetSpeaker(Speaker.Jenny)
    Mhm.
    ~ SetSpeaker(Speaker.Lupe)
    What do you do for fun?
    ~ SetSpeaker(Speaker.Jenny)
    ...
    Public artwork.
    ~ CompleteQuest(jenny_personal_info)
    -> jenny_questions

= jenny_KR_question
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
    ~ StartQuest(jenny_crazies)
    -> jenny_questions
    
= jenny_winery_question
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
    {IsClueFound(evidence_roy):
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
        ~ CompleteQuest(jenny_winery)
        -> DONE
    }
    -> jenny_questions

    
= jenny_crazies_question
    ~ SetSpeaker(Speaker.Jenny)
    .
    As in, crazy people.
    ~ SetSpeaker(Speaker.Lupe)
    Yeah, I got that part.
    ~ SetSpeaker(Speaker.Jenny)
    Uh, it's kinda self explanatory. 
    {IsQuestComplete(roy_tragedy):
        ~ SetSpeaker(Speaker.Lupe)
            Are you talking about the people who disappeared in 1940?
        ~ SetSpeaker(Speaker.Jenny)
            Maybe.
            Mom tells me to not speak ill of the dead.
    }
    ~ CompleteQuest(jenny_crazies)
    -> jenny_questions
    
= jenny_suspicion_question
    ~ SetSpeaker(Speaker.Lupe)
    Say I wanted to learn more about the Winery...anyone been talking about it lately?
    ~ SetSpeaker(Speaker.Jenny)
    Oh, God. 
    Jenkins won't shut up about it.
    Blah blah this, blah blah that.
    Gah.
    Dumb old Goat.
    ~ DiscoverClue(Mystery2.evidence_jenny)
    -> DONE