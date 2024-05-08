# Level 5 - The Winery, Night, Day 1 

# This Level holds both a Dating Sim and Gameplay. We start in the interior of Lupe's car during a Stakeout with Misra, outside the Old Winery. The Dating Sim can be cycled through, and at a certain point is interrupted by a loud, unexplained disturbance from inside the Winery. Misra is quick to get out of the car to invesitigate.

* [BEGIN] -> scene5_1

// ====== SPEAKER ======
LIST Speaker = Lupe, Misra
VAR curSpeaker = Speaker.Lupe
== function set_speaker(value)
    ~ curSpeaker = value

// ====== QUEST CHAIN =======
LIST QuestChain = VISIT_LOCKED_DOOR, CORRECT_CODE, VISIT_TABLE_PAPERS, VISIT_NEWS_ARTICLE, VISIT_NOTE, VISIT_ECONOMICS
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
LIST Clues = EVIDENCE_WINERY_BLUEPRINT, EVIDENCE_NEWSPAPER_ARTICLE, EVIDENCE_NOTE, EVIDENCE_GRAPH
VAR knowledgeState = ()
=== function DiscoverClue(clue)
    ~ knowledgeState += clue
    
=== function IsClueFound(clue)
    { LIST_ALL(Clues) ? clue:
        // clues are either found, or not
        ~ return knowledgeState ? clue
    }


// Objectives:
// Find Misra


=== scene5_1 ===
# DATING SIM
# Location: Lupe's car, interior. Parked outside old Winery, Night.
# fade to black

-> DONE


=== scene5_2 ===
# over black screen, we hear the car door open and close, paired with Misra's retreating footsteps as they run after the noise. Fade in. We are standing outside the Winery, with the parked car in view. The front door to the Winery is left ajar. Misra is nowhere in sight.

# OBJECTIVE: Find Misra

[Lupe] Misra, WAIT! -> DONE

= outdoor_choices
+ [front_door_to_Winery] -> DONE

* [window]
   Ugh, I can't see inside, it's too dark. -> DONE
    
=== scene5_2INSIDE ===
# Lupe enters the main room inside the Winery. The door on the back wall that was closed during their morning visit to the Winery is now ajar. Misra is still nowhere to be found.

* [more_goop]
    God, more of this gunk. It reeks - smells like vinegar. -> DONE

+ [door_to_backroom] -> DONE

=== scene5_3 ===
# Lupe enters the office. Across the room, there is a locked door with a keypad that requires a code. There are some filing storage cabinets, a table with papers strewn across it, and a corkboard against the wall with flyers and stuff on it. No Misra.

+ [handwritten_note_on_corkboard] 
    {IsQuestComplete(VISIT_NOTE):
    [Lupe] Another goat reference...
    -> DONE
    
    -else:
     # Lupe looks at a handwritten note pinned to the board, written in messy handwriting. The Note Reads: "Those blasted goats have damned us to hell." ADD NOTE TO SYNTHESIS 
       // [Lupe] <i>"Those blasted goats have damned us to hell."</i> - Edited out because art will have asset, Lupe doesn't need to read
       
    ~ CompleteQuest(VISIT_NOTE)
    ~ DiscoverClue(EVIDENCE_NOTE)

  
    -> scene5_3
}


+ [table_papers] -> DONE

+ [locked_door]
    {IsQuestComplete(VISIT_LOCKED_DOOR):
        [Lupe]{Is someone there? | Hello?} ->DONE
    - else: 
        # Lupe leans close to the locked door and hears strange noises from the other side of it, of unidentified origin.
        [Lupe] Misra? Is that you?
        ~ CompleteQuest(VISIT_LOCKED_DOOR)
        -> DONE
}

+ [number_pad] -> DONE

+ [newspaper_article_pinned_on_corkboard]
    {IsQuestComplete(VISIT_NEWS_ARTICLE):
     [Lupe] Well, that's depressing. Seems like the Winery closing was the last straw.
        -> DONE
    - else:
    # Commented out becuase art will have asset, lupe doesn't need to read.
    
        // [Lupe] <i> "Headline: The Death of Another Small Town
        
        // Local Winery closes after devasting month in profits, Kettle Rock's economy taking yet another plunge. Will this be the final nail in the coffin in this town's rocky history of highs and lows? Word is that there are talks about demolishing the downtown strip to make way for a new Highway in the Idaho transit pipeline. With many locals already packed up and move on, Kettle Rock faces a very possible erasure off the map."
        
        // </i>
    ~ CompleteQuest(VISIT_NEWS_ARTICLE)
    ~ DiscoverClue(EVIDENCE_NEWSPAPER_ARTICLE)
    # ADD TO SYNTHESIS
    
    -> DONE
}

+ [winery_graph]
    {IsQuestComplete(VISIT_ECONOMICS):
        # The graph clearly shows the rise and fall, and rise and fall, and rise and fall of the Winery's tumultous profit history, withe a clear and unnatural sharp drop off in 1995. this can be an art asset, or can be read aloud by Lupe, whatever scope allows.
        [Lupe] Business seems to be...not good.
        
        ->DONE
        
    -else:
        [Lupe] This place has really seen some bad days. And some good ones, too. I've never seen profit be this erratic and inconsistent.
    ~ CompleteQuest(VISIT_ECONOMICS)
    ~ DiscoverClue(EVIDENCE_GRAPH)

    # add to Synthesis
-> DONE
}

=table_papers
# Lupe finds a design blueprint-esque paper that details the layout of the factory, including the secret room (that is unaccessible to players/the entrance to it is not in plain sight).
{IsQuestComplete(VISIT_TABLE_PAPERS):
    [Lupe] I'm in the Main Office...where could Misra have gone?
    -> DONE
-else:
    [Lupe] What's this...?
~ CompleteQuest(VISIT_TABLE_PAPERS)
~DiscoverClue(EVIDENCE_WINERY_BLUEPRINT)

# ADD TO SYNTHESIS
-> DONE
 }

= number_pad
# PROGRAMMMINGGG HELPPPPP SKY HELPPPPPP

# If Lupe inputs the CORRECT code, the pin pad flashes the green light and gives the little affirmative beep. The player then becomes spooked by Misra (and the FIND MISRA objective becomes complete), who suddenly reappears and launches them back into a Dating Sim moment, where they briefly discuss Misra running off. This moment is interrupted by the crytid's tentacle reaching out, putting us back in the main game.

{IsQuestComplete(CORRECT_CODE):
    [Lupe] There we go. 
        -> misra_found_dating_sim
        
    -else: 
        That's not right. -> DONE
        
        }

# If Lupe inputs the INCORRECT CODE, the pin pad flashes red and the player must go find the correct code


= misra_found_dating_sim

This is the closing Dating Sim of the level, where Misra spooks Lupe. The two talk for a bit before Lupe brings up the sounds behind the door. The Dating Sim ends when the sounds restart, and we are put back in the level. -> DONE

=ending_sequence
#The door, now unlocked, swings open to reveal blackness beyond when the player clicks it. For a moment, Lupe and Misra both stand unmoving. Then, a tentacle swings out from the dark doorway. The screen cuts to black and we hear Misra scream for Lupe to run. We hear death noises, and our day is done.

-> DONE




