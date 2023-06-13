#enter:Apple #enter:Blueberry
-> main

=== main ===
Blueberry! Do you have any notes from our last fight? #speaker:Apple # option:good
Yes, actually. Would you like to hear them? #speaker:Blueberry
Of course. #speaker:Apple
Well, I noticed that Strawberry took a little damage that could have been avoided if we had positioned them more defensively. #speaker:Blueberry
Noted. Next time, I'll keep in front of them, so they don't get hurt. #speaker:Apple
...But then you'll get hurt. #speaker:Blueberry
I can take a little bit of bruising! #speaker:Apple
The point is not to, though. #speaker:Blueberry
#speaker:Apple
+ [Focus on maneuvering] # option:good
    -> good
+ [Insist on self-sacrifice] # option:bad
    -> bad
    
=== good ===
Okay, I think I get it. We'll all try to work on our maneuvering? #speaker:Apple
That would be wise, in my opinion. #speaker:Blueberry
Perfect. Where would we be without you, Blueberry? #speaker:Apple
<i>(Very clearly running through all the worst case scenarios in their head)</i> #speaker:Blueberry
-> END

=== bad ===
Well, it's much easier for me to just protect everyone else. #speaker:Apple
If you say so. #speaker:Blueberry
-> END

