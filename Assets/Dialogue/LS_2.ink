#enter:Strawberry #enter:Lemon
-> main

=== main ===
This is some sample dialogue #speaker:Strawberry
+ Good Choice # option:good
    -> good
+ Bad Choice # option:bad
    -> bad
    
=== good ===
That was a good choice! #speaker:Lemon
-> END

=== bad ===
That was a bad choice... #speaker:Lemon
-> END

