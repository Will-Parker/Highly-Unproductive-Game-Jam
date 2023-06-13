#enter:Apple #enter:Blueberry
-> main

=== main ===
Hey, Blueberry. Got a second? #speaker:Apple
I think I can make time. Is something wrong? #speaker:Blueberry
No, I'm just thinking about team-building. What do you think is the best quality to have on the battlefield? #speaker:Apple
Oh, a clever plan, of course. #speaker:Blueberry
Really? I'm biased towards raw strength. Don't get me wrong, it's important to have brains on the team, but in a fight, I like to end it quickly. #speaker:Apple
You can do that without throwing a single hit, if you plan ahead. #speaker:Blueberry
Sometimes, but strength is versatile. You can use it to solve all sorts of problems without thinking too hard. #speaker:Apple
Where's the fun in that? I love having a puzzle to solve.
+ I guess it's just not for me. I get frustrated by puzzles. # option:good
    -> good
+ Sorry, but in a fight brawn still wins. # option:bad
    -> bad
    
=== good ===
That's fair. I guess everybody has something to bring to the table. #speaker:Blueberry
-> END

=== bad ===
Says you. #speaker:Blueberry
-> END

