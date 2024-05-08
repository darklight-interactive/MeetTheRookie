VAR current_npc = ""

// ====== SPEAKER ======
LIST Speaker = Misra, Lupe, Chief_Thelton, Employee, Mel, Random_Dude
VAR curSpeaker = Speaker.Lupe
== function set_speaker(value)
    ~ curSpeaker = value

// ====== QUEST CHAIN =======
LIST QuestChain = first_interact, paid_for_gas, fixed_goop_for_mel, first_interact_mel
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
LIST Clues = knows_cash_is_broken, knows_how_to_fix_cash_from_mel, tree_fell
VAR knowledgeState = ()
=== function DiscoverClue(clue)
    ~ knowledgeState += clue
    
=== function IsClueFound(clue)
    { LIST_ALL(Clues) ? clue:
        // clues are either found, or not
        ~ return knowledgeState ? clue
    }

// STARTING OBJECTIVE OF LEVEL: Pay for Gas
//  ENDING OBJECTIVE: REPORT ROAD BLOCK AT KETTLE ROCK PRECINCT

// Quest is marked where it should begin, and when it ends

=== intro ===
# name : Chief DI Thelton
# location : unknown
~ set_speaker(Speaker.Chief_Thelton)
You've reached Chief Detective Inspector Thelton, Boise Precinct. I'm not available right now. You know what to do.
// We hear a generic voicemail beep.

* [CONTINUE] 
# name : Lupe
~ set_speaker(Speaker.Lupe)
Hey...it's Lupe. Had to change my route; tank was running low. I pit stopped outside of Kettle Rock, Idaho. Hoping to get back on the road soon...sorry, I'm a bit scatterbrained right now. Don't jump on me for being late. I'll debrief the Watchowski Case with you when I get back. Alright. Bye.
    -> scene6_1

=== scene6_1 ===
// FUNC_SCENE_CHANGE
// PLAYER_PROMPT -> highlight gas pump

{first_interact:
    *[car] -> DONE
    *[outside_npc] -> DONE
    *[enter_store] -> DONE
}

*[gas_pump] 
    -> DONE
    
*{tree_fell}[fallen_tree] -> DONE
*{fallen_tree}[car] -> DONE
*{fallen_tree}[gas_pump] -> DONE

= fallen_tree
#Lupe sees a tree has fallen and blocked the way out of town.
* Huh...that's...weird... -> DONE

= gas_pump
~ CompleteQuest(first_interact)
{IsQuestComplete(paid_for_gas): 
    God, I feel weird. The Deja Vu is killing me right now. Alright. Let's get this over with. I hope it doesn't take too long.  -> DONE //{goto("precinct")} 
- else:
        {stopping:
            -   "Out of order. Pay inside." Of course. Just my luck. 
                # OBJECTIVE: PAY FOR GAS
                -> DONE
            - Sooner I pay, sooner I can get back on the road, sooner Thelton won't bite my head off.
                -> DONE
        } 
}

= car
{IsClueFound(tree_fell): 
    God, I feel weird. The Deja Vu is killing me right now. Alright. Let's get this over with. I hope it doesn't take too long. -> DONE //{goto("precinct")} 
- else: 
    Still gotta pay. -> DONE
}

= npc
~ current_npc = "[Random Dude]"
{tree_fell: 
    ~ set_speaker(Speaker.Random_Dude)
    {current_npc} Timber. Heh heh. Hope you didn't need to be anywhere anytime soon.
    +  ...yeah....should I head down to the precinct to file a complaint?
    ~ set_speaker(Speaker.Random_Dude)
    {current_npc} You read my mind! Eheheh.
    # OBJECTIVE: REPORT ROAD BLOCK AT KETTLE ROCK PRECINCT
        -> DONE
- else :
    ~ set_speaker(Speaker.Random_Dude)
    You're stuck at the pump too, eh? Good luck getting that lazybones to help you. Heh. Youth these days, am I right? -> DONE
}


=== scene6_2 ===
// FUNC_SCENE_CHANGE
# location : MelOMart Store
-> DONE
= main
~ current_npc = "[employee]"

* [talk_to_employee] -> DONE
    
* {employee3}[talk_to_employee]
~ current_npc = "[employee]"
    {IsClueFound(knows_how_to_fix_cash_from_mel):
         #employee
         {current_npc} I told you--
             #lupe 
            * Mel said to hit it. In the side. -> DONE

        - else:
            ~ set_speaker(Speaker.Employee)
            {current_npc} Answer's the same as two seconds ago:
            {current_npc} Register drawer is busted, okay?
            ~ set_speaker(Speaker.Lupe)
            [Lupe] Right...
            [Lupe] Can you try...hitting the register?
            ~ set_speaker(Speaker.Employee)
            {current_npc} Uh, what? 
            {current_npc} You want me to hit it
            ~ set_speaker(Speaker.Lupe)
            [Lupe] Yes. 
            [Lupe] It's just like, intuition.
            ~ set_speaker(Speaker.Employee)
            {current_npc} Uh, okay.
            -> DONE
}

