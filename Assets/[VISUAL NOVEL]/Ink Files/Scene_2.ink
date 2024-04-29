// SCENE 2.1: The Kettle Rock Police Station
// NOTES: this whole scene happens in the Dating sim style! (Cannot access this location like other locations)
//        this also appears to be a sort of cutscene so there isnt much logic to do! :)
//Add voice memo 

// VARIABLES HERE:
VAR reported_incident = false
VAR case_file_received = false
VAR love_points = 0

// not sure how the differing responses will work... note to self: add different dialogue options for Lupe once dating sim is more fleshed out!


* [BEGIN] -> intro

=== intro ===
You open the door to the precinct, feeling the rusty hinges struggle to keep the door from scratching the floor. To say that this place is abandoned wouldn't be right, as the dustless desks give the space some form of life. However, the absence of...well people would argue otherwise.  

# name : Lupe
"Hello?" 

...

# name : Lupe
"Hel-" 

A hand is placed on your shoulder. 

"AUGH!" 

# name : Misra
"WOAH!" 

You both jump back.

+ ["You scared me."] -> introductions
+ ["Hello."] -> scared


= scared

# name : Misra
"Hey! Woah! Sorry, didn't mean to scare you!" 

+["I wasn't scared."] -> wasntScared 
+["..."] -> introductions

=wasntScared
~ love_points ++
# name : Misra
"Really? You did scream pretty loud. 

# name : Misra
"I think the whole town heard you." 

They laugh to themselves, their cheeks with a slight hint of red. 

#name: Lupe
"I- well- I'm sorry, who are you?!" 

-> introductions

= introductions
//meow meow add their introductions here!

# name : Misra
"Oh! Sorry! I'm Misra! It's nice to meet you!"

+ [Shake their hand in silence] -> silence 
+ ["Detective Lupe."] -> nameCorrection
+ ["Nice to meet you too."] -> niceToMeet

= silence
You shake their hand and watch their smile slightly curl in silence. 
# name : Misra
"Yeah...yeah." 
You let go of their hand. 
...
# name : Misra
 -> precinct

=niceToMeet
~ love_points ++
You grab their hand, and they give a vigorous shake you can feel up to your shoulder. 
->precinct

= nameCorrection 
# name : Misra
"Right! Right! Detective Lupe! Say, do you mind if I call you just...Lupe? I mean you don't have to call me <i> sheriff </i> Misra so let's just drop the formalities?" 

* ["Sure."]

# name : Misra
"Alright!! Lupe it is!!" 

-> precinct

= precinct

# name : Misra
"So! What brings you into town?"

They gesture at a desk with chairs adjacent to each other. 

You sit down on one side, and watch them fling themselves into the other chair. 

Some of the wheels lift off the ground as they flail to re-gain their balance, eventually grabbing onto the desk and pulling themselves to stability.

