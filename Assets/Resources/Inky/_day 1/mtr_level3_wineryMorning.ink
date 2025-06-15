// SCENE 3: The Winery
// --- scene 3 ---
=== function outsideCluesFound() ===
    ~ return IsClueFound(evidence_fence) && IsClueFound(evidence_broken_window) && IsClueFound(evidence_footsteps)
=== function insideCluesFound() ===
    ~return  IsClueFound(evidence_damages) && IsClueFound(evidence_claw_marks) && IsClueFound(evidence_handprint) && IsClueFound(evidence_broken_window)

=== scene3_1 ===
// location : The Winery, outside

// Lupe and Misra arrive at the Winery. For a place having just closed a short time ago, it looks pretty run down. The whole building is in obvious disrepair with loose wood on the ground and vines creeping up the sides of the walls. Kind of creepy.
* [cut fence] -> fence
* [window] -> window
* [tracks] -> strange_footsteps
* [door] -> door_main_room
* [misra] -> Misra_Dialogue.3_1
// QUEST OBJECTIVE: What Happened at the Winery?
// Needed evidence to progress to inside the Winery: window evidence, fence evidence, footsteps evidence


= intro
    ~ StartQuest(Level3_Quests)
    ~ SetSpeaker(Speaker.Misra)
    Here we are! Kettle Rock's pride and joy. Best wine north of California.
    ~ SetSpeaker(Speaker.Lupe)
    You said this place closed a few weeks ago? It looks like it's been abandoned for years.
    ~ SetSpeaker(Speaker.Misra)
    Well, it <i>has</i> been a bit neglected recently. But make no mistake, this is a piece of town history! 
    ~ SetSpeaker(Speaker.Lupe)
    Right... 
    ~ CompleteQuest(talk_to_misra_quest)
    ~ DiscoverMystery(1)
    -> DONE

= misra 
-> Misra_Dialogue.3_1    

= door_main_room
{outsideCluesFound():
    ~ SetSpeaker(Speaker.Lupe)
    In we go.
    ~ openDoor()
    ~ ChangeGameScene("scene3_2", 0, 0)
    -> DONE
- else:
    ~ SetSpeaker(Speaker.Lupe)
    This door must lead inside.
    -> DONE
}

= fence 
// Lupe walks over to the fence. A large tear had been ripped through the middle of the chain link fence. Just big enough for the average height of an adult.
{IsQuestComplete(talk_to_misra_quest):
    // ADD TO SYNTHESIS
    ~ SetSpeaker(Speaker.Lupe)
    Seems like the fence has been cut through. That gap looks just big enough for a person...
    ~ DiscoverClue(evidence_fence)
    -> DONE

- else:
    ~ SetSpeaker(Speaker.Lupe)
    It's a torn fence.
    -> DONE
}

= window
    // Lupe approaches a window on the front of the Winery that has been shattered into a million pieces on the ground. Puzzling...
    // ADD TO SYNTHESIS
    ~ DiscoverClue(evidence_broken_window)
    ~ SetSpeaker(Speaker.Misra)
    Hm. I'd say this is a pretty solid piece of evidence.
    ~ SetSpeaker(Speaker.Lupe)
    Yeah, it's definitely been broken by force. But something about this seems off to me...
    -> DONE


= strange_footsteps
// Lupe looks down at the ground and sees a series of strange footsteps that look humanoid...ish..
// ADD TO SYNTHESIS
{IsQuestComplete(talk_to_misra_quest):
    {IsClueFound(evidence_footsteps):
        ~ SetSpeaker(Speaker.Misra)
        Maybe they didn't want to leave shoe prints? -> DONE 
    
    -else:
        ~ DiscoverClue(evidence_footsteps)
        ~ SetSpeaker(Speaker.Lupe)
        ...What's this?
        ~ SetSpeaker(Speaker.Misra)
        Footprints at the scene of the crime. <i>Classic</i>.
        ~ SetSpeaker(Lupe)
        Who would be walking around barefoot? 
        -> DONE
    }
- else:
    ~ SetSpeaker(Speaker.Lupe)
    Those are some interesting footprints.
    -> DONE
}


===scene3_2===
TODO SFX door close
//Lupe and Misra enter the Winery. Inside is in shambles, clearly ransacked and clearly abandoned (large equipment and wine barrels still there). Dried, spilled wine is smeared on the floor and on some equipment.

// Note, only the main room is accessible during this time. The door that leads to the office area is closed and locked, located on the back wall.

* [misra] -> misra
* [wine barrels] -> wine_barrels
* [claw marks] -> claw_marks
* [damaged equipment] -> equipment
* [floor wine] -> floor_splatters
* [backroom door] -> door_office
* [winery front door] -> door_winery_exterior
* [handprint] -> handprint
* [inside_window] ->inside_window

