#include<stdio.h>
#include<ctype.h>

int main(){
    FILE *fp1 = fopen("file3.txt","r");
    FILE *fp2 = fopen("file2.txt","w");
    char prev = ' ';
    
    char ch = fgetc(fp1);
    
    while(ch!=EOF){
        if(prev == ' ' || prev=='\t' || prev == '\n'){
            fputc(toupper(ch),fp2);
        }
        else fputc(ch,fp2);
        prev = ch;
        ch = fgetc(fp1);
    }
    
    fclose(fp1);
    fclose(fp2);
}