+ {tree_fell}[talk_to_employee]
    {current_npc} {You're good to go. | Thanks for paying. | And for fixing it, I guess. Bye-Bye.} -> main
    
+ [door_to_backroom] 
    {knows_cash_is_broken:
        ->backroom
        
        - else:
        [Lupe] I probably should pay.
        -> main
      }      
            
+ {tree_fell}[door_to_outside] -> scene6_1

= employee
# name : employee
~ current_npc = "[employee]"
* Hey man. How're you?
    {current_npc} I work at a gas station. How do you think I am?
    ->DONE
* ...the pump's broken. I pay here, right?
    {current_npc} Yeahhhhh. You can't pay right now. The register drawer is busted. 
    -> DONE


= employee2
* Can you, uh, fix it?
    {current_npc} That's kinda above my paygrade? ->employee3
    
=employee3
* ...do you have a manager around?
    {current_npc} He's in the back.
    ~ knows_cash_is_broken = true
    -> main 
    
=pay_for_gas
#the employee whacks the side of the register.The Drawer pops open.
{knows_how_to_fix_cash_from_mel:
    {current_npc} Cool. That'll be $76.45.
    ~ paid_for_gas = true
    # OBJECTIVE COMPLETE
    [Lupe] Finally.
    #Lupe pays. Suddenly, a loud crashing noise is heard from outside.
    #Lupe
    * What was that? 
    ~tree_fell = true
    -> main

- else: 
    {current_npc} Wow. 
    {current_npc} Are you like, a psychic or somethin?
    [Lupe] Call it luck, I guess...
    {current_npc} That'll be $76.45.
    ~ paid_for_gas = true
    [Lupe] Finally.
    #Lupe pays. Suddenly, a loud crashing noise is heard from outside.
    * What was that? 
    ~tree_fell = true
    -> scene6_2
   
}  
   
= backroom
 ~current_npc = "[Mel]"
   #Lupe pushes through the door to the backroom of the gas station. The room is a small breack room. The presumed manager stands at the sink, scrubbing goop. There is MORE goop than the night before.
   
 * [talk_to_mel] 
        [Lupe] You're Mel, right? 
        [Lupe] The manager?
             -> mel 
 
 + [door_to_mainroom] -> main   
 
 * {first_interact_mel}[talk_to_mel]
        -> mel 
 
 * {fixed_goop_for_mel} [talk_to_mel]
            {current_npc} Thanks again for your help.
            {current_npc} Don't know how you knew that would work.
            -> backroom
 
= goop 
   {knows_how_to_fix_cash_from_mel:
    {current_npc} No idea--was here when I unlocked this morning. Darn stuff won't scrub off. 
    * That's odd...
        -> backroom
    *Have you tried bleach?
    {current_npc} Bleach?
    [Lupe] Yeah. To scrub it off.
    # Mel switches to a spray bottle of bleach, and uses it. The goop disappears.
    {current_npc} Would you look at that.
    {current_npc} Thank you, whoever you are. 
    [Lupe] You're welcome...
    ~ fixed_goop_for_mel = true
        -> backroom
    
   -else:
     {current_npc} No idea--was here when I unlocked this morning. Darn stuff won't scrub off. Can I help you with something? 
   * Erm, no, sorry. -> backroom
   * Have you tried bleach?
    {current_npc} Bleach?
    [Lupe] Yeah. To scrub it off.
    # Mel switches to a spray bottle of bleach, and uses it. The goop disappears.
    {current_npc} Would you look at that.
    {current_npc} Thank you, whoever you are. 
    ~ fixed_goop_for_mel = true
    [Lupe] You're welcome...
   
   -> backroom
 }
   
= mel
    {first_interact_mel:
        * Uh, what is that stuff? -> goop
    
    - else:
    
    {current_npc} The one and only. Who are you?
    [Lupe] Just someone passing through.
    {paid_for_gas:
         ~ first_interact_mel = true
        -> backroom
    - else:
        [Lupe] I just need to pay for gas. 
        [Lupe] The register is broken. 
         ~ first_interact_mel = true
            -> is_broken
}            
}

= is_broken
 {current_npc} That old piece of junk. The drawer is jut jammed, it--
 [Lupe] --just needs to be whacked in the side a bit?
 {current_npc} Well, aren't you sharp?
 {current_npc} Yeah, try that.
 ~knows_how_to_fix_cash_from_mel = true
    #Lupe
    *Thanks.
        -> backroom
 

  
