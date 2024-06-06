=== scene4_4 ===
// FUNC SCENE CHANGE
# Location: The Rockin Kettle, Bar 
# Only accessible after Gen Store and Arcade have been visited

+ [Irene] -> irene_text
+ {IsQuestComplete(irene_intro)} [memorial_plaque] -> memorial_plaque
+ [slumped_man] -> man
+ {irene_intro}[goop] -> goop
+ [bleach_bottle] -> DONE

= intro
    -> Irene_Dialogue.irene_intro_cutscene
= irene_text
    -> Irene_Dialogue
= man 
    -> Jenkins_Dialogue


= goop
{IsQuestComplete(gooptalk):
    ~ SetSpeaker(Speaker.Lupe)
    I know better than to touch that stuff. 
    -> DONE

-else:
    ~ SetSpeaker(Speaker.Lupe)
     You've been getting this stuff too, huh?
    ~ SetSpeaker(Speaker.Irene)
     Oh, yeah.
     No idea what it is.
     Maybe like some kinda fungus?
     Or like, mold?
     It's been coming up out of the drains like crazy.
    ~ SetSpeaker(Speaker.Misra)
    You don't sound very concerned.
    ~ SetSpeaker(Speaker.Irene)
     Oh, nah.
     It's annyoing, but it comes off easy enough.
    ~ SetSpeaker(Speaker.Lupe)
     It does?
    ~ SetSpeaker(Speaker.Irene)
     Sure. 
     I just dump all the bleach I've got in the backroom on it and poof!
     Melts away.
     ~ CompleteQuest(gooptalk)
    // Add to Synthesis - The Town of KR
    -> DONE
}
    

= memorial_plaque
# Lupe leans close to a memorial plaque on the wall. It reads "In Memory of those lost in the Tragedy of 1940. Rest in Peace, Never Forgotten." Over the corner of it, someone has sharpied over it with "THOSE STUPID GOATS!"
    TODO implement expandable window
~ CompleteQuest(memorial_plaque_visited)
    // Add to Synthesis - The Town of KR
-> DONE


