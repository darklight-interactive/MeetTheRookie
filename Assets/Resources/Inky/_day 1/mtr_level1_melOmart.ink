// ------------------------------------------------------------------ //
//      MEET THE ROOKIE
//      - Scene 1.0 - 1.5
// ---------------------------------------------- >>/^*
// ====== INCLUDE == >>
VAR gas_pumps = 0

=== level1 ===
* [Scene1_0] -> scene1_0
* [Scene1_1] -> scene1_1
* [Scene1_2] -> scene1_2
* [Scene1_3] -> scene1_3
-> DONE



// == ( SCENE 1 ) ================================= >>
=== scene1_0 ===
    //# hide: Misra
    # emote : Misra \| Voice_call
    # sfx : on
    //TODO SFX phone ringing and ringing,
    //~PlayLoopingSFX("phoneCallingTone")
    ...
    //~StopLoopingSFX("phoneCallingTone")
    ~ SetSpeaker(Speaker.Chief_Thelton)
    You've reached Chief Detective Inspector Thelton, Boise Precinct. 
    I'm not available right now. You know what to do!
    // We hear a generic voicemail beep.
    ~ PlaySFX("phoneVoicemailBeep")
    
    # name: Lupe
    # emote : Lupe \| Serious_2
    ...
    Hey, it's Lupe. Had to change my route; 
    tank was running low. 
    I pit stopped outside of Kettle Rock, Idaho. 
    Should be back on the road soon
    don't jump on me for being late. 
    I'll debrief the Watchowski Case with you when I get back.
    Alright. Bye.
    ~PlaySFX("phoneDialBeep")
    ~ChangeGameScene("scene1_1", 0, 0)
    -> DONE

// ------------- SCENE1.1 Outside MelOMarket
=== scene1_1 ===
// FUNC_SCENE_CHANGE
// PLAYER_PROMPT -> highlight gas pump

* [gas pump] -> gas_pump
* [front door] -> enter_store

= gas_pump
    ~ CompleteQuest(Level1_Quests.first_interact)
    ~ DiscoverMystery(0)
    ~ DiscoverClue(Mystery0.evidence_broken_gas_pump)
    ~ gas_pumps += 1
    {
    - IsQuestComplete(pay_for_gas) && (look_at_tree):
        ~ SetSpeaker(Speaker.Lupe)
        "Sorry I was so late to the debrief boss, I had to go report a suspicious fallen tree." 
        Ugh. Guess I'll be more than a little late...
        Thelton's gonna kill me. Gah. Let's blow this popsicle stand. 
         ~ ChangeGameScene("scene2_DS", 0, 0)
        -> DONE 
        TODO SFX Car closing door noise and leaving
        //~PlaySFX("carStartAndLeave") Doesn't play audio after scene changes!
        //fade to black, go to precinct day 1 
    - IsQuestComplete(pay_for_gas):
        That noise didn't sound good. I should probably see what's up.
    - else:
        ~ SetSpeaker(Speaker.Lupe)
        {
        	- gas_pumps == 1:
        		"Out of order. Pay inside." Of course. Just my luck. 
                ~ StartQuest(Level1_Quests.pay_for_gas)
                ~ DiscoverMystery(0)
        		-> DONE
            - gas_pumps == 2: 
                Let's get this over with. Sooner I pay, sooner I can get back on the road, sooner Thelton won't bite my head off. -> DONE
            - gas_pumps == 3: 
                ... Why am I still looking at this gas pump? -> DONE
            - gas_pumps < 6: 
                ... -> DONE
            - gas_pumps == 7: 
                .. This gas pump is kind of freaky. -> DONE
            - else: 
                I should really go pay for gas. -> DONE
	    }
    }

= enter_store
    ~ SetSpeaker(Speaker.Lupe)
    I guess I'll find someone to help me inside here.
    TODO SFX Door open
    ~openDoor()
    
    ~ ChangeGameScene("scene1_2", 0, 0)
    -> DONE

// ------------- SCENE1.2 MelOMarket Store
=== scene1_2 ===
TODO SFX Door close
~closeDoor()
//The gas station door chimes with a slightly out of tune jingle.
//Lupe hears the employee mutter a very unattentive "Welcome to MelOMart".
* [vending_machine] -> vending_machine
* [employee] -> employee
* [backrooms] -> door_to_backroom
* [exit door] -> door_to_outside

