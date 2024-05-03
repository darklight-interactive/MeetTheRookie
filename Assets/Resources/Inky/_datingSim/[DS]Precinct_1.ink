// SCENE 2.1: The Kettle Rock Police Station
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

// not sure how the differing responses will work... note to self: add different dialogue options for Lupe once dating sim is more fleshed out!


* [BEGIN] -> intro

=== intro ===
# name: description
You open the door to the precinct and are met with... no one. The place is frozen in time, with dust collecting on the window sills. 

+ [Ding the bell] -> ding

+ [Call out] -> hello

= ding

You ding the bell and no one responds.

-> soul

= hello
# name : Lupe
Hello? 

... No response.
-> soul

= soul
# name : Lupe
Is there a single person in this town that's helpful?


# name : Lupe
"Hmph... well can't say I didn't try. I'm wasting my time here anywa-"


# name : Misra
Hey!

# name : Lupe
AYE! DIOS MIO!

# name : Misra
Sorry for the wait! We're a bit understaffed.

# name : Misra
...You ok?

// Also: [Chastise them for scaring you], [Play it off.] 
+ ["You almost gave me a heart attack!"] -> chastise //scared
+ ["Yeah, I'm fine."] -> scared //sarcastic expression

= chastise 
~ sincerity_level ++
#name : Misra
Oh! I'm truly sorry!

Their words stutter from holding back a laugh.

#name: Misra
I didn't mean to scare you. //laughing

#name: Lupe
Oh, ha ha, very funny. //sarcastic

#name : Misra
And to whom do I owe the pleasure of almost sending into cardiac arrest?
-> introductions

= scared
~ tease_level ++
# name : Misra
Oh really? I must have been confused then.

# name : Misra
I don't understand spanish, but last I checked "AYE! DIOS MIO!" means scared. 

# name : Lupe
I wasn't scared. Just surprised. That's all.

# name : Misra
Yeah, yeah! //laughing

# name : Misra
Well, does this fearless person have a name?
-> introductions

= introductions
# name : Lupe
Detective Lupe. And you are...?

# name : Misra
Sheriff Misra! Nice to meet you, Lupe!
//also : [Shake their hand in silence], [Return the greeting]
+ [Shake their hand in silence.] -> silence
+ ["Nice to meet you too."] -> niceToMeet

= silence
You attempt to ignore that they've already decided to drop your title as "Detective" and shake their hand.

#name: Misra 
...

#name: Lupe 
...

#name: Misra
Yep...very nice to meet you.  

 -> precinct

=niceToMeet
~ sincerity_level ++
 You grab their hand, and they give a vigorous shake you can feel up to your shoulder. Despite how much it bothers you, you attempt to ignore that they've already decided to drop your title as "Detective".

-> precinct

= precinct

#name: Misra
So, what's the issue? Normally folks don't come here just to chit-chat. 
+["Is it illegal to chit-chat?"] -> Company
+["A tree fell."] -> treeFell
= Company
~ tease_level++
# name : Misra
Not here, and I'm not opposed to some company. //flirty

//flustured Lupe
Your train of thought scatters for a brief moment with a slight raise of your heartrate.

How...strange. 

Why would you say that?!

Pull it together! What happened to your professionalism?!

You clear your throat.

->treeFell

= treeFell

# name : Lupe
There's a tree blocking the road out of town by the gas station. I need it cleared as soon as possible. 
# name : Misra
{ tease_level > sincerity_level:  Leaving already?! | Ah, I see. } Well, I'll call the tree people and get that cleared out. 

May take a while though! As you may have noticed, we don't have many people who can help around here. 
//they gesture to an empty room
# name : Misra
{ sincerity_level >= tease_level: Sorry about that! } { tease_level >= sincerity_level:  Looks like we might get to know each other after all. } 

They excuse themselves to make the call.

You wait patiently...
and wait...
and keep waiting...
...
There has to be something to do around here.
+ [Snoop around.] -> snoop
+ [Keep waiting.] -> wait

