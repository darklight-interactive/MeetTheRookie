=== jenny_intro ===
~ current_npc = "[Jenny]"
[Lupe] Hey--
~ set_speaker(Speaker.Jenny)
{current_npc} Do you mind?
{current_npc} We're kinda busy.
~ set_speaker(Speaker.Lupe)
[Lupe] I just want to ask you a few questions.
~ set_speaker(Speaker.Jenny)
{current_npc} I don't talk to cops.
{current_npc} Especially <i>you</i>, Rookie.
~ set_speaker(Speaker.Misra)
[Misra] Come on, Jenny.
[Misra] You're still mad about the HO:SI thing?
~ set_speaker(Speaker.Lupe)
[Lupe] HO:SI...?
 ~ DiscoverClue(HOSI_mentioned)
    -> DONE

=== jenny_questions ===  
~ current_npc = "[Jenny]"
* Tell me about Kettle Rock. -> DONE

* The Old Winery on the hill... -> DONE

* You're a local? -> DONE

* {jenny_crazies} "Crazies"?
    -> DONE

* {IsClueFound(personal_info_jenny) && IsClueFound(winery_jenny) && IsClueFound(KR_Jenny)} [Right then.]
    ~ CompleteQuest(visited_jenny)
    -> DONE

= KR_Jenny
~ set_speaker(Speaker.Jenny)
{current_npc} Small ass town in the middle of nowhere.
{current_npc} Not much to see here.
~ set_speaker(Speaker.Lupe)
[Lupe] Yeah.
[Lupe] I noticed that there's not too many people around.
~ set_speaker(Speaker.Jenny)
{current_npc} Yeah, all the sane people have moved away in the last few years.
{current_npc} My mom wants to leave, too,
{current_npc} but she isn't sure where we would go.
~ set_speaker(Speaker.Lupe)
[Lupe] Do you like it here?
~ set_speaker(Speaker.Jenny)
{current_npc} Eh. This town is full of crazies and people stuck in denial.
~ set_speaker(Speaker.Misra)
[Misra] That's not very nice, Jenny.
~ set_speaker(Speaker.Jenny)
{current_npc} Case in point.
~ set_speaker(Speaker.Lupe)
~ DiscoverClue(jenny_crazies)
    -> DONE
    
    
= personal_info_jenny
~ set_speaker(Speaker.Jenny)
{current_npc} Mhm.
~ set_speaker(Speaker.Lupe)
[Lupe] What do you do for fun?
~ set_speaker(Speaker.Jenny)
{current_npc} ...
{current_npc} Public artwork.
~ set_speaker(Speaker.Lupe)
    -> DONE
    
= winery_jenny
~ set_speaker(Speaker.Jenny)
{current_npc} What about it?
~ set_speaker(Speaker.Lupe)
[Lupe] I don't know.
[Lupe] It seemed kinda important, to like, the town
~ set_speaker(Speaker.Jenny)
{current_npc} Oh, sure.
{current_npc} I mean, the stupid Goats sure liked it up there.
~ set_speaker(Speaker.Lupe)
// Add to Synthesis: WHo broke into the Old Winery?
    {IsClueFound(roys_suspicion):
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Someone told us that you guys like to hang around up there.
        ~ set_speaker(Speaker.Jenny)
        {current_npc} Psh. Whatever.
        ~ set_speaker(Speaker.Lupe)
        [Lupe] You know, I have knack for when someone's not telling me the whole truth, too.
        ~ set_speaker(Speaker.Jenny)
        {current_npc}...
        {current_npc} I've never set foot inside that place.
        {current_npc} That's the truth.
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Hmm.
            -> DONE
    - else:
    -> DONE
 }
 
= Crazies
~ set_speaker(Speaker.Jenny)
{current_npc} As in, crazy people.
~ set_speaker(Speaker.Lupe)
[Lupe] Yeah, I got that part.
~ set_speaker(Speaker.Jenny)
{current_npc} Uh, it's kinda self explanatory.
    {IsClueFound(tragedy):
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Are you talking about the people who disappeared in 1940?
        ~ set_speaker(Speaker.Jenny)
        {current_npc} Maybe.
        {current_npc} Mom tells me to not speak ill of the dead.
        ~ set_speaker(Speaker.Lupe)
            -> DONE
    - else:
        -> DONE
    }


=== j_suspects ===
~ current_npc = "[Jenny]"
~ set_speaker(Speaker.Jenny)
{current_npc} Oh, God. 
{current_npc} Jenkins won't shut up about it.
{current_npc} Blah blah this, blah blah that.
{current_npc} Gah.
{current_npc} Dumb old Goat.
~ set_speaker(Speaker.Lupe)
// Add to Synthesis: Who broke into the Old Winery?
~ DiscoverClue(jenny_suspects)
-> DONE
