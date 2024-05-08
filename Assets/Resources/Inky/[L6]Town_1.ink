INCLUDE Roy Rodgerson.ink
INCLUDE Calvin .ink
INCLUDE Jenny.ink
INCLUDE Josh.ink
INCLUDE Jenkins.ink
INCLUDE Irene.ink

// applicable everywhere
VAR current_npc = ""


//Variables for Main St
VAR closed_signs = 0

// Quest Objective of Scene: Who broke into the Old Winery?
// Current Synthesis Pages: Who broke into the Old Winery?
// AND The Town of Kettle Rock (for general town information)

// ====== SPEAKER ======
LIST Speaker = Misra, Lupe, Roy_Rodgerson, Jenny, Calvin, Josh, Irene, Jenkins
VAR curSpeaker = Speaker.Lupe
== function set_speaker(value)
    ~ curSpeaker = value

// ====== QUEST CHAIN =======
LIST QuestChain = visited_goop, visited_symbol, visited_misra, visited_gen_store, interact_first_store, visited_arcade, interact_first_arcade, visited_machines, visited_jenny, visited_calvin, visited_josh, irene_intro, memorial_plaque_visited, irene_convo_1, irene_convo_2, jenkins_wakes_up
VAR activeQuests = ()
VAR completedQuests = ()
=== function StartQuest(quest)  
    ~ activeQuests += quest
=== function CompleteQuest(quest)
    ~ activeQuests -= quest
    ~ completedQuests += quest
=== function IsQuestComplete(quest)
    ~ return completedQuests ? quest
    
// ====== CLUES =======
LIST Clues = merch_pamphlet, roys_suspicion, roy_personal_info, roy_winery_closing, golden_age, tragedy, rocky_years, roy_town_history, HOSI_mentioned, jenny_crazies, HOSI_calvin, jenny_suspects, josh_suspects, calvin_suspects, goats_mentioned, sacrifice_mentioned, KR_irene, closed_shops_irene, jenkins_winery, sarah_mentioned, council_mentioned, sacrifice_mentioned_jenkins, symbol_evidence, goop_evidence
VAR knowledgeState = ()
=== function DiscoverClue(clue)
    ~ knowledgeState += clue
    
=== function IsClueFound(clue)
    { LIST_ALL(Clues) ? clue:
        // clues are either found, or not
        ~ return knowledgeState ? clue
    }


=== scene4_1 === 
# Location: Main Street
# Lupe and Misra find themselves on the Main Street of Kettle Rock, in front of a Fountain. 
# The General Store and Arcade are accessible.
# The Bar is not.
# Any store marked with a closed sign is not accessible.

* [Misra]
    ~ CompleteQuest(visited_misra)
    ~ set_speaker(Speaker.Misra)
    [Misra] Here we are! 
    [Misra] Kettle Rock, Main Street. Heart of the Downtown. 
    [Misra] There's bound to be some locals around - where do you want to start?
    ~ set_speaker(Speaker.Lupe)
    -> DONE
    
 *{IsQuestComplete(visited_arcade)}[Misra]
    ~ CompleteQuest(visited_misra)
    ~ set_speaker(Speaker.Misra)
     [Misra] Those guys are the worst.
    ~ set_speaker(Speaker.Lupe)
     [Lupe] They're definitely hiding <i>something</i>.
    ~ set_speaker(Speaker.Misra)
     [Misra] Good luck getting anything out of them.
    ~ set_speaker(Speaker.Lupe)
     // Add to Synthesis: Who broke into the Winery?
    -> DONE
    
* {IsQuestComplete(visited_gen_store) && IsQuestComplete(visited_arcade)}[the_rockin_kettle]
        ~ set_speaker(Speaker.Misra)
        [Misra] Looks like the Bar is open! Shall we?
        ~ set_speaker(Speaker.Lupe)
        [Lupe] I shouldn't drink.
        [Lupe] I still need to drive back out tonight once the road is cleared.
        ~ set_speaker(Speaker.Misra)
        [Misra] Right, about that...
        [Misra] I forgot to tell you, but the earliest I can get someone out there to clear the tree is tomorrow morning.
        [Misra] I'm really sorry.
        [Misra] I know you had somewhere to be.
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Ah...
        [Lupe] Well, that's not great.
        [Lupe] But I suppose there's nothing you can do about it.
        ~ set_speaker(Speaker.Misra)
        [Misra] Think of it this way...we get more time to crack this case!
        [Misra] But in the meantime, let's take a bit of a break...
        -> DONE
    
 
 
 
 
= main_street

