// SCENE 4.5: The Kettle Rock Police Station
// NOTES: this whole scene happens in the Dating sim style! (Cannot access this location like other locations)
//        this also appears to be a sort of cutscene so there isnt much logic to do! :)

// VARIABLES HERE:
//VAR reported_incident = false
//VAR case_file_received = false
//VAR love_points = 0
//VAR tease_level = 0
//VAR sincerity_level = 0
//VAR spooked = false
//VAR snooped = false

=== scene7_DS ===
# sfx : off
# name: description
# hide : Misra
# emote : Lupe \| Serious_2
# sfx : on
<i> You open the door to the precinct and are met with... no one.</i> 

<i> Wait...that's not right.</i>

<i> A memory begins to arise from the back of your mind. </i> 

<i> And an overwhelming feeling of...Deja Vu? </i> 

<i> No, that's not right. </i> 

<i> To have Deja Vu, you would've had to have been here before. </i> 

<i> But this isn't a guess. </i> 

<i> You know there has to be someone here with you.</i>

<i> Your gut tells you so.</i>


+ [Ding the bell.] -> ding

+ [Call out.] -> hello

= ding

<i>You ding the bell, but no one responds.</i>

-> soul

= hello
#name:Lupe
# emote : Lupe \| Serious_2
"Hello?" 

<i>... No response.</i>

+[Call out their name.] -> name
+[Wait.]-> soul

=name 
~tease_level++
# sfx : off
#name:Lupe
# emote : Lupe \| Inquisitive_1
"..."
"......."
# sfx : on
# emote : Lupe \| Inquisitive_1
"MISRA!!"

#name: Misra 
# emote : Misra \| Curious_1
"Um...hey?" 

"Do I...uh..know you?" 

#name: Lupe 
# emote : Lupe \| Fright_1
"Y-yeah. But.. but.." 

#name: Lupe 
"H-how?!" 
->BreakCycle

= soul

<i> Something's wrong.</i>
<i> Deep down, you know for a fact there is someone here.</i>
#name:Lupe
# emote : Lupe \| Inquisitive_1
"Where...are they?"

#name:Misra
# emote : Misra \| Neutral_1
"Hey!"

#name:Misra
# emote : Lupe \| Fright_1
"AYE! DIOS MIO!"

#name:Misra
# emote : Misra \| Nervous_1
"Sorry for the wait! We're a bit understaffed."

# name : Misra
"...You ok?"

// Also: [Chastise them for scaring you], [Play it off.] 
+ ["You almost gave me a heart attack!"] -> chastise //scared
+ ["Yeah, I'm fine."] -> scared //sarcastic expression
+["Misra?"] -> BreakCycle



= chastise 
~ sincerity_level ++
#name:Misra
# emote : Misra \| Neutral_1
"Oh! I'm truly sorry!"

<i>Their words stutter from holding back a laugh.</i>

#name: Misra
"I didn't mean to scare you." //laughing

#name:Lupe
# emote : Lupe \| Serious_2
"Oh, ha ha, very funny." //sarcastic

#name : Misra
"And to whom do I owe the pleasure of almost sending into cardiac arrest?"
-> introductions

= scared
~ tease_level ++
#name:Misra
# emote : Misra \| Inquisiting_1
"Oh really? I must have been confused then."

# name : Misra
"I don't understand spanish, but last I checked 'AYE! DIOS MIO!' means scared." 

#name:Lupe
# emote : Lupe \| Serious_2
"I wasn't scared. Just surprised. That's all."

#name:Misra
# emote : Misra \| Neutral_1
"Yeah, yeah!" //laughing

# name : Misra
"Well, does this fearless person have a name?"
-> introductions

= introductions
#name:Lupe
# emote : Lupe \| Neutral_2
"Detective Lupe. And you are...?"

#name:Misra
# emote : Misra \| Neutral_1
"Sheriff Misra."
+ ["It's nice to meet you."] -> niceToMeet
+["I...know you."]  -> BreakCycle


=niceToMeet
~ sincerity_level ++
 <i>You grab their hand, and give an unexpected shake</i> 
 
 <i>Despite how much it bothers you, you attempt to ignore that they've already decided to drop your title as "Detective".</i>
 
 #name: Misra 
 "Oh! Ok, cutting straight to the point I guess!" 

