#include "stdio.h"

int main() {

    FILE *fp, *fp2, *fp3;
    fp = fopen("file1.txt", "r");
    fp2 = fopen("file2.txt", "a");
    char ch = fgetc(fp);

    
    while(ch != EOF) {
        fputc(ch, fp2);
        ch = fgetc(fp);
    }

    fclose(fp);
    return  0;
}