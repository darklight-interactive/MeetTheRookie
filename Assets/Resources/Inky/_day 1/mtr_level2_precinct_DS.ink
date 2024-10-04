// SCENE 4.5: The Kettle Rock Police Station
// NOTES: this whole scene happens in the Dating sim style! (Cannot access this location like other locations)
//        this also appears to be a sort of cutscene so there isnt much logic to do! :)

// VARIABLES HERE:
 VAR reported_incident = false
 VAR case_file_received = false
 VAR love_points = 0
 VAR tease_level = 0
 VAR sincerity_level = 0
 VAR spooked = false
 VAR snooped = false

* [Scene 2 - The Precinct] -> scene2_DS

=== scene2_DS ===
    ~ SetSpeaker(Speaker.Unknown)
    # name : description
    # hide : Misra
    # emote : Lupe \| Serious_2
    <i> You open the door to the precinct and are met with... no one. The place is frozen in time, with dust collecting on the window sills.</i> 
    
    + [Ding the bell.] -> ding
    + [Call out.] -> hello

= ding
    <i>You ding the bell, but no one responds.</i>
    -> soul

= hello
    ~ SetSpeaker(Speaker.Lupe)
    # emote : Lupe \| Serious_2
    "Hello?"
    <i>... No response.</i>
    -> soul

= soul
<i>Is there a single person in this town that's helpful?</i>

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Annoyed_1
"Hmph... well can't say I didn't try. I'm wasting my time here anywa-"

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Hey!"

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Fright_1
"AYE! DIOS MIO!"

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Nervous_1
"Sorry for the wait! We're a bit understaffed."

~ SetSpeaker(Speaker.Misra)
"...You ok?"

+ ["You almost gave me a heart attack!"] -> chastise //scared
+ ["Yeah, I'm fine."] -> scared //sarcastic expression

= chastise 
~ sincerity_level ++
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Oh! I'm truly sorry!"

<i>Their words stutter a bit from holding back a laugh.</i>

~ SetSpeaker(Speaker.Misra)
"I didn't mean to scare you." //laughing

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Serious_2
"Oh, ha ha, very funny." //sarcastic

~ SetSpeaker(Speaker.Misra)
"And to whom do I owe the pleasure of almost sending into cardiac arrest?"
-> introductions

= scared
~ tease_level ++
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Inquisiting_1
"Oh really? I must have been confused then."

~ SetSpeaker(Speaker.Misra)
"I don't understand spanish, but last I checked 'AYE! DIOS MIO!' means scared." 

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Serious_2
"I wasn't scared. Just surprised. That's all."

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Yeah, yeah!" //laughing

~ SetSpeaker(Speaker.Misra)
"Well, does this fearless person have a name?"
-> introductions

= introductions
~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Neutral_2
"Detective Lupe. And you are...?"

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Sheriff Misra! Nice to meet you, Lupe!"
//also : [Shake their hand in silence], [Return the greeting]
+ [Shake their hand in silence.] -> silence
+ ["Nice to meet you too."] -> niceToMeet

= silence
<i>You attempt to ignore that they've already decided to drop your title as "Detective" and shake their hand.</i>

~ SetSpeaker(Speaker.Misra) 
"..."

~ SetSpeaker(Speaker.Lupe) 
"..."

~ SetSpeaker(Speaker.Misra)
"Yep...very nice to meet you. " 

 -> precinct

=niceToMeet
~ sincerity_level ++
 <i>You grab their hand, and they give a vigorous shake that you can feel up to your shoulder.</i> 
 
 <i>Despite how much it bothers you, you attempt to ignore that they've already decided to drop your title as "Detective".</i>

-> precinct

= precinct
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Curious_1
"So, what's the issue? Normally folks don't come here just to chit-chat." 
+["Is it illegal to chit-chat?"] -> Company
+["A tree fell."] -> treeFell
= Company
~ tease_level++
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Nervous_1
"Not here, plus I'm not opposed to some company." //flirty


<i>Your train of thought scatters for a brief moment with the slight raise of your heart rate.</i>

<i>How...strange. </i>

<i>Why would you say that?!</i>

<i>Pull it together! What happened to professionalism?!</i>

<i>You clear your throat.</i>

->treeFell

= treeFell

~ SetSpeaker(Speaker.Lupe)
"There's a tree blocking the road out of town by the gas station. I need it cleared as soon as possible." 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Curious_1
{ tease_level > sincerity_level:  "Leaving already?! | "Ah, I see. } Well, I'll call the tree people and get that cleared out." 

