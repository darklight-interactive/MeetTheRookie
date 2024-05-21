// SCENE 5.1: Stakeout
// Location: Exterior Old Winery, Night
// NOTES: this whole scene happens in the Dating sim style! (Cannot access this location like other locations)
//

// VARIABLES HERE:
VAR love_points = 0
VAR tease_level = 0
VAR sincerity_level = 0



* [BEGIN] -> intro

=== intro ===
# name: description
You sit in the car in silence, keeping a close eye on the Winery.  

# name: Misra
"..."

#name: Lupe
"..."

#name: Misra 
"This is a lot more boring than the movies." 

#name: Lupe 
+["Get used to it."] -> usedToIt
+["Would you like some music?"] -> music 

= usedToIt 
#name: Lupe 
"Get used to it. Detectives do this kind of stuff a lot." 

#name: Misra 
"..." 
"Can we at least put on some music?" 

+["No."] -> turnOffRadio 
+["Sure."] -> music 

= music
~ tease_level++
#name: Misra 
{usedToIt: "Awesome!"}

Misra immedinately turns on the radio and adjusts it to a station. A chirpy radio broadcaster's voice blasts through the car before you turn down the volume.  

"This is 99.7! Here with the latest hit from Britney Smears!" 

The station erupts with sound effects before the song plays. 

#name: Misra 
"I love Britney Smears! How about you?" 

#name: Lupe 
"Never heard of her." 

#name: Misra 
"What?! Ok, then what kind of music do you listen to?" 
+["Classical."] -> classical
+["Pop."] -> pop 
+[Turn off the radio.] -> turnOffRadio

= classical 
~ sincerity_level ++
#name: Misra 
"Classical?! Who's favorite genre is classical?!" 

#name: Lupe 
"Mine." 

#name: Misra 
"Then what's your favorite song? Beethoven the 28th?" 

#name: Lupe 
{ tease_level > sincerity_level:  "It's actually 'Distant Light' by Peteris Vasks."  | "I don't have a favorite." }

#name: Misra
{ tease_level > sincerity_level:  "Oh, I didn't know that."  | "...that's just sad." } 

#name: Misra 
"So, any guess as to who we might catch up here, if anyone?"
-> questions

= pop 
~ tease_level++
#name: Misra
"So you do listen to pop but never heard of Britney Smears?" 

#name: Lupe 
"Never."

#name: Misra 
"Then who do you listen to?!" 
+["Radiobutt."] -> youCool
+["Blue Cold Chili Peppers."] -> youCool
+["Green Night."] -> youCool 

=youCool
#name: Misra 
"Ah, you're into the rockers!" 

#name: Lupe 
"Yeah." 

#name: Misra 
"Nevermind! You're cool once again!" 

#name: Lupe
{ tease_level > sincerity_level:  "You thought I was cool to begin with?"  | "Thanks." }

#name: Misra 
{ tease_level > sincerity_level:  "Only a little bit, you've redeemed your slightly cool status!"  | "..." }

#name: Misra 
"..."

#name: Lupe
"..."

#name: Misra 
"So, any guess as to who we might catch up here, if anyone?"
->questions

=turnOffRadio 
{music: You turn off the radio.}

#name: Misra 
"Come on!!" 

#name: Lupe 
"We need to be quiet. The perpetrator can't know we're here if we want this to work." 

#name: Misra 
"Fine, guess we'll sit in silence." 

# name: Misra
"..."

#name: Lupe
"..."

#name: Misra 
"So, any guess as to who we might catch up here, if anyone?"
->questions

-> questions

==questions==
+["The 'Goats'."] -> goats
+["Those teenagers."] -> teens 

==goats== 
#name: Misra 
"...you think?" 

#name: Lupe 
"Maybe. It's a shot in the dark, but it's a possibility." 

#name: Misra 
"Hmph." 

"By the way, 'Goats' is kind of an insulting name to call those who disappeared." 

"It's bad taste, and disrespectful." 

+["Sorry."] -> didntKnowBetter 
+["Why does Kettle Rock have an insult for missing people?"] -> insulted 

=didntKnowBetter 
#name: Misra 
"It's alright, you didn't know any better!" 

"Just something to keep in mind." 

#name: Lupe 
"Say, what <i> was </i> that group?" 

#name: Misra 
"What do you mean?" 

#name: Lupe 
"I mean there have been many terms tossed around." 

"Goats..." 

"Council..." 

"Sacrifice..." 

"Were they some sort of cult?" 

#name: Misra 
"Cult is a strong term, but I guess..." 

"Yeah you could consider them a cult..." 

->misguided

= insulted 
#name: Misra 
"'Cause people think they're insane." 

#name: Misra 
"Actually, it's more like people want to believe they were insane to give reasoning behind their disappearances."

#name: Lupe 
"And were they not? They sounded like some sort of cult." 

#name: Misra 
"..." 

#name: Lupe 
{ tease_level > sincerity_level: "I'm sorry if I'm overstepping, just trying to get an understanding in case if this pertains to the case." | "Misra?" }

#name: Misra

{ tease_level > sincerity_level: "It's ok, it's just..." | "..." }

-> misguided 

=misguided 

"They were misguided more than anything." 

#name: Lupe 
"How so?" 

#name: Misra 
"People here have a tendency to talk on their behalf after the tragedy..." 

"And I've heard through the grapevine that the Council was convinced that their sacrifice would bring prosperity back to Kettle Rock." 

#name: Lupe 
"Hmmm..." 

