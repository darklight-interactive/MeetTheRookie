

=== Irene_intro ===
~ current_npc = "[Irene]"
~ set_speaker(Speaker.Lupe)
[Lupe] Detective Lupe.
~ set_speaker(Speaker.Irene)
{current_npc} Ooh, a <i>Detective</i>.
{current_npc} Like Sherlock Holmes.
{current_npc} Or Nancy Drew.
{current_npc} Or Scooby-Doo.
~ set_speaker(Speaker.Lupe)
[Lupe] ...
[Lupe] Yes.
[Lupe] Like Scooby-Doo.
~ set_speaker(Speaker.Irene)
{current_npc} A Detective, in my bar!
{current_npc} Feel free to look around.
{current_npc} Let me know if you need anything.
~ CompleteQuest(irene_intro)
-> DONE



=== irene_questions ===
~ current_npc = "[Irene]"
{current_npc} {IsQuestComplete(irene_convo_1) && IsQuestComplete(irene_convo_2)}):
     {What's up? | Whatchya need? | Anything else? | Whatchya need? }
     -> DONE
    - else: 


* {IsClueFound(goats_mentioned)} People have been mentioning goats a lot.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] I get the feeling they're not talking about the animal. 
    ~ set_speaker(Speaker.Irene)
    {current_npc} Oh.
    {current_npc} No, they're not.
    {current_npc} Misra, you haven't filled them in on this?
    ~ set_speaker(Speaker.Misra)
    [Misra] ...
    [Misra] Like you said, it's disrespectful.
    [Misra] I don't like talking about it.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] If it's important to the case, I should know.
    ~ set_speaker(Speaker.Misra)
    [Misra]...
    ~ set_speaker(Speaker.Irene)
    {current_npc} It's what they call them.
    {current_npc} The people who disappeared in the tragedy. 
    {current_npc} "Goats".
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Why?
    ~ set_speaker(Speaker.Misra)
    [Misra] Irene...
    ~ set_speaker(Speaker.Irene)
    {current_npc} Come on, Misra.
    {current_npc} It's just silly town folklore meant to scare kids and give people a reason to gossip.
    {current_npc} They call them "Goats" because people think they sacrificied themselves.
    {current_npc} Like, you know, a sacrificial lamb. 
    {current_npc} But less cute. 
    {current_npc} So, goat.
    ~ DiscoverClue(sacrifice_mentioned)
    // Add to Synthesis: The Town of KR
    -> DONE

* {memorial_plaque_visited} Someone's drawn on your memorial plaque over there.
    ~ set_speaker(Speaker.Irene)
    {current_npc} Damn it! 
    {current_npc} People keep doing that.
    {current_npc} It's disrespectful. 
    {current_npc} I'll clean it off when I close up.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] It says something about "goats"?
    ~ set_speaker(Speaker.Irene)
    {current_npc} Yeah, that's usually the message...
    ~ DiscoverClue(goats_mentioned)
    -> DONE
    
* {sacrifice_mentioned} Sorry, a sacrifice?
    ~ set_speaker(Speaker.Misra)
    [Misra] It's nothing.
    [Misra] Like Irene said, stupid gossip.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] I'm intrigued.
    ~ set_speaker(Speaker.Irene)
    {current_npc} Of course!
    {current_npc} Who better to be interested in a small town mystery than a Detective?
    ~ set_speaker(Speaker.Misra)
    [Misra] That's not the case we're working on, though.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Humor me.
    ~ set_speaker(Speaker.Irene)
    {current_npc} I'm sure Misra's told you about Kettle Rock's <i> rocky </i> past?
    ~ set_speaker(Speaker.Lupe)
    [Lupe] That part, yes.
    ~ set_speaker(Speaker.Irene)
    {current_npc} Well, legend says that in order to save the town, in desperation, a group of locals turned to darker alternatives.
    {current_npc} I don't know if they were dancin' with the Devil,
    {current_npc} or one of the many demons that want to sink their fangs into humanity's naviety,
    {current_npc} but they were in kahoots with something of that nature.
    {current_npc} And one day in 1940, when it came time to pay up and face the music...
    {current_npc} They <i>vanished</i>.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] ...
    ~ set_speaker(Speaker.Misra)
    [Misra] ...
    ~ set_speaker(Speaker.Irene)
    {current_npc} SpoOooOooOky.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] I feel like you've given that speech before
    ~ set_speaker(Speaker.Irene)
    {current_npc} Hey, I gotta keep the drunks entertained somehow.
    ~ CompleteQuest(irene_convo_1)
     // Add to Synthesis: The Town of KR
    -> DONE

