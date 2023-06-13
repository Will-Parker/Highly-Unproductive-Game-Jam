#enter:Strawberry #enter:Blueberry
-> main

=== main ===
<i>Blueberry is tinkering with their Bluebombs.</i> #speaker:system
Hey, Blueberry? #speaker:Strawberry
What is it? #speaker:Blueberry
There's something I've been wondering about. #speaker: Strawberry
What? #speaker:Blueberry
Well... #speaker:Strawberry
+ How do your Bluebombs work? # option:good
    -> good
+ Nevermind. Sorry to bother you. # option:bad
    -> bad
    
=== good ===
Well you see... #speaker:Blueberry
<i>Blueberry begins explaining the design specs of the bombs, somehow leading to a 30-minute tangent on the different Vaccinium species</i> #speaker:system
...Sorry I just realized I've gotten really off topic. #speaker:Blueberry
No, No, keep going! It's really nice to hear you talk about things you are passionate about. #speaker:Strawberry
In that case... <i>(continues their explanation)</i> #speaker:Blueberry
-> END

=== bad ===
In that case, I'm going to keep tinkering with my Bluebombs. #speaker:Blueberry
-> END

