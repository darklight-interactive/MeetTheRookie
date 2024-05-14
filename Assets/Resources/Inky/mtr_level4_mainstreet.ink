

// applicable everywhere
VAR current_npc = ""


//Variables for Main St
VAR closed_signs = 0




// Quest Objective of Scene: Who broke into the Old Winery?
// Current Synthesis Pages: Who broke into the Old Winery?
// AND The Town of Kettle Rock (for general town information)

// ====== QUEST CHAIN =======
LIST QuestChain = visited_goop, visited_symbol, visited_misra, visited_gen_store, interact_first_store, visited_arcade, interact_first_arcade, visited_machines, visited_jenny, visited_calvin, visited_josh, irene_intro, memorial_plaque_visited, irene_convo_1, irene_convo_2, jenkins_wakes_up

    
// ====== CLUES =======
LIST Clues = merch_pamphlet, roys_suspicion, roy_personal_info, roy_winery_closing, golden_age, tragedy, rocky_years, roy_town_history, HOSI_mentioned, jenny_crazies, HOSI_calvin, jenny_suspects, josh_suspects, calvin_suspects, goats_mentioned, sacrifice_mentioned, KR_irene, closed_shops_irene, jenkins_winery, sarah_mentioned, council_mentioned, sacrifice_mentioned_jenkins, symbol_evidence, goop_evidence

=== level4 ===
Hello Main - Level 4
* [Scene 4_1 - Main Street] -> scene4_1

=== scene4_1 === 
# Location: Main Street
# Lupe and Misra find themselves on the Main Street of Kettle Rock, in front of a Fountain. 
# The General Store and Arcade are accessible.
# The Bar is not.
# Any store marked with a closed sign is not accessible.

+ [talk to misra] -> talk_to_misra
+ [the rockin kettle] -> the_rockin_kettle
+ [strange symbol] -> strange_symbol_on_fountain
+ [goop] -> goop_on_fountain
+ [idahome and goods] -> scene4_2
+ [power up arcade] -> scene4_3
+ [laundromat] -> laundromat_closed_sign
+ [einab sign] -> einab_closed_sign
+ [apartments for lease] -> apartments_for_lease_sign
+ [diner closed sign] -> diner_closed_sign
+ [clothing store closed sign] -> clothing_store_closed_sign
+ {closed_signs >= 5} ["The Heart of Kettle Rock" seems a bit...barren.]
-> DONE

= talk_to_misra
-> Misra_Dialogue

= strange_symbol_on_fountain
    {IsQuestComplete(visited_symbol):
    ~ SetSpeaker(Speaker.Misra)
    [Misra]{It's probably just some historical symbol. | I don't think it's anything important. | We should move on. | Where to next?}
    ~ SetSpeaker(Speaker.Lupe)
    // Add to Synthesis - The Town of KR
    -> DONE
    - else:
    [Lupe] What's this?
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Hm. Looks like some sort of silly engravement.
    ~ SetSpeaker(Speaker.Lupe)
    ~  DiscoverClue(symbol_evidence)
        ~ CompleteQuest(visited_symbol)
    # ADD TO SYNTHESIS
    -> DONE
    }

= goop_on_fountain
    {IsQuestComplete(visited_goop):
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] {So weird...|It looks...squishy?| It smells kinda acidic. | Weird.}
        -> DONE
    - else: 
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] More of this stuff.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] You've seen it before?
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] Yeah. 
        [Lupe] It was at the Gas Station near the edge of town. 
     ~ CompleteQuest(visited_goop)
     ~ DiscoverClue(goop_evidence)
    // Add to Synthesis - The Town of KR

     -> DONE
    }


   
