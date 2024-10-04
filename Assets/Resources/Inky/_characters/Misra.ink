
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

= 3_2
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
~ SetSpeaker(Speaker.Misra)
* Any updates on that fallen tree? -> updates
* Jeez, this place is like a ghost town. -> ghosttown
* {IsQuestComplete(jenny_suspicion) or IsQuestComplete(calvin_suspicion) or IsQuestComplete(josh_suspicion)}The teens mentioned someone named Jenkins? -> teens_jenkins

= teens_jenkins
~SetSpeaker(Speaker.Misra)
Oh, yeah, Jenkins.
He's a local, too.
I really don't know him that well.
Roy used to tell me to steer clear--
said Jenkin was "troubled".
~SetSpeaker(Speaker.Lupe)
Interesting...
Maybe it's worth asking about.
-> DONE 

= updates
~SetSpeaker(Speaker.Misra)
Not yet!
I assue you though, it's the second most urgent thing to take care of on my list!
~SetSpeaker(Speaker.Lupe)
...
The first most being this case?
~SetSpeaker(Speaker.Misra)
Yup!
~SetSpeaker(Speaker.Lupe)
Sheriff Misra...
Trust me, I of all people can understand putting your work first.
But I also really need to get back on the road, and get back to <i>my</i> work, and my life.
Is there any way you could bump the priorty on that?
~SetSpeaker(Speaker.Misra)
...
Right.
I know you want to get back to your own life.
I'm sorry you're stuck here with me, really.
...
I'll make sure they're focused on clearing the tree.
~SetSpeaker(Speaker.Lupe)
Thank you, Sheriff.
~SetSpeaker(Speaker.Misra)
You're welcome.
And really, call me Misra.
We've known each other long enough to drop the title.
~SetSpeaker(Speaker.Lupe)
Well, three hours isn't <i>that</i> long, but...
Thank you, Misra.
-> DONE

= ghosttown
~SetSpeaker(Speaker.Lupe)
I have to admit, it's going to be hard to do questioning with so few people around.
This place is practically a ghost town.
~SetSpeaker(Speaker.Misra)
It's just a bit of a slow day! 
There should be a few people around.
    -> DONE

