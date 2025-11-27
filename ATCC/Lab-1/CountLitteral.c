#include<stdio.h>

int main() {
    FILE *fp;
    char fileName[] = "sample.txt";
    char ch;

    int chars = 0, tabs = 0, newline = 0, space= 0;
    int spaceCount = 0;
    fp = fopen(fileName, "r");

    ch = fgetc(fp);

    while(ch != EOF) {
        if(ch == ' ') {
            space++;
        }
        else if(ch == '\t') {
            tabs++;
        }
        else if(ch == '\n') {
            newline++;
        }
        else {
            chars++;
        }
        ch = fgetc(fp);
    }

    printf("Number of chars:- %d \n", chars);
    printf("Number of spaces %d \n", space);
    printf("Number of New lines:- %d \n", newline);
    printf("Number of tabs:- %d", tabs);

    return 0;
}