VAR paid_for_gas = false
VAR tree_fell = false
VAR knows_gas_is_broken = false
VAR knows_cash_is_broken = false
VAR knows_how_to_fix_cash = false


*employee -> employee
*gas_pump -> gas_pump

== employee ==
{ 
- paid_for_gas: 
    thanks for fixing this again. -> DONE
- knows_how_to_fix_cash: 
    your boss said all you have to do is wack it. #lupe
    ...um... ok i guess i'll do that.
    
- else: 
    sorry you can't pay right now since the register is broken you'll have to wait for my boss to get back #employee
    
    not an option #lupe
    
    well i guess you can ask them how to fix it, idk i don't really care? #employee
    
    <thought> oh my god #lupe -> DONE
}
--> END

== gas_pump ==

{paid_for_gas: 
    don't need to pay for gas anymore, now i just gotta leave -> DONE 
- else:
        {stopping:
            -   "out of order" i have to go inside to pay? URGH! 
                ~ knows_gas_is_broken = true 
                -> DONE
            - I can't believe this is broken -> DONE
        } 
}

== car ==
{tree_fell: 
    guess i'm headed to the precinct -> DONE //{goto("precinct")} 
- else: 
    can't leave before i pay for gas -> END
}

== outside_npc ==
{tree_fell: i think you should put in a police report @ the station if you want to get this cleared. -> DONE | can't believe the gosh darned pump is broken -> DONE}

=== mel ===
{knows_cash_is_broken:
    {what are you doing back here? go back to the general store please | ...are you going to... leave? | please get out...} -> DONE
- else:
    the cash register is broken #lupe
    oh that old thing? all you gotta do is wack it! #mel
        ~knows_how_to_fix_cash = true
        -> DONE
}

