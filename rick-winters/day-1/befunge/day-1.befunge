"28L 41R 99L 1L 55L 06R 5L 84R 03L 68L"v                     |
v                                      <
 "Loop over the numbers and try to store the instructions"
 x "store ascii value of zero"                       88*3g
 x "current counter of y location for instruction"   88*4g
 x "current index of instruction"                    88*5g
 x "current x location for instruction"              88*6g
 x "number of instructions left"                     88*7g

 Push initial Values to stack
> 0 88*3p 25*2* 88*4p  v
 v                     <                       < THis loops stores a single instruction
                       > 15g 1- 15p 16g 1+ 16p ^
 > 16g 14g p 15g 13g ` | "If count is not 0" 
 ^ <                   > 17g 1- 17p 14g 1+ 14p "0" 16p "3" 15p v
   | ` g31 g71                                                 <
   @ 