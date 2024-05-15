# Level 5 - The Winery, Night, Day 1 

# This Level holds both a Dating Sim and Gameplay. We start in the interior of Lupe's car during a Stakeout with Misra, outside the Old Winery. The Dating Sim can be cycled through, and at a certain point is interrupted by a loud, unexplained disturbance from inside the Winery. Misra is quick to get out of the car to invesitigate.

// Variables for scene5_2INSIDE:
VAR locked_door_visited = false
VAR correct_code = false // programming needs to do their magic with this one. When the correct code is found by the player, it must be entered the right way into the keypad. If it isn't, it won't trigger the next part of gameplay. Not quite sure how to do this with a true/false variable.
VAR table_papers_visited = false
VAR newspaper_article_visited = false
VAR handwritten_note_visited = false
VAR economics_of_winery_visited = false

// Evidence Variables:
VAR winery_blueprint_evidence = false
VAR newspaper_article_evidence = false
VAR winery_handwritten_note_evidence = false
VAR winery_graph_evidence = false



// Objectives:
// Find Misra

* [BEGIN] -> scene5_1

=== scene5_1 ===
# DATING SIM
# Location: Lupe's car, interior. Parked outside old Winery, Night.
# fade to black

-> scene5_2


=== scene5_2 ===
# over black screen, we hear the car door open and close, paired with Misra's retreating footsteps as they run after the noise. Fade in. We are standing outside the Winery, with the parked car in view. The front door to the Winery is left ajar. Misra is nowhere in sight.

# OBJECTIVE: Find Misra

[Lupe] Misra, WAIT! -> outdoor_choices

= outdoor_choices
+ [front_door_to_Winery] -> scene5_2INSIDE

* [window]
   Ugh, I can't see inside, it's too dark. -> outdoor_choices
    
=== scene5_2INSIDE ===
# Lupe enters the main room inside the Winery. The door on the back wall that was closed during their morning visit to the Winery is now ajar. Misra is still nowhere to be found.

* [more_goop]
    God, more of this gunk. It reeks - smells like vinegar. -> scene5_2INSIDE

+ [door_to_backroom] -> scene5_3

=== scene5_3 ===
# Lupe enters the office. Across the room, there is a locked door with a keypad that requires a code. There are some filing storage cabinets, a table with papers strewn across it, and a corkboard against the wall with flyers and stuff on it. No Misra.

+ [handwritten_note_on_corkboard] 
    {handwritten_note_visited:
    [Lupe] Another goat reference...
    -> scene5_3
    
    -else:
     # Lupe looks at a handwritten note pinned to the board, written in messy handwriting. The Note Reads: "Those blasted goats have damned us to hell." ADD NOTE TO SYNTHESIS 
       // [Lupe] <i>"Those blasted goats have damned us to hell."</i> - Edited out because art will have asset, Lupe doesn't need to read
       
    ~ handwritten_note_visited = true
    ~ winery_handwritten_note_evidence = true
  
    -> scene5_3
}


+ [table_papers] -> table_papers

+ [locked_door]
    {locked_door_visited:
        [Lupe]{Is someone there? | Hello?} ->scene5_3
    - else: 
        # Lupe leans close to the locked door and hears strange noises from the other side of it, of unidentified origin.
        [Lupe] Misra? Is that you?
        ~ locked_door_visited = true
        -> scene5_3
}

+ [number_pad] -> number_pad

+ [newspaper_article_pinned_on_corkboard]
    {newspaper_article_visited:
     [Lupe] Well, that's depressing. Seems like the Winery closing was the last straw.
        -> scene5_3
    - else:
    # Commented out becuase art will have asset, lupe doesn't need to read.
    
        // [Lupe] <i> "Headline: The Death of Another Small Town
        
        // Local Winery closes after devasting month in profits, Kettle Rock's economy taking yet another plunge. Will this be the final nail in the coffin in this town's rocky history of highs and lows? Word is that there are talks about demolishing the downtown strip to make way for a new Highway in the Idaho transit pipeline. With many locals already packed up and move on, Kettle Rock faces a very possible erasure off the map."
        
        // </i>
    ~ newspaper_article_visited = true
    ~ newspaper_article_evidence = true
    // ADD TO SYNTHESIS
    
    -> scene5_3
}

+ [winery_graph]
    {economics_of_winery_visited:
        // The graph clearly shows the rise and fall, and rise and fall, and rise and fall of the Winery's tumultous profit history, withe a clear and unnatural sharp drop off in 1995. this can be an art asset, or can be read aloud by Lupe, whatever scope allows.
        [Lupe] Business seems to be...not good.
        
        ->scene5_3
        
    -else:
        [Lupe] This place has really seen some bad days. And some good ones, too. I've never seen profit be this erratic and inconsistent.
    ~ economics_of_winery_visited = true
    ~ winery_graph_evidence = true
    # add to Synthesis
-> scene5_3
}

=table_papers
// Lupe finds a design blueprint-esque paper that details the layout of the factory, including the secret room (that is unaccessible to players/the entrance to it is not in plain sight).
{table_papers_visited:
    [Lupe] I'm in the Main Office...where could Misra have gone?
    -> scene5_3
-else:
    [Lupe] What's this...?
~ table_papers_visited = true
~ winery_blueprint_evidence = true 
# ADD TO SYNTHESIS
-> scene5_3
 }

= number_pad
// PROGRAMMMINGGG HELPPPPP SKY HELPPPPPP

// If Lupe inputs the CORRECT code, the pin pad flashes the green light and gives the little affirmative beep. The player then becomes spooked by Misra (and the FIND MISRA objective becomes complete), who suddenly reappears and they briefly discuss Misra running off. This moment is interrupted by the crytid's tentacle reaching out and killing them.

{correct_code:
    [Lupe] There we go. 
        -> misra_found
        
    -else: 
        That's not right. -> scene5_3
        
        }

// If Lupe inputs the INCORRECT CODE, the pin pad flashes red and the player must go find the correct code


= misra_found
// the player has inputed the correct code, and when they back out of keypad close up, Misra is standing there, behind them. Lupe is startled.
[Misra] BOO!
[Lupe] Jesus! 
[Misra] Hahahaha!
[Misra] Gotcha!
[Lupe] Are you serious?
[Lupe] Was this just all a bit for you to scare me again?
[Misra] I mean, no.
[Misra] I don't know what that noise was.
[Misra] I looked around and found nothing.
[Misra] So I took the opportunity to spook ya.
[Lupe]...
[Lupe] Not cool.
[Lupe] Or very professional.
[Lupe] Splitting up like that is a rookie move.
[Misra] Sorry, sorry. 
[Misra] You're right. 
[Misra] It wasn't in good taste.
[Lupe] Whatever.
[Lupe] I'm just...glad you're alright.
[Lupe] But if this wasn't all a joke, then what about the noises?
[Misra] I haven't heard anything else.
[Misra] Was probably just the wind or something.
[Lupe] But what about--
//they are interrupted by another noise coming from behind the locked door. this one is more monster like. 
[Lupe] ...
[Misra] ...
[Lupe] What was that?
[Misra] Run.
[Lupe] But what-
[Misra] RUN. Now!
//A tentacle bursts through the door and makes a grab toward Misra.
[Lupe] WHAT THE-
//Screen cuts to black, and over the black we hear monster noises, screams, and death. EL fin.


-> DONE //{go to gas station, day 2}




