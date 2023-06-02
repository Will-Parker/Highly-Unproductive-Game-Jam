#enter:Lemon #enter:Blueberry
-> main

=== main ===
This is some sample dialogue #speaker:Lemon
+ Good Choice # option:good
    -> good
+ Bad Choice # option:bad
    -> bad
    
=== good ===
That was a good choice! #speaker:Blueberry
-> END

=== bad ===
That was a bad choice... #speaker:Blueberry
-> END