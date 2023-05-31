#enter:Lemon #enter:Apple
-> main

=== main ===
This is some sample dialogue #speaker:Lemon
+ Good Choice # option:good
    -> good
+ Bad Choice # option:bad
    -> bad
    
=== good ===
That was a good choice! #speaker:Apple
-> END

=== bad ===
That was a bad choice... #speaker:Apple
-> END