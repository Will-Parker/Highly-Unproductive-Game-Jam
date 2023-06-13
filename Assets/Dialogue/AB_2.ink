#enter:Blueberry #enter:Apple
-> main

=== main ===
Amazing. They've been staring at it for almost ten minutes now. #speaker:Blueberry
Alright to-do list. Prepare to meet your doom. #speaker:Apple
... #speaker:Apple
Any second now. Aaany second. #speaker:Apple
... #speaker:Apple
By the roots in the ground, I will outlast you. #speaker:Apple
#speaker:Blueberry
+ Maybe you should take a break? # option:good
    -> good
+ You've got this! # option:bad
    -> bad
    
=== good ===
Ugh. You're right. I'll be back for round two, though, mark my words! #speaker:Apple
-> END

=== bad ===
Of course I do! Of course. Thanks, Blueberry. You hear that, list! Blueberry believes in me. #speaker:Apple
-> END

