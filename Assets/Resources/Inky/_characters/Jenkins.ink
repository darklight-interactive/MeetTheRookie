=== Jenkins_Dialogue ===
    {IsQuestComplete (irene_gives_cue):
    
        {IsQuestComplete (jenkins_first_interact):
        ~ SetSpeaker(Speaker.Jenkins)
         {Can't you let a tired man sleep? | God, my head is killing me. Oh wait. It's just the sound of your voice. | Ugh, what do you want? | Eckup. Eckup. Bleh. | Night Night.}
        -> jenkins_questions
        
        - else:
        
         ~ CompleteQuest(jenkins_first_interact)
         ~ SetSpeaker(Speaker.Jenkins)
         Eh? ARGH. Whaddya want Irene?
         S'closing time already?
        ~ SetSpeaker(Speaker.Lupe)
        ~ SetSpeaker(Speaker.Irene)
         Nope.
         These folks have some questions for you.
         Be nice.
        ~ SetSpeaker(Speaker.Jenkins)
         I ain't got nothin to say to the Rookie Sheriff.
        ~ SetSpeaker(Speaker.Irene)
         What about their pretty friend?
        ~ SetSpeaker(Speaker.Jenkins)
         Pshh.
        ~ SetSpeaker(Speaker.Irene)
         How about this?
         You answer their questions or I'm closing your tab.
        ~ SetSpeaker(Speaker.Jenkins)
         Arghhhh.
         The hell do you want to know?
        -> jenkins_questions
        
        }
        
    - else:
          ~ SetSpeaker(Speaker.Jenkins)
          Arggghhh.
          Five more minutes.
        -> DONE
    }


= jenkins_questions
* [The Old Winery on the hill...] -> jenkins_winery_
* {IsClueFound(jenkins_winery)}[Were you at the Winery last night?] -> jenkins_last_night
* [So, Kettle Rock...] -> jenkins_kettle_rock
* {IsClueFound(council_mentioned)} [Hear who?] -> jenkins_council
* {IsClueFound(sarah_mentioned)} [Sarah?] -> jenkins_sarah
* {IsClueFound(sacrifice_mentioned_jenkins)} [Sacrifice...?] -> jenkins_sacrifice


= jenkins_winery_
    ~ SetSpeaker(Speaker.Jenkins)
     Will I ever stop hearing about the godforsaken place? 
     S'worth nothing but the dirt it stands on.
    ~ SetSpeaker(Speaker.Lupe)
     You used to work there?
    ~ SetSpeaker(Speaker.Jenkins)
     Work, heh.
     Of a sort.
     For many, many many, many...
    ...
    ~ SetSpeaker(Speaker.Lupe)
    ~ DiscoverClue(jenkins_winery)
    -> jenkins_questions

= jenkins_last_night
    ~ SetSpeaker(Speaker.Jenkins)
     Last night?
     S'was here last night, I was.
    ~ SetSpeaker(Speaker.Lupe)
     You haven't been up there lately, then?
    ~ SetSpeaker(Speaker.Jenkins)
     Hell, no.
     I'm reminded of 'em everytime I set foot in the building.
     I feel like I can still hear them...
    ~ SetSpeaker(Speaker.Lupe)
    ~ DiscoverClue(council_mentioned)
    -> jenkins_questions

= jenkins_kettle_rock
    ~ SetSpeaker(Speaker.Jenkins)
     Kettle goddammed Rock.
     All that blood sunk into this town.
     All that blood turned into fleeting money.
     Was it worth it, do you think?
     Was <i>this</i> worth it?
     What have you done, Sarah?
    // Add to Synthesis: The Town of KR 
    ~ DiscoverClue(sarah_mentioned)
    -> jenkins_questions
    
    
= jenkins_council
    ~ SetSpeaker(Speaker.Jenkins)
     <i>Them</i>. 
     I feel them.
     I feel their sacrifice.
    ~ SetSpeaker(Speaker.Lupe)
     The...Goats?
    ~ SetSpeaker(Speaker.Jenkins)
     NO!
     DON'T CALL THEM THAT!
     The Council was s'much more than that.
     Their sacrifice s'not somethin to throw away,
     or make fun of.
     or point fingers at.
     They did what they did for <i>us</i>.
     To give us time and--and--
     ...
    ~ DiscoverClue(sacrifice_mentioned_jenkins)
    -> jenkins_questions

= jenkins_sarah
    ~ SetSpeaker(Speaker.Jenkins)
     Such a good heart.
     But desperation misguides the good.
     She thought she was right,
     I thought she was right,
     They all thought she was right.
     Now look at us.
     End of the line, ship s'goin down.
     Everyone s'jumping overboard.
     Not me.
    ~ SetSpeaker(Speaker.Lupe)
     Who's Sarah?
    ~ SetSpeaker(Speaker.Jenkins)
     ...
     Rookie Sheriff, do you remember her?
    ~ SetSpeaker(Speaker.Misra)
     I..
     I-I don't know who you're talking about.
    -> jenkins_questions
    
= jenkins_sacrifice
    ~ SetSpeaker(Speaker.Lupe)
    That's the second time I've heard that word tossed around. Are you talking about the people who vanished? The ..Council?
~ SetSpeaker(Speaker.Jenkins)
     'Vanished', psh...
     That's what this town slaps over the truth.
     But what <i>really<i> happened...
     Argh...Eckup...I don't feel so good...
     Bachitan...
    ~ SetSpeaker(Speaker.Misra)
     Okay, I think that's enough.
    ~ SetSpeaker(Speaker.Lupe)
     What?
     We're starting to pick up some steam with this.
     Now you want to stop?
    ~ SetSpeaker(Speaker.Misra)
     We've been questioning people all day.
     Let's take a break.
     Irene!
     Can we get a drink?
     ~ChangeGameScene("scene4_5_DS")
    -> DONE //{Go to Dating Sim: Bar}




