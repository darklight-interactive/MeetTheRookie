~  current_npc = "[Roy_Rodgerson]"
=== first_interact_roy ===
    ~ set_speaker(Speaker.Misra)
    [Misra] Hey, Roy!
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} Misra, mijo! 
    {current_npc} It's good to see you.
    ~ set_speaker(Speaker.Misra)
    [Misra] It's good to see you too, Uncle. 
    [Misra] How's business?
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} Meh. Not very good. But I am used to it by now.
    {current_npc} Who's your friend?
    
* Detective Lupe.
    {current_npc} A Detective? What's this about, Misra?
    ~ set_speaker(Speaker.Misra)
    [Misra] We're just asking a few questions about a disturbance last night.
    [Misra] Would you mind helping us?
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} Of course, in anyway that I can.
    ~ set_speaker(Speaker.Lupe)
-> DONE

=== questions ===

* Tell me about yourself. -> DONE
* Tell me about the Town. -> DONE
* Tell me about the Winery. -> DONE
* {IsClueFound(roy_personal_info) && IsClueFound(roy_winery_closing) && IsClueFound(roy_town_history)} Thank you for your cooperation. 
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} Of course. 
    {current_npc} Feel free to look around, if it helps any.
    ~ set_speaker(Speaker.Lupe)
    ~ CompleteQuest(interact_first_store)
    ~ CompleteQuest(visited_gen_store)
        -> DONE
 
 
=== window ===
    ~ set_speaker(Speaker.Lupe)
    [Lupe] You have a perfect view of the Winery from here.
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} I do.
    {current_npc} It's not much to look at these days.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Did you...see anything out of place, last night?
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} What do you mean?
    ~ set_speaker(Speaker.Misra)
    [Misra] We're investigating a break in that happened.
    [Misra] At the Winery, last night. 
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} Ay, Misra, you should not be poking around up there.
    ~ set_speaker(Speaker.Misra)
    [Misra] It's my job.
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} There's nothing there of interest.
    ~ set_speaker(Speaker.Misra)
    [Misra] Did you see anything, Uncle?
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc}...
    {current_npc} No. 
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Any ideas on who would want to break in and trash the place? 
        -> potential_suspects
        

=== merch_shirt ===
        [Lupe] Really leaning into that small town appeal, I see.
        ~ set_speaker(Speaker.Roy_Rodgerson)
        {current_npc} Believe it or not, those used to fly off the shelves.
        {current_npc} I'd sell out of them one month, and then by the month after, I couldn't pay people to touch them.
        ~ set_speaker(Speaker.Lupe)
-> DONE

=== merch_sticker ===
        ~ set_speaker(Speaker.Lupe)
        [Lupe] ...
        [Lupe] People seriously buy these?
        ~ set_speaker(Speaker.Roy_Rodgerson)
        {current_npc} You'd be surprised!
        ~ set_speaker(Speaker.Misra)
        [Misra] I have one!
        ~ set_speaker(Speaker.Lupe)
        [Lupe] I feel like you'd need witchcraft to get these things to sell.
        ~ set_speaker(Speaker.Roy_Rodgerson)
        {current_npc} Aha!
        ~ set_speaker(Speaker.Lupe)
-> DONE

=== pamphlet ===
* The Rocky Years?
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} Dios Mio; that was a hard time.
    {current_npc} Although I'd argue that we've fallen on harder times now.
    ~ set_speaker(Speaker.Misra)
    [Misra] Come on Roy, don't say that!
    [Misra] Have a little hope.
    [Misra] We'll bounce back, I'm sure!
    [Misra] We just need more time.
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} So optimistic, and I love you for that.
    {current_npc} But not very realistic.
    {current_npc} The Rocky Years was a time of inconsistency.
    {current_npc} We would flourish for the first part of the decade and struggle the latter.
    {current_npc} Disease of an unexplained nature swept through town.
    {current_npc} We couldn't link it to anything -
    {current_npc} not spoiled food or contaminated water,
    {current_npc} No known infection or flu.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] That bad, huh?
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} It was. I do not miss that time.
    ~ set_speaker(Speaker.Lupe)
    ~ DiscoverClue(rocky_years)
      // Add to Synthesis - The Town of KR
    -> DONE

* The Tragedy?
    {current_npc}...
    {current_npc} Terrible.
    {current_npc} Delusional people.
    {current_npc} Folk around here speak ill of them, but don't listen.
    {current_npc} Certain people even like to gossip, call them names, 
    {current_npc} insinuate that there was something else going on.
    [Lupe] Like what?
    {current_npc} Nothing real, and nothing worth talking about.
    ~ DiscoverClue(tragedy)
    // Add to Synthesis - The Town of KR
    -> pamphlet

