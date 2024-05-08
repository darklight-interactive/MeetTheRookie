=== jenkins_intro ===
~ current_npc = "[Jenkins]"
~ set_speaker(Speaker.Irene)
[Irene] Nope.
[Irene] These folks have some questions for you.
[Irene] Be nice.
~ set_speaker(Speaker.Jenkins)
{current_npc} I ain't got nothin to say to the Rookie Sheriff.
~ set_speaker(Speaker.Irene)
[Irene] What about their pretty friend?
~ set_speaker(Speaker.Jenkins)
{current_npc} Pshh.
~ set_speaker(Speaker.Irene)
[Irene] How about this?
[Irene] You answer their questions or I'm closing your tab.
~ set_speaker(Speaker.Jenkins)
{current_npc} Arghhhh
{current_npc} The hell do you want to know?

-> DONE


=== jenkins_questions ===

* The Old Winery on the hill... 
    ~ set_speaker(Speaker.Jenkins)
    {current_npc} Will I ever stop hearing about the godforsaken place? 
    {current_npc} S'worth nothing but the dirt it stands on.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] You used to work there?
    ~ set_speaker(Speaker.Jenkins)
    {current_npc} Work, heh.
    {current_npc} Of a sort.
    {current_npc} For many, many many, many...
    {current_npc}...
    ~ set_speaker(Speaker.Lupe)
~ DiscoverClue(jenkins_winery)
-> DONE

* {IsClueFound(jenkins_winery)} You wouldn't have happened to be up there last night?
~ set_speaker(Speaker.Jenkins)
{current_npc} Last night?
{current_npc} S'was here last night, I was.
~ set_speaker(Speaker.Lupe)
[Lupe] You haven't been up there lately, then?
~ set_speaker(Speaker.Jenkins)
{current_npc} Hell, no.
{current_npc} I'm reminded of 'em everytime I set foot in the building.
{current_npc} I feel like they can still hear them...
~ set_speaker(Speaker.Lupe)
~ DiscoverClue(council_mentioned)
// Add to Synthesis: Who broke into the Old Winery?
-> DONE

* So, Kettle Rock...
    ~ set_speaker(Speaker.Jenkins)
    {current_npc} Kettle goddammed Rock.
    {current_npc} All that blood sunk into this town.
    {current_npc} All that blood turned into fleeting money.
    {current_npc} Was it worth it, do you think?
    {current_npc} Was <i>this</i> worth it?
    {current_npc} What have you done, Sarah?
    // Add to Synthesis: The Town of KR 
    ~ DiscoverClue(sarah_mentioned)
    -> DONE
    
    
* {council_mentioned} Hear who?
    ~ set_speaker(Speaker.Jenkins)
    {current_npc} <i>Them</i>. 
    {current_npc} I feel them.
    {current_npc} I feel their sacrifice.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] The...Goats?
    ~ set_speaker(Speaker.Jenkins)
    {current_npc} NO!
    {current_npc} DON'T CALL THEM THAT!
    {current_npc} The Council was s'much more than that.
    {current_npc} Their sacrifice s'not somethin to throw away,
    {current_npc} or make fun of.
    {current_npc} or point fingers at.
    {current_npc} They did what they did for <i>us</i>.
    {current_npc} To give us time and--and--
    {current_npc} ...
    ~ DiscoverClue(sacrifice_mentioned_jenkins)
    // Add to Synthesis: the Town of KR
    -> DONE

* {sarah_mentioned} Sarah?
        ~ set_speaker(Speaker.Jenkins)
        {current_npc} Such a good heart.
        {current_npc} But desperation misguides the good.
        {current_npc} She thought she was right,
        {current_npc} I thought she was right,
        {current_npc} They all thought she was right.
        {current_npc} Now look at us.
        {current_npc} End of the line, ship s'goin down.
        {current_npc} Everyone s'jumping overboard.
        {current_npc} Not me.
        ~ set_speaker(Speaker.Lupe)
        [Lupe] Who's Sarah?
        ~ set_speaker(Speaker.Jenkins)
        {current_npc} ...
        {current_npc} Rookie Sheriff, do you remember her?
        ~ set_speaker(Speaker.Misra)
        [Misra] I..
        [Misra] I-I don't know who you're talking about.
        //Add to Synthesis: The Town of KR
        
-> jenkins_questions

    *{IsClueFound(sacrifice_mentioned_jenkins)} That's the second time I've heard the word 'sacrifice' tossed around. Are you talking about the people who vanished? The ..Council?
    ~ set_speaker(Speaker.Jenkins)
    {current_npc} 'Vanished', psh...
    {current_npc} That's what this town slaps over the truth.
    {current_npc} But what <i>really<i> happened...
    {current_npc} Argh...Eckup...I don't feel so good...
    {current_npc} Bachitan...
    ~ set_speaker(Speaker.Misra)
    [Misra] Okay, I think that's enough.
    ~ set_speaker(Speaker.Lupe)
    [Lupe] What?
    [Lupe] We're starting to pick up some steam with this.
    [Lupe] Now you want to stop?
    ~ set_speaker(Speaker.Misra)
    [Misra] We've been questioning people all day.
    [Misra] Let's take a break.
    [Misra] Irene!
    [Misra] Can we get a drink?
-> DONE //{Go to Dating Sim: Bar}