-> precinct

= precinct
#name:Misra
# emote : Misra \| Curious_1
"So, what's the issue? Normally folks don't come here just to chit-chat." 
+["Is it illegal to chit-chat?"] -> Company
+["A tree fell."] -> treeFell
= Company
~ tease_level++
#name:Misra
# emote : Misra \| Nervous_1
"Not here, and I'm not opposed to some company." //flirty

//flustured Lupe
<i>Your train of thought scatters for a brief moment with the slight raise of your heart rate.</i>

<i>How...strange. </i>

<i>You clear your throat.</i>

->treeFell

= treeFell

# name : Lupe
"There's a tree blocking the road out of town by the gas station. I need it cleared as soon as possible." 
#name:Misra
# emote : Misra \| Curious_1
{ tease_level > sincerity_level:  "Leaving already?! | "Ah, I see. } Well, I'll call the tree people and get that cleared out." 

#name:Misra
"May take a while though! As you may have noticed, we don't have many people who can help around here. "
//they gesture to an empty room
#name:Misra
# emote : Misra \| Nervous_1
{ sincerity_level >= tease_level: "Sorry about that! } { tease_level >= sincerity_level:  Looks like we might get to know each other after all." } 

# hide : Misra
# sfx : off
# emote : Lupe \| Neutral_2
# sfx : on
<i>They excuse themselves to make the call.</i>

<i>You wait patiently...</i>
<i>and wait...</i>
<i>and keep waiting...</i>
<i>...</i>
<i>There has to be something to do around here.</i>
+ [Snoop around.] -> snoop
+ [Keep waiting.] -> wait

= snoop
# emote : Lupe \| Inquisitive_1
~ tease_level++
<i>You lean over the front desk to see where the Sheriff went, but they're nowhere to be seen. They probably entered a different room to make the call. </i>

<i>What you do see, however, is a manilla folder with a label...</i> 

<i>"Unexplained Disturbance at Old Winery." </i>

<i>The curiosity in the back of your mind calls for you to take just a quick look. </i>

<i> You feel a memory tug at the back of your mind. </i> 

<i> You know what's going to be in here. </i> 

