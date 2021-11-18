using System;
using System.Collections.Generic;
using System.IO;


namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Transaction> transactions = new List<Transaction>();
            Dictionary<string, Person> people = new Dictionary<string, Person>();
            
            using (var reader = new StreamReader(@"C:\Work\Training\SupportBank\SupportBank\Transactions2014.csv"))
            {
                bool firstLine = true;
                
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().ToLower();
                    if (line == "") { continue; }
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }
                    
                    string[] values = line.Split(',');

                    // add from person
                    if (!people.ContainsKey(values[1])) { people.Add(values[1], new Person(values[1])); }
                    // add to person
                    if (!people.ContainsKey(values[2])) { people.Add(values[2], new Person(values[2])); }
                    
                    string date = values[0];
                    Person from = people[values[1]];
                    Person to = people[values[2]];
                    string narrative = values[3];
                    float amount = float.Parse(values[4]);

                    Transaction transaction = new Transaction(  date,
                                                                from,
                                                                to,
                                                                narrative,
                                                                amount
                                                            );
                    
                    transactions.Add(transaction);
                    from.sentTransactions.Add(transaction);
                    to.recievedTransactions.Add(transaction);
                }
            }
            
            foreach (Transaction transaction in transactions)
            {
                transaction.from.SendTransaction(transaction.amount);
                transaction.to.RecieveTransaction(transaction.amount);
            }

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "list")
                {
                    if (args.Length >= 2 && args[1].ToLower() == "all")
                    {
                        ListAll(people);
                    }
                    else if (args.Length >= 3)
                    {
                        ListPerson((args[1] + " " + args[2]).ToLower(), people);
                    }
                }
            }
        }
        
        public static void ListAll(Dictionary<string, Person> people)
        {
            foreach (Person person in people.Values)
            {
                Console.WriteLine(person.Repr());
            }
        }

        public static void ListPerson(string name, Dictionary<string, Person> people)
        {
            if (people.ContainsKey(name))
            {
                Person person = people[name];
                Console.WriteLine(person.Repr());
                foreach (var transaction in person.sentTransactions)
                {
                    Console.WriteLine(transaction.Repr());
                }
                
                foreach (var transaction in person.recievedTransactions)
                {
                    Console.WriteLine(transaction.Repr());
                }
            }
            else { Console.WriteLine("Name not found"); }
        }
    }
    
    class Person
    {
        public string name;
        public float balance = 0;
        public List<Transaction> sentTransactions = new List<Transaction>();
        public List<Transaction> recievedTransactions = new List<Transaction>();

        public Person(string personName)
        {
            name = personName;
        }
        public void RecieveTransaction(float amount)
        {
            balance += amount;
        }

        public void SendTransaction(float amount)
        {
            balance -= amount;
        }

        public string Repr()
        {
            string conjunction = balance > 0 ? " owes: " : " owed: ";
            return $"{Global.CapitaliseFirstLetter(name)} {conjunction} {balance}";
        }
    }

    class Transaction
    {
        public string date;
        public Person from;
        public Person to;
        public string narrative;
        public float amount;
        
        public Transaction( string transactionDate,
                            Person transactionFrom,
                            Person transactionTo,
                            string transactionNarrative,
                            float transactionAmount
                            )
        {
            date = transactionDate;
            from = transactionFrom;
            to = transactionTo;
            narrative = transactionNarrative;
            amount = transactionAmount;
        }

        public string Repr()
        {
            return $"{date} from: {Global.CapitaliseFirstLetter(from.name)} to: {Global.CapitaliseFirstLetter(to.name)} narrative: {narrative} amount: {amount}";
        }
    }

    public static class Global
    {
        public static string CapitaliseFirstLetter(string text)
        {
            string[] words = text.Split(" ");
            string output = "";
            foreach (string word in words)
            {
                output += " " + word[0].ToString().ToUpper() + word.Substring(1);
            }
            
            return output.Substring(1);
        }
    }
}