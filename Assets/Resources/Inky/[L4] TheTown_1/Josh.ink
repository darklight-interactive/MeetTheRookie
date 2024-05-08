=== josh_intro ===
~ current_npc = "[Josh]"
~ set_speaker(Speaker.Lupe)
[Lupe] Can I have a minute?
~ set_speaker(Speaker.Josh)
{current_npc} I DUNNO, CAN YOU?
~ set_speaker(Speaker.Lupe)
[Lupe] ...
    -> DONE
    
=== josh_questions ===
~ current_npc = "[Josh]"
* So. Who're you?
    -> DONE

* You a local Kettle Rockian?
    -> DONE

* The Old Winery on the hill...
    -> DONE

* {IsClueFound(josh_personal_info) && IsClueFound(josh_questions) && IsClueFound(KR_josh)} [Well...] 
    ~ CompleteQuest(visited_josh)
    -> DONE

= josh_personal_info
~ set_speaker(Speaker.Josh)
{current_npc} Josh.
{current_npc} Josh the Squash.
~ set_speaker(Speaker.Lupe)
[Lupe] Sorry, squash?
~ set_speaker(Speaker.Josh)
{current_npc} Yuh. 
{current_npc} Because it rhymes.
~ set_speaker(Speaker.Lupe)
[Lupe] ...
~ set_speaker(Speaker.Josh)
{current_npc} You don't get it?
~ set_speaker(Speaker.Lupe)
[Lupe] I, uh...
    -> DONE

= KR_josh
    ~ set_speaker(Speaker.Josh)
    {current_npc} This place is SO boring.
    ~ set_speaker(Speaker.Misra)
    [Misra] That's a bit rude.
    ~ set_speaker(Speaker.Josh)
    {current_npc} Nothing cool happens.
    {current_npc} Everything's been closing, not like there was much to begin with.
    ~ set_speaker(Speaker.Misra)
    [Misra] It's okay, we'll get back on our feet. 
    [Misra] All we need is time. 
    [Misra] Time and--
    ~ set_speaker(Speaker.Josh)
    {current_npc} Yawwwnnnn. Boring.
    ~ set_speaker(Speaker.Lupe)
        -> DONE

= winery_josh
~ set_speaker(Speaker.Josh)
{current_npc} It's got good walls. 
~ set_speaker(Speaker.Lupe)
[Lupe] What does that have to do with anything?
~ set_speaker(Speaker.Josh)
{current_npc} I dunno, bro.
{current_npc} You asked me.
~ set_speaker(Speaker.Lupe)
{IsClueFound(roys_suspicion):
    ~ set_speaker(Speaker.Lupe)
    [Lupe] You wouldn't have happened to go up there recently?
    [Lupe] Say, last night?
    ~ set_speaker(Speaker.Josh)
    {current_npc} I, uh--
    # Josh turns his music back up.
    [Josh] SORRY, CAN'T HEAR YOU.
    [Lupe] You--
    [Josh] SORRY. 
    [Josh] THIS IS A REALLY GOOD SONG.
    ~ set_speaker(Speaker.Lupe)
        -> DONE
-else:
 -> DONE
}


=== jo_suspects ===
~ current_npc = "[Josh]"
~ set_speaker(Speaker.Josh)
{current_npc} I'D SAY JENKINS, BUT HE'S PROBABLY TOO DRUNK TO TELL YOU ANYTHING.
    ~ DiscoverClue(josh_suspects)
    // Add to Synthesis: Who broke into the Old Winery?
  -> DONE

