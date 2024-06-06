// SCENE 5.1: Buy Me a Drink?
// NOTES: this whole scene happens in the Dating sim style! (Cannot access this location like other locations)
//        this also appears to be a sort of cutscene so there isnt much logic to do! :)

// VARIABLES HERE:
// VAR reported_incident = false
// VAR case_file_received = false
// VAR love_points = 0
// VAR tease_level = 0
// VAR sincerity_level = 0

* [Scene 5.1 - The Stakeout] -> scene5_1_DS

=== scene5_1_DS ===
# name: description
# emote : Lupe \| Neutral_2
# emote : Misra \| Neutral_1
<i>You sit in the car in silence, keeping a close eye on the winery.</i>

# name: Misra
"..."

#name: Lupe
"..."

#name: Misra 
# emote : Misra \| Nervous_1
"This is alot more boring than in the movies." 

#name: Lupe 
+["Get used to it."] -> usedToIt
+["Would you like some music?"] -> music 

= usedToIt 
#name: Lupe 
# emote : Lupe \| Serious_2
"Get used to it. Detectives do this alot." 

#name: Misra 
"..." 
"Can we at least put on some music?" 

+["No."] -> turnOffRadio 
+["Sure."] -> music 

= music
~ tease_level++
#name: Misra
# emote : Misra \| Neutral_1
{usedToIt: "Awesome!"}

<i>Misra immedinately turns on the radio and adjusts it to a station. A chirpy radio broadcaster's voice blasts through the car before you turn down the volume.</i>  

"This is 99.7! Here with the latest hit from Britney Smears!" 

<i>The station erupts with sound effects before the song plays. </i>

#name: Misra 
"I love Britney Smears! How about you?" 

#name: Lupe 
"Never heard of her." 

#name: Misra 
# emote : Lupe \| Neutral_2
# emote : Misra \| Neutral_1
"What?! Ok, then what kind of music do you listen to?" 
+["Classical."] -> classical
+["Pop."] -> pop 
+[Turn off the radio.] -> turnOffRadio

= classical 
~ sincerity_level ++
#name: Misra 
# emote : Misra \| Surprised_2
"Classical?! Who's favorite genre is classical?!" 

#name: Lupe 
"Mine." 

#name: Misra 
# emote : Misra \| Neutral_1
"Then what's your favorite song? Beethoven the 28th?" 

#name: Lupe 
# emote : Lupe \| Neutral_2
{ tease_level > sincerity_level:  "It's actually 'Distant Light' by Peteris Vasks."  | "I don't have a favorite." }

#name: Misra
# emote : Misra \| Neutral_1
{ tease_level > sincerity_level:  "Oh, I didn't know that."  | "...that's just sad." } 

#name: Misra 
# emote : Misra \| Nervous_1
"So who do you think we're going to see in the winery?"

-> questions_bar_5_1

= pop 
~ tease_level++
#name: Misra
# emote : Misra \| Inquisiting_1
"So you do listen to pop, but have never heard of Britney Smears?" 

#name: Lupe 
# emote : Lupe \| Inquistive_1
"Never."

#name: Misra 
"Then who do you listen to?!" 
+["Radiobutt."] -> youCool
+["Blue Hot Chili Peppers."] -> youCool
+["Green Night."] -> youCool 

=youCool
#name: Misra 
# emote : Misra \| Curious_1
"Ah, you're into the rockers!" 

#name: Lupe 
# emote : Lupe \| Neutral_2
"Yeah." 

#name: Misra 
# emote : Misra \| Neutral_1
"Nevermind! You're cool once again!" 

#name: Lupe
{ tease_level > sincerity_level:  "You thought I was cool?"  | "Thanks." }

#name: Misra 
{ tease_level > sincerity_level:  "Only a little bit; you've redeemed your slightly cool status!"  | "..." }

#name: Misra 
"..."

#name: Lupe
"..."

#name: Misra 
# emote : Misra \| Nervous_1
"So who do you think we're going to see in the winery?"
-> questions_bar_5_1

=turnOffRadio 
{music: <i> You turn off the radio.</i>}

#name: Misra
# emote : Misra \| Surprised_2
"Come on!!" 

#name: Lupe 
# emote : Lupe \| Serious_2
"We need to remain quiet. The perpetrator can't know we're here if we want this to work." 

#name: Misra 
# emote : Misra \| Neutral_1
"Fine, guess we'll sit in silence." 

# name: Misra
"..."

#name: Lupe
"..."

#name: Misra 
# emote : Misra \| Curious_1 
"So who do you think we're going to see?"
-> questions_bar_5_1

-> questions_bar_5_1

==questions_bar_5_1==
+["The 'goats'."] -> goats
+["The teens."] -> teens_bar_5_1 

==goats== 
#name: Misra 
# emote : Misra \| Curious_1
"...you think?" 

#name: Lupe 
# emote : Lupe \| Serious_2
"Maybe. It's a shot in the dark, but it's a possibility." 