~ SetSpeaker(Speaker.Misra)
"May take a while though! As you may have noticed, we don't have many people who can help around here. "

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Nervous_1
{ sincerity_level >= tease_level: "Sorry about that!" } { tease_level >= sincerity_level:  "Looks like we might get to know each other after all." } 

# hide : Misra
# emote : Lupe \| Neutral_2
<i>They excuse themselves to make the call.</i>

<i>You wait patiently...</i>
<i>and wait...</i>
<i>and keep waiting...</i>
<i>...</i>
<i>There has to be something to do around here.</i>
+ [Snoop around.] -> snoop
+ [Keep waiting.] -> wait

= snoop
# emote : Lupe \| Inquistive_1
~ tease_level++
<i>You lean over the front desk to see where the Sheriff went, but they're nowhere to be seen. They probably entered a different room to make the call. </i>

<i>What you do see, however, is a manilla folder with a label...</i> 

<i>"Unexplained Disturbance at Old Winery." </i>

<i>The curiosity in the back of your mind calls for you to take just a quick look. Who can resist an unsolved mystery? </i>

+ [Take a peek.] -> peek
+ [Don't be ridiculous.] -> wait

= peek
# emote : Lupe \| Inquistive_1
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

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Inquisiting_1
"So...thoughts?" 

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Fright_1
"AYE!!"

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Fright_1
"You CAN'T keep scaring me like that."

{- spooked: 
    ~ SetSpeaker(Speaker.Misra)
    # emote : Misra \| Nervous_1
    "Sorry, sorry!" 
  - else: 
    ~ SetSpeaker(Speaker.Misra)
    # emote : Misra \| Surprised_2
    "So, you admit you were scared! "
}

<i>They're going to give you a headache. </i>

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Inquisiting_1
"I'd also like to point out you probably just broke some kind of law by peeking at that. You know, confidentiality and such."

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Neutral_2 
"My apologies."

<i>You try to hand the file back to them, but they push it towards your direction.</i>

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Surprised_2
"No! No worries, 'Detective'! You didn't answer my question..."

~ SetSpeaker(Speaker.Misra)
"What are your thoughts?"
-> police_report


= wait
~ sincerity_level++
<i>You may be bored out of your mind, but not crazy enough to go snooping around a precinct.</i> 

<i>Even if it's incredibly barren here, it would look bad on your record if you're caught looking into other case files. Expecially outside of your jurisdiction. </i> 

<i>You continue waiting, and after what feels like an eternity, the Sheriff returns.</i>

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Yep, as expected! They said it'll take a while."
+ ["Is it just you here?"] -> solo
+ ["Exactly how long is a while?"] -> time

= solo
~ sincerity_level ++
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Nervous_1
"Yup! At the moment it's just me!"

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Neutral_2
"When are the other staff or patrol Units coming?" 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Inquisiting_1
"..."

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Annoyed_1
"Is it <i> just </i> you?! "

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Nervous_1
"Yep!" 

~ SetSpeaker(Speaker.Lupe)
"...Wow."
-> transition_to_case

= transition_to_case
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Well, since you're going to be here a while, 'Detective', I could use your help!"

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Serious_2
"My help?"

~ SetSpeaker(Speaker.Misra)
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

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Inquisiting_1
"Well, what are your thoughts?"
-> police_report

= time
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Inquisiting_1
"It's hard to say. The time tends to range between a couple hours to a couple...days." //pondering misra

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Fright_1
"DAYS?! "

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Nervous_1
"Yeah. Things happen a lot...slower here in Kettle Rock. But don't worry! I'll make sure they know it's urgent."

-> transition_to_case
->police_report

= police_report
~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Inquistive_1
"Is this the whole file?" 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Yeah!" 

+["There's nothing here."] -> nothingHere
+["Where's the rest?"] -> theRest

=theRest
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Curious_1 
"The rest?" 

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Annoyed_1
"Yes! The rest of the file!" 

->nothingHere

=nothingHere
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Curious_1 
"What do you mean?" 

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Annoyed_1
"I mean this isn't a case. This is a bunch of bull-"

~ SetSpeaker(Speaker.Misra) 
"No, no. The case is right there! What else is there supposed to be?" 

~ SetSpeaker(Speaker.Lupe) 
"Oh, I don't know. Maybe the actual problem you're trying to solve." 

~ SetSpeaker(Speaker.Lupe) 
"Or witnesses." 

~ SetSpeaker(Speaker.Lupe) 
"Or anything else pertaining to the case." 

~ SetSpeaker(Speaker.Lupe) 
"I mean, there's some stuff there, sure, but it's flimsy."

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Inquisiting_1 
"Oh." 

~ SetSpeaker(Speaker.Lupe) 
"Oh?" 

~ SetSpeaker(Speaker.Misra)
"Yeah, I just didn't know what goes into a case file. "

~ SetSpeaker(Speaker.Lupe)
"..."

~ SetSpeaker(Speaker.Lupe)
"How long have you been a Sheriff?" 

~ SetSpeaker(Speaker.Misra)
"Uhhhhhhhh..."

~ SetSpeaker(Speaker.Misra)
"A week." 

~ SetSpeaker(Speaker.Lupe) 
"..."
~ SetSpeaker(Speaker.Lupe) 
"And no one is here to help you?" 

<i>Misra looks around the station.</i>

~ SetSpeaker(Speaker.Misra) 
"Nope. Just me, myself, and I!" 

~ SetSpeaker(Speaker.Lupe) 
"..."

~ SetSpeaker(Speaker.Lupe) 
"Well you're going to get nowhere with this." 

~ SetSpeaker(Speaker.Misra) 
"Well then what do I need?" 
+["Evidence."] ->Evidence 
+["Witnesses."]->Witnesses 
+["Help."] -> Help

=Evidence 
~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Serious_2
"Well for starters, evidence. You're not sure who placed this call, so any clues will narrow down suspects." 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"So we go to the Winery and look around!" 

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Inquistive_1
"I'm sorry, 'we'?" 
-> We 

=Witnesses
~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Inquistive_1
"Well for starters, witnesses. If something was going down, someone must have heard or seen something." 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"So, go into town and question people!"


~ SetSpeaker(Speaker.Lupe) 
"No. First go to the crime scene, see what evidence you can gather before questioning." 

~ SetSpeaker(Speaker.Misra) 
"So we go to the Winery!" 

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Inquistive_1
"I'm sorry, 'we'?" 
-> We 

= Help 
~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Serious_2
"What you need is someone to help you on this case. You're too fresh to handle a cold case." 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Nervous_1
"Someone like yourself?" 

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Inquistive_1
"I'm sorry?" 
-> We 

=We
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Yeah! I mean, what else are you going to do in the meantime?"

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Fluster_1
"I mean...I don't know." 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Come on! What if reality hangs in the balance, and we must solve this in order to save the world!! Woaaaahhhh!!! It's your destinyyyyyyyy!"

~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Neutral_2
{ tease_level > sincerity_level:  <i>Slight laughter escapes your nose.</i>  | "... "}

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
{ tease_level > sincerity_level:  "AH HA! Gotcha laughing!" | "Anyways."}

~ SetSpeaker(Speaker.Lupe) 
"Fine." 

~ SetSpeaker(Speaker.Misra) 
"Fine?" 

~ SetSpeaker(Speaker.Lupe) 
"I'll help you." 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Surprised_2
{ tease_level > sincerity_level:  "HELL YEAH!" | "Oh! Thank you!" }

~ SetSpeaker(Speaker.Lupe) 
"Let me just make a quick call first." 

~ case_file_received = true
// TO PROGRAMMERS: ADD EVIDENCE TO SYNTHESIS GAME! 

~ SetSpeaker(Speaker.Lupe) 
* "[It's a simple case."] Shouldn't take me more than a day to figure this out." 
-> phone_call
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
~ SetSpeaker(Speaker.Lupe)
# emote : Lupe \| Inquistive_1
"Thelton, Lupe here. Currently stuck in Kettle Rock. Should be back in Dafenport by tomorrow morning. I'll keep you updated." 

<i>You hang up.</i> 

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"So, what's the plan?" 

~ SetSpeaker(Speaker.Lupe)
"Well I'm going to need to see this place itself. Can't really figure out much from this file alone." 

~ SetSpeaker(Speaker.Misra)
"Alright!! Road trip!!" 

<i>They spring out from behind the desk, jokingly gesturing towards the door. </i>

~ SetSpeaker(Speaker.Misra)
"Eh-shall we?"

*["We shall."] ->JokeBack
*[Walk through door.] -> door 

= JokeBack
# emote : Lupe \| Fluster_1
<i>That slight joke may have drained your social battery for the week.</i> 

<i>Misra giggles as they walk out the door.</i>

~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Alright! Misra and Lupe on the case! Watch out Kettle Rock, we gon'na figure you OUT!" 
~ ChangeGameScene("scene3_1") 
-> DONE

= door 
# hide : Lupe
<i>You walk through the door, not saying a word to Misra.</i> 
~ SetSpeaker(Speaker.Misra)
# emote : Misra \| Neutral_1
"Alright! Misra and Lupe on the case! Watch out Kettle Rock, we gon'na figure you OUT!"
~ ChangeGameScene("scene3_1") 
-> DONE