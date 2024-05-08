=== calvin_intro ===
~ current_npc = "[Calvin]"
[Lupe] Excuse me?
~ set_speaker(Speaker.Calvin)
{current_npc} ...
~ set_speaker(Speaker.Lupe)
[Lupe] Hello?
~ set_speaker(Speaker.Calvin)
{current_npc} hisorryifeelbadnottalkingtoyou
{current_npc} butjennysaidishouldkeepmymouthshut
{current_npc} sounlessyouwanttotalkabout
{current_npc} HOSI
{current_npc} ican'thelpyou. 
~ set_speaker(Speaker.Lupe)
[Lupe] ...what??
    -> DONE
    
    
    
=== calvin_questions ===
~ current_npc = "[Calvin]"
* So...you like HO:SI?
    ~ set_speaker(Speaker.Calvin)
    {current_npc} ...yes.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] That's cool. -> DONE
    
* {IsClueFound(HOSI_calvin)}[Are you from around here?] -> DONE

* {IsClueFound(HOSI_calvin)}[The Old Winery on the hill...] -> DONE

* {IsClueFound(HOSI_calvin)}[So, Kettle Rock...] -> DONE

* {IsClueFound(personal_info_calvin) && IsClueFound(KR_calvin) && IsClueFound(winery_calvin)} [Okay, thanks...]
    ~ CompleteQuest(visited_calvin)
    -> DONE

=cal_hosi
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Do you...
* ...play as the hamster?
    -> DONE
* ...play as the space invader?
    -> DONE
    
= HOSI_questions
~ set_speaker(Speaker.Calvin)
{current_npc} Yeah.
{current_npc} I...
{current_npc} think it's cool.
{current_npc} being a hamster, in space.
~ set_speaker(Speaker.Lupe)
~ DiscoverClue(HOSI_calvin)
-> DONE

= HOSI_questions2
~ set_speaker(Speaker.Calvin)
{current_npc} No.
{current_npc} The invaders are the bad guys.
{current_npc} I don't like being a bad guy, so I'm the hamster.
~ set_speaker(Speaker.Lupe)
~ DiscoverClue(HOSI_calvin)
-> DONE

= personal_info_calvin
~ set_speaker(Speaker.Calvin)
{current_npc} Yeah.
{current_npc} I live on Oak Avenue.
{current_npc} With my parents.
{current_npc} And my dog.
{current_npc} And my Grandma.
{current_npc} My bedrooms on the second floor.
{current_npc} I used to go to school here, too, until Kettle Rock Junior High shut down.
{current_npc} I was in Mrs. Peterson's class.
{current_npc} I got B+'s, usually.
{current_npc} I tried out for soccer but didn't make the team.
~ set_speaker(Speaker.Lupe)
[Lupe] That's ah, more than enough info. 
[Lupe] But uh, what was your name?
~ set_speaker(Speaker.Calvin)
{current_npc} Calvin. Calvin Coorsby.
~ set_speaker(Lupe)
[Lupe] Thanks, Calvin.
-> DONE

= winery_calvin
~ set_speaker(Speaker.Calvin)
{current_npc} That place gives me the heebie-jeebies.
{current_npc} It's probably haunted or cursed or something.
~ set_speaker(Speaker.Lupe)
[Lupe] Why do you say that?
~ set_speaker(Speaker.Calvin)
{current_npc} Because it never could stay open.
{current_npc} It was always closed, open, bankrupt, making money, then losing money, on and on.
{current_npc} And the stupid Goats were always up there, too.
~ set_speaker(Speaker.Lupe)
// Add to Synthesis: Who broke into the Old Winery?
    {IsClueFound(roys_suspicion):
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Someone said you maybe were up at the Winery? 
        [Lupe] Last night?
        ~ set_speaker(Speaker.Calvin)
        {current_npc} ...
        {current_npc} Ihavenoideawhatyou'retalkingaboutIwasathome.
        ~ set_speaker(Speaker.Lupe)
       -> DONE
        - else:
    -> DONE
}

= KR_calvin
~ set_speaker(Speaker.Calvin)
{current_npc} It's fine, I guess.
{current_npc} Not much to do other than game and--
~ set_speaker(Speaker.Lupe)
[Lupe] ...and?
~ set_speaker(Speaker.Calvin)
{current_npc} Er, nothing.
~ set_speaker(Speaker.Lupe)
    -> DONE

=== c_suspects ===
~ current_npc = "[Calvin]"
~ set_speaker(Speaker.Calvin)
{current_npc} Jenkins Tomm.
{current_npc} He used to work there.
~ set_speaker(Speaker.Lupe)
// Add to Synthesis: Who Broke into the Old Winery?
~ DiscoverClue(calvin_suspects)
-> DONE