* How long have you lived in Kettle Rock?
    ~ set_speaker(Speaker.Irene)
    {current_npc} I moved here when I was 13!
    {current_npc} I actually when to highschool with Misra.
    ~ set_speaker(Speaker.Misra)
    [Misra] Class of '86!
    ~ set_speaker(Speaker.Irene)
    {current_npc} That's right!
    {current_npc} Misra's a total sweetheart. 
    {current_npc} But I'm sure you know that already.
    ~ DiscoverClue(KR_irene)
    -> DONE

*{KR_irene && closed_shops_irene} What do you know about the Old Winery?
    ~ set_speaker(Speaker.Irene)
    {current_npc} Sheesh, that old shack on the hill?
    {current_npc} More trouble than it's worth, in my opinion.
    {current_npc} Why?
    ~ set_speaker(Speaker.Misra)
    [Misra] There was a break in, it looks like.
    [Misra] Last night.
    [Misra] We're trying to find probable cause.
    ~ set_speaker(Speaker.Irene)
    {current_npc} Heck, what are you talking to me for?
    {current_npc} You should be talking to Jenkins over there.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] That's Jenkins?
    [Lupe] He's completely passed out.
    ~ set_speaker(Speaker.Irene)
    {current_npc} Yeah, he's been taking it rough.
    {current_npc} He worked at the place for years.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] Would he have any reason to go up there and trash the place?
    ~ set_speaker(Speaker.Irene)
    {current_npc} Hell, I don't know. But I can tell you he's been here every night for the past week. Hasn't budged from his Whiskey Neat.
    {current_npc} But why don't you ask him yourselves?
    {current_npc} Give him a good shove, and he'll come to.
    {current_npc} I've done it a dozen times when he passes out like this.
    ~ CompleteQuest(jenkins_wakes_up)
    ~ CompleteQuest(irene_convo_2)
    -> DONE

* Despite all the closed shops and stores, this place is still open...
    ~ set_speaker(Speaker.Irene)
    {current_npc} Yeahhhhh.
    {current_npc} It's a little concerning that the bar is one of 
    {current_npc} the businesses that is still able to stay afloat.
    {current_npc} People need somewhere to forget about their troubles, I guess.
    {current_npc} To be honest, I've been debating closing up shop myself.
    ~ set_speaker(Speaker.Misra)
    [Misra] What?
    ~ set_speaker(Speaker.Irene)
    {current_npc} Yeah; Roy offered to buy me out. 
    {current_npc} Says he'll pay me up front for the place, too.
    ~ set_speaker(Speaker.Misra)
    [Misra] Roy's trying to push you out?
    ~ set_speaker(Speaker.Irene)
    {current_npc} I wouldn't say "push"; 
    {current_npc} He's just trying to help.
    {current_npc} I'm not the only person he's made this offer too.
    {current_npc} He bought out the Diner, I'm pretty sure.
    ~ set_speaker(Speaker.Misra)
    [Misra] Why would he do that? 
    [Misra] That's just hurting the town more.
    [Misra] Causing more people to leave.
    ~ set_speaker(Speaker.Irene)
    {current_npc} I couldn't say.
    ~ DiscoverClue(closed_shops_irene)
    // Add to Synthesis: the Town of KR
    -> DONE
    
}



