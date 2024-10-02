
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
~ SetSpeaker(Speaker.Misra)
{Find anything, Detective? | What are you looking at me for? I thought you were the expert. | ... | yes? | Okaayyy... | ...}
- else:
    -> DONE
}

= 3_2
{IsClueFound(evidence_claw_marks) && IsClueFound(evidence_damages):
    ~ SetSpeaker(Speaker.Misra)
    Thoughts?
    ~ SetSpeaker(Speaker.Lupe)
    It's...obscure. 
    Someone was definitely here, but I can't speak to motive. Who would want to break into an abandoned Winery?
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
    TODO SFX once fade to black, car leave noises
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


= 4_1
* Any updates regarding the blocked road? -> updates
* Jeez, where is everyone? -> where_is_everyone
* {IsQuestComplete(jenny_suspicion) or IsQuestComplete(calvin_suspicion) or IsQuestComplete(josh_suspicion)} The teens mentioned someone named Jenkins? -> someone_named_jenk

= someone_named_jenk
~ SetSpeaker(Speaker.Lupe)
Oh, yeah, Jenkins.
He's a local, too.
I really don't know him that well.
Roy used to tell me to steer clear.
Said he was "trouble with legs".

~SetSpeaker(Speaker.Lupe)
Interesting...
Could be worth asking about.
-> DONE

= updates
~ SetSpeaker(Speaker.Misra)
None yet! 
But I assure you, it's my second most urgent thing to take care of.
~ SetSpeaker(Speaker.Lupe)
...
The first most being this case?

~ SetSpeaker(Speaker.Misra)
Yup! 

~ SetSpeaker(Speaker.Lupe)
Sheriff Misra...
Trust me, I of all people can understand putting your work first.
But I also need to get back on the road, and back to <i>my case</i> and my life.
Is there any way you can bump the priority on that?

~SetSpeaker(Speaker.Misra)
...
Right.
I know you want to get back to your own life.
I'm sorry you're stuck here, really.
...
I'll make sure they're focused on clearing the tree.

~SetSpeaker(Speaker.Lupe)
Thank you, Sheriff.

~SetSpeaker(Speaker.Misra)
You're welcome. 
It's uh, the least I can try to do.
And really,
call me Misra.

~SetSpeaker(Speaker.Lupe)
Thank you, Misra.
-> DONE


= where_is_everyone
~SetSpeaker(Speaker.Lupe)
This place is practically a ghost town.

~ SetSpeaker(Speaker.Misra)
Yeah, it's a bit of a slow day!

~SetSpeaker(Speaker.Lupe)
Right...
-> DONE


//Where to next, Detective?