= snoop
~ tease_level++
You lean over the front desk to see where the Sheriff went, but they're nowhere to be seen. They probably entered a different room to make the call. What you do see, however, is a manilla folder with a label: "Unexplained Disturbance at Old Winery." 

The curiosity in the back of your mind calls for you to take just a quick look. Who can resist an unsolved mystery? 

+ [Take a peek.] -> peek
+ [Don't be ridiculous.] -> wait

= peek
~ tease_level++
~ snooped = true
You quietly reach over the front desk and take a look at the case.

You realize that <i>case</i> is an overstatement for what's in this folder. 

/* 
- Date: August 29th, 1995
- Time: 1:02 AM
- Type: Anonymous Complaint
- Transcription is as follows:
    - *“Hey man, there’s something happening at the Winery on the hill.* *There’s these crazy loud noises and some weird lights going on—not sure what’s up, isn’t that place abandoned? Weird. Squash out.”*

And a black and white picture of the winery stained with age.
*/

# name : Misra
"So...thoughts?" 

# name : Lupe
"AYE!!"

"You CAN'T keep scaring me like that."
# name : Misra
{ 
- spooked: 
Sorry, sorry! 
- else:
    So, you admit you were scared! 
}

They're going to give you a headache. 

#name: Misra 
I'd also like to point out you probably just broke some kind of law.

# name: Lupe 
My apologies.

You try to hand the file back to them, but they push it towards your direction.

# name : Misra
No! No worries, "Detective"! You didn't answer my question... What are your thoughts?
-> police_report


= wait
~ sincerity_level++
You may be bored out of your mind, but not crazy enough to go snooping around a precinct. Even if it's incredibly barren here, it would look bad on your record if you're caught looking into other case files. Expecially outside of your jurisdiction.  

You continue waiting, and after what feels like an eternity the Sheriff returns.

#name : Misra
Yep, as expected! They said it'll take a while.
+ ["Is it just you here?"] -> solo
+ ["Exactly how long is a while?"] -> time

= solo
~ sincerity_level ++
# name : Misra
Yup! At the moment it's just me!

# name : Lupe
When are the other Staff or Patrol Units coming? 

# name : Misra
...

# name : Lupe
Is it <i> just </i> you?! 

# name : Misra
Yep! 


# name : Lupe
...Wow.
-> transition_to_case

= transition_to_case
# name : Misra
Well, since you're going to be here a while, "Detective", I could use your help!

#name: Lupe 
My help?

#name: Misra
Yeah! We got a bit of a cold-case going on. 

They hand you a thin manilla folder.

/* 
Date: August 29th, 1995
- Time: 1:02 AM
- Type: Anonymous Complaint
- Transcription is as follows:
    - *“Hey man, there’s something happening at the Winery on the hill.* *There’s these crazy loud noises and some weird lights going on—not sure what’s up, isn’t that place abandoned? Weird. Squash out.”*

And a black and white picture of the winery stained with age.
*/
# name : Misra

Well, what are your thoughts?
-> police_report

= time
#name : Misra
It's hard to say. The time tends to range between a couple hours to a couple...days. //pondering misra

#name: Lupe 
DAYS?! 

#name: Misra
Yeah. Things happen a lot...slower here in Kettle Rock. But don't worry! I'll make sure they know it's urgent. 

-> transition_to_case
->police_report

= police_report

Is this the whole file? 

#name: Misra
Yeah! 

+["There's nothing here"] -> nothingHere
+["Where's the rest?"] -> theRest

=theRest 
#name: misra 
The rest? 

#name: Lupe 
Yes! The rest of the file! 

->nothingHere

=nothingHere
#name: Misra 
What do you mean? 

#name:Lupe 
I mean this isn't a case. This is a bunch of bull-

#name: Misra 
No, no. The case is right there! What else is there supposed to be? 

#name: Lupe 
Oh, I don't know. Maybe the actual problem you're trying to solve. 

Or witnesses. 

Or anything else pertaining to the case. 

I mean, there's some stuff there, sure, but it's flimsy.

#name: Misra 
Oh. 

#name: Lupe 
Oh? 

#name: Misra
Yeah, I just didn't know what goes into a case file. 

#name: Lupe
...
How long have you been a Sheriff? 

#name: Misra
Uhhhhhhhh

A week. 

#name: Lupe 
...
And no one is here to help you? 

Misra looks around the station.

#name: Misra 
Yep. Just me, myself, and I! 

#name: Lupe 
...
Well you're going to get nowhere with this. 

#name: Misra 
Well then what do I need? 
+["Evidence."] ->Evidence 
+["Witnesses."]->Witnesses 
+["Help."] -> Help

=Evidence 
#name: Lupe 
Well for starters, evidence. You're not sure who placed this call, so any clues will narrow down suspects. 

#name: Misra 
So we go to the Winery and look around! 

#name: Lupe 
I'm sorry, we? 
-> We 

=Witnesses
#name: Lupe 
Well for starters, witnesses. If something was going down, someone must have heard or seen something. 

#name: Misra 
So go into town and ask people!

#name: Lupe 
No. First go to the crime scene, see what evidence you can gather before questioning. 

#name: Misra 
So we go to the Winery! 

#name: Lupe 
I'm sorry, we? 
-> We 

= Help 
#name: Lupe 
What you need is someone to help you on this case. You're too fresh to handle a cold case. 

#name: Misra 
Someone like yourself? 

#name: Lupe 
I'm sorry? 
-> We 

=We
#name: Misra 
Yeah! I mean what else are you going to do in the meantime? 

#name: Lupe 
I mean...I don't know. 

#name: Misra 
Come on! What if reality hangs in the balance, and we must figure this out to save the world!! Woaaaahhhh!!! It's your destinyyyyyyyy!

#name: Lupe 
{ tease_level > sincerity_level:  A slight laughter escapes your nose | ... }

#name:Misra 
{ tease_level > sincerity_level:  AH HA! Gotcha laughing! | Anyways.}

#name: Lupe 
Fine. 

#name: Misra 
Fine? 

#name:Lupe 
I'll help you. 

#name: Misra
{ tease_level > sincerity_level:  HELL YEAH! | Oh! Thank you! }

#name: Lupe 
Let me just make a quick call first. 
#name: Misra 
// TATI NOTE: THIS IS WHERE I LEFT OFF

~ case_file_received = true
// TO PROGRAMMERS: ADD EVIDENCE TO SYNTHESIS GAME! 

* "[It's a simple case."] Shouldn't take me more than a day to figure this out." 
-> phone_call
+ [Call your boss]
    -> phone_call
    
