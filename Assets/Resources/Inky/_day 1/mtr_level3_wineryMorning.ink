// SCENE 3: The Winery
// --- scene 3 ---
LIST Clues_3 = (EVIDENCE_BROKEN_WINDOW) , (EVIDENCE_FOOTSTEPS), (EVIDENCE_FENCE), (EVIDENCE_CLAW_MARKS), (EVIDENCE_DAMAGES)
VAR knowledgeState_1_3 = ()

LIST QuestChain_3 = FIRST_INTERACT_2, VISIT_FENCE, VISIT_WINDOW, DISCUSS_MISRA, VISIT_FOOTSTEPS, VISIT_FLOOR, VISIT_BARRELS, VISIT_BACKROOM_DOOR
VAR activeQuests_1_3 = (FIRST_INTERACT_2)
VAR completedQuests_1_3 = ()

=== level3 ===
Level3
-> scene3_1

=== scene3_1 ===
// location : The Winery, outside

// Lupe and Misra arrive at the Winery. For a place having just closed a short time ago, it looks pretty run down. The whole building is in obvious disrepair with loose wood on the ground and vines creeping up the sides of the walls. Kind of creepy.
~ SetActiveQuestChain(QuestChain_3)
-> DONE

= investigate

// QUEST OBJECTIVE: What Happened at the Winery?
// Needed evidence to progress to inside the Winery: window evidence, fence evidence, footsteps evidence

{IsQuestComplete(completedQuests_1_3, FIRST_INTERACT):
    *[fence] -> DONE
    *[window] -> DONE
    + {evidence_discussed_Misra}[front_door] -> scene3_2
}

*[Misra] 
    -> DONE


*{IsClueFound(Clues_1_3, completedQuests_1_3, EVIDENCE_FENCE) && IsClueFound(Clues_1_3, completedQuests_1_3, EVIDENCE_BROKEN_WINDOW) && IsClueFound(Clues_1_3, completedQuests_1_3, EVIDENCE_FOOTSTEPS)}[Misra] ->DONE



= evidence_discussed_Misra
    ~ set_speaker(Speaker.Misra)
    Well, what do you make of it, Detective?
    
        * It all points to an intruder, yeah. Whoever made the noise complaint probably heard something legit.
    
    The plot thickens!! Shall we check inside? -> DONE
        


=Misra_
  ~ set_speaker(Speaker.Misra)
  Here we are! Kettle Rock's pride and joy. Best wine north of California.
   + You said this place closed a few weeks ago? It looks like it's been abandoned for years. -> DONE
   

=Misra_dialogue
     ~ CompleteQuest(activeQuests_1_3, completedQuests_1_3, FIRST_INTERACT)
     ~ set_speaker(Speaker.Misra)
    Well, it <i>has</i> been a bit neglected recently. But make no mistake, this is a piece of town history! 
    ~ set_speaker(Speaker.Lupe)
    Right... -> DONE
        
=fence
// Lupe walks over to the fence. A large tear had been ripped through the middle of the chain link fence. Just big enough for the average height of an adult.

// ADD TO SYNTHESIS
~ set_speaker(Speaker.Lupe)
Seems like the fence has been cut through. That gap looks just big enough for a person...
~ CompleteQuest(activeQuests_1_3, completedQuests_1_3, VISIT_FENCE)
~ DiscoverClue(knowledgeState_1_3, EVIDENCE_FENCE)

-> DONE

=window
// Lupe approaches a window on the front of the Winery that has been shattered into a million pieces on the ground. Puzzling...
// ADD TO SYNTHESIS
~ DiscoverClue(knowledgeState_1_3, EVIDENCE_BROKEN_WINDOW)
~ set_speaker(Speaker.Misra)
Hm. I'd say that's a pretty solid piece of evidence.
~ set_speaker(Speaker.Lupe)
Yeah, it's definitely been broken. But something about this seems off to me...
    
     + [footsteps]-> DONE

=strange_footsteps
// Lupe looks down at the ground and sees a series of strange footsteps that look humanoid...ish..
// ADD TO SYNTHESIS
{IsQuestComplete(completedQuests_1_3, VISIT_FOOTSTEPS):
    ~ set_speaker(Speaker.Misra)
    Maybe they didn't want to leave shoe prints? -> DONE 

-else:
    ~ CompleteQuest(activeQuests_1_3, completedQuests_1_3, VISIT_FOOTSTEPS)
    ~ DiscoverClue(knowledgeState_1_3, EVIDENCE_FOOTSTEPS)
    ...what's this?
    ~ set_speaker(Speaker.Misra)
    Footprints at the scene of the crime. <i>Classic</i>.
    * Who would be walking around barefoot? -> DONE
}


===scene3_2===
//Lupe and Misra enter the Winery. Inside is in shambles, clearly ransacked and clearly abandoned (large equipment and wine barrels still there). Dried, spilled wine is smeared on the floor and on some equipment.

