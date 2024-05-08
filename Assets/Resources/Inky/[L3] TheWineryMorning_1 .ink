// SCENE 3: The Winery

// VARIABLES for 3_1
VAR evidence_broken_window = false
VAR evidence_footsteps = false
VAR evidence_fence = false

VAR first_interact = false


VAR fence_visited = false
VAR window_visited = false
VAR footsteps_visited = false


// VARIABLES for 3_2
VAR evidence_claw_marks = false
VAR evidence_damages = false

VAR floor_visited = false
VAR barrels_visited = false
VAR backroom_door_visited = false


* [BEGIN] -> scene3_1

=== scene3_1 ===
# location : The Winery, outside

# Lupe and Misra arrive at the Winery. For a place having just closed a short time ago, it looks pretty run down. The whole building is in obvious disrepair with loose wood on the ground and vines creeping up the sides of the walls. Kind of creepy.

-> investigate

= investigate

# QUEST OBJECTIVE: What Happened at the Winery?
# Needed evidence to progress to inside the Winery: window evidence, fence evidence, footsteps evidence

{first_interact:
    *[fence] -> fence
    *[window] -> window
    + {evidence_discussed_Misra}[front_door] -> scene3_2
}

*[Misra] 
    -> Misra

    
*{evidence_fence && evidence_broken_window && evidence_footsteps}[Misra] ->evidence_discussed_Misra



= evidence_discussed_Misra
    [Misra] Well, what do you make of it, Detective?
    
        * It all points to an intruder, yeah. Whoever made the noise complaint probably heard something legit.
        
    [Misra] The plot thickens!! Shall we check inside? -> investigate
        


=Misra
  [Misra] Here we are! Kettle Rock's pride and joy. Best wine north of California.
   + You said this place closed a few weeks ago? It looks like it's been abandoned for years. -> Misra_dialogue
   

=Misra_dialogue
     ~ first_interact = true
    [Misra] Well, it <i>has</i> been a bit neglected recently. But make no mistake, this is a piece of town history! 
    
        [Lupe] Right... -> investigate
        
=fence
# Lupe walks over to the fence. A large tear had been ripped through the middle of the chain link fence. Just big enough for the average height of an adult.

# ADD TO SYNTHESIS
 Seems like the fence has been cut through. That gap looks just big enough for a person...
~ fence_visited = true
~ evidence_fence = true
-> investigate

=window
# Lupe approaches a window on the front of the Winery that has been shattered into a million pieces on the ground. Puzzling...
# ADD TO SYNTHESIS
~ evidence_broken_window = true
[Misra] Hm. I'd say that's a pretty solid piece of evidence.

    Yeah, it's definitely been broken. But something about this seems off to me...
    
     + [footsteps]-> strange_footsteps

=strange_footsteps
# Lupe looks down at the ground and sees a series of strange footsteps that look humanoid...ish..
# ADD TO SYNTHESIS
{footsteps_visited:
    [Misra] Maybe they didn't want to leave shoe prints? -> investigate 

-else:
    ~footsteps_visited = true
    ~ evidence_footsteps = true
    ...what's this?
    [Misra] Footprints at the scene of the crime. <i>Classic</i>.
    * Who would be walking around barefoot? -> strange_footsteps
}


===scene3_2===
#Lupe and Misra enter the Winery. Inside is in shambles, clearly ransacked and clearly abandoned (large equipment and wine barrels still there). Dried, spilled wine is smeared on the floor and on some equipment.

# Note, only the main room is accessible during this time. The door that leads to the office area is closed and locked, located on the back wall.

* Wow. The inside is <i>so</i> much better than the outside. ->intro_conversation

=intro_conversation

[Misra] Never judge a book by it's cover! 
* That was sarcasm, you know that, right?

[Misra] I do! The advice still applies.

-> investigate_inside

=investigate_inside
# CLUES NEEDED TO PROGRESS: CLAW MARKS AND DAMAGES

* [claw_marks] -> claw_marks

* [floor_splatters] -> floor_splatters

* [additional_damages] -> additional_damages

* [wine_barrels] -> wine_barrels

* [backroom_door] -> backroom_door

*{evidence_claw_marks && evidence_damages}[Misra] ->evidence_discussed_Misra_inside

*{closing_discussion}[Winery_front_door] -> Winery_front_door

= evidence_discussed_Misra_inside
[Misra] Thoughts?

It's obscure. Someone was definitely here, but I can't speak to motive. Who would want to break into an abandoned Winery?

[Misra] That sounds like it's time for some profiling! We need a suspect list.

Any leads on who it could be?

[Misra] Our best bet is questioning some locals. 

I agree.


-> closing_discussion

=closing_discussion
# PREVIOUS OBJECTIVE FULFILLED: WHAT HAPPENED AT THE Winery
    # ANSWER: AN INTRUDER BROKE INTO THE WINERY
# OBJECTIVE CHANGES TO: WHO WOULD BREAK INTO THE WINERY?
    {floor_visited && barrels_visited && backroom_door:
        [Misra] Copy that, Detective. Lead the way! 
           *I...don't know the way to Downtown. -> closing_discussion_2
           
      - else:
         [Misra] Copy that, Detective! We'll head out, unless you have anything else you want to look at. ->investigate_inside
}

=closing_discussion_2
[Misra] Oh, right! Yeah, of course you don't. Follow me. -> Winery_front_door


=claw_marks
# ADD TO SYNTHESIS
What could've left those?

[Misra] Some kind of animal, maybe? Maybe a raccoon is our culprit.

Those are some big marks.

[Misra] A...large...raccoon..?
~ evidence_claw_marks = true

-> investigate_inside

=floor_splatters
[Misra] I know what you're thinking! Don't worry, it's not blood. I'm sure you've already deduced this, but it's dried up wine from when this place was used.
~floor_visited = true
-> investigate_inside

=additional_damages
~ evidence_damages = true
# ADD TO SYNTHESIS
Some of this stuff looks like it's been ripped apart. -> investigate_inside


=wine_barrels
~ barrels_visited = true
[Misra] Back in the day, this place really did make good wine.

*Is any of that wine left in there? ->wine_barrels2

=wine_barrels2
[Misra] Are you asking if I want to get a drink?

...it would just be a motive, if anyone was looking to take some.

[Misra] Oh, no. They drained all of it when the Winery closed.

I don't drink when I'm on a case.

[Misra] I know, I know. Professional, as always. -> investigate_inside

=backroom_door
~ backroom_door_visited = true
Where does that lead?

[Misra] To one of the backrooms. It was locked when the Winery shut down, and is still locked. 

So they wouldn't have been able to access that room?

[Misra] Not unless they had a key. -> investigate_inside

=Winery_front_door
# Clicking on the door fades to black, and we hear the door open and close, and a car starting
->DONE