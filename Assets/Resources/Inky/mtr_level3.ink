// SCENE 3: The Winery

// ====== SPEAKER ======
LIST Speaker = Lupe, Misra
VAR curSpeaker = Speaker.Lupe
== function set_speaker(value)
    ~ curSpeaker = value

// ====== QUEST CHAIN =======
LIST QuestChain = FIRST_INTERACT, VISIT_FENCE, VISIT_WINDOW, DISCUSS_MISRA, VISIT_FOOTSTEPS, VISIT_FLOOR, VISIT_BARRELS, VISIT_BACKROOM_DOOR
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
LIST Clues = (EVIDENCE_BROKEN_WINDOW) , (EVIDENCE_FOOTSTEPS), (EVIDENCE_FENCE), (EVIDENCE_CLAW_MARKS), (EVIDENCE_DAMAGES)
VAR knowledgeState = ()
=== function DiscoverClue(clue)
    ~ knowledgeState += clue
    
=== function IsClueFound(clue)
    { LIST_ALL(Clues) ? clue:
        // clues are either found, or not
        ~ return knowledgeState ? clue
    }

// ===== SCENE HANDLING =====
VAR currentScene = -> scene3_1

=== GoToSubScene(int) ===
    {int:
        - 1: -> scene3_1
        - 2: -> scene3_2

    }


=== scene3_1 ===
// location : The Winery, outside

// Lupe and Misra arrive at the Winery. For a place having just closed a short time ago, it looks pretty run down. The whole building is in obvious disrepair with loose wood on the ground and vines creeping up the sides of the walls. Kind of creepy.
~ StartQuest(FIRST_INTERACT)
~ StartQuest(DISCUSS_MISRA)
-> DONE

= investigate

// QUEST OBJECTIVE: What Happened at the Winery?
// Needed evidence to progress to inside the Winery: window evidence, fence evidence, footsteps evidence

{IsQuestComplete(FIRST_INTERACT):
    *[fence] -> DONE
    *[window] -> DONE
    + {evidence_discussed_Misra}[front_door] -> scene3_2
}

*[Misra] 
    -> DONE


*{IsClueFound(EVIDENCE_FENCE) && IsClueFound(EVIDENCE_BROKEN_WINDOW) && IsClueFound(EVIDENCE_FOOTSTEPS)}[Misra] ->DONE



= evidence_discussed_Misra
    [Misra] Well, what do you make of it, Detective?
    
        * It all points to an intruder, yeah. Whoever made the noise complaint probably heard something legit.
        
    [Misra] The plot thickens!! Shall we check inside? -> DONE
        


=Misra_
  [Misra] Here we are! Kettle Rock's pride and joy. Best wine north of California.
   + You said this place closed a few weeks ago? It looks like it's been abandoned for years. -> DONE
   

=Misra_dialogue
     ~ CompleteQuest(FIRST_INTERACT)
    [Misra] Well, it <i>has</i> been a bit neglected recently. But make no mistake, this is a piece of town history! 
    
        [Lupe] Right... -> DONE
        
=fence
// Lupe walks over to the fence. A large tear had been ripped through the middle of the chain link fence. Just big enough for the average height of an adult.

// ADD TO SYNTHESIS
 Seems like the fence has been cut through. That gap looks just big enough for a person...
~ CompleteQuest(VISIT_FENCE)
~ DiscoverClue(EVIDENCE_FENCE)

-> DONE

=window
// Lupe approaches a window on the front of the Winery that has been shattered into a million pieces on the ground. Puzzling...
// ADD TO SYNTHESIS
~ DiscoverClue(EVIDENCE_BROKEN_WINDOW)
[Misra] Hm. I'd say that's a pretty solid piece of evidence.

    Yeah, it's definitely been broken. But something about this seems off to me...
    
     + [footsteps]-> DONE

=strange_footsteps
// Lupe looks down at the ground and sees a series of strange footsteps that look humanoid...ish..
// ADD TO SYNTHESIS
{IsQuestComplete(VISIT_FOOTSTEPS):
    [Misra] Maybe they didn't want to leave shoe prints? -> DONE 

-else:
    ~ CompleteQuest(VISIT_FOOTSTEPS)
    ~ DiscoverClue(EVIDENCE_FOOTSTEPS)
    ...what's this?
    [Misra] Footprints at the scene of the crime. <i>Classic</i>.
    * Who would be walking around barefoot? -> DONE
}


