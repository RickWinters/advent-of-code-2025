"28L 41R 99L 10L 55L 06R 50L 84R 03L 86L" v
v                                         <
 x current direction (12g)
 G current position (13g)     G is ascii 71, limits are 21 (inclusive) and 121 (exclusive)
 0 amount on 0 (14g)          21 = 52*2*1+    121 = 52*52**52*2*1++
 0 ten's digit (15g)
 0 unit's digit (16g)       > 12g,15g,16g," ", v
 9 instruction to do (17g)
 0 n wrap aronuds (18g)                    
                >           ^v                 <                                            
                      v      <      >1+v        
> 12p 15p 16p > ^     > 13g 12g "L"`|  > 13p v Increment / Decrement 12g by 1
$                                   >1-^            
                      ^                              <  
 v                            p61 -1 g61     <         
  Check counter bigger than 121         
                       > 13g 52*52**- 13p 18g1+18p v                        
 > 13g 52*52**52*2*+1+`| Check > 120                                                   
 v                     <                           <                        
  Check counter less than 21                                                
               >                          v                                
 > 13g 52*2*1+`| check > 20                                                           
               > 13g 52*52**+ 13p 18g1+18pv                                   
 v          .-+1*2*25g31                  <                                
   Check units digit == "0",                                                
   if so add 10, substract 1 from tens digit                                
            >                                    v         
 > 16g "0"` |          > 16g 52*+ 16p 15g 1- 15p >   ^ 
            > 15g "0"` |  check if tens digit is 0, check for next instruction
                                           > 14g1+14p v
                       > 13g 52*52**52*2*+`| 
^                    p71 -1 g71           <>          v
                                          |< `"0" g71 <
   @,g81," ",g41," ".-+1*2*25 g31         <