#include <stdio.h>
#include <stdlib.h>
#include <string.h>


int main()
{
    FILE* fp = fopen("input.txt", "r");

    if (fp == NULL) {
        printf("The file is not opened.");
    } else {
        int counter = 0;
        int rotation = 50;

        int first_char_of_line = fgetc(fp);
        char rest_of_line[sizeof(char) * 5];
        while (first_char_of_line != EOF) {
            int current_rotation = rotation;
            fgets(rest_of_line, sizeof(rest_of_line), fp);
            int number = atoi(rest_of_line);
            if (first_char_of_line == 'L') {
                current_rotation -= number;
                counter += abs(current_rotation / 100); // if for current_rotation is -539, counter is increased with 5
                
                current_rotation = current_rotation % 100;
                
                int current_rotation_negative =  current_rotation < 0;
                current_rotation += current_rotation_negative ? 100 : 0;
                counter += rotation != 0 && current_rotation_negative ? 1 : 0; // if current_rotation < 0, it will move past 0 to 99

                if (current_rotation == 0) {
                    counter++;
                }
            } else {
                current_rotation += number;
                counter += current_rotation / 100; // if for example current_rotation is 539, counter is increased with 5 as it turns 5 rounds

                current_rotation = current_rotation % 100;
            }
            rotation = current_rotation;
           
            first_char_of_line = fgetc(fp);
        }
        fclose(fp);
        printf("Part 2 answer: %i", counter);
    }

    return 0;
}