// LISTS are Enums that can toggle
// VARS are one or more data values that store a single type of data

// ====== SPEAKER ======
LIST Speaker = Unkown, Lupe, Misra, Employee, Mel, Chief_Thelton
VAR curSpeaker = Speaker.Lupe
== function set_speaker(value)
    ~ curSpeaker = value

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

// == ( SCENE 1 ) ================================= >>

=== scene1_0 ===
~ set_speaker(Speaker.Chief_Thelton)
You've reached Chief Detective Inspector Thelton, Boise Precinct. 
I'm not available right now. You know what to do.
// We hear a generic voicemail beep.

~ set_speaker(Speaker.Lupe)
Hey, it's Lupe. Had to change my route; tank was running low. I pit stopped outside of Kettle Rock, Idaho. Should be back on the road soon--don't jump on me for being late. I'll debrief the Watchowski Case with you when I get back. Alright. Bye.
    -> scene1_1

// ------------- SCENE1.1 Outside MelOMarket
=== scene1_1 ===
// FUNC_SCENE_CHANGE
// PLAYER_PROMPT -> highlight gas pump
~ StartQuest(FIRST_INTERACT)
-> DONE

= fallen_tree
//Lupe sees a tree has fallen and blocked the way out of town.
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
{IsQuestComplete(PAY_FOR_GAS): 
    "Sorry I was so late to the debrief boss, I had to go report a suspicious fallen tree." Ugh. Guess I'll be more than a little late...Thelton's gonna kill me. -> DONE //{goto("precinct")} 
- else: 
    Still gotta pay. -> DONE
}
= npc
{IsQuestComplete(PAY_FOR_GAS):
    ~ set_speaker(Speaker.Unkown)
    Timber. Heh heh. Hope you didn't need to be anywhere anytime soon.
    ~ set_speaker(Speaker.Lupe)
    ...great. Any way I can get this fixed?
    ~set_speaker(Speaker.Unkown)
    Got a chainsaw on ya? 
    Heheheheh. 
    Ah, don't look so grumpy. 
    If you put in a complaint with the local Police I'm sure you'll be out of here in no time.-> DONE
- else :
    You're stuck at the pump too, eh? Good luck getting that lazybones to help you. Heh. Youth these days, am I right? -> DONE
}

= enter_store
I guess I'll find someone to help me inside here.
-> DONE

// ------------- SCENE1.2 MelOMarket Store

=== scene1_2 ===
//The gas station door chimes with a slightly out of tune jingle.
//Lupe hears the employee mutter a very unattentive "Welcome to MelOMart".
//-> main
//= main
//*[talk_to_employee] 
//    Lupe approaches the employee. They are pretending to sweep but really they are watching videos on their phone.
//  -> employee

= vending_machine
    {Sugar-Flavored Snack Bites. Pickle Chips. Jerkied Ox Sticks...I think I'll pass.| Is this stuff FDA approved? | I though Pop Pops Gum went out of production in the 90s? Weird.}
    -> DONE

= employee
{IsClueFound(CASHREG_BROKEN):
    {IsClueFound(CASHREG_FIX):
        ~set_speaker(Speaker.Employee)
        I told you--
        ~set_speaker(Speaker.Lupe)
        Mel said to hit it. In the side.
        // the employee whacks the side of the register.The Drawer pops open.
        ~ set_speaker(Speaker.Employee) 
        Cool. $76.45.
        ~ set_speaker(Speaker.Lupe)
        Finally
        ~ CompleteQuest(PAY_FOR_GAS)
        -> scene1_3
    - else:
        ~ set_speaker(Speaker.Employee)
        {cycle:
            - The register drawer is busted.
            - Register is still busted.
            - Stillllllll busted, buddy.
        }
    }
-else:
    // -- DISCOVER CASHREG_BROKEN
    ~ set_speaker(Speaker.Lupe)
    Hey man. How're you?
    ~ set_speaker(Speaker.Employee)
    I work at a gas station. How do you think I am?
    ~ set_speaker(Speaker.Lupe)
    ...the pump's broken. It says to pay inside?
    ~ set_speaker(Speaker.Employee)
    Yeahhhhh. You can't pay right now. The register drawer is busted
    ~ set_speaker(Speaker.Lupe)
    Can you, uh, fix it?
    ~ set_speaker(Speaker.Employee)
    That's kinda above my paygrade?
    ~ set_speaker(Speaker.Lupe)
    Whose paygrade is it <i>not</i> above?
    ~ set_speaker(Speaker.Employee)
    My manager's in the back.
    ~ DiscoverClue(CASHREG_BROKEN)
}

= door_to_backroom
{IsClueFound(CASHREG_BROKEN):
    -> scene1_3
    - else:
        ~ set_speaker(Speaker.Lupe)
        I've got no reason to poke around. -> DONE
}

= door_to_outside
{IsQuestComplete(PAY_FOR_GAS):
    -> DONE
    -else:
        ~ set_speaker(Speaker.Lupe)
        Still gotta pay. -> DONE
}

// ------------- SCENE1.3 Breakroom ---- >>
=== scene1_3 ===
// Lupe pushes through the door to the backroom of the gas station. The room is a small break room. The presumed manager stands at the sink, scrubbing goop.

= mel
{IsClueFound(CASHREG_FIX) == false:
    ~ set_speaker(Speaker.Lupe)
         Hey. Are you the manager?
    ~ set_speaker(Speaker.Mel)
    Mel's the name. What are you doing back here?
    ~ set_speaker(Speaker.Lupe)
    I need to pay for gas. The cash register is broken.
    ~ set_speaker(Speaker.Mel)
     That old piece of junk. 
    The drawer is just jammed, it just needs to be whacked in the side a bit
    ~ set_speaker(Speaker.Lupe)
    Thanks.
    ~ DiscoverClue(CASHREG_FIX)
    -> DONE
-else:
    ~set_speaker(Speaker.Mel)
    {cycle:
        - Look, I'd like to help more, but I gotta get this stuff out. 
        - This is <i>definitely</i> a code violation.
        - It almost looks like mold? Or jello. Gross, ain't it?
        - I love my job. I love my job. Ugh.
    }

}

= goop 
    ~ set_speaker(Speaker.Lupe)
        Uh. What is that?
    ~ set_speaker(Speaker.Mel)
        No idea, it was here when I unlocked this morning. 
        Darn stuff won't scrub off. ->DONE

// ------------- SCENE1.4 Tree Falls
=== scene1_4 ===
    #Lupe pays. Suddenly, a loud crashing noise is heard from outside.
    ~ set_speaker(Speaker.Lupe)
    What was that? -> DONE
  