#name: Misra 
"Hmph." 

#name: Misra 
# emote : Misra \| Nervous_1
"By the way, 'goats' is an insulting name to those who disappeared." 

#name: Misra 
"It's best to not call them that." 

+["Sorry."] -> didntKnowBetter 
+["Why does Kettle Rock have an insult for missing people?"] -> insulted 

=didntKnowBetter 
#name: Misra 
"It's alright, you didn't know any better!" 

#name: Misra 
"Just something to keep in mind." 

#name: Lupe 
"Say, what <i> was </i> that group?" 

#name: Misra 
# emote : Misra \| Curious_1
"What do you mean?" 

#name: Lupe 
# emote : Lupe \| Inquistive_1
"I mean there have been many terms tossed around." 

#name: Lupe 
"Goats..." 

#name: Lupe 
"Council..." 

#name: Lupe 
"Sacrifice..." 

#name: Lupe 
"Were they some sort of cult?" 

#name: Misra 
# emote : Misra \| Fright_2
"Cult is a strong term, but I guess..." 

#name: Misra 
"But yeah, you could consider them a cult..." 

->misguided

= insulted 
#name: Misra 
# emote : Misra \| Nervous_1
"'Cause people think they're insane." 

#name: Misra 
"Actually, it's more like people want to believe they were insane to give reasoning behind their disappearances."

#name: Lupe
# emote : Lupe \| Inquistive_1
"And were they not? They sounded like some sort of cult." 

#name: Misra
# emote : Misra \| Fright_2
"..." 

#name: Lupe 
{ tease_level > sincerity_level: "I'm sorry if I'm overstepping, just trying to get an understanding if it this pertains to the case." | "Misra?" }

#name: Misra

{ tease_level > sincerity_level: "It's ok it's just..." | "..." }

-> misguided 

=misguided 
# name: Misra
# emote : Misra \| Nervous_1
"They were misguided more than anything." 

#name: Lupe 
"How so?" 

#name: Misra 
"People here have a tendency to talk on their behalf after the tragedy..." 

#name: Misra 
"And I've heard through the grapevine that the Council was convinced their sacrifice would bring prosperity back to Kettle Rock." 

#name: Lupe 
"Hmmm..." 

#name: Lupe 
"And what? Do you believe that they instead set loose some sort of curse?" 

#name: Misra 
# emote : Misra \| Surprised_2
"Of course not!" 

#name: Misra 
"Do you?" 

+["Yes, maybe."] -> yeah
+["No."] -> no

=yeah 
#name: Lupe
# emote : Lupe \| Inquistive_1
"Yes, maybe." 

#name: Lupe
# emote : Lupe \| Annoyed_1
"I'm not much for magic, but who knows what's out there?"

#name: Misra 
# emote : Misra \| Nervous_1
"Yeah...I guess so." 

-> Preserving

=no 
#name: Lupe
# emote : Lupe \| Annoyed_1
"No. Curses aren't real." 

#name: Misra 
# emote : Misra \| Nervous_1
"Hmm, yeah maybe not." 

-> Preserving

=Preserving 

#name: Lupe
# emote : Lupe \| Inquistive_1
"But if what you say is true, there's a possibility that the remaining members of this supposed 'Council' are trying to preserve the town." 

"And maybe their 'preservation methods' have something to do with this winery." 

#name: Misra 
# emote : Misra \| Nervous_1
"Yeah, maybe." 

#name: Misra 
"..." 

#name: Misra 
"Would be kinda nice, wouldn't it?" 

#name: Lupe 
# emote : Lupe \| Inquistive_1
"What? { tease_level > sincerity_level: A giant group sacrifice?}" 

#name: Misra 
{ tease_level > sincerity_level: "Yeah! Sure! Let me just quickly get the KoolAid! I'm just joking."} 

#name: Misra 
{ tease_level > sincerity_level: "But..."} 

#name: Misra 
"If we did find some way to help out little ol' Kettle Rock, that could be nice." 

+["Could be."]-> couldBe 
+["Or leave."]-> leave

=couldBe 
#name: Lupe 
# emote : Lupe \| Neutral_2
"Could be. You all seem like lovely people." 

#name: Misra 
# emote : Misra \| Neutral_1
"Yeah. Alot of us here are just looking for a quiet place to call home. You know?" 

#name: Lupe 
# emote : Lupe \| Neutral_2
"I hear you..." 

+["And I wish you all the best."] -> allTheBest
+["But is that realistic here?"] -> realistic

=allTheBest 
#name: Misra 
# emote : Misra \| Curious_1
"Thank you, Lupe! That means alot." 

{ tease_level > sincerity_level: "Of course" | "It's <i> Detective </i> Lupe." }

-> RuhRoah

=realistic
#name: Misra 
# emote : Misra \| Curious_1
"What do you mean?" 