+ [Take a peek.] -> peek
+ [Don't be ridiculous.] -> wait

= peek
# emote : Lupe \| Inquisitive_1
~ tease_level++
~ snooped = true
<i>You quietly reach over the front desk and take a look at the case.</i>

<i>You realize that <i>case</i> is an overstatement for what's in this folder. </i>

/* 
- Date: August 29th, 1995
- Time: 1:02 AM
- Type: Anonymous Complaint
- Transcription is as follows:
    - *“Hey man, there’s something happening at the Winery on the hill.* *There’s these crazy loud noises and some weird lights going on—not sure what’s up, isn’t that place abandoned? Weird. Squash out.”*

And a black and white picture of the winery stained with age.
*/

#name: Lupe 
<i> You look up, meeting eyes with Misra. </i> 

#name: Misra
# emote : Misra \| Inquisiting_1
"So...thoughts?" 

#name: Lupe 
"I've...I've seen this already." 

->BreakCycle


= wait
~ sincerity_level++
<i>You may be bored out of your mind, but not crazy enough to go snooping around a precinct.</i> 

<i>Even if it's incredibly barren here, it would look bad on your record if you're caught looking into other case files. Expecially outside of your jurisdiction. </i> 

<i>Besides, Misra is just around the corner. They'll probably spot you if you start snooping.</i>

#name:Misra
# emote : Misra \| Neutral_1
"Yep, as expected! They said it'll take a while."
+ ["Is it just you here?"] -> solo
+ ["Let me guess, a couple days?"] -> time

= solo
~ sincerity_level ++
#name:Misra
# emote : Misra \| Nervous_1
"Yup! At the moment it's just me!"

#name:Lupe
# emote : Lupe \| Neutral_2
"When are the other staff or patrol Units coming?" 

#name:Misra
# emote : Misra \| Inquisiting_1
"..."

#name:Lupe
# emote : Lupe \| Annoyed_1
"Is it <i> just </i> you?! "

#name:Misra
# emote : Misra \| Nervous_1
"Yep!" 


# name : Lupe
"...Wow."
-> transition_to_case

= transition_to_case
#name:Misra
# emote : Misra \| Neutral_1
"Well, since you're going to be here a while, 'Detective', I could use your help!"

#name:Lupe
# emote : Lupe \| Serious_2
"Really?"

#name:Misra
# emote : Misra \| Neutral_1
"Yeah! We got a bit of a cold-case going on." 

<i>They hand you a thin manilla folder.</i>

/* 
Date: August 29th, 1995
- Time: 1:02 AM
- Type: Anonymous Complaint
- Transcription is as follows:
    - *“Hey man, there’s something happening at the Winery on the hill.* *There’s these crazy loud noises and some weird lights going on—not sure what’s up, isn’t that place abandoned? Weird. Squash out.”*

And a black and white picture of the winery stained with age.
*/
#name:Misra
# emote : Misra \| Inquisiting_1

"Well, what are your thoughts?"
-> police_report

= time
#name:Misra
# emote : Misra \| Inquisiting_1
"It's hard to say. The time tends to range between-"

#name:Misra
"...Oh, yeah. A couple days.

#name:Lupe
"Yeah."

#name: Misra 
"...How'd you...?" 

#name: Lupe 
"It's to be...expected." 

#name: MISRA
"Huh."

#name:Misra
# emote : Misra \| Nervous_1
"Well, things happen a lot...slower here in Kettle Rock. But don't worry! I'll make sure they know it's urgent."

-> transition_to_case

= police_report
#name:Lupe
"I've...seen this already." 

-> BreakCycle

=theRest
#name:Misra
# emote : Misra \| Curious_1 
"The rest?" 

#name:Lupe
# emote : Lupe \| Annoyed_1
"Yes! The rest of the file!" 

->nothingHere

=nothingHere
#name:Misra
# emote : Misra \| Curious_1 
"What do you mean?" 

#name:Lupe
# emote : Lupe \| Annoyed_1
"I mean this isn't a case. This is a bunch of bull-"

#name: Misra 
"No, no. The case is right there! What else is there supposed to be?" 

#name: Lupe 
"Oh, I don't know. Maybe the actual problem you're trying to solve." 

#name: Lupe 
"Or witnesses." 

#name: Lupe 
"Or anything else pertaining to the case." 

#name: Lupe 
"I mean, there's some stuff there, sure, but it's flimsy."

#name:Misra
# emote : Misra \| Inquisiting_1 
"Oh." 

#name: Lupe 
"Oh?" 

#name: Misra
"Yeah, I just didn't know what goes into a case file. "

#name: Lupe
"..."

#name: Lupe
"How long have you been a Sheriff?" 

#name: Misra
"Uhhhhhhhh."


#name: Lupe 
# emote : Lupe \| Inquisitive_1
"Let me guess, a week?" 

#name: Misra
# emote : Misra \| Nervous_1
"Yeah...actually." 

#name: Lupe 
"Interesting." 

#name: Misra 
"Yeah..."

#name: Misra 
# emote : Misra \| Inquisiting_1
"Well then what do I need?" 
+["Evidence."] ->Evidence 
+["Witnesses."]->Witnesses 
+["Help."] -> Help

=Evidence 
#name:Lupe
# emote : Lupe \| Serious_2
"Well for starters, evidence. You're not sure who placed this call, so any clues will narrow down suspects." 

#name:Misra
# emote : Misra \| Neutral_1
"So we go to the Winery and look around!" 

#name:Lupe
# emote : Lupe \| Inquisitive_1
"I'm sorry, we?" 
-> We 

=Witnesses
#name:Lupe
# emote : Lupe \| Inquisitive_1
"Well for starters, witnesses. If something was going down, someone must have heard or seen something." 

#name:Misra
# emote : Misra \| Neutral_1
"So go into town and ask people!"


#name: Lupe 
"No. First go to the crime scene, see what evidence you can gather before questioning." 

#name: Misra 
"So we go to the Winery!" 

#name:Lupe
# emote : Lupe \| Inquisitive_1
"I'm sorry, we?" 
-> We 

= Help 
#name:Lupe
# emote : Lupe \| Serious_2
"What you need is someone to help you on this case. You're too fresh to handle a cold case." 

#name:Misra
# emote : Misra \| Nervous_1
"Someone like yourself?" 

#name:Lupe
# emote : Lupe \| Inquisitive_1
"I'm sorry?" 
-> We 

=We
#name:Misra
# emote : Misra \| Neutral_1
"Yeah! I mean what else are you going to do in the meantime?"

#name:Lupe
# emote : Lupe \| Serious_2
"Find out what's going on in that damn winery." 

#name:Misra
# emote : Misra \| Surprised_2
"Oh..."

#name:Lupe
# emote : Lupe \| Neutral_2
"What?"

#name: Misra 
# emote : Misra \| Nervous_1
"Nothing. Just..." 

#name: Misra 
"You seem..." 

#name:Lupe
# emote : Lupe \| Inquisitive_1
"...?" 

#name: Misra 
"Interested."

 
+ [Call your boss.]
    -> phone_call
    
= phone_call 
# hide : Misra
<i>You {phone_call < 0: grab your phone and begin to} dial chief Thelton. </i>

<i>The phone rings...</i>

<i>And rings...</i>

<i>And rings...</i>

"You've reached Chief Detective Inspector Thelton, Boise Precinct. I'm not available right now. You know what to do." 

<i>Weird...</i>

<i>She always picks up.</i> 

+[Call again.] -> phone_call
+[Leave a voicemail.] -> voicemail

= voicemail
#name:Lupe
# emote : Lupe \| Inquisitive_1
"Thelton, Lupe here. Currently stuck in Kettle Rock..."

+[Step away from Misra.] -> stepAway 
+[Stay put.] -> StayPut 

=stepAway 
~sincerity_level++
<i> You take a couple steps away from Misra and lower you voice. </i> 

#name:Lupe
"Something's off with this town." 

#name:Lupe
"With these people." 

#name:Lupe
"With the winery up on the hill." 

#name:Lupe
"It's just a hunch, or a weird feeling, or....well, I don't know."

#name:Lupe
"If you could formally look into the record of the Kettle Rock Winery that would be greatly appreciated." 

#name:Lupe
"More specifically..." 

#name:Lupe
"Missing persons cases in the area." 

#name:Lupe
"...and if any of their last known locations are connected to that winery." 

#name:Lupe
"I'd...greatly appreciate it if you could call back. Thanks."

<i>You hang up.</i> 

<i>You sent a voicemail before...didn't you?</i> 

<i> Upon checking the history of voicemails sent, this is the only one sent within the last week. </i>

<i>...strange.</i>

->WhatsThePlan

=StayPut
~tease_level++

#name:Lupe
"Something's off with this town." 

#name:Lupe
"With these people." 

#name:Lupe
"With the winery up on the hill." 

#name:Lupe
"It's just a hunch, or a weird feeling, or....well, I don't know."

#name:Lupe
"If you could formally look into the record of the Kettle Rock Winery that would be greatly appreciated." 

#name:Lupe
"More specifically..." 

#name:Lupe
"Missing persons cases in the area." 

#name:Lupe
"...and if any of their last known locations are connected to that winery." 

#name:Lupe
"I'd...greatly appreciate it if you could call back. Thanks."

<i>You hang up.</i> 

<i>You sent a voicemail before...didn't you?</i> 

<i> Upon checking the history of voicemails sent, this is the only one sent within the last week. </i>

<i>...strange.</i>

#name: Misra 
# emote : Misra \| Nervous_1
"Wow, callin' in the big guns?" 

#name: Lupe 
# emote : Lupe \| Inquisitive_1
"If she gets my message, possibly." 

#name: Misra 
# emote : Misra \| Neutral_1
"Cool." 

->WhatsThePlan

=WhatsThePlan

#name:Misra
# emote : Misra \| Neutral_1
"So, what's the plan?" 

# name: Lupe 
"Well I'm going to need to see this place itself."

# name : Misra
"Alright!! Road trip!!" 

<i>They spring out from behind the desk, jokingly gesturing towards the door. </i>

# name : Misra
"Eh-shall we?"

*"We shall." ->JokeBack
*[Walk through door] -> door 

= JokeBack
#name:Lupe
# emote : Lupe \| Fluster_1
<i>That slight joke may have drained your social battery for the week.</i> 

<i>Misra giggles as they walk out the door</i>

#name:Misra
# emote : Misra \| Neutral_1
"Alright! Misra and Lupe on the case! Watch out Kettle Rock, we gon'na figure you OUT!" 
-> END

=door 
# hide : Lupe
<i>You walk through the door, not saying a word to Misra.</i> 
# name : Misra
# emote : Misra \| Neutral_1
"Alright! Misra and Lupe on the case! Watch out Kettle Rock, we gon'na figure you OUT!" 
-> DONE

==BreakCycle==
# sfx : off
#name: Misra 
# emote : Misra \| Curious_1
"..." 

# sfx : on
# emote : Misra \| Nervous_1
"I'm...sorry?" 

#name: Lupe 
# emote : Lupe \| Serious_2
"Misra. Your name is Sheriff Misra, correct?" 

#name: Misra 
# emote : Misra \| Curious_1
"Y-yeah....how'd you...?" 

+["Okay, what's going on here?"] -> WhatsGoingOn
+["I'm going insane."] -> insane

=insane 
#name: Misra 
# emote : Misra \| Curious_1
# sfx : off
# emote : Lupe \| Fright_1 
"Wh-what...?" 

<i> You feel a laugh slip between your words. </i> 

# sfx : on
#name: Lupe 
"I...I've done this already." 

#name: Misra 
"Are you ok?" 

#name: Lupe 
"No." 

#name: Misra 
"...ok." 

#name: Misra 
"Let's just...take a moment to breathe." 

#name: Lupe 
# emote : Lupe \| Fright_1 
"Am I going crazy?! I-I-I've done this already." 

#name: Misra 
"Ok, you're not crazy. Let's just-" 

#name: Lupe 
"The cash register-" 

#name: Misra
"What cash register?" 

#name: Lupe 
"A-a-and the tree fell-" 

#name: Misra 
"A tree fell?" 

#name: Lupe
"And today's Tuesday...."

# sfx : off
# emote : Misra \| Surprised_2
"RIGHT?!" 

# sfx : on
#name: Misra 
# emote : Misra \| Nervous_1
"Today's....Monday." 

#name: Lupe
"..." 

#name: Misra 
"Let's just take a seat." 

+[Sit down.] -> sitDown
+["The Winery."] -> TheWinery

= sitDown
~sincerity_level++ 
#name: Misra 
# emote : Misra \| Neutral_1
# sfx : off
# emote : Lupe \| Fright_1
# sfx : on
"There you go...breathe." 

#name: Misra 
"Come on, breathe with me Lupe." 

#name: Misra 
"Breathe in..." 
*[Breathe in.] -> breathingExercise 
+["The Winery."] -> TheWinery

=breathingExercise
<i> You take a deep breathe in, following the same pace as Misra. </i>

#name: Misra 
"And breathe out..." 

<i> As you go to breathe out, you feel a quake in your chest.</i> 

#name: Misra 
"Ok, there we go! One more time. Can you do that for me?" 

#name: Misra 
"Breathe in..." 

<i> They offer their hand. </i> 

+[Take their hand.] -> hand
+[Breathe.] -> breathe
+["The Winery." ] -> TheWinery

=hand
~tease_level++
<i> You lay your hand in theirs, and they gently place their other hand on top. </i> 
#name: Misra 
"There you go...come on. One more breath." 

+[Breathe.] -> breathe 
+["The Winery."] -> TheWinery

=breathe 
<i> You take a deep breath in, feeling some pressure release from your chest. </i> 

{ tease_level > sincerity_level:  <i> They gently place a hand on your face </i> }

#name: Misra
"And breathe out." 

<i> A slow trembling breath comes out, following the pace of Misra's once again. </i> 

#name: Misra 
"Better?" 

#name: Lupe 
"Better." 

#name: Misra 
"Ok...sounds like you're going through something-"

#name: Lupe 
{ tease_level > sincerity_level:  "That's an understatement." | "Yeah." }

#name: Misra
"Can I get you anything?"
+["Water."] -> Water
+["The case file."] -> TheWinery

=Water
#name: Misra 
"Ok! One water, coming right up!" 

<i> They get up and walk to the back to get a glass of water. </i> 

+[Call Chief Thelton.] -> CallTheltonBreak
+[Wait.] -> waitForWater

=CallTheltonBreak
#name:Lupe
# emote : Lupe \| Inquisitive_1
"Thelton, Lupe here. Currently stuck in Kettle Rock..."

<i> You take a couple steps away from Misra and lower you voice. </i> 

#name:Lupe
"Something's off with this town." 

#name:Lupe
"With these people." 

#name:Lupe
"With the winery up on the hill." 

#name:Lupe
"There's...something there."

#name:Lupe
"It's just a hunch, or a weird feeling, or....well, I don't know."

#name:Lupe
"If you could formally look into the record of the Kettle Rock Winery that would be greatly appreciated." 

#name:Lupe
"More specifically..." 

#name:Lupe
"Missing persons cases in the area." 

#name:Lupe
"...and if any of their last known locations are connected to that winery." 

#name:Lupe
"I'd...greatly appreciate it if you could call back. Thanks."

<i>You hang up.</i> 

<i>You sent a voicemail before...didn't you?</i> 

<i> Upon checking the history of voicemails sent, this is the only one sent within the last week. </i>

<i>...strange.</i>

->waitForWater 

= waitForWater
<i> Misra comes back with a glass of water. </i> 

#name: Misra 
# emote : Misra \| Neutral_1
{ tease_level > sincerity_level:  "I've got one water for Mx. Deja vu!" | "Here you go."} 

#name: Lupe
# emote : Lupe \| Inquisitive_1
{ tease_level > sincerity_level:  "I wish it was Deja Vu."} 

{ tease_level > sincerity_level: -> WaterFlirty}

-> dontTakeHandwater

=WaterFlirty

 <i> You lightly laugh to yourself. </i>

#name: Misra 
# emote : Misra \| Neutral_1
 "Well...whatever it is, I would like to help." 
 
 <i> They sit next to you, holding out a hand. </i> 
 
 +[Take their hand.] -> takeHandWater
 +[Don't take their hand.] -> dontTakeHandwater 
 
 =takeHandWater
 <i> You take their hand</i>
 
 #name: Lupe 
 # emote : Lupe \| Neutral_2
 "I...don't know what's going on." 
 
 #name: Misra 
 # emote : Misra \| Neutral_1
 "And that's ok! Take some time to get your groundings." 
 
  #name: Lupe 
# emote : Lupe \| Inquisitive_1
"That...thing." 

#name: Misra 
# emote : Misra \| Curious_1
"...?"

 #name: Lupe 
 "It...killed you." 
 
 #name: Lupe 
 "It...gored me." 
 
 #name: Misra 
 # emote : Misra \| Surprised_2
 "Is there a big scary monster!?! Out for blooooooddd!!!" 
 
 #name: Lupe 
 # emote : Lupe \| Fright_1
 "...!" 
 
 #name: Misra 
 # emote : Misra \| Nervous_1
 "...Oh, you're serious?" 
 
 #name: Lupe 
 "Dead serious." 
-> TheWinery

=dontTakeHandwater 
#name: Lupe
# emote : Lupe \| Inquisitive_1
"Thanks." 

#name: Lupe
"..."

#name: Lupe
"You know what would be a big help?" 

#name: Misra 
# emote : Misra \| Curious_1
"Hmph?" 

#name: Lupe
# emote : Lupe \| Neutral_2
"If you could hand me the winery case file, I'd greatly appreciate it." 

#name: Misra
{ tease_level > sincerity_level:  "That's some strong Deja Vu!" | "Yeah, sure."} 

->TheWinery

=WhatsGoingOn
#name: Misra 
# emote : Misra \| Curious_1
"Huh?" 

+["You died."] -> died
+["Don't play stupid."] -> stupid
+["I've done this before."] -> Before


=stupid 
#name: Misra 
# emote : Misra \| Surprised_2
"Woah! Hey there!" 

# emote : Misra \| Nervous_1
"I have no clue what you're talking about." 

#name: Lupe 
# emote : Lupe \| Serious_2
"C'mon. I remember it all." 

->Before

=Before 

#name: Misra 
# emote : Misra \| Curious_1
"...?" 

#name: Lupe 
"You showed me the case file-" 

#name: Misra 
"The case file?" 

#name: Lupe 
"The Winery file." 

#name: Misra 
"I did?" 

#name: Lupe 
"And then we investigated." 

#name: Misra 
"We did?" 

#name: Lupe 
"And then..." 

#name: Lupe 
# emote : Lupe \| Fright_1
"And then...you died." 

#name: Lupe 
"And...I got gored by..."

#name: Lupe 
"That..." 

#name: Misra 
# emote : Misra \| Inquisiting_1
"Um...question." 

#name: Misra 
"Am I a ghost then?" 

#name: Lupe 
# emote : Lupe \| Annoyed_1
"No because-" 

#name: Lupe 
"...you, you really don't remember?" 

#name: Misra 
# emote : Misra \| Nervous_1
"Can't say I do! Last I checked, I'm pretty alive." 

->FeelingDumb
=died 
#name: Misra 
# emote : Misra \| Nervous_1
"Um...." 
"Last time I checked I'm...pretty alive."

#name: Lupe 
# emote : Lupe \| Annoyed_1
"No-" 

#name: Lupe 
# emote : Lupe \| Serious_2
"In the Winery!" 

#name: Misra 
# emote : Misra \| Nervous_1
"...I'm  sorry, I'm confused." 

#name: Lupe 
"We went to the Winery." 

#name: Misra 
# emote : Misra \| Curious_1
"Uh-Huh." 

#name: Lupe 
"And...and there was..." 

->FeelingDumb

=FeelingDumb

#name: Lupe 
# emote : Lupe \| Annoyed_1
"Nevermind. This is stupid.I don't know what I'm doing." 

<i> You begin to walk out of the precinct...</i> 

{ tease_level > sincerity_level:  <i> But Misra quickly grabs your hand. </i> | <i> But Misra calls out. </i> }

#name: Misra 
# emote : Misra \| Surprised_2
"No wait! I want to hear you out!" 

{ tease_level > sincerity_level: <i> Your face becomes warm for a brief moment. </i> }

#name: Lupe 
# sfx : off
# emote : Lupe \| Inquisitive_1
# sfx : on
"..." 

->TheWinery

=TheWinery
 #name: Lupe 
 # emote : Lupe \| Inquisitive_1
 "Let me see that winery case file again." 
 
  #name: Misra 
 # emote : Misra \| Curious_1
 "Uh...Sure!" 
 
 <i> Misra confusingly grabs the file for the Winery and hands it to you. </i> 
 
 /* 
Date: August 29th, 1995
- Time: 1:02 AM
- Type: Anonymous Complaint
- Transcription is as follows:
    - *“Hey man, there’s something happening at the Winery on the hill.* *There’s these crazy loud noises and some weird lights going on—not sure what’s up, isn’t that place abandoned? Weird. Squash out.”*

And a black and white picture of the winery stained with age.
*/

#name: Lupe 
# emote : Lupe \| Inquisitive_1
"Hmph." 

{ tease_level > sincerity_level:  <i> They pose themselves to reflect you. </i> ->MimicLupe} 

->WineryContinued

= MimicLupe
~tease_level++
{MimicLupe > 5: -> laugh} 
{MimicLupe > 0: <i> You pose yourself to better reflect Misra's imitation of you. </i>} 

{MimicLupe > 0: 

#name: Lupe 
"Hmph."

} 


#name: Misra 
# emote : Misra \| Curious_1
"Hmph." 

+[Mimic Misra] -> MimicLupe
+[Laugh] -> laugh

=laugh
<i> You lightly laugh through your distress. </i> 
#name: Lupe 
# emote : Lupe \| Fluster_1
"Ok ok that's enough." 

-> WineryContinued

=WineryContinued
#name: Lupe 
# sfx : off
# emote : Lupe \| Neutral_2
# sfx : on
"Come on." 

#name: Misra 
# emote : Misra \| Curious_1
"What?" 

#name: Lupe 
"We need to go to this winery as soon as possible." 

#name: Lupe 
"Something is going on." 

#name: Lupe 
"And this has to be the origin point." 

<i> You begin walking towards the door </i> 

#name: Misra 
# emote : Misra \| Surprised_2
"Oh!! Ok!!!" 

#name: Misra 
{ tease_level > sincerity_level:  "Alright! Misra and Lupe on the case! Watch out Kettle Rock, we gon'na figure you OUT!" | <i> They quickly follow behind. </i> } 

#hide: Misra
#hide: Lupe

<b><i>To Be Continued...</i><b>
~ ChangeGameScene("scene_default", 0, 0)
-> DONE