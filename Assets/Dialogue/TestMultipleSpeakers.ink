#enter:Strawberry #enter:Apple
-> main

=== main ===
Hello! I am Apple. Who are you? #speaker:Apple
What should I say? #speaker:Strawberry,true
+ [Strawberry]
    I am Strawberry. Nice to meet you. #speaker:Strawberry,false
    
    Nice to meet you too. #speaker:Apple
    -> DONE
#speaker:Strawberry
+ [Pikachu]
    You may not believe me, but my real name is Pikachu. #speaker:Strawberry,false
    
    Hmmmm... #speaker:Apple
    I don't believe you. Let's try that again.
    -> suprise
    
=== suprise ===
Suprise its me Mr. Mystery!!! #enter: Mr. Mystery,1 #speaker:Mr. Mystery
WHAHAHAHAHAHAHAHHAHAHAHAHAHAHAHAHAHHAHAHAHAHHA #exit: Mr. Mystery
Uhhhh... I guess I'll just ignore that # speaker: Apple, true
-> main 