{IsQuestComplete(visited_misra):

+ [strange_symbol_on_fountain]
    {IsQuestComplete(visited_symbol):
    ~ set_speaker(Speaker.Misra)
    [Misra]{It's probably just some historical symbol. | I don't think it's anything important. | We should move on. | Where to next?}
    ~ set_speaker(Speaker.Lupe)
    // Add to Synthesis - The Town of KR
    -> DONE
    - else:
    [Lupe] What's this?
    ~ set_speaker(Speaker.Misra)
    [Misra] Hm. Looks like some sort of silly engravement.
    ~ set_speaker(Speaker.Lupe)
    ~  DiscoverClue(symbol_evidence)
        ~ CompleteQuest(visited_symbol)
    # ADD TO SYNTHESIS
    -> DONE
}

+ [goop_on_fountain]
    {IsQuestComplete(visited_goop):
        [Lupe] {So weird...|It looks...squishy?| It smells kinda acidic. | Weird.}
        -> DONE
    - else: 
        [Lupe] More of this stuff.
        ~ set_speaker(Speaker.Misra)
        [Misra] You've seen it before?
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Yeah. 
        [Lupe] It was at the Gas Station near the edge of town. 
     ~ CompleteQuest(visited_goop)
     ~ DiscoverClue(goop_evidence)
    // Add to Synthesis - The Town of KR

     -> DONE
}

+ [idahome_and_goods] -> scene4_2

+ [power_up_arcade] -> scene4_3
   
+ [the_rockin_kettle]
    {IsQuestComplete(visited_gen_store) && IsQuestComplete(visited_arcade):
        ~ set_speaker(Speaker.Misra)
        [Misra] Looks like the Bar is open! Shall we?
        ~ set_speaker(Speaker.Lupe)
        [Lupe] I shouldn't drink.
        [Lupe] I still need to drive back out tonight once the road is cleared.
        ~ set_speaker(Speaker.Misra)
        [Misra] Right, about that...
        [Misra] I forgot to tell you, but the earliest I can get someone out there to clear the tree is tomorrow morning.
        [Misra] I'm really sorry.
        [Misra] I know you had somewhere to be.
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Ah...
        [Lupe] Well, that's not great.
        [Lupe] But I suppose there's nothing you can do about it.
        ~ set_speaker(Speaker.Misra)
        [Misra] Think of it this way...we get more time to crack this case!
        [Misra] But in the meantime, let's take a bit of a break...
        ~ set_speaker(Speaker.Lupe)
        -> scene4_4
        
    - else:
        ~ set_speaker(Speaker.Misra)
        [Misra] Someone wants a drink, I see.
        [Misra] The Rockin Kettle doesn't open until happy hour! 
        [Misra] We can come back later.
        ~ set_speaker(Speaker.Lupe)
    -> DONE
    
  
}

 *{IsQuestComplete(visited_gen_store)}[Misra]
    ~ CompleteQuest(visited_misra)
    ~ set_speaker(Speaker.Misra)
    [Misra] I'm sorry if Roy seems like a bit of a downer.
    [Misra] He has no faith.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] He seems like he's got a pretty good acceptance of the situation.
    [Lupe] From what I can tell.
    ~ set_speaker(Speaker.Misra)
    [Misra] Well, you've only been here a day...
    ~ set_speaker(Speaker.Lupe)
    -> DONE
    
* [laundromat_closed_sign]
    ~ closed_signs ++ 
    ~ set_speaker(Speaker.Misra)
    [Misra] The good ol' 24/7 Laundromat!
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Doesn't seem very 24/7 to me.
        -> DONE


* [einab_closed_sign]
    ~ closed_signs ++ 
    [Lupe] A lot of things seem to be closed...
    -> DONE


* [apartments_for_lease_sign]
    ~ closed_signs ++ 
    [Lupe] Hm..
        -> DONE
    

* [diner_closed_sign]
    ~ closed_signs ++ 
    [Lupe] Jeez, what <i>is</i> open?
    -> DONE


* [clothing_store_closed_sign]
   ~ closed_signs ++ 
   [Lupe] Another closed business.
   ~ set_speaker(Speaker.Misra)
   [Misra] Things have been rough lately.
   ~ set_speaker(Speaker.Lupe)
        -> DONE
        
 * {closed_signs >= 5} ["The Heart of Kettle Rock" seems a bit...barren.]
        ~ set_speaker(Speaker.Misra)
        [Misra] It has slowed down.
        [Misra] A bit.
        [Misra] Usually it's busy and full of life,
        [Misra] but things have been bumpy for a lot of locals lately,
        [Misra] Not enough profit to keep them going.
        [Misra] Breaks my heart a bit.
        ~ set_speaker(Speaker.Lupe)
        // Add to Synthesis - The Town of KR
        -> DONE
}


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