===scene3_2===
//Lupe and Misra enter the Winery. Inside is in shambles, clearly ransacked and clearly abandoned (large equipment and wine barrels still there). Dried, spilled wine is smeared on the floor and on some equipment.

// Note, only the main room is accessible during this time. The door that leads to the office area is closed and locked, located on the back wall.

* Wow. The inside is <i>so</i> much better than the outside. -> DONE

=intro_conversation

[Misra] Never judge a book by it's cover! 
* That was sarcasm, you know that, right?

[Misra] I do! The advice still applies.

-> DONE

=investigate_inside
// CLUES NEEDED TO PROGRESS: CLAW MARKS AND DAMAGES

* [claw_marks] -> DONE

* [floor_splatters] -> DONE

* [additional_damages] -> DONE

* [wine_barrels] -> DONE

* [backroom_door] -> DONE

*{IsClueFound(EVIDENCE_CLAW_MARKS) && IsClueFound(EVIDENCE_DAMAGES)}[Misra] ->DONE

// ????
*{closing_discussion}[Winery_front_door] -> DONE

= evidence_discussed_Misra_inside
[Misra] Thoughts?

It's obscure. Someone was definitely here, but I can't speak to motive. Who would want to break into an abandoned Winery?

[Misra] That sounds like it's time for some profiling! We need a suspect list.

Any leads on who it could be?

[Misra] Our best bet is questioning some locals. 

I agree.


-> DONE

=closing_discussion
// PREVIOUS OBJECTIVE FULFILLED: WHAT HAPPENED AT THE Winery
    // ANSWER: AN INTRUDER BROKE INTO THE WINERY
// OBJECTIVE CHANGES TO: WHO WOULD BREAK INTO THE WINERY?
    {IsQuestComplete(VISIT_FLOOR) && IsQuestComplete(VISIT_BARRELS) && IsQuestComplete(VISIT_BACKROOM_DOOR):
        [Misra] Copy that, Detective. Lead the way! 
           *I...don't know the way to Downtown. -> DONE
           
      - else:
         [Misra] Copy that, Detective! We'll head out, unless you have anything else you want to look at. -> DONE
}

=closing_discussion_2
[Misra] Oh, right! Yeah, of course you don't. Follow me. -> DONE


=claw_marks
// ADD TO SYNTHESIS
What could've left those?

[Misra] Some kind of animal, maybe? Maybe a raccoon is our culprit.

Those are some big marks.

[Misra] A...large...raccoon..?
~ DiscoverClue(EVIDENCE_CLAW_MARKS)

-> DONE

=floor_splatters
[Misra] I know what you're thinking! Don't worry, it's not blood. I'm sure you've already deduced this, but it's dried up wine from when this place was used.
~ CompleteQuest(VISIT_FLOOR)
-> DONE

=additional_damages
~ DiscoverClue(EVIDENCE_DAMAGES)
// ADD TO SYNTHESIS
Some of this stuff looks like it's been ripped apart. -> DONE


=wine_barrels
~ CompleteQuest(VISIT_BARRELS)
[Misra] Back in the day, this place really did make good wine.

*Is any of that wine left in there? ->DONE

=wine_barrels2
[Misra] Are you asking if I want to get a drink?

...it would just be a motive, if anyone was looking to take some.

[Misra] Oh, no. They drained all of it when the Winery closed.

I don't drink when I'm on a case.

[Misra] I know, I know. Professional, as always. -> DONE

=backroom_door
~ CompleteQuest(VISIT_BACKROOM_DOOR)

Where does that lead?

[Misra] To one of the backrooms. It was locked when the Winery shut down, and is still locked. 

So they wouldn't have been able to access that room?

[Misra] Not unless they had a key. -> DONE

=Winery_front_door
// Clicking on the door fades to black, and we hear the door open and close, and a car starting
->DONE