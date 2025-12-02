position = 50
zero_counter = 0

with open('input.txt', 'r') as file:
    for line in file:

        #lettertje en nummertje scheiden
        rotation = line[:1]
        amount = int(line[1:])

        #omhoog of omlaag? L = omlaag R = omhoog
        multiply = 1 if rotation == 'R' else -1
        position += (amount * multiply)
            
        #eem constrainen die hap
        while position < 0:
            position = 100 - abs(position)
        while position > 99:
            position = position - 100

        #nul gevonden, eentje bij op de teller
        if position == 0:
            zero_counter += 1

print(zero_counter)