* {IsQuestComplete(visited_gen_store)} [Roy_Rodgerson]
    ~ set_speaker(Speaker.Roy_Rodgerson)
    [Roy_Rodgerson] Let me know if you need anything from me!
        {CompleteQuest(visited_arcade):
            -> DONE
        - else:
        -> scene4_2
    }
    
 + {questions}[front_door]
     ~ CompleteQuest(visited_gen_store)
    [Lupe] Thanks again.
    ~ set_speaker(Speaker.Misra)
    [Misra] Bye, Roy!
    {current_npc} Take care!
    // ?
    ~ set_speaker(Speaker.Lupe)
        -> scene4_1.main_street

=== scene4_3 ===
// FUNC SCENE CHANGE
# Location: Power Up Arcade

+ {CompleteQuest(visited_arcade)}[front_door] -> scene4_1.main_street

+ {IsQuestComplete(visited_arcade)}[teens]
     ~ set_speaker(Speaker.Jenny)
     [Jenny] {Look, unless you want to talk about HOSI, we're done. | Get lost.}
     ~ set_speaker(Speaker.Lupe)
        -> DONE


*[Misra]
        ~ set_speaker(Speaker.Misra)
        [Misra] Okay...
        [Misra] Just so you know, these guys are a litte...
        [Misra] ...<i>intense.</i>
        ~ set_speaker(Speaker.Lupe)
        [Lupe] I deal with intense people all the time.
        ~ set_speaker(Speaker.Misra)
        [Misra] Yeah?
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Criminals.
        [Lupe] Convicts.
        [Lupe] Killers.
        [Lupe] I'm not afraid of whoever is in there.
        ~ set_speaker(Speaker.Misra)
        [Misra] If you say so...
        ~ set_speaker(Speaker.Lupe)
        ~ CompleteQuest(interact_first_arcade)
        -> DONE
= arcade
{IsQuestComplete(interact_first_arcade):

    + [arcade_machines]
        {IsQuestComplete(visited_machines):
            [Lupe]{"Mac Pan"... huh | "Donkey King"...that's...okay.. | "GalaxyBattles!" | "Sidewalk Fighter" | So weird...}
        - else:
        [Lupe] I don't recognize any of these games.
        ~ set_speaker(Speaker.Misra)
        [Misra] Yeah...
        [Misra] They're all knock offs of knock offs.
        [Misra] They're cheaper that way.
        [Misra] And twice the fun!
        ~ set_speaker(Speaker.Lupe)
        -> DONE
        }
    
    + [teens] 
        {IsQuestComplete(visited_arcade):
            ~ set_speaker(Speaker.Jenny)
            [Jenny] {Look, unless you want to talk about HOSI, we're done. | Get lost.}
            ~ set_speaker(Speaker.Lupe)
            -> DONE
        - else:
        -> DONE
    }
    
        
}

= teens
[Lupe] Is that...
[Lupe] Who you were talking about...?
~ set_speaker(Speaker.Misra)
[Misra] Yes.
[Misra] Don't be fooled.
[Misra] They're vicious.
~ set_speaker(Speaker.Lupe)
[Lupe] They're....
[Lupe] ...fourteen year olds.
~ set_speaker(Speaker.Misra)
[Misra] I know.
~ set_speaker(Speaker.Lupe)
[Lupe] They're barely five feet tall.
~ set_speaker(Speaker.Misra)
[Misra] <i> I know </i>.
~ set_speaker(Speaker.Lupe)
[Lupe] They--
~ set_speaker(Speaker.Jenny)
[Jenny] Hey Nimrods. 
[Jenny] We can hear you.
~ set_speaker(Speaker.Lupe)
    -> DONE
    
    
=== teens_roulette ===

* [Jenny] -> DONE

* [Calvin] -> DONE

* [Josh] -> DONE

*{HOSI_mentioned} [What's HOSI?]
    ~ set_speaker(Speaker.Jenny)
    [Jenny] Hamster Origins: Space Invaders.
    ~ set_speaker(Speaker.Misra)
    [Misra] It's the game they're playing.
    [Misra] They've been trying to beat it for a while now.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] What does that have to do with anything? 
    ~ set_speaker(Speaker.Misra)
    [Misra] It's the only game in here they <i>don't</i> have the high score on.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Why does that matter?
    ~ set_speaker(Speaker.Misra)
    [Misra] ...
    [Misra] Because <i>I</i> have the high score.
    ~ set_speaker(Speaker.Lupe)
-> teens_roulette

*{jenny_intro} [Jenny]
    [Lupe] Well, <i>I'm</i> not a cop.
    [Lupe] And I'm not the one blocking you from your high score.
    ~ set_speaker(Speaker.Jenny)
    [Jenny] ...
    [Jenny] What do you want, then?
    ~ set_speaker(Speaker.Lupe)
    [Lupe] I'm...just passing through. 
    [Lupe] I wanted to learn a little more about the town.
    ~ set_speaker(Speaker.Jenny)
    [Jenny] I can tell when someone's lying, you know.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Am I lying?
    ~ set_speaker(Speaker.Jenny)
    [Jenny] ...
    ~ set_speaker(Speaker.Lupe)
        -> DONE

*{calvin_intro} [Calvin]
    [Lupe] Can you, uh, speak a little slower?
    ~ set_speaker(Speaker.Calvin)
    [Calvin] I'mnotsupposedtobetalkingtoyouatall.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Dude, you're not in trouble.
    ~ set_speaker(Speaker.Calvin)
    [Calvin] ...
    ~ set_speaker(Speaker.Lupe)
        -> DONE
    
* {josh_intro} [Josh]
    [Lupe] Hey man, can you turn that down a bit?
    ~ set_speaker(Speaker.Josh)
    [Josh] YOU DON'T LIKE THE BEAT?
    ~ set_speaker(Speaker.Lupe)
    [Lupe] It's just, really loud
    ~ set_speaker(Speaker.Josh)
    [Josh] THAT SOUNDS LIKE A YOU PROBLEM.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] ...
    [Lupe] Okay.
    [Lupe] You must not have anything important to say, then.
    ~ set_speaker(Speaker.Josh)
    [Josh] WHAT?
    ~ set_speaker(Speaker.Lupe)
    [Lupe] I can just talk to your friends, then.
    [Lupe] You probably don't matter as much, anyway.
    ~ set_speaker(Speaker.Josh)
    [Josh] HEY.
    [Josh] I MATTER.
    # Josh turns down the music a bit.
    [Josh] What do you wanna know?
    ~ set_speaker(Speaker.Lupe)
        -> DONE
        
* {IsQuestComplete(visited_josh) && IsQuestComplete(visited_calvin) && IsQuestComplete(visited_jenny)} [Say I wanted to learn more about the Winery...anyone been talking about it lately?]
    -> DONE

=== teens_suspect ====
* [Jenny] -> DONE

* [Calvin] -> DONE

* [Josh] -> DONE

* {IsClueFound(jenny_suspects) && IsClueFound(josh_suspects) && IsClueFound(calvin_suspects)} [So. Anything else you wanna get off your chests?]
    ~ set_speaker(Speaker.Jenny)
    [Jenny] Unless the Rookie Sheriff wants to conceit their HO:SI score, no.
    [Jenny] We're done talking with you losers.
    ~ set_speaker(Speaker.Lupe)
    ~ CompleteQuest(visited_arcade)
        -> DONE
        

=== scene4_4 ===
// FUNC SCENE CHANGE
# Location: The Rockin Kettle, Bar 
# Only accessible after Gen Store and Arcade have been visited

+ [Irene]
    ~ current_npc = "[Irene]"
    {IsQuestComplete(Irene_intro):
        -> DONE
    - else:
    ~ set_speaker(Speaker.Irene)
    {current_npc} Is that Ritam Misra I spy?
    ~ set_speaker(Speaker.Misra)
    [Misra] Hi, Irene!
    ~ set_speaker(Speaker.Irene)
    {current_npc} Damn, it's been a sec! 
    {current_npc} How're you?
    {current_npc} Who's your broody looking pal?
    ~ set_speaker(Speaker.Lupe)
      -> DONE
    
}
+ {IsQuestComplete(irene_intro)} [memorial_plaque] -> DONE
     
    + [slumped_man] 
        {IsQuestComplete(jenkins_wakes_up):
            ~ current_npc = "[Jenkins]"
            ~ set_speaker(Speaker.Jenkins)
            {current_npc} Eh? ARGH. Whaddya want Irene?
            {current_npc} S'closing time already?
            ~ set_speaker(Speaker.Lupe)
                -> DONE
        - else:
        ~ set_speaker(Speaker.Lupe)
        [Lupe]{Man, how much do you have to drink to be knocked out that bad? | That's gonna hurt when he comes to. | KO'ed.}
        -> DONE
    }
        

    
    * {irene_intro}[goop] -> DONE


= goop
~ current_npc = "[Irene]"
    ~ set_speaker(Speaker.Lupe)
    [Lupe] You've been getting this stuff too, huh?
    ~ set_speaker(Speaker.Irene)
    {current_npc} Oh, yeah.
    {current_npc} No idea what it is.
    {current_npc} Maybe like some kinda fungus?
    {current_npc} Or like, mold?
    {current_npc} It's been coming up out of the drains like crazy.
    ~ set_speaker(Speaker.Misra)
    [Misra] You don't sound very concerned.
    ~ set_speaker(Speaker.Irene)
    {current_npc} Oh, nah.
    {current_npc} It's annyoing, but it comes off easy enough.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] It does?
    ~ set_speaker(Speaker.Irene)
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