= the_rockin_kettle
    {IsQuestComplete(visited_gen_store) && IsQuestComplete(visited_arcade):
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Looks like the Bar is open! Shall we?
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] I shouldn't drink.
        [Lupe] I still need to drive back out tonight once the road is cleared.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Right, about that...
        [Misra] I forgot to tell you, but the earliest I can get someone out there to clear the tree is tomorrow morning.
        [Misra] I'm really sorry.
        [Misra] I know you had somewhere to be.
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] Ah...
        [Lupe] Well, that's not great.
        [Lupe] But I suppose there's nothing you can do about it.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Think of it this way...we get more time to crack this case!
        [Misra] But in the meantime, let's take a bit of a break...
        ~ SetSpeaker(Speaker.Lupe)
        -> scene4_4
        
    - else:
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Someone wants a drink, I see.
        [Misra] The Rockin Kettle doesn't open until happy hour! 
        [Misra] We can come back later.
        ~ SetSpeaker(Speaker.Lupe)
    -> DONE
    }
    
= laundromat_closed_sign
    ~ closed_signs ++ 
    ~ SetSpeaker(Speaker.Misra)
    [Misra] The good ol' 24/7 Laundromat!
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] Doesn't seem very 24/7 to me.
        -> DONE


= einab_closed_sign
    ~ closed_signs ++ 
    [Lupe] A lot of things seem to be closed...
    -> DONE

= apartments_for_lease_sign
    ~ closed_signs ++ 
    [Lupe] Hm..
        -> DONE
    
= diner_closed_sign
    ~ closed_signs ++ 
    [Lupe] Jeez, what <i>is</i> open?
    -> DONE


= clothing_store_closed_sign
   ~ closed_signs ++ 
   [Lupe] Another closed business.
   ~ SetSpeaker(Speaker.Misra)
   [Misra] Things have been rough lately.
   ~ SetSpeaker(Speaker.Lupe)
        -> DONE
        
= heart_of_kettle
        ~ SetSpeaker(Speaker.Misra)
        [Misra] It has slowed down.
        [Misra] A bit.
        [Misra] Usually it's busy and full of life,
        [Misra] but things have been bumpy for a lot of locals lately,
        [Misra] Not enough profit to keep them going.
        [Misra] Breaks my heart a bit.
        ~ SetSpeaker(Speaker.Lupe)
        // Add to Synthesis - The Town of KR
        -> DONE


=== scene4_2 ===
// FUNC SCENE CHANGE
# Location: IdaHome and Goods, General Store

{IsQuestComplete(interact_first_store):
   
   * [store_window] -> DONE
        
   
   * [merch_shirt] -> DONE
        
   
   * [merch_sticker] -> DONE
        
   
   + [merch_pamphlet]
        # Lupe leans close to a pamphlet that has four pages detailing the town history.
        ~ DiscoverClue(merch_pamphlet)
    -> DONE
   
   * {IsClueFound(merch_pamphlet)} Do you mind explaining this pamphlet a bit? -> DONE

}

* [Roy_Rodgerson] -> DONE

* {IsQuestComplete(visited_gen_store)} [Roy_Rodgerson] -> DONE

// + {questions}[front_door]


= 4_2_roy_rogerson
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
    [Roy_Rodgerson] Let me know if you need anything from me!
        {CompleteQuest(visited_arcade):
            -> DONE
        - else:
        -> scene4_2
    }

= 4_2_front_door
     ~ CompleteQuest(visited_gen_store)
    [Lupe] Thanks again.
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Bye, Roy!
    {current_npc} Take care!
    // ?
    ~ SetSpeaker(Speaker.Lupe)
->DONE

=== scene4_3 ===
// FUNC SCENE CHANGE
# Location: Power Up Arcade

//+ {CompleteQuest(visited_arcade)}[front_door] -> scene4_1.main_street

+ {IsQuestComplete(visited_arcade)}[teens]
     ~ SetSpeaker(Speaker.Jenny)
     [Jenny] {Look, unless you want to talk about HOSI, we're done. | Get lost.}
     ~ SetSpeaker(Speaker.Lupe)
        -> DONE


*[Misra]
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Okay...
        [Misra] Just so you know, these guys are a litte...
        [Misra] ...<i>intense.</i>
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] I deal with intense people all the time.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Yeah?
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] Criminals.
        [Lupe] Convicts.
        [Lupe] Killers.
        [Lupe] I'm not afraid of whoever is in there.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] If you say so...
        ~ SetSpeaker(Speaker.Lupe)
        ~ CompleteQuest(interact_first_arcade)
        -> DONE