+[Laugh] -> laugh 
+[Don't laugh] ->dontLaugh

=laugh
~ love_points ++
You let out a light puff of air from your nose and uninentionally make eye contact. 
Their ears slightly glow red. 

# name : Misra
"Sorry! Sorry!" 

...

# name : Misra
"That was...a bit embarassing" 

* ["It was a little funny."]

# name : Misra
"Yeah, I guess so!" 
    -> questions

=dontLaugh

Who is this-?! 

...

You can already hear her nagging voice telling you "You should stop calling people morons, Lupe."

"You need to give people chances, Lupe."

"I can't bail you out of HR again, Lupe." 

...

This is going to be a challenge. 

You unintentionally make eye contact with Misra. 

# name : Misra
"Sorry! Sorry!" 

...

"That was...a bit embarassing" 

-> questions 

=questions 
*["A tree fell on the way here."] -> fallen_tree
*["I could use some coffee."] -> coffee
*["Where's the case file?"] -> police_report

=coffee
# name : Misra
"Yeah, there's a coffee pot over there."

You get up from your desk and walk over to the coffee pot. It's lukewarm, and you could've sworn there was a fly in there. Maybe your mind is just playing tricks on you. 

*[Pour yourself a coffee] -> selfishCoffee 
*{love_points > 0}[Offer Misra a coffee] -> MisraCoffee 
*[Go back to desk] -> RejectCoffee 

= RejectCoffee
It's probably best you don't drink this. You have a case to solve, and the last thing you need is a fly in your stomach. 

Heading back from the desk, Misra makes eye contact with you. 

->questions

=selfishCoffee
You pour yourself a black cup of coffee, hoping that the possible fly didn't get in there. 

Sitting back down at your desk, you watch Misra get up and walk over to the coffee table. 

They begin pouring enough creamer and sugar to the point where the liquid is practically white. There's hardly a difference between that concoction and a sugary cup of milk. 

They sit across from you once again. 

->questions

=MisraCoffee 
# name : Lupe
"Misra?"

# name : Misra
"Yeah?"

# name : Lupe
"Coffee?"

# name : Misra
"Oh! Sure!"

You pour two black cups of coffee. 
*[Go back to desk] -> BlackCoffeeMisra
*[Add creamer to Misra's] -> MisraCoffeeWrong
*[Add sugar to Misra's] -> MisraCoffeeWrong
*{love_points > 2}[Create a concoction of sugar and creamer] -> MisraCoffeeRight 

=BlackCoffeeMisra 
You grab the two cups, head back to the desk, and put the black cup in front of Misra. 

# name : Misra
"Thanks!"

They look into the cup. 

# name : Misra
"Oh."

# name : Lupe
"Is something wrong?"

# name : Misra
"No! No! It's just...one second."

You watch Misra get up and walk over to the coffee table. 

They begin pouring enough creamer and sugar to the point where the coffee is practically white. There's hardly a difference between that concoction and a sugary cup of milk. 

They sit across from you once again. 

# name : Misra
"Thanks again, I do appreciate it!"

...

->questions

= MisraCoffeeWrong

You take an educated guess that Misra may not like their coffee black. They would appreciate you adding some sweeteners. 

After making some adjustments, you head back to the desk, and put the cup in front of Misra. 

# name : Misra
"Thanks!"

They take a sip. 

# name : Misra
"Oh."

# name : Lupe
"Is something wrong?"

# name : Misra
"No! No! It's just...one second."

You watch Misra get up and walk over to the coffee table. 

They begin pouring enough creamer and sugar to the point where the coffee is practically white. There's hardly a difference between that concoction and a sugary cup of milk. 

They sit across from you once again. 

# name : Misra
"Thanks again, I do appreciate it!"

...

->questions

= MisraCoffeeRight 

You take an educated guess that Misra may not like their coffee black. They would appreciate you adding some sweeteners. 

A gut feeling begins to arise, and you follow your intuition. 

3 sugar packets. 

3 creamer cups. 

You grab the two cups, head back to the desk, and put the cup in front of Misra.

# name : Misra
"Thanks!"

They take a sip. 

# name : Misra
"Oh."

# name : Lupe
"Is something wrong?"

They give you a bewildered look. 

# name : Misra
"No! No! It's...just how I like it. Thank you."

Predictable. 

->questions


= fallen_tree
# name : Lupe
"There's a tree blocking the road." 

# name: Misra 
"Oh, where?" 

# name : Lupe
"Not too far from here, maybe 2 minutes out." 
~ reported_incident = true

# name : Misra
"aw shit that sucks. i'll call the tree people here and get that cleared asap so you can get going. but just so you know that could take days, this down isn't exactly the most busy if you can tell." They gestured around the police station with open arms.

# name: Misra 
"Good to know! I'll file a report."

*["How long would it take to clear?] -> clear 
*[Ok.] -> questions

=clear
# name: Misra 
"Oh! Good question! Well, I'm not sure." 

They gesture around the station 

# name: Misra 
"We don't have many working hands here, so it may be a bit before that road gets cleared." 
* ["Right, that's why you folks called."] -> police_report
*["Hm."] Misra nods their head in silence, their lips curling into an awkward smile. -> questions
    -> police_report

->police_report

= police_report
# name : Misra
"Right! The case! Almost forgot about that, ha ha!"

They take out a thin file and place it in front of you.

Sitting back in the office chair {coffee > 0: and taking a sip of your coffee}, you begin to read through the 3 pages there.

Seems simple enough. 

[NAME] Winery, a rickety local winery that recently closed, is currently slated for demolition.

Anonymous noise complaint filed at 1:02 am. 

And a black and white picture of the winery stained with age. 

# name : Misra
"So...thoughts?" 

# name: Lupe 

"Not much to conclude right now, this is quite bare-bones information." 

"But I'll let my supervisor know what's going on to keep on record." 


~ case_file_received = true
// TO PROGRAMMERS: ADD EVIDENCE TO SYNTHESIS GAME! 

* "Seems [pretty simple."] like a pretty simple job. Shouldn't take me more than a day to figure this out." 
-> phone_call
+ [Call your boss]
    -> phone_call
    
= phone_call    
You {phone_call < 0: grab your phone and begin to} dial chief Thelton. 

The phone rings...

And rings...

And rings...

"Hello this is Sheryl Thelton. I currently can't come to the phone right now, but please leave a message after the beep. If this is Lupe, just call again like you usually do. I'll pick up eventually. Thank you." 

Weird...

She always picks up. 

+[Call again] -> phone_call
+[Leave a voicemail] -> voicemail

= voicemail
# name : Lupe
"Thelton, Lupe here. No cracks on the case yet but seems like may be a simple case. Should be back in Dafenport by tomorrow morning. I'll keep you updated." 

You hang up. 

# name : Misra
"So, what's the plan?" 

# name: Lupe 
"Well I'm going to need to see this place myself. Can't really figure out much from this file alone." 

# name : Misra
"Alright!! Road trip!!" 

They spring out of their desk, jokingly gesturing towards the door. 

# name : Misra
"Eh-shall we?"

*"We shall." ->JokeBack
*[Walk through door] -> door 
= JokeBack
That slight joke may have drained your social battery for the week. 
-> END
=door 
You walk through the door, not saying a word to Misra. 
# name : Misra
"Alright! Misra and Lupe on the case! Watch out Kettle Rock, we gon'na figure you OUT!" 
-> DONE