#enter:Apple #enter:Lemon
-> main

=== main ===
Hey Lemon, would you feel up to spar with me? #speaker:Apple
Huh? You wanna fight me? Right here? Right now?? #speaker:Lemon
That is what I asked, yes #speaker:Apple
... #speaker:Lemon
... #speaker:Apple
... I thought you'd never ask! I'd be honored! #speaker:Lemon
Great! Even when resting it's good to keep the mind and body ready for action. #speaker:Apple
(I can't believe I've been challenged to a one on one duel by Apple of all people, I'm freaking out!!!) #speaker:Lemon #internal:true
Hey Apple! Before the fight I just want to ask you, don't hold back! If I'm going to beat you I want to beat you for real! #speaker:Lemon #internal:false
Sure thing Lemon! <i>(ah geez they're getting real excited about this)</i> #speaker:Apple
+ [Give it your all] # option:good
    -> good
+ [Give Lemon the victory] # option:bad
    -> bad
    
=== good ===
<i>Its a close match, but eventually Lemon has to tap</i> #speaker:system
<i>\*huff\* \*puff\*</i> ... good ... match #speaker:Lemon
<i>\*wheeze\*</i> sorry bud... yous said... <i>\*wheeze\*</i> not to hold back #speaker:Apple
What are you apologizing for? That was awesome! You kicked my butt! #speaker:Lemon
Hey, you made me work for it! #speaker:Apple
I know I got a lot to learn, but I'm glad I got you as a teacher #speaker:Lemon
-> END

=== bad ===
<i>Not putting in 100%, Apple lets Lemon win</i> #speaker:system
That was a good fight Lemon! You really handed it to me at the end there! #speaker:Apple
Yeah. Thanks for the match Apple, it was fun. #speaker:Lemon
You've really improved since being on the team, and I'm really proud of you. #speaker:Apple
... Sure. Thanks Apple, it means a lot. #speaker:Lemon
-> END