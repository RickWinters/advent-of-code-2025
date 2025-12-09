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
        int last_count = -1;
        int counter = 0;

        char* entire_file = malloc(sizeof(char) * 20000);
        fread(entire_file, sizeof(char), 20000, fp);

        int line_length = strchr(entire_file, '\n') - entire_file + 1;
        int file_length = strlen(entire_file);
        file_length = file_length - (file_length / line_length);

        while (last_count < counter) {
            last_count = counter;

            int current_line_index = 0;
            int y_index = 0;
            while (current_line_index < file_length) {
                int last_line_index = current_line_index - line_length;
                int next_line_index = current_line_index + line_length;

                for (int x = 0; x < line_length-1; x++) {
                    int nr_of_near_rolls = 0;

                    if (entire_file[current_line_index + x] == '.') {
                        continue;
                    }
    
                    // Check left
                    if (x > 0 && entire_file[current_line_index + x - 1] == '@') {
                        nr_of_near_rolls++;
                    }

                    // Check right
                    if (x < line_length - 2 && entire_file[current_line_index + x + 1] == '@') {
                        nr_of_near_rolls++;
                    }

                    // Check top-left
                    if (last_line_index >= 0 && x > 0 && entire_file[last_line_index + x - 1] == '@') {
                        nr_of_near_rolls++;
                    }

                    // Check top
                    if (last_line_index >= 0 && entire_file[last_line_index + x] == '@') {
                        nr_of_near_rolls++;
                    }

                    // Check top-right
                    if (last_line_index >= 0 && x < line_length - 2 && entire_file[last_line_index + x + 1] == '@') {
                        nr_of_near_rolls++;
                    }

                    // Check bottom-left
                    if (next_line_index < file_length && x > 0 && entire_file[next_line_index + x - 1] == '@') {
                        nr_of_near_rolls++;
                    }

                    // Check bottom
                    if (next_line_index < file_length && entire_file[next_line_index + x] == '@') {
                        nr_of_near_rolls++;
                    }

                    // Check bottom-right
                    if (next_line_index < file_length && x < line_length - 2 && entire_file[next_line_index + x + 1] == '@') {
                        nr_of_near_rolls++;
                    }

                    if (nr_of_near_rolls < 4) {
                        // printf("Position (%i, %i) can be accessed\n", y_index, x);
                        entire_file[current_line_index + x] = '.';
                        counter++;
                    }
                }

                y_index++;
                current_line_index = next_line_index;
            }
        }

        printf("Part 2 answer: %i", counter);

        free(entire_file);
        fclose(fp);
    }

    return 0;
}