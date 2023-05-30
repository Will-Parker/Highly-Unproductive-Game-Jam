#enter:Strawberry #enter:Apple
-> main

=== main ===
Hello! I am Apple. Who are you?                                 #speaker:Apple          #internal:false
What should I say?                                              #speaker:Strawberry     #internal:true
+ [Strawberry]
    I am Strawberry. Nice to meet you.                          #speaker:Strawberry     #internal:false
    Nice to meet you too.                                       #speaker:Apple
    -> DONE
    
+ [Pikachu]
    You may not believe me, but my real name is Pikachu.        #speaker:Strawberry     #internal:false
    Hmmmm...                                                    #speaker:Apple
    I don't believe you. Let's try that again.
    -> suprise
    
=== suprise ===
Suprise its me Mr. Mystery!!!                                   #enter:Mr. Mystery,1    #speaker:Mr. Mystery
WHAHAHAHAHAHAHAHHAHAHAHAHAHAHAHAHAHHAHAHAHAHHA HAHHAHAHHAHHAHAHHAHAHHAHAHHAHAHAHHAHAHAHAHHAHAHAHHAHAHAHAHHAHAHHAHAHAHAHHAHAHAHHAHA                                                             #exit:Mr. Mystery
Uhhhh... I guess I'll just ignore that                          #speaker:Apple          #internal:true
-> main 