#include "stdio.h"

void isValid(char str[]) {

    if(str == "da" || str == "bda" || str == "dc" || str == "bdc") {
        printf("Valid\n");
    }
    else {
        printf("Not valid\n");
    }


}

int main() {

    isValid("bdc");
    isValid("aa");

    return 0;
}