= arcade
{IsQuestComplete(interact_first_arcade):

    + [arcade_machines]
        {IsQuestComplete(visited_machines):
            [Lupe]{"Mac Pan"... huh | "Donkey King"...that's...okay.. | "GalaxyBattles!" | "Sidewalk Fighter" | So weird...}
        - else:
        [Lupe] I don't recognize any of these games.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Yeah...
        [Misra] They're all knock offs of knock offs.
        [Misra] They're cheaper that way.
        [Misra] And twice the fun!
        ~ SetSpeaker(Speaker.Lupe)
        -> DONE
        }
    
    + [teens] 
        {IsQuestComplete(visited_arcade):
            ~ SetSpeaker(Speaker.Jenny)
            [Jenny] {Look, unless you want to talk about HOSI, we're done. | Get lost.}
            ~ SetSpeaker(Speaker.Lupe)
            -> DONE
        - else:
        -> DONE
    }
    
        
}

= teens
[Lupe] Is that...
[Lupe] Who you were talking about...?
~ SetSpeaker(Speaker.Misra)
[Misra] Yes.
[Misra] Don't be fooled.
[Misra] They're vicious.
~ SetSpeaker(Speaker.Lupe)
[Lupe] They're....
[Lupe] ...fourteen year olds.
~ SetSpeaker(Speaker.Misra)
[Misra] I know.
~ SetSpeaker(Speaker.Lupe)
[Lupe] They're barely five feet tall.
~ SetSpeaker(Speaker.Misra)
[Misra] <i> I know </i>.
~ SetSpeaker(Speaker.Lupe)
[Lupe] They--
~ SetSpeaker(Speaker.Jenny)
[Jenny] Hey Nimrods. 
[Jenny] We can hear you.
~ SetSpeaker(Speaker.Lupe)
    -> DONE
    
    
=== teens_roulette ===

* [Jenny] -> DONE

* [Calvin] -> DONE

* [Josh] -> DONE

*{HOSI_mentioned} [What's HOSI?]
    ~ SetSpeaker(Speaker.Jenny)
    [Jenny] Hamster Origins: Space Invaders.
    ~ SetSpeaker(Speaker.Misra)
    [Misra] It's the game they're playing.
    [Misra] They've been trying to beat it for a while now.
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] What does that have to do with anything? 
    ~ SetSpeaker(Speaker.Misra)
    [Misra] It's the only game in here they <i>don't</i> have the high score on.
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] Why does that matter?
    ~ SetSpeaker(Speaker.Misra)
    [Misra] ...
    [Misra] Because <i>I</i> have the high score.
    ~ SetSpeaker(Speaker.Lupe)
-> teens_roulette

//*{jenny_intro} [Jenny]
    [Lupe] Well, <i>I'm</i> not a cop.
    [Lupe] And I'm not the one blocking you from your high score.
    ~ SetSpeaker(Speaker.Jenny)
    [Jenny] ...
    [Jenny] What do you want, then?
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] I'm...just passing through. 
    [Lupe] I wanted to learn a little more about the town.
    ~ SetSpeaker(Speaker.Jenny)
    [Jenny] I can tell when someone's lying, you know.
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] Am I lying?
    ~ SetSpeaker(Speaker.Jenny)
    [Jenny] ...
    ~ SetSpeaker(Speaker.Lupe)
        -> DONE

//*{calvin_intro} [Calvin]
    [Lupe] Can you, uh, speak a little slower?
    ~ SetSpeaker(Speaker.Calvin)
    [Calvin] I'mnotsupposedtobetalkingtoyouatall.
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] Dude, you're not in trouble.
    ~ SetSpeaker(Speaker.Calvin)
    [Calvin] ...
    ~ SetSpeaker(Speaker.Lupe)
        -> DONE
    
