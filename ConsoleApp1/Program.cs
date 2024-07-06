using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace ConsoleApp1
{
    internal class Program
    {
        static List<Member> members = new List<Member>();
        static List<Book> books = new List<Book>();
        static List<IssuedBook> issues = new List<IssuedBook>();

        static string getDateTime()
        {
            return DateTime.Now.ToString();
        }

        static string path = "action_logs.txt";

        static void Main(string[] args)
        {
            bool loopController = true; // used to handlle the while loop

            if (!File.Exists(path))
            {
                File.Create(path);
            }

            while (loopController)
            {
                Console.WriteLine("|----- Select a operator -----|\n");
                Console.WriteLine("1 - Add Book");
                Console.WriteLine("2 - Add Member");
                Console.WriteLine("3 - Issue Book");
                Console.WriteLine("4 - Return Book");
                Console.WriteLine("5 - View Issued Books");
                Console.WriteLine("6 - Exit\n");

                Console.Write("Enter the operator :");
                int Operator = Convert.ToInt32(Console.ReadLine());

                switch(Operator)
                {
                    case 1:
                        AddBook();
                        break;
                    case 2:
                        AddMember();
                        break;
                    case 3:
                        IssueBooK();
                        break;
                    case 4:
                        ReturnBook();
                        break;
                    case 5:
                        ViewIssuedBooks();
                        break;
                    default:
                        loopController = false;
                        break;
                }
            }

        }

        static void AddMember()
        {
            Console.Write("Enter the NIC number :");
            string NIC = Console.ReadLine();

            bool isUserExist = false;
            foreach(var member in Program.members) //used to prevent creating same member
            {
                if(member.NIC == NIC)
                {
                    isUserExist = true;
                    Console.WriteLine("This member is already added.");
                    break;
                }
            }

            if (!isUserExist) //if user is not registed, Now continue...
            {
                Console.Write("Enter the name :");
                string name = Console.ReadLine();

                Console.Write("Enter the phone number :");
                string mobile = Console.ReadLine();

                Console.Write("Enter the address :");
                string address = Console.ReadLine();

                Program.members.Add(new Member(NIC,name, mobile, address));
                Console.WriteLine("New member has been added successfully.");
                File.AppendAllText(Program.path , $"{NIC} - Member has been added successfully at {getDateTime()}\n");
            }

           

        }

        static void AddBook()
        {
            Console.Write("Enter the ISBN :");
            string ISBN = Console.ReadLine();

            bool checkISBN = true;

            foreach(var book in Program.books) //used to prevent ISBN number conflit
            {
                if(ISBN == book.ISBN) 
                {
                    checkISBN = false;
                    Console.WriteLine("ISBN number is already used in the system.");
                    break;
                }
            }

            if (checkISBN) //if ISBN number OK, continue...
            {
                Console.Write("Enter the title :");
                string title = Console.ReadLine();

                Console.Write("Enter the author :");
                string author = Console.ReadLine();

                Program.books.Add(new Book(title, author,ISBN));
                Console.WriteLine("New book has been added successfully.");
                File.AppendAllText(Program.path, $"{ISBN} - New book has been added successfully at {getDateTime()}\n");
            }

            
        }

        static void IssueBooK()
        {
            Console.Write("Enter the ISBN :");
            string ISBN = Console.ReadLine();

            bool ckeckRequest = false;
            bool availablity = true;

            foreach(var book in Program.books) // find ISBN number valid or not
            {
                if(book.ISBN == ISBN)
                {
                    ckeckRequest = true;
                    break;
                }
            }

            if (!ckeckRequest) // this notify that ISBN is not valid
            {
                Console.WriteLine("Bad request, please check ISBN");
            }

            else
            {
                foreach (var issue in Program.issues) // check that book is issued or not using issued-Book-List
                {
                    if (issue.ISBN == ISBN)
                    {
                        Console.WriteLine("This book is not available now");
                        availablity = false;
                    }
                }

                if (availablity) // if is not issued, go to the next step
                {
                    bool isMember = false; 

                    Console.Write("Enter the NIC number :");
                    string NIC = Console.ReadLine();

                    foreach (var item in Program.members) // check member is registered or not using member-List
                    {
                        if (item.NIC == NIC)
                        {
                            isMember = true;
                            break;
                        }
                    }

                    if (isMember) // if user is registed as member, now continue...
                    {
                        string[] dateAndTime = getDateTime().ToString().Split();
                        string date = dateAndTime[0];

                        DateTime newDate = DateTime.Now.AddDays(14);
                        string[] deadlineDateAndTime = newDate.ToString().Split();
                        string deadline = deadlineDateAndTime[0];

                        Program.issues.Add(new IssuedBook(ISBN, NIC, date, deadline));
                        Console.WriteLine("Book has been issued successfully.");
                        File.AppendAllText(Program.path, $"{ISBN} - Book has been issued to {NIC} successfully at {getDateTime()}\n");
                    }

                    else
                    {
                        Console.WriteLine("This user isn't registered as a member"); // if user is not registed, not allowed to do that
                    }
                }

                else
                {
                    Console.WriteLine("Book is not available now"); // if book is already issued, display this error message
                }
            }

        }

        static void ViewIssuedBooks()
        {
            foreach (var issue in Program.issues)
            {
                string issuedDetailsRow = "";

            
                foreach(var book in Program.books)
                {
                    if(issue.ISBN == book.ISBN)
                    {
                        issuedDetailsRow += "ISBN :" + book.ISBN + "\n";
                        issuedDetailsRow += "Title :" + book.Title + "\n";
                        issuedDetailsRow += "Author :" + book.Author + "\n";
                    }
                }
                foreach(var member  in Program.members)
                {
                    if(member.NIC == issue.NIC)
                    {
                        issuedDetailsRow += "NIC :" + member.NIC + "\n";
                        issuedDetailsRow += "Name :" + member.Name + "\n";
                        issuedDetailsRow += "Mobile :" + member.Mobile + "\n";
                        issuedDetailsRow += "Address :" + member.Address + "\n";
                        issuedDetailsRow += "Issued Date :" + issue.IssuedDate + "\n";
                        issuedDetailsRow += "Deadline :" + issue.Deadline + "\n";
                    }
                }
                Console.WriteLine(issuedDetailsRow+"\n");
            }

            
        }

        static void ReturnBook()
        {
            Console.Write("Enter the ISBN :");
            string ISBN = Console.ReadLine();

            bool isFound = false;

            for(int i = 0; i<Program.issues.Count;i++)
            {
                if (Program.issues[i].ISBN == ISBN) // check that ISBN number in issued-Book-List
                {
                    isFound = true;
                    Program.issues.RemoveAt(i);
                    Console.WriteLine("Book has been successfully returned.");
                    File.AppendAllText(Program.path, $"{ISBN} - Book has been successfully returned at {getDateTime()}\n");
                    break;
                }
            }
            if(!isFound) // if ISBN number not in issued-Book-List
            {
                Console.WriteLine("Issued book not found, please check ISBN again.");
            }
        }
    }

    class Book
    {
        public string Title{get; set;}
        public string Author { get; set;}
        public string ISBN { get; set;}

        public Book(string title, string author, string isbn)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
        }
    }

    class Member
    {
        public string NIC { get; set;}
        public string Name { get; set;} 
        public string Mobile { get; set;}
        public string Address { get; set;}

        public Member(string nic, string name, string mobile, string address)
        {
            NIC = nic;
            Name = name;
            Mobile = mobile;
            Address = address;
        }
    }

    class IssuedBook
    {
        public string ISBN { get; set; }
        public string NIC { get; set; }
        public string IssuedDate { get; set; }
        public string Deadline { get; set; }

        public IssuedBook(string iSBN, string nic, string issuedDate, string deadline)
        {
            ISBN = iSBN; //used to find about book
            NIC = nic;  //used to find about member
            IssuedDate = issuedDate;
            Deadline = deadline;
        }
    }
}