* The Golden Age?
    {current_npc} A time of true prosperity.
    {current_npc} Business was, as you say, booming.
    {current_npc} The Winery sales were good, tourism was up.
    {current_npc} people recovered from whatever mysterious disease had passed over us.
    {current_npc} It was a good time, but it's over now.
    [Misra] For now. Good times will come again!
    {current_npc} Sometimes that's not the case, mijo.
    {current_npc} Sometimes, the good times are just behind us.
    {current_npc} And that's okay.
    [Misra] ...
    ~ DiscoverClue(golden_age)
        // Add to Synthesis - The Town of KR

    -> pamphlet
  
 // TODO : * {golden_age && tragedy && rocky_years} -> scene4_2  
 
=== personal_info ===
~ current_npc = "[Roy_Rodgerson]"
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} Well, I was born and raised here in Kettle Rock.
{current_npc} Lived here my whole life.
{current_npc} IdaHome is my family's business.
~ set_speaker(Speaker.Lupe)
[Lupe] Are they local, as well?
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} No. 
{current_npc} They've all passed. 
{current_npc} My sister was the last, in 1940. 
{current_npc} Unless, of course, you count Misra.
~ set_speaker(Speaker.Lupe)
[Lupe] You two are related?
~ set_speaker(Speaker.Misra)
[Misra] Not biologically.
[Misra] Roy's my godfather.
[Misra] When my parent's moved away, he became my guardian.
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} I begged and begged for Misra to leave, too. 
{current_npc} But they wouldn't go.
~ set_speaker(Speaker.Misra)
[Misra] Kettle Rock is my home.
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} Of course. 
{current_npc} But things are changing.
~ set_speaker(Speaker.Misra)
~ DiscoverClue(roy_personal_info)
// Add to Synthesis - The Town of KR
[Misra]...
~ set_speaker(Speaker.Lupe)

-> questions

=== town_history ===
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} As I'm sure you've noticed, it's not in the best shape.
{current_npc} Lots of places shutting down, going out of business.
{current_npc} Lots of locals moving away.
{current_npc} I think it's just unsustainable as it is.
~ set_speaker(Speaker.Misra)
[Misra] Roy, come on...
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} I know you don't like to hear it, but it's the truth.
{current_npc} You stepped up as Sheriff once the rest of the force retired to take care of this place
{current_npc} but it seems to me like it might be time to move on.
~ set_speaker(Speaker.Misra)
[Misra] I'm doing my best. 
[Misra] Someone's got to try and turn things around.
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} Ah, mijo...
~ set_speaker(Speaker.Lupe)
    // Add to Synthesis - The Town of KR
~ DiscoverClue(roy_town_history)
-> DONE

=== winery_closing ===
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} Ah, the straw that broke the camels back.
{current_npc} It finally shut down, about a month ago.
{current_npc} They closed it up tight.
{current_npc} I have no idea what will become of it.
{current_npc} There are only so many things you can do with an old Winery.
~ set_speaker(Speaker.Lupe)
~ DiscoverClue(roy_winery_closing)

-> DONE

=== potential_suspects ===
{IsQuestComplete(visited_arcade):
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} If I had to place blame,
    {current_npc} Those delinquents at Power Up.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Ah, them.
    ~ set_speaker(Speaker.Misra)
    [Misra] We talked with them earlier.
    [Misra] They didn't let much slip.
    ~ set_speaker(Speaker.Lupe)
    // Add to Synthesis: Who broke into the Old Winery?
    -> DONE
    - else:
    
    ~ set_speaker(Speaker.Roy_Rodgerson)
    {current_npc} If I had to pick someone...
    {current_npc} I'd say those troublemakers down at the Arcade.
    ~ set_speaker(Speaker.Misra)
    [Misra] Oh.
    [Misra] <i>Those</i> guys.
    // Add to Synthesis: Who broke into the Old Winery?
    ~ DiscoverClue(roys_suspicion)
    -> DONE

}

=== roy_teenagers ===
~ current_npc = "[Roy_Rodgerson]"
~ set_speaker(Speaker.Lupe)
[Lupe] We spoke to the teens at Power Up.
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} Those rascals.
{current_npc} Jenny still giving you a hard time?
~ set_speaker(Speaker.Misra)
[Misra] Ugh.
[Misra] Yes.
~ set_speaker(Speaker.Lupe)
[Lupe] They denied having anything to do with the Winery.
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} I'm sure they did.
~ set_speaker(Speaker.Lupe)
[Lupe] They did point us towards a "Jenkins Tomm".
~ set_speaker(Speaker.Roy_Rodgerson)
{current_npc} ...
{current_npc} I'm not sure he'll be any help to you.
{current_npc} Really, Misra, I think you two should leave this alone.
~ set_speaker(Speaker.Misra)
[Misra] We've got to get to the bottom of this!
~ set_speaker(Speaker.Lupe)
-> DONE

 


