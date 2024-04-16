VAR current_npc = ""
VAR first_interact = false
VAR paid_for_gas = false
VAR tree_fell = false
VAR knows_cash_is_broken = false
VAR knows_how_to_fix_cash = false

* [BEGIN] -> intro
=== intro ===
# name : Chief Thelton
# location : unknown
You've reached Chief Thelton. I'm not available right now. Leave your message after the beep.

* [CONTINUE] 
# name : Lupe
Chief Thelton, it's Lupe. Had to change our route; gas was running low. We're at a pit stop, edge of Kettle Rock, Idaho. Will update soon. 
    -> scene1_1

=== scene1_1 ===
// FUNC_SCENE_CHANGE
// PLAYER_PROMPT -> highlight gas pump

{first_interact:
    *[car] -> car
    *[outside_npc] -> npc
    *[enter_store] -> scene1_2
}
+[gas_pump] 
    -> gas_pump
Lupe 


= gas_pump
~ first_interact = true
{paid_for_gas: 
    don't need to pay for gas anymore, now i just gotta leave -> DONE 
- else:
        {stopping:
            -   "out of order" i have to go inside to pay? URGH! 
                -> scene1_1
            - I can't believe this is broken. 
                -> scene1_1
        } 
}

= car
{tree_fell: 
    guess i'm headed to the precinct -> DONE //{goto("precinct")} 
- else: 
    can't leave before i pay for gas -> scene1_1
}

= npc
{tree_fell: 
i think you should put in a police report @ the station if you want to get this cleared. -> scene1_1
- else :
    can't believe the gosh darned pump is broken -> scene1_1
}

=== scene1_2 ===
// FUNC_SCENE_CHANGE
# location : MelOMart Store
The gas station door chimes with a slightly out of tune jingle.
Lupe hears the employee mutter "Welcome to MelOMart" from the chip aisle.
-> main
= main
*[talk_to_employee] 
    You approach the employee. They are pretending to sweep but really they are watching videos on their phone.
    -> employee
+ [pick up a bag of chips]
    you pick a bag of chips from the aisle
    -> main

= employee
# name : employee
~ current_npc = "[employee]"
* hows life?
    {current_npc} i hate it 
    ->employee
* um.. the gas pump isnt working
    sorry you can't pay right now since the register is broken you'll
    have to wait for my boss to get back
    -> main
    
    
/*
    { 
    - paid_for_gas: 
        thanks for fixing this again. -> DONE
    - knows_how_to_fix_cash: 
        your boss said all you have to do is wack it. #lupe
        ...um... ok i guess i'll do that.
        
    * else: 
        
        
        not an option #lupe
        
        well i guess you can ask them how to fix it, idk i don't really care? #employee
        
        <thought> oh my god #lupe -> DONE
    }

= mel
{knows_cash_is_broken:
    {what are you doing back here? go back to the general store please | ...are you going to... leave? | please get out...} -> DONE
- else:
    the cash register is broken #lupe
    oh that old thing? all you gotta do is wack it! #mel
        ~knows_how_to_fix_cash = true
        -> DONE
}
*/



