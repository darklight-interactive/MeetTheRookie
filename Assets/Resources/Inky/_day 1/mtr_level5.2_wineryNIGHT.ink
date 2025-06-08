// SCENE 5: The Winery - NIGHTTIME
// --- scene 5 ---

    
=== scene5_2 ===

+ [cut fence] -> fence
+ [window] -> window
+ [tracks] -> strange_footsteps
+ [door] -> door_to_inside_winery_night

= lupe_intro
TODO SFX CAR DOOR CLOSE NOISE
~PlaySFX("carDoorOpenAndClose")
~ SetSpeaker(Speaker.Lupe)
Misra, WAIT!
-> DONE 

= fence 
~ SetSpeaker(Speaker.Lupe)
I can't leave, I have to find Misra.
-> DONE

= window
~ SetSpeaker(Speaker.Lupe)
Ugh, I can't see inside, it's too dark.
-> DONE
   

= strange_footsteps
~ SetSpeaker(Speaker.Lupe)
Whoever left these here...are they here again? 
-> DONE

= door_to_inside_winery_night
    ~ openDoor()
    ~ ChangeGameScene("scene5_3") 
    -> DONE

===scene5_3===
//Inside the winery
    //TODO SFX DOOR CLOSE


* [wine barrels] -> wine_barrels
* [claw marks] -> claw_marks
* [damaged equipment] -> equipment
* [floor wine] -> floor_splatters
* [backroom door] -> door_office
* [exit door] -> exit
* [handprint] -> handprint
* [inside_window] -> inside_window

= lupe_inside_intro
//~ closeDoor()
~ SetSpeaker(Speaker.Lupe)
Misra??
-> DONE
    
= inside_window
~ SetSpeaker(Speaker.Lupe)
Where's Misra?
-> DONE
   

= handprint
~ SetSpeaker(Speaker.Lupe)
 How could Misra run off like that? 
 Someone dangerous could be hiding in here...
 -> DONE

= claw_marks
~ SetSpeaker(Speaker.Lupe)
 What could've left these.... 
 -> DONE

= floor_splatters
 ~ SetSpeaker(Speaker.Lupe)
 It really does look like blood...
 Gah. 
 I need to keep moving.
 -> DONE


= equipment
~ SetSpeaker(Speaker.Lupe)
What really happened here...
-> DONE 

= wine_barrels
~ SetSpeaker(Speaker.Lupe)
This place is somehow even more creepy at night...
-> DONE

= goop
~ SetSpeaker(Speaker.Lupe)
God, more of this gunk. It reeks - smells like vinegar. 
-> DONE
   
= door_office
~ SetSpeaker(Speaker.Lupe)
Wait...
Wasn't this closed before?
~ openDoor()

~ ChangeGameScene("scene5_4")
-> DONE

= exit
~ SetSpeaker(Speaker.Lupe)
I can't leave--not without Misra.
-> DONE

=== scene5_4 ====
+ [handwritten_note_on_corkboard] -> handwritten_note
+ [winery_blueprint] -> winery_blueprint
//+ [locked_door_number_pad] ->number_pad
+ [newspaper_article_pinned_on_corkboard] -> read_newspaper
+ [door back to main room] -> door_back_to_main_room
+ [winery_graph] -> winery_graph

= read_newspaper
    ~ DiscoverClue(Mystery3.evidence_newspaper)
    ~ SetSpeaker(Speaker.Lupe)
    Well, that's depressing. Seems like the Winery closing was the last straw.
    -> DONE

= winery_graph
    ~ DiscoverClue(Mystery3.evidence_winerygraph)
    ~ SetSpeaker(Speaker.Lupe)
    This place has really seen some bad days. 
    And some good ones, too. 
    I've never seen profit be this erratic and inconsistent.
    -> DONE

= winery_blueprint
    ~ DiscoverClue(Mystery3.evidence_blueprint)
    ~ SetSpeaker(Speaker.Lupe)
    What's this...?
    Huh...someone's birthday...
    Why does Sarah sound familiar...
    -> DONE

= handwritten_note
    ~ DiscoverClue(Mystery3.evidence_handwrittennote)
    ~ SetSpeaker(Speaker.Lupe)
    Another goat reference... 
    -> DONE


= door_back_to_main_room
    ~ SetSpeaker(Speaker.Lupe)
    {No, no, no--Misra's got to be here somewhere. | I can't leave yet. | I know they're here <i> somewhere </i>. }
    -> DONE
 

= door_pinpad
TODO The Number Pad Interaction, Misra appearing in the scene, and the animation
~ DiscoverClue(evidence_pinpad)
{IsQuestComplete(Level5_Quests.discover_pinpad):
    ~ SetSpeaker(Speaker.Lupe)
    ~ CompleteQuest(discover_pinpad)
    Hmm.
    What could the code be?
- else:
    ~ SetSpeaker(Speaker.Lupe)
    Why not try the basic option...
    someone's birthday...
}
~ RequestSpecialUI(su_pinpad)
-> DONE


// = number_pad

// {winery_blueprint_evidence:
//     [Lupe] Why not try the basic option...
//     [Lupe] someone's birthday...
//         -> misra_found
        
//     -else: 
//         Hmm.
//         What could the code be? -> scene5_3
        
//         }


// //stardew valley
//     {locked_door_visited:
//         [Lupe]{Is someone there? | Hello?} ->scene5_3
//     - else: 
//         # Lupe leans close to the locked door and hears strange noises from the other side of it, of unidentified origin.
//         [Lupe] Misra? Is that you?
//         ~ locked_door_visited = true
//         -> scene5_3
// }

= locked_door
    ~ SetSpeaker(Speaker.Misra)
    BOO!
    ~ SetSpeaker(Speaker.Lupe)
    Jesus! 
    ~ SetSpeaker(Speaker.Misra)
    Hahahaha!
    Gotcha!
    ~ SetSpeaker(Speaker.Lupe)
    Are you serious?
    Was this just all a bit for you to scare me again?
    ~ SetSpeaker(Speaker.Misra)
    I mean, no.
    I don't know what that noise was.
    I looked around and found nothing.
    So I took the opportunity to spook ya.
    ~ SetSpeaker(Speaker.Lupe)
    ...
    Not cool.
    Or very professional.
    Splitting up like that is a rookie move.
    ~ SetSpeaker(Speaker.Misra)
    Sorry, sorry. 
    You're right. 
    It wasn't in good taste.
    ~ SetSpeaker(Speaker.Lupe)
    Whatever.
    I'm just...glad you're alright.
    But if this wasn't all a joke, then what about the noises?
    ~ SetSpeaker(Speaker.Misra)
    I haven't heard anything else.
    Was probably just the wind or something.
    ~ SetSpeaker(Speaker.Lupe)
    But what about--
    // TODO: Add monster noise SFX here
    ~ PlaySFX("monsterGrowl")
    ~ SetSpeaker(Speaker.Lupe)
    ...
    ~ SetSpeaker(Speaker.Misra)
    ...
    ~ SetSpeaker(Speaker.Lupe)
    What was that?
    ~ SetSpeaker(Speaker.Misra)
    Run.
    ~ SetSpeaker(Speaker.Lupe)
    But what-
    ~ SetSpeaker(Speaker.Misra)
    RUN. Now!
    // TODO: Add tentacle animation and SFX
    ~ SetSpeaker(Speaker.Lupe)
    WHAT THE-
    ~ SetSpeaker(Speaker.Misra)
    ~ PlaySpecialAnimation(Speaker.Misra)
    -> DONE