= vending_machine
    {Sugar-Flavored Snack Bites. Pickle Chips. Jerkied Ox Sticks...I think I'll pass.| Is this stuff FDA approved?}
    -> DONE

= employee
{IsClueFound(evidence_broken_cash_reg):
    {IsClueFound(evidence_cashreg_fix):
        ~ SetSpeaker(Speaker.Beth)
        I told you--
        ~ SetSpeaker(Speaker.Lupe)
        Mel said to hit it. In the side.
        // the employee whacks the side of the register.The Drawer pops open.
        //TODO ^^ SFX, cash register hit noise
        ~ PlaySpecialAnimation(Speaker.Beth)
        ~ PlaySFX("cashRegisterWhack")
        ~ PlaySFX("treeFalls")
        ~ ShakeCamera(0.25, 0.05, 4)
        ~ SetSpeaker(Speaker.Beth) 
        Cool. $20.34.
        ~ SetSpeaker(Speaker.Lupe)
        <i>Fiiiinally</i>
        ~ CompleteQuest(pay_for_gas)
        // Lupe pays. Suddenly, a loud crashing noise is heard from outside.
        What was that?
        ~ StartQuest(look_at_tree)
        -> DONE
    - else:
        ~ SetSpeaker(Speaker.Beth)
        {The register drawer is busted. | Register is still busted. | Stillllllll busted, buddy.}
        -> DONE
    }
-else:
    // -- DISCOVER CASHREG_BROKEN
    ~ SetSpeaker(Speaker.Lupe)
    Hey man. How're you?
    ~ SetSpeaker(Speaker.Beth) 
    I work at a gas station. How do you think I am?
    ~ SetSpeaker(Speaker.Lupe)
    ...The pump's broken. It says to pay inside?
    ~ SetSpeaker(Speaker.Beth) 
    Yeahhhhh. You can't pay right now. The register drawer is busted
    ~ SetSpeaker(Speaker.Lupe)
    Can you, uh, fix it?
    ~ SetSpeaker(Speaker.Beth) 
    That's kinda above my paygrade?
    ~ SetSpeaker(Speaker.Lupe)
    Whose paygrade is it <i>not</i> above?
    ~ SetSpeaker(Speaker.Beth) 
    My manager's in the bathroom.
    ~ SetSpeaker(Speaker.Lupe)
    Should I wait or...?
    ~ SetSpeaker(Speaker.Beth)
    Oh he's not using it just cleaning some weird thing.
    ~ SetSpeaker(Speaker.Lupe)
    Okay...
    ~DiscoverClue(evidence_broken_cash_reg)
    -> DONE
}

= door_to_backroom
TODO SFX
{IsClueFound(evidence_broken_cash_reg):
    ~ openDoor()
    ~ ChangeGameScene("scene1_3", 0, 0)
    -> DONE
    - else:
        ~ SetSpeaker(Speaker.Lupe)
        I've got no reason to poke around.
        -> DONE
}

= door_to_outside
{IsQuestComplete(pay_for_gas):
    //TODO SFX
    ~ openDoor()
    ~ChangeGameScene("scene1_4", 0, 0)

    -> DONE
    -else:
        ~ SetSpeaker(Speaker.Lupe)
        Still gotta pay.
        -> DONE
}

// ------------- SCENE1.3 Breakroom ---- >>
=== scene1_3 ===
//TODO SFX door close
~closeDoor()
// Lupe pushes through the door to the backroom of the gas station. The room is a small break room. The presumed manager stands at the sink, scrubbing goop.
 
+ [mel] -> mel

= mel
{IsClueFound(evidence_cashreg_fix) == false:
    ~ SetSpeaker(Speaker.Lupe)
    Hey. Are you the manager?
    ~ SetSpeaker(Speaker.Mel)
    Mel's the name. What are you doing in here?
    ~ SetSpeaker(Speaker.Lupe)
    I need to pay for gas. The cash register is broken.
    ~ SetSpeaker(Speaker.Mel)
     That old piece of junk. 
    The drawer is just jammed, it just needs to be whacked in the side a bit.
    ~ SetSpeaker(Speaker.Lupe)
    Thanks.
    ...
    Uh. What is that stuff?
    ~ SetSpeaker(Speaker.Mel)
        No idea, it was here when I unlocked this morning. 
        Darn stuff won't scrub off. 
     ~ SetSpeaker(Speaker.Lupe)
     Huh.
     Weird.
    ~ DiscoverClue(evidence_cashreg_fix)
    -> DONE
    -else:
        ~ SetSpeaker(Speaker.Mel)
        {Look, I'd like to help more, but I gotta get this stuff out. | This is <i>definitely</i> a code violation. | It almost looks like mold? Or jello. Gross, ain't it? | I love my job. I love my job. Ugh.}
        -> DONE
}



= door_back_to_interior
    ~ ChangeGameScene("scene1_2", 1, 0) 
    //TODO SFX Door open
    ~openDoor()
    ->DONE
    
// ------------- SCENE1.4 Tree Falls
=== scene1_4 ===
// Lupe pays. Suddenly, a loud crashing noise is heard from outside.
~ SetSpeaker(Speaker.Lupe)
* [car] -> car
* [fallen tree] -> fallen_tree

= marlowe
    -> fallen_tree
    
= car
    {
    - IsQuestComplete(look_at_tree):
        ~ SetSpeaker(Speaker.Lupe)
        "Sorry I was so late to the debrief boss, I had to go report a suspicious fallen tree." Ugh. Guess I'll be more than a little late...Thelton's gonna kill me.
        ~ ChangeGameScene("scene2_DS", 0, 0)
                TODO SFX Car closing door noise and leaving
                //~PlaySFX("carStartAndLeave")
        -> DONE
    - IsQuestComplete(pay_for_gas):
        That noise didn't sound good. I should probably see what's up.
    - else: 
        ~ SetSpeaker(Speaker.Lupe)
        Still gotta pay.
        -> DONE
    }

= fallen_tree 
    // Lupe sees a tree has fallen and blocked the way out of town.
    ~ SetSpeaker(Speaker.Marlowe)
    Timber. Heh heh. Hope you didn't need to be anywhere anytime soon.
    ~ SetSpeaker(Speaker.Lupe)
    ...great. Any way I can get this fixed?
    ~ SetSpeaker(Speaker.Marlowe)
    Got a chainsaw on ya? 
    Heheheheh. 
    Ah, don't look so grumpy. 
    If you put in a complaint with the local Police 
    I'm sure you'll be out of here in no time.
    ~ CompleteQuest(look_at_tree)
    -> DONE
  
