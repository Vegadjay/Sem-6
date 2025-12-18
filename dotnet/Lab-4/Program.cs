// ! P-1

// public class Program {
//     public static void Main(string[] args) {
//         Stack<string> s = new Stack<string>();
//         AddNewTask(s,"Hey there");
//         AddNewTask(s,"How are you");
//         AddNewTask(s,"Nothing new here");
//         AddNewTask(s,"Dotnet");
//         PrintAllTask(s);
//         RemoveTask(s);
//         RemoveTask(s);
//         RemoveTask(s);
//         Console.WriteLine();
//         PrintAllTask(s);
//         Console.WriteLine();
//         TopMostDescription(s);
//     }


//     static void AddNewTask(Stack<string> stack, string description) {
//         stack.Push(description);
//     }


//     static void PrintAllTask(Stack<string> stack) {
//         foreach(string name in stack) {
//             Console.WriteLine(name);
//         }        
//     }
//     static void RemoveTask(Stack<string> stack) {
//         if(stack.Count > 0) {
//             stack.Pop();
//         }
//         else {
//             Console.WriteLine("Stack has nothing pop");
//         }
//     }

//     static void TopMostDescription(Stack<string> stack) {
//         if(stack.Count > 0) {
//             Console.WriteLine(stack.Peek());
//         }
//         else {
//             Console.WriteLine("Stack has nothing top");
//         }
//     }
// }



// * Description:=
// Simulate a customer service queue using a Queue<string>. Customers arrive
// in sequence and are served in the order they arrived (FIFO). The program
// should allow adding customers, serving one, and displaying who’s next.

// public class Program {
//     public static void Main(string[] args) {

//         Queue<string> q = new Queue<string>();
//         AddCustomer(q, "Bihari");
//         AddCustomer(q, "Darshan");
//         AddCustomer(q, "Gujrati");
//         AddCustomer(q, "Panjabi");
//         DisplayNextCustomer(q);
//         ServeCustomer(q);
//         DisplayNextCustomer(q);
//         ServeCustomer(q);
//         DisplayNextCustomer(q);
//         ServeCustomer(q);
//     }

//     static void AddCustomer(Queue<string> queue, string customer) {
//         queue.Enqueue(customer);
//     }

//     static void ServeCustomer(Queue<string> queue) {
//         if(queue.Count > 0) {
//             Console.WriteLine(queue.Dequeue());
//         }
//         else {
//             Console.WriteLine("Queue has nothing serve");
//         }
//     }

//     static void DisplayNextCustomer(Queue<string> queue) {
//         if(queue.Count > 0) {
//             Console.WriteLine(queue.Peek());
//         }
//         else {
//             Console.WriteLine("Queue has nothing next");
//         }
//     }

// }



// ! Program - 3

// * Description:=

// Write a C# program to count the number of vowels and consonants in a
// given string. Ignore spaces and handle both uppercase and lowercase
// characters.

// * Solution:=

// public class Program {

//     public static void Main(string[] args) { 

//         string? s = Console.ReadLine();
//         CountVowelsAndConsonants(s);
//     }

//     static void CountVowelsAndConsonants(string s) {
//         int vowels = 0;
//         int consonants = 0;
//         foreach(char c in s) {
//             if(c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u') {
//                 vowels++;
//             }
//             else {
//                 consonants++;
//             }
//         }
//         Console.WriteLine("Vowels: " + vowels);
//         Console.WriteLine("Consonants: " + consonants);
//     }

//  }


// ! Program - 4

// Write a C# program to check whether a given string is a palindrome (reads
// the same backward and forward). Ignore case and spaces.


// * Solution:=

public class Program {
    public static void Main(string[] args) {
        string? s = Console.ReadLine();
        CheckPalindrome(s);
    }

    static void CheckPalindrome(string s) {
        string reversed = new string(s.Reverse().ToArray());
        if(s == reversed) {
            Console.WriteLine("Palindrome");
        }
        else {
            Console.WriteLine("Not a palindrome");
        }
    }
}   