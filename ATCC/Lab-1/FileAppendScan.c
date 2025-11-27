#include<stdio.h>
int main(){
    FILE *fp1,*fp2;

    fp1 = fopen("file1.txt","r");
    fp2 = fopen("file2.txt","a");
    char c;
    
    while (fscanf(fp1, "%c", &c)!=EOF) {
        fprintf(fp2, "%c",c);
    }
    
    fclose(fp1);
    fclose(fp2);
    return 0;
}