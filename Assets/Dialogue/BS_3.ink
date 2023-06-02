#enter:Blueberry #enter:Strawberry
-> main

=== main ===
This is some sample dialogue. #speaker:Blueberry
+ Good Choice # option:good
    -> good
+ Bad Choice # option:bad
    -> bad
    
=== good ===
That was a good choice! #speaker:Strawberry
-> END

=== bad ===
That was a bad choice... #speaker:Strawberry
-> END

