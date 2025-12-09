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

        char first_char_of_line = fgetc(fp);
        char rest_of_line[sizeof(char) * 5];
        while (first_char_of_line != EOF) {
            fgets(rest_of_line, sizeof(rest_of_line), fp);
            int number = atoi(rest_of_line);
            if (first_char_of_line == 'L') {
                rotation -= number;
                rotation = rotation % 100;
                rotation += rotation < 0 ? 100 : 0;
            } else {
                rotation += number;
                rotation = rotation % 100;
            }
            
            if (rotation == 0) {
                counter++;
            }
           
            first_char_of_line = fgetc(fp);
        }
        fclose(fp);
        printf("Part 1 answer: %i", counter);
    }

    return 0;
}