"And what? You believe that they triggered some sort of curse? Some sort of reaction for their dabblings into the forbidden, or something like that?" 

#name: Misra 
"Of course not!" 

"Do you?" 

+["Yes, maybe."] -> yeah
+["No."] -> no

=yeah 
#name: Lupe
"Maybe. I'm not much for magic, but who knows what's out there?"

#name: Misra 
"Yeah...I guess so." 

-> Preserving

=no 
"No. Curses aren't real." 

#name: Misra 
"Hmm, yeah maybe not." 

-> Preserving

=Preserving 

#name: Lupe 
"But if what you say is true, there's a possibility that if there are remaining members of this supposed 'Council' out there, they're still trying to preserve the town somehow." 

"And maybe their 'preservation methods' have something to do with this Winery." 

"Jenny did say that they used to come up here a lot."

#name: Misra 
"Yeah, maybe." 

"..." 

"Would be kinda nice, wouldn't it?" 

#name: Lupe 
"What? { tease_level > sincerity_level: A giant group sacrifice?}" 

#name: Misra 
{ tease_level > sincerity_level: "Yeah! Sure! Let me just quickly get the KoolAid! I'm just joking."} 

{ tease_level > sincerity_level: "But..."} 

"If we did find some way to help out little ol' Kettle Rock, that could be nice." 

+["Could be."]-> couldBe 
+["Or leave."]-> leave

=couldBe 
#name: Lupe 
"Could be. You all seem like lovely people." 

#name: Misra 
"Yeah. A lot of us here are just looking for a quiet place to call home. You know?" 

#name: Lupe 
"I hear you..." 

+["And I wish you all the best."] -> allTheBest
+["But is that realistic here?"] -> realistic

=allTheBest 
#name: Misra 
"Thank you, Lupe! That means alot." 

{ tease_level > sincerity_level: "Of course." | "It's <i> Detective </i> Lupe." }

-> RuhRoah

=realistic
#name: Misra  
"What do you mean?" 

#name: Lupe 
"Seems like 'little ol' Kettle Rock' has had { tease_level > sincerity_level: ...well a <i> rocky </i> history." | a lot of turbulance." }

#name: Misra  
{ tease_level > sincerity_level: "Ha ha! I see what you did there." | "Yeah, it's had ups and downs" }

->leave2

=leave 
#name: Misra 
"What?" 

-> leave2

=leave2
#name: Lupe 
"I'm just saying that..." 

"Curse or not, I understand why people are leaving." 

"Seems like citizens here are struggling these days."

"So it makes sense if they would like to look for...something better?"

#name: Misra 
"Even if this place is our home?" 

#name: Lupe 
"Yes, even if they consider this place home." 

#name: Misra 
"..."
-> strangeQuestion 

=strangeQuestion
"If your home was going through a hard time, would you leave?" 

+["Yes."] -> homeYes
+["No."] -> homeNo 

=homeYes 
#name: Misra 
"Really?" 

#name: Lupe 
"It's not pleasant, but sometimes things happen." 

"Change is..." 

+["Scary."] -> scary 
+["Hard."] -> hard
+["Normal."] -> normal

= scary
#name: Lupe 
"Change is scary because it's a dive into the unknown that most people are not willing to take..." 
-> preSound

= hard 
#name: Lupe 
"Change is hard because you have to willingly accept it before you can really move on..."
->preSound

=normal
~ sincerity_level++
#name: Lupe 
"Change is normal. As absurd as it sounds, it's just a part of life we have no control over." 

"Plants change..." 

"The sky changes..." 

"People change..." 

"Places change..." 

->preSound

=preSound
"But it's what's needed to progress life towards something better." 

"Don't you think so?" 

#name: Misra 
"Well, is <i>this</i> change necessary? I mean, it feels like everyone's running for the hills when we could be-" 

->RuhRoah 

=homeNo 
#name: Misra 
{goats: "See?"}
{teens: "Ok! So I'm not crazy."} 

#name: Lupe
"But this town is...different." 

#name: Misra 
"How?" 

#name: Lupe 
"I don't know. Something is...off." 

#name: Misra 
"What do you mean by 'off'?" 

#name: Lupe 
"Somethings been strange since I've first stepped foot in this town."

"It all feels...familiar." 

"..."

"<b>You</b> feel...familar." 

->RuhRoah

==teens==
#name: Lupe 
"It's probably those teenagers." 

#name: Misra 
"What do you think they were doing up here?" 

"Drinking?" 

"Smoking?" 

"Graffiti?" 

#name: Lupe
{ tease_level > sincerity_level: "Maybe all three at the same time." | "Don't know. Teens can be unpredictable." }

#name: Misra 
{ tease_level > sincerity_level: "I don't think they have enough arms to do that!" | "You're right." }

{ tease_level > sincerity_level: You lightly laugh to yourself. }

#name: Lupe
"..." 

#name: Misra 
"..." 

{ tease_level > sincerity_level: "Can I ask you a strange question?" | ->RuhRoah }

{ tease_level > sincerity_level: 

+["Sure."] -> goats.strangeQuestion

+["Please don't."] -> ratherNot
}

=ratherNot
#name: Misra 
"Ok!" 

"..." 

-> RuhRoah

==RuhRoah==
//INSERT BIG BASH SOUND 

#name: Misra
"What was that!?" 

#name: Lupe 
"Misra?!" 

#name: Lupe 
"Misra, what are you doing?! Get back here!" 

-> DONE