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
            for (uint64_t id = range_start; id <= range_end;)
            {
                char id_str[25];
                sprintf(id_str, "%" PRIu64, id);
                int length = strlen(id_str);
                
                // Skip strings of uneven lengths by going the lowest number with 1 digit more
                if (length % 2 == 1) {
                    uint64_t new_id = 1;
                    for (size_t i = 0; i < length; i++)
                    {
                        new_id = new_id * 10;
                    }
                    id = new_id;
                    
                    continue;
                }
                
                // Check if same string is repeated twice
                int half_size = length / 2;
                for (size_t i = 0; i < half_size; i++)
                {
                    
                    if (id_str[i] != id_str[i + half_size]) {
                        break;
                    }

                    if (i + 1 == half_size) {
                        result += id;
                    }
                }

                id++;
            }

            range_start_token = strtok_s(NULL, "-", &next_token);
        }

        free(fcontent);
        fclose(fp);
        printf("Part 1 answer: %" PRIu64 "\n", result);
    }

    return 0;
}