//* {josh_intro} [Josh]
    [Lupe] Hey man, can you turn that down a bit?
    ~ SetSpeaker(Speaker.Josh)
    [Josh] YOU DON'T LIKE THE BEAT?
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] It's just, really loud
    ~ SetSpeaker(Speaker.Josh)
    [Josh] THAT SOUNDS LIKE A YOU PROBLEM.
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] ...
    [Lupe] Okay.
    [Lupe] You must not have anything important to say, then.
    ~ SetSpeaker(Speaker.Josh)
    [Josh] WHAT?
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] I can just talk to your friends, then.
    [Lupe] You probably don't matter as much, anyway.
    ~ SetSpeaker(Speaker.Josh)
    [Josh] HEY.
    [Josh] I MATTER.
    # Josh turns down the music a bit.
    [Josh] What do you wanna know?
    ~ SetSpeaker(Speaker.Lupe)
        -> DONE
        
* {IsQuestComplete(visited_josh) && IsQuestComplete(visited_calvin) && IsQuestComplete(visited_jenny)} [Say I wanted to learn more about the Winery...anyone been talking about it lately?]
    -> DONE

=== teens_suspect ====
* [Jenny] -> DONE

* [Calvin] -> DONE

* [Josh] -> DONE

* {IsClueFound(jenny_suspects) && IsClueFound(josh_suspects) && IsClueFound(calvin_suspects)} [So. Anything else you wanna get off your chests?]
    ~ SetSpeaker(Speaker.Jenny)
    [Jenny] Unless the Rookie Sheriff wants to conceit their HO:SI score, no.
    [Jenny] We're done talking with you losers.
    ~ SetSpeaker(Speaker.Lupe)
    ~ CompleteQuest(visited_arcade)
        -> DONE
        

=== scene4_4 ===
// FUNC SCENE CHANGE
# Location: The Rockin Kettle, Bar 
# Only accessible after Gen Store and Arcade have been visited

+ [Irene]
    ~ current_npc = "[Irene]"
    {IsQuestComplete(irene_intro):
        -> DONE
    - else:
    ~ SetSpeaker(Speaker.Irene)
    {current_npc} Is that Ritam Misra I spy?
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Hi, Irene!
    ~ SetSpeaker(Speaker.Irene)
    {current_npc} Damn, it's been a sec! 
    {current_npc} How're you?
    {current_npc} Who's your broody looking pal?
    ~ SetSpeaker(Speaker.Lupe)
      -> DONE
    
}
+ {IsQuestComplete(irene_intro)} [memorial_plaque] -> DONE
     
    + [slumped_man] 
        {IsQuestComplete(jenkins_wakes_up):
            ~ current_npc = "[Jenkins]"
            ~ SetSpeaker(Speaker.Jenkins)
            {current_npc} Eh? ARGH. Whaddya want Irene?
            {current_npc} S'closing time already?
            ~ SetSpeaker(Speaker.Lupe)
                -> DONE
        - else:
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe]{Man, how much do you have to drink to be knocked out that bad? | That's gonna hurt when he comes to. | KO'ed.}
        -> DONE
    }
        

    
    * {irene_intro}[goop] -> DONE


= goop
~ current_npc = "[Irene]"
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] You've been getting this stuff too, huh?
    ~ SetSpeaker(Speaker.Irene)
    {current_npc} Oh, yeah.
    {current_npc} No idea what it is.
    {current_npc} Maybe like some kinda fungus?
    {current_npc} Or like, mold?
    {current_npc} It's been coming up out of the drains like crazy.
    ~ SetSpeaker(Speaker.Misra)
    [Misra] You don't sound very concerned.
    ~ SetSpeaker(Speaker.Irene)
    {current_npc} Oh, nah.
    {current_npc} It's annyoing, but it comes off easy enough.
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] It does?
    ~ SetSpeaker(Speaker.Irene)
    {current_npc} Sure. 
    {current_npc} I just dump all the bleach I've got in the backroom on it and poof!
    {current_npc} Melts away.
    // Add to Synthesis - The Town of KR
    -> DONE

= memorial_plaque
# Lupe leans close to a memorial plaque on the wall. It reads "In Memory of those lost in the Tragedy of 1940. Rest in Peace, Never Forgotten." Over the corner of it, someone has sharpied over it with "THOSE STUPID GOATS!"
~ CompleteQuest(memorial_plaque_visited)
    // Add to Synthesis - The Town of KR
-> DONE


