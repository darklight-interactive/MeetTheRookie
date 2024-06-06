// SCENE 5: The Winery - NIGHTTIME
// --- scene 5 ---

    
=== scene5_2 ===
TODO SFX CAR DOOR CLOSE NOISE
+ [cut fence] -> fence
+ [window] -> window
+ [tracks] -> strange_footsteps
+ [door] -> door_to_inside_winery_night

= lupe_intro
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
    ~ ChangeGameScene("scene5_3") 
    TODO SFX DOOR OPEN 
    -> DONE

===scene5_3===
//Inside the winery
    TODO SFX DOOR CLOSE


* [wine barrels] -> wine_barrels
* [claw marks] -> claw_marks
* [damaged equipment] -> equipment
* [floor wine] -> floor_splatters
* [backroom door] -> door_office
* [exit door] -> exit
* [handprint] -> handprint
* [inside_window] -> inside_window

= lupe_inside_intro
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
    TODO SFX DOOR OPEN 

~ ChangeGameScene("scene5_4")
-> DONE

= exit
~ SetSpeaker(Speaker.Lupe)
I can't leave--not without Misra.
-> DONE

=== scene5_4 ====
    TODO SFX DOOR CLOSE 


  + [handwritten_note_on_corkboard] -> handwritten_note
   

+ [winery_blueprint] -> winery_blueprint

//+ [locked_door_number_pad] ->number_pad

+ [newspaper_article_pinned_on_corkboard] -> read_newspaper

+ [door back to main room] -> door_back_to_main_room

= read_newspaper
    {IsQuestComplete(newspaper):
    ~SetSpeaker(Speaker.Lupe)
     Well, that's depressing. Seems like the Winery closing was the last straw.
        -> DONE
        
    - else:
        This place really did have bad luck.
     ~ CompleteQuest(newspaper)
    
    -> DONE
}

+ [winery_graph] -> winery_graph

= winery_graph
    {IsQuestComplete(winerygraph):
        ~SetSpeaker(Speaker.Lupe)
        Hmm...
        
        ->scene5_3
        
    -else:
        ~SetSpeaker(Speaker.Lupe)

        This place has really seen some bad days. 
        And some good ones, too. 
        I've never seen profit be this erratic and inconsistent.
    ~ CompleteQuest(winerygraph)
    -> DONE
}

=winery_blueprint
{IsQuestComplete (blueprint):
     ~SetSpeaker(Speaker.Lupe)
    I'm in the Main Office...where could Misra have gone?
    -> DONE
-else:
    ~SetSpeaker(Speaker.Lupe)
    What's this...?
    Huh...someone's birthday...
    Why does Sarah sound familiar...
~ CompleteQuest (blueprint)
    -> DONE
 }


=handwritten_note
 // stardew valley thing
    {IsQuestComplete:
        ~ SetSpeaker(Speaker. Lupe)
        Another goat reference... 
        -> DONE
    
    -else:
         ~ SetSpeaker(Speaker. Lupe)
        What's this...?
        ~ CompleteQuest(handwrittennote)
        -> DONE
    }


= door_back_to_main_room
 ~ SetSpeaker(Speaker. Lupe)
 {No, no, no--Misra's got to be here somewhere. | I can't leave yet. | I know they're here <i> somewhere </i>. }
 -> DONE
 

= door_pinpad
TODO The Number Pad Interaction, Misra appearing in the scene, and the animation
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




// = misra_found
// // the player has inputed the correct code, and when they back out of keypad close up, Misra is standing there, behind them. Lupe is startled.
// [Misra] BOO!
// [Lupe] Jesus! 
// [Misra] Hahahaha!
// [Misra] Gotcha!
// [Lupe] Are you serious?
// [Lupe] Was this just all a bit for you to scare me again?
// [Misra] I mean, no.
// [Misra] I don't know what that noise was.
// [Misra] I looked around and found nothing.
// [Misra] So I took the opportunity to spook ya.
// [Lupe]...
// [Lupe] Not cool.
// [Lupe] Or very professional.
// [Lupe] Splitting up like that is a rookie move.
// [Misra] Sorry, sorry. 
// [Misra] You're right. 
// [Misra] It wasn't in good taste.
// [Lupe] Whatever.
// [Lupe] I'm just...glad you're alright.
// [Lupe] But if this wasn't all a joke, then what about the noises?
// [Misra] I haven't heard anything else.
// [Misra] Was probably just the wind or something.
// [Lupe] But what about--
// //they are interrupted by another noise coming from behind the locked door. this one is more monster like. 
// [Lupe] ...
// [Misra] ...
// [Lupe] What was that?
// [Misra] Run.
// [Lupe] But what-
// [Misra] RUN. Now!
// //A tentacle bursts through the door and makes a grab toward Misra.
// [Lupe] WHAT THE-
// //Screen cuts to black, and over the black we hear monster noises, screams, and death. EL fin.


// -> DONE //{go to gas station, day 2}