= phone_call    
You {phone_call < 0: grab your phone and begin to} dial chief Thelton. 

The phone rings...

And rings...

And rings...

"You've reached Chief Detective Inspector Thelton, Boise Precinct. I'm not available right now. You know what to do." 

Weird...

She always picks up. 

+[Call again] -> phone_call
+[Leave a voicemail] -> voicemail

= voicemail
# name : Lupe
"Thelton, Lupe here. Currently stuck in Kettle Rock. Should be back in Dafenport by tomorrow morning. I'll keep you updated." 

You hang up. 

# name : Misra
"So, what's the plan?" 

# name: Lupe 
"Well I'm going to need to see this place itself. Can't really figure out much from this file alone." 

# name : Misra
"Alright!! Road trip!!" 

They spring out from behind the desk, jokingly gesturing towards the door. 

# name : Misra
"Eh-shall we?"

*"We shall." ->JokeBack
*[Walk through door] -> door 

= JokeBack
That slight joke may have drained your social battery for the week. 

Misra giggles as they walk out the door

"Alright! Misra and Lupe on the case! Watch out Kettle Rock, we gon'na figure you OUT!" 
-> END

=door 
You walk through the door, not saying a word to Misra. 
# name : Misra
"Alright! Misra and Lupe on the case! Watch out Kettle Rock, we gon'na figure you OUT!" 
-> DONE