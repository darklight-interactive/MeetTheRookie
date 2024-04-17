VAR current_npc = ""

// LISTS are Enums that can toggle
// VARS are one or more data values that store a single type of data

// ====== QUEST CHAIN =======
LIST QuestChain = FIRST_INTERACT, PAY_FOR_GAS
VAR activeQuests = (FIRST_INTERACT)
VAR completedQuests = ()
=== function StartQuest(quest)
    ~ activeQuests += quest
=== function CompleteQuest(quest)
    ~ activeQuests -= quest
    ~ completedQuests += quest
=== function IsQuestComplete(quest)
    ~ return completedQuests ? quest
    
// ====== CLUES =======
LIST Clues = (GAS_PUMP_BROKEN), (CASHREG_BROKEN), (CASHREG_FIX)
VAR knowledgeState = ()
=== function DiscoverClue(clue)
    ~ knowledgeState += clue
    
=== function IsClueFound(clue)
    { LIST_ALL(Clues) ? clue:
        // clues are either found, or not
        ~ return knowledgeState ? clue
    }

// ===== SCENE HANDLING =====
VAR currentScene = -> scene1_1

=== GoToSubScene(int)
    {int:
        - 1: -> scene1_1
        - 2: -> scene1_2
    }

// =================================== ( SCENE 1 ) ================================================================ >>
-> GoToSubScene(1)


=== intro ===
# name : Chief DI Thelton
# location : unknown

You've reached Chief Detective Inspector Thelton, Boise Precinct. I'm not available right now. You know what to do.
// We hear a generic voicemail beep.

* [CONTINUE] 
# name : Lupe
Hey, it's Lupe. Had to change my route; tank was running low. I pit stopped outside of Kettle Rock, Idaho. Should be back on the road soon--don't jump on me for being late. I'll debrief the Watchowski Case with you when I get back. Alright. Bye.
    -> scene1_1

=== scene1_1 ===
// FUNC_SCENE_CHANGE
// PLAYER_PROMPT -> highlight gas pump
~ StartQuest(FIRST_INTERACT)
-> DONE

= fallen_tree
#Lupe sees a tree has fallen and blocked the way out of town.
* What the hell... -> npc

= gas_pump
~ CompleteQuest(FIRST_INTERACT)
~ DiscoverClue(GAS_PUMP_BROKEN)
{IsQuestComplete(PAY_FOR_GAS):
    "Sorry I was so late to the debrief boss, I had to go report a suspicious fallen tree." 
    Ugh. Guess I'll be more than a little late...
    Thelton's gonna kill me. Gah. Let's blow this popsicle stand.  -> DONE 
- else:
        {stopping:
            -   "Out of order. Pay inside." Of course. Just my luck. -> DONE
            - Let's get this over with. Sooner I pay, sooner I can get back on the road, sooner Thelton won't bite my head off. -> DONE
        } 
}

= car
{PAY_FOR_GAS: 
    "Sorry I was so late to the debrief boss, I had to go report a suspicious fallen tree." Ugh. Guess I'll be more than a little late...Thelton's gonna kill me. -> DONE //{goto("precinct")} 
- else: 
    Still gotta pay. -> scene1_1
}

= npc
~ current_npc = "[Random Dude]"
{PAY_FOR_GAS: 
    {current_npc} Timber. Heh heh. Hope you didn't need to be anywhere anytime soon.
    * ...great. Any way I can get this fixed?
    {current_npc} Got a chainsaw on ya? Heheheheh. Ah, don't look so grumpy. If you put in a complaint with the local Police I'm sure you'll be out of here in no time.
        -> scene1_1
- else :
    You're stuck at the pump too, eh? Good luck getting that lazybones to help you. Heh. Youth these days, am I right? -> scene1_1
}


=== scene1_2 ===
// FUNC_SCENE_CHANGE
# location : MelOMart Store
The gas station door chimes with a slightly out of tune jingle.
Lupe hears the employee mutter a very unattentive "Welcome to MelOMart".
-> main
= main
*[talk_to_employee] 
    Lupe approaches the employee. They are pretending to sweep but really they are watching videos on their phone.
    -> employee
+ [vending_machine]
    {Sugar-Flavored Snack Bites. Pickle Chips. Jerkied Ox Sticks...I think I'll pass.| Is this stuff FDA approved? | I though Pop Pops Gum went out of production in the 90s? Weird.}
    -> main
    
+ {employee3}[talk_to_employee]
~ current_npc = "[employee]"
     {CASHREG_FIX:
         #employee
         {current_npc} I told you--
             #lupe 
            *Mel said to hit it. In the side. -> pay_for_gas
    - else:
        {The register drawer is busted. | Register is still busted. | Stillllllll busted, buddy.} -> main
        
}        
     
+ {employee3}[door_to_backroom] 
        ->backroom
    
+ {not employee3}[door_to_backroom]
            I've got no reason to poke around.
            -> main
            
+ [door_to_outside]
        Still gotta pay. ->main

    
   
       
= employee
# name : employee
~ current_npc = "[employee]"
* Hey man. How're you?
    {current_npc} I work at a gas station. How do you think I am?
    ->employee
* ...the pump's broken. It says to pay inside?
    {current_npc} Yeahhhhh. You can't pay right now. The register drawer is busted. 
    -> employee2
    
    
= employee2
*Can you, uh, fix it?
    {current_npc} That's kinda above my paygrade? ->employee3
    
=employee3
*Whose paygrade is it <i>not</i> above?
    {current_npc} My manager's in the back. -> main 
    
    
=pay_for_gas
#the employee whacks the side of the register.The Drawer pops open.
    {current_npc} Cool. $76.45.
    ~ CompleteQuest(PAY_FOR_GAS)
    Finally. #Lupe
    #Lupe pays. Suddenly, a loud crashing noise is heard from outside.
    #Lupe
    * What was that? 
    -> DONE
    //-> scene1_1
    
=backroom
 ~current_npc = "[Mel]"
   #Lupe pushes through the door to the backroom of the gas station. The room is a small breack room. The presumed manager stands at the sink, scrubbing goop.
 +[talk_to_mel] 
    {CASHREG_BROKEN:
    {current_npc}{Look, I'd like to help more, but I gotta get this stuff out. It's <i>definitely</i> a code violation. | It almost looks like mold? Or jello. | Gross, ain't it? | I love my job. I love my job. Ugh.} -> backroom
- else:
     Hey. Are you the manager?
       -> mel
}
   
 *[comment_on_goop] 
    Uh. What is that?
        -> goop
        
  +[door_to_mainroom] -> main     

 
= goop 
   {CASHREG_BROKEN:
    {current_npc} No idea--was here when I unlocked this morning. Darn stuff won't scrub off. -> backroom
   -else:
     {current_npc} No idea--was here when I unlocked this morning. Darn stuff won't scrub off. Can I help you with something?
   *I need to pay for gas. The cash register is broken. -> is_broken
   }
= mel
 {current_npc} Mel's the name. What are you doing back here?
    *I need to pay for gas. The cash register is broken. ->is_broken
   
= is_broken
 {current_npc} 
 That old piece of junk. 
 The drawer is jut jammed, it just needs to be whacked in the side a bit
 ~ DiscoverClue(CASHREG_FIX)
    *Thanks.
        -> backroom
 

  