= misra 
    {IsClueFound(evidence_claw_marks) && IsClueFound(evidence_damages):
        ~ SetSpeaker(Speaker.Misra)
        Thoughts?
        ~ SetSpeaker(Speaker.Lupe)
        It's obscure. Someone was definitely here, but I can't speak to motive. Who would want to break into an abandoned Winery?
        ~ SetSpeaker(Speaker.Misra)
        That sounds like it's time for some profiling! We need a suspect list.
        ~ SetSpeaker(Speaker.Lupe)
        Any leads on who it could be?
        ~ SetSpeaker(Speaker.Misra)
        Our best bet is questioning some locals. 
        ~ SetSpeaker(Speaker.Lupe)
        I agree.
        ~ SetSpeaker(Speaker.Misra)
        Copy that, Detective. Lead the way! 
        ~ SetSpeaker(Speaker.Lupe)
        I...don't know the way to Downtown.
        ~ SetSpeaker(Speaker.Misra)
        Oh, right! Yeah, of course you don't. 
        I can give you directions in the car.
        ~ CompleteQuest(discover_inside_clues)
        ~ openDoor()
        ~ PlaySFX("carStartAndLeave")
        ~ ChangeGameScene("scene4_1", 0, 0)
        -> DONE
        
    - else:
        ~ SetSpeaker(Speaker.Lupe)
        Wow. The inside is <i>so</i> much better than the outside.
        ~ SetSpeaker(Speaker.Misra)
        Never judge a book by it's cover! 
        ~ SetSpeaker(Speaker.Lupe)
        That was sarcasm, you know that, right?
        ~ SetSpeaker(Speaker.Misra)
        I do! The advice still applies.
        -> DONE
    }
    
= inside_window
    ~ SetSpeaker(Speaker.Lupe)
    Something about this broken window doesn't make sense to me...
    ~ DiscoverClue(evidence_broken_window)
    -> DONE

= handprint
    ~ SetSpeaker(Speaker.Lupe)
     Well, someone didn't care about leaving behind evidence.
    ~ SetSpeaker(Speaker.Misra)
     Could we use that for prints?
     ~ SetSpeaker(Speaker.Lupe)
     Do you have a lab here in Kettle Rock?
    ~ SetSpeaker(Speaker.Misra) 
    ...
    Erm, no.
     ~ SetSpeaker(Speaker.Lupe)
     Then, no.
     ~ DiscoverClue(evidence_handprint)
     -> DONE


= claw_marks
    ~ SetSpeaker(Speaker.Lupe)
    What could've left those?
    ~ SetSpeaker(Speaker.Misra)
    Some kind of animal, maybe? Maybe a raccoon is our culprit.
    ~ SetSpeaker(Speaker.Lupe)
    Those are some big marks.
    ~ SetSpeaker(Speaker.Misra)
    A...large...raccoon..?
    ~ DiscoverClue(evidence_claw_marks)
    -> DONE

= floor_splatters
    ~ SetSpeaker(Speaker.Misra)
    I know what you're thinking! Don't worry, it's not blood. I'm sure you've already deduced this, but it's dried up wine from when this place was used.
    -> DONE

= equipment
    ~ DiscoverClue(evidence_damages)
    // ADD TO SYNTHESIS
    ~ SetSpeaker(Speaker.Lupe)
    Some of this stuff looks like it's been ripped apart.
    -> DONE
    

= wine_barrels
    ~ SetSpeaker(Speaker.Misra)
    Back in the day, this place really did make good wine.
    ~ SetSpeaker(Speaker.Lupe)
    Is any of that wine left in there?
    ~ SetSpeaker(Speaker.Misra)
    Are you asking if I want to get a drink?
    ~ SetSpeaker(Speaker.Lupe)
    ...It would just be a motive, if anyone was looking to take some.
    ~ SetSpeaker(Speaker.Misra)
    Oh, no. They drained all of it when the Winery closed.
    ~ SetSpeaker(Speaker.Lupe)
    I don't drink when I'm on a case.
    ~ SetSpeaker(Speaker.Misra)
    I know, I know. Professional, as always.
    ~ DiscoverClue(evidence_claw_marks)
    -> DONE

= door_office
    ~ SetSpeaker(Speaker.Lupe)
    Where does this lead?
    ~ SetSpeaker(Speaker.Misra)
    To one of the backrooms. It was locked when the Winery shut down, and is still locked. 
    ~ SetSpeaker(Speaker.Lupe)
    So they wouldn't have been able to access that room?
    ~ SetSpeaker(Speaker.Misra)
    Not unless they had a key.
    //DON'T GO INTO OFFICE
    -> DONE

= door_winery_exterior
    {IsClueFound(evidence_claw_marks) && IsClueFound(evidence_damages):
        -> misra
        -> DONE
    - else:
        ~ SetSpeaker(Speaker.Misra)
        We probably shouldn't leave just yet! More investigating to do!
        -> DONE
    }