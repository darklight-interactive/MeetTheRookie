
=== Misra_Dialogue ===
#NPC tag
-> DONE

= default
Woah .. something scary is going on
I think you - you didnt set a proper stitch?
Maybe you did it on purpose
Idk to each their own , just take it away now. -> DONE

= 3_1
{IsClueFound(evidence_fence) && IsClueFound(evidence_broken_window) && IsClueFound(evidence_footsteps):
    ~ SetSpeaker(Speaker.Misra)
    Well, what do you make of it, Detective?
    ~ SetSpeaker(Speaker.Lupe)
    It all points to an intruder, yeah. Whoever made the noise complaint probably heard something legit.
    ~ SetSpeaker(Speaker.Misra)
    The plot thickens!! Shall we check inside? 
    ~ CompleteQuest(discover_outside_clues)
    -> DONE
- else:
~ SetSpeaker(Speaker.Misra)
{Find anything, Detective? | What are you looking at me for? I thought you were the expert. | ... | yes? | Okaayyy... | ...}
    -> DONE
}


