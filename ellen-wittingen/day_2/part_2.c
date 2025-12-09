#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include <inttypes.h>
#include <math.h>

int main()
{
    FILE* fp = fopen("input.txt", "r");

    if (fp == NULL) {
        printf("The file is not opened.");
    } else {
        // Get length of file
        fseek(fp, 0, SEEK_END);
        int size = ftell(fp);
        fseek(fp, 0, SEEK_SET);

        // Read entire file into memory
        char* fcontent = malloc(size);
        fread(fcontent, 1, size, fp);
        

        // Main loop that loops over all segments delimeted by ','
        uint64_t result = 0;
        char* next_token;
        char* range_start_token = strtok_s(fcontent, "-", &next_token);
        char* range_end_token;
        int stop = 0;
        while (!stop)
        {
            // Check if we have reached the end by seeing if there is a comma ahead or not
            char *end_reached = strchr(next_token, ',');
            if (end_reached == NULL) {
                range_end_token = strtok_s(NULL, "\n", &next_token);
                stop = 1;
            } else {
                range_end_token = strtok_s(NULL, ",", &next_token);
            }

            // Loop over range and find string repetitions
            uint64_t range_start = strtoull(range_start_token, nullptr, 10);
            uint64_t range_end = strtoull(range_end_token, nullptr, 10);
            char max_str[25];
            sprintf(max_str, "%" PRIu64, range_end);
            int max_length = strlen(max_str);
           
            for (uint64_t id = range_start; id <= range_end; id++)
            {
                char id_str[25];
                sprintf(id_str, "%" PRIu64, id);
                int length = strlen(id_str);
                if (length == 1) {
                    continue;
                }
                int found = 0;
                for (size_t repeatable_length = 1; repeatable_length <= max_length / 2; repeatable_length++)
                {
                    
                    // Skip strings of uneven lengths by going the lowest number with 1 digit more
                    if (length % repeatable_length != 0) {
                        // uint64_t new_id = 1;
                        // for (size_t i = 0; i < length; i++)
                        // {
                        //     new_id = new_id * 10;
                        // }
                        // id = new_id;
                        // repeatable_length++;
                        
                        continue;
                    }
                    
                    // Check if same string is repeated repeatable_length
                    int nr_of_sections = length / repeatable_length;
                    for (size_t char_in_rep_index = 0; char_in_rep_index < repeatable_length; char_in_rep_index++)
                    {
                        int break_now = 0;
                        for (size_t i = 0; i < nr_of_sections; i++)
                        {
                            if (id_str[char_in_rep_index] != id_str[char_in_rep_index + repeatable_length * i]) {
                                break_now = 1;
                                // printf("%s - %i - %i\n", id_str, i, i + repeatable_length * section);
                                break;
                            }
                            
                        }

                        if (break_now) {
                            break;
                        }
                        
                        if (char_in_rep_index + 1 == repeatable_length) {
                            printf("MATCH %s - %i - %i\n", id_str, nr_of_sections, repeatable_length);
                            result += id;
                            found = 1;
                            break;
                        }
                    }

                    if (found) {
                        break;
                    }
                }
            }

            range_start_token = strtok_s(NULL, "-", &next_token);
        }

        free(fcontent);
        fclose(fp);
        printf("Part 2 answer: %" PRIu64 "\n", result);
    }

    return 0;
}