#name: Lupe 
# emote : Lupe \| Inquistive_1
"Seems like 'little ol' Kettle Rock' has had { tease_level > sincerity_level: ...well a <i> rocky </i> history." | a lot of turbulance." }

#name: Misra  
# emote : Misra \| Nervous_1
{ tease_level > sincerity_level: "Ha ha! I see what you did there." | "Yeah, it's had ups and downs." }

->leave2

=leave 
#name: Misra 
# emote : Misra \| Curious_1
"What?" 

-> leave2

=leave2
#name: Lupe 
# emote : Lupe \| Inquistive_1
"I'm just saying that..." 

#name: Lupe 
"Curse or not, I understand why people are leaving." 

#name: Lupe 
"Seems like citizens here are struggling to get by."

#name: Lupe
"So it makes sense if they would like to look for...something better?"

#name: Misra 
"Even if this place is our home?" 

#name: Lupe 
"Yes, even if they consider this place home." 

#name: Misra 
"..."
-> strangeQuestion 

=strangeQuestion
#name: Misra 
# emote : Misra \| Curious_1
"If your home was going through a hard time, would you leave?" 

+["Yes."] -> homeYes
+["No."] -> homeNo 

=homeYes 
#name: Misra 
# emote : Misra \| Surprised_2
"Really?" 

#name: Lupe 
# emote : Lupe \| Neutral_2
# emote : Misra \| Neutral_1
"It's not pleasent, but sometimes things happen." 

#name: Lupe 
"Change is..." 

+["Scary."] -> scary 
+["Hard."] -> hard
+["Normal."] -> normal

= scary
#name: Lupe
"Change is scary because it's a dive into the unknown that most people are hesitant  to take..." 
-> preSound

= hard 
#name: Lupe 
"Change is hard because you have to willingly accept it before you can move on..."
->preSound

=normal
~ sincerity_level++
#name: Lupe 
"Change is normal. As absurd as it sounds, it's just a part of life we have no control over." 

#name: Lupe 
"Plants change..." 

#name: Lupe 
"The sky changes..." 

#name: Lupe 
"People change..." 

#name: Lupe 
"Places change..." 

->preSound

=preSound
#name: Lupe 
"But it's what's needed to progress life towards something better." 

#name: Lupe 
"Don't you think so?" 

#name: Misra 
# emote : Misra \| Curious_1
# emote : Lupe \| Annoyed_1
"Well is this change necessary? I mean it feels like everyone's running for the hills when we can be-" 

->RuhRoah 

=homeNo 
#name: Misra
# emote : Misra \| Neutral_1
{goats: "See?"}
{teens_bar_5_1: "Ok! So I'm not crazy."} 

#name: Lupe
# emote : Lupe \| Inquistive_1
"But this town is...different." 

#name: Misra 
"How?" 

#name: Lupe 
"I don't know. Something is...off." 

#name: Misra 
# emote : Misra \| Curious_1
"What do you mean by 'off'?" 

#name: Lupe 
"Maybe I'm having some sort of deja vu..."

#name: Lupe 
"Somethings been strange since I've first stepped foot in this town."

#name: Lupe 
"It all feels...familar." 

#name: Lupe 
"..."

#name: Lupe 
"<b>You</b> feel...familiar, somehow. Like I've met you before. Or someone like you, at least." 

->RuhRoah

==teens_bar_5_1==
#name: Lupe 
# emote : Lupe \| Inquistive_1
"It's probably those teens." 

#name: Misra 
# emote : Misra \| Curious_1
"What do you think they're doing?" 

#name: Misra 
"Drinking?" 

#name: Misra 
"Smoking?" 

#name: Misra 
"Graffiti?" 

#name: Lupe
{ tease_level > sincerity_level: "Maybe all three at the same time." | "Don't know. Teens can be unpredictable." }

#name: Misra 
{ tease_level > sincerity_level: "I don't think they have enough arms to do that!" | "You're right." }

{ tease_level > sincerity_level: <i> You lightly laugh to yourself </i>}

#name: Lupe
# emote : Lupe \| Neutral_2
"..." 

#name: Misra
# emote : Misra \| Neutral_1
"..." 

#name: Misra
{ tease_level > sincerity_level: "Can I ask you a strange question?" | ->RuhRoah }

{ tease_level > sincerity_level: 

+["Sure."] -> goats.strangeQuestion

+["Please don't."] -> ratherNot
}

=ratherNot
#name: Misra 
# emote : Misra \| Nervous_1
"Ok!" 

"..." 

-> RuhRoah

==RuhRoah==
//INSERT BIG BASH SOUND 

#name: Lupe 
# emote : Lupe \| Fright_1
# emote : Misra \| Surprised_2
"What was that!?" 

# hide : Misra
#name: Lupe 
"Misra?!" 

#name: Lupe 
"Misra, what are you doing?! Get back here!" 
~ ChangeGameScene("scene5_2")
-> DONE