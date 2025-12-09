#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>


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
        int counter = 0;

        char* bank_str = malloc(sizeof(char) * 102);
        while (fgets(bank_str, sizeof(char) * 102, fp) != NULL) {
            int line_length = strlen(bank_str);
            int last_index = line_length - 2; // Do -2 to ignore newline character

            // Find first highest digit that is not at the end of the bank
            char first_char;
            char* first_char_address;
            for (first_char = '9'; first_char >= '1'; first_char--) {
                first_char_address = strchr(bank_str, first_char);
                if (first_char_address != NULL && (first_char_address - bank_str) < last_index) {
                    break;
                }
            }

            // Find second highest digit that comes after the first highest digit
            char second_char;
            for (second_char = '9'; second_char >= '1'; second_char--) {
                char* second_char_address = strchr(first_char_address + 1, second_char);
                if (second_char_address != NULL) {
                    break;
                }
            }

            counter += atoi((char[]){first_char, second_char, '\0'});
        }

        free(bank_str);
        fclose(fp);
        printf("Part 1 answer: %i", counter);
    }

    return 0;
}