// Note, only the main room is accessible during this time. The door that leads to the office area is closed and locked, located on the back wall.

* Wow. The inside is <i>so</i> much better than the outside. -> DONE

=intro_conversation
~ set_speaker(Speaker.Misra)
Never judge a book by it's cover! 
* That was sarcasm, you know that, right?
~ set_speaker(Speaker.Misra)
I do! The advice still applies.

-> DONE

=investigate_inside
// CLUES NEEDED TO PROGRESS: CLAW MARKS AND DAMAGES

* [claw_marks] -> DONE

* [floor_splatters] -> DONE

* [additional_damages] -> DONE

* [wine_barrels] -> DONE

* [backroom_door] -> DONE

*{IsClueFound(Clues_1_3, completedQuests_1_3, EVIDENCE_CLAW_MARKS) && IsClueFound(Clues_1_3, completedQuests_1_3, EVIDENCE_DAMAGES)}[Misra] ->DONE

// ????
*{closing_discussion}[Winery_front_door] -> DONE

= evidence_discussed_Misra_inside
~ set_speaker(Speaker.Misra)
Thoughts?
~ set_speaker(Speaker.Lupe)
It's obscure. Someone was definitely here, but I can't speak to motive. Who would want to break into an abandoned Winery?
~ set_speaker(Speaker.Misra)
That sounds like it's time for some profiling! We need a suspect list.
~ set_speaker(Speaker.Lupe)
Any leads on who it could be?
~ set_speaker(Speaker.Misra)
Our best bet is questioning some locals. 
~ set_speaker(Speaker.Lupe)
I agree.


-> DONE

=closing_discussion
// PREVIOUS OBJECTIVE FULFILLED: WHAT HAPPENED AT THE Winery
    // ANSWER: AN INTRUDER BROKE INTO THE WINERY
// OBJECTIVE CHANGES TO: WHO WOULD BREAK INTO THE WINERY?
    {IsQuestComplete(completedQuests_1_3, VISIT_FLOOR) && IsQuestComplete(completedQuests_1_3, VISIT_BARRELS) && IsQuestComplete(completedQuests_1_3, VISIT_BACKROOM_DOOR):
        ~ set_speaker(Speaker.Misra)
        Copy that, Detective. Lead the way! 
           *I...don't know the way to Downtown. -> DONE
           
      - else:
         ~ set_speaker(Speaker.Misra)
         Copy that, Detective! We'll head out, unless you have anything else you want to look at. -> DONE
}

=closing_discussion_2
~ set_speaker(Speaker.Misra)
Oh, right! Yeah, of course you don't. Follow me. -> DONE


=claw_marks
// ADD TO SYNTHESIS
~ set_speaker(Speaker.Lupe)
What could've left those?
~ set_speaker(Speaker.Misra)
Some kind of animal, maybe? Maybe a raccoon is our culprit.
~ set_speaker(Speaker.Lupe)
Those are some big marks.
~ set_speaker(Speaker.Misra)
A...large...raccoon..?
~ DiscoverClue(knowledgeState_1_3, EVIDENCE_CLAW_MARKS)

-> DONE

=floor_splatters
~ set_speaker(Speaker.Misra)
I know what you're thinking! Don't worry, it's not blood. I'm sure you've already deduced this, but it's dried up wine from when this place was used.
~ CompleteQuest(activeQuests_1_3, completedQuests_1_3, VISIT_FLOOR)
-> DONE

=additional_damages
~ DiscoverClue(knowledgeState_1_3, EVIDENCE_DAMAGES)
// ADD TO SYNTHESIS
~ set_speaker(Speaker.Lupe)
Some of this stuff looks like it's been ripped apart. -> DONE


=wine_barrels
~ CompleteQuest(activeQuests_1_3, completedQuests_1_3, VISIT_BARRELS)
~ set_speaker(Speaker.Misra)
Back in the day, this place really did make good wine.

* Is any of that wine left in there? ->DONE

=wine_barrels2
~ set_speaker(Speaker.Misra)
Are you asking if I want to get a drink?
~ set_speaker(Speaker.Lupe)
...It would just be a motive, if anyone was looking to take some.
~ set_speaker(Speaker.Misra)
Oh, no. They drained all of it when the Winery closed.
~ set_speaker(Speaker.Lupe)
I don't drink when I'm on a case.
~ set_speaker(Speaker.Misra)
I know, I know. Professional, as always. -> DONE

=backroom_door
~ CompleteQuest(activeQuests_1_3, completedQuests_1_3, VISIT_BACKROOM_DOOR)
~ set_speaker(Speaker.Lupe)
Where does that lead?
~ set_speaker(Speaker.Misra)
To one of the backrooms. It was locked when the Winery shut down, and is still locked. 
~ set_speaker(Speaker.Lupe)
So they wouldn't have been able to access that room?
~ set_speaker(Speaker.Misra)
Not unless they had a key. -> DONE

=Winery_front_door
// Clicking on the door fades to black, and we hear the door open and close, and a car starting
->DONE