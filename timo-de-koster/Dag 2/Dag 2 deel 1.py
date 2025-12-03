invalid_ids = []

with open('input.txt', 'r') as file:

    #nummerreeksen isoleren
    raw = file.read()
    ranges = raw.split(',')

for range in ranges:

    #ondergrens en bovengrens isoleren
    left, right = range.split('-')

    lenLeft = len(left)
    lenRight = len(right)
    lowerBound = int(left)
    upperBound = int(right)

    #isoleer eerste helft linkerkant
    first = left[:lenLeft // 2]

    #als de linkerkant oneven is, begin bij de eerstvolgende vermenigvuldiging van 10
    #(die is dan namelijk automatisch wel even en tevens het eerstvolgende valide nummer)
    if len(left) % 2 != 0:
        first = str(1) + str(0)*len(first)

    #eerste kandidaat voor invalide ID is de linkerhelft van mijn nummertje
    #maar dan een keer herhaald met zichzelf
    candidate = int(first + first)

    #hier tel ik gwn elke keer eentje bij op mijn linkerhelft van het palindroom
    #en vervolgens maak ik er weer een kandidaat van, kijken of het nog binnen de reeks past
    while(candidate < upperBound):
        if(candidate >= lowerBound):
            invalid_ids.append(candidate)
        
        first = str(int(first) + 1)
        candidate = int(first + first)

#ok tijd om alle nummertjes bij elkaar op te tellen :D
result = 0
for num in invalid_ids:
    result += int(num)

print(f"Final result: {result}")