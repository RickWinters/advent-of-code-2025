#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <stdint.h>
#include <inttypes.h>


int index_of_digit(char* bank, char digit) {
    char* index_pointer = strchr(bank, digit);
    if (index_pointer == NULL) {
        return -1;
    }
    return index_pointer - bank;
}

int main()
{
    FILE* fp = fopen("input.txt", "r");

    if (fp == NULL) {
        printf("The file is not opened.");
    } else {
        uint64_t counter = 0;

        char* bank_str = malloc(sizeof(char) * 102);
        while (fgets(bank_str, sizeof(char) * 102, fp) != NULL) {
            int line_length = strlen(bank_str);
            int last_index = line_length - 2; // Do -2 to ignore newline character

            char bank_result_str[13];
            bank_result_str[12] = '\0';
            char* next_address = bank_str;
            for (int battery_nr = 11; battery_nr >= 0; battery_nr--) {
                // Find the highest digit that is not at the end of the bank
                for (char first_char = '9'; first_char >= '1'; first_char--) {
                    char* first_char_address = strchr(next_address, first_char);
                    if (first_char_address != NULL && (first_char_address - bank_str) <= last_index - battery_nr) {
                        bank_result_str[11 - battery_nr] = first_char;
                        next_address = first_char_address + 1;
                        break;
                    }
                }
            }

            counter += strtoull(bank_result_str, nullptr, 10);
        }

        free(bank_str);
        fclose(fp);
        printf("Part 2 answer: %" PRIu64, counter);
    }

    return 0;
}