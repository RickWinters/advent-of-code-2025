#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>


int main()
{
    FILE* fp = fopen("input.txt", "r");

    if (fp == NULL) {
        printf("The file is not opened.");
    } else {
        int counter = 0;

        char* last_line = malloc(sizeof(char) * 150);
        char* current_line = malloc(sizeof(char) * 150);
        char* next_line = malloc(sizeof(char) * 150);

        fgets(current_line, sizeof(char) * 150, fp);
        int line_length = strlen(current_line);
        int line_counter = 0;
        char* next_line_present;
        do {
            next_line_present = fgets(next_line, sizeof(char) * 150, fp);

            for (int x = 0; x < line_length - 1; x++) {
                int nr_of_near_rolls = 0;

                if (current_line[x] == '.') {
                    continue;
                }
  
                // Check left
                if (x > 0 && current_line[x - 1] == '@') {
                    nr_of_near_rolls++;
                }

                // Check right
                if (x < line_length - 2 && current_line[x + 1] == '@') {
                    nr_of_near_rolls++;
                }

                // Check top-left
                if (last_line != NULL && x > 0 && last_line[x - 1] == '@') {
                    nr_of_near_rolls++;
                }

                // Check top
                if (last_line != NULL && last_line[x] == '@') {
                    nr_of_near_rolls++;
                }

                // Check top-right
                if (last_line != NULL && x < line_length - 2 && last_line[x + 1] == '@') {
                    nr_of_near_rolls++;
                }

                // Check bottom-left
                if (next_line_present != NULL && x > 0 && next_line[x - 1] == '@') {
                    nr_of_near_rolls++;
                }

                // Check bottom
                if (next_line_present != NULL && next_line[x] == '@') {
                    nr_of_near_rolls++;
                }

                // Check bottom-right
                if (next_line_present != NULL && x < line_length - 2 && next_line[x + 1] == '@') {
                    nr_of_near_rolls++;
                }

                if (nr_of_near_rolls < 4) {
                    counter++;
                }
            }

            line_counter++;

            last_line = memcpy(last_line, current_line, sizeof(char) * 150);
            current_line = memcpy(current_line, next_line, sizeof(char) * 150);
        }
        while (next_line_present != NULL);
        printf("Part 1 answer: %i", counter);

        free(last_line);
        free(current_line);
        free(next_line);
        fclose(fp);
    }

    return 0;
}