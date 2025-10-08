using System.Collections.Generic;
using StudentDataManager.FileManager;
using StudentDataManager.People;

namespace StudentDataManager.ConsoleMenu;

static class ConsoleMenu {
	static void Main() {
		Person[] people = [];
		var db = new DatabaseManager(string.Empty);
		bool isRunning = true;
		string? userInput;

		// Initial database setup
		while (db.FilePath == string.Empty) {
			Console.WriteLine(@"Welcome to the student database manager!
Select an action:
1. Enter file path to read/write to
2. Exit");
			userInput = Console.ReadLine();
			switch (userInput) {
				case "1":
					Console.Write("Enter file path: ");
					do {
						userInput = Console.ReadLine();
					} while (userInput == null);
					db.FilePath = userInput;
					break;
				case "2":
					isRunning = false;
					return;
				default:
					Console.WriteLine("Invalid input!");
					break;
			}
		}

		var choices = new Dictionary<string,Action>() {
			{ "1", () => {
				// Adding a person
				Console.WriteLine("What type of person would you like to add (Student/Musician/Pilot)");
				userInput = Console.ReadLine()?.ToLower();
				if (userInput == "student") {
					try {
						Console.Write("Enter student's first name: ");
						string first = Console.ReadLine() ?? "";
						Console.Write("Enter student's last name: ");
						string last = Console.ReadLine() ?? "";
						Console.Write("Enter the year/course: ");
						int course = int.Parse(Console.ReadLine() ?? "1");
						Console.Write("Enter student ID (format XX12345678): ");
						string studentId = Console.ReadLine() ?? "";
						Console.Write("Enter gender (M/F): ");
						string gender = Console.ReadLine() ?? "";
						Console.Write("Enter residence (if dormitory, format DormNumber.Room): ");
						string residence = Console.ReadLine() ?? "";

						people = [..people,new Student(first,last,course,studentId,gender,residence)];
					} catch (Exception e) {
						Console.WriteLine($"Error adding a student: {e.Message}");
						return;
					}
				} else if (userInput == "musician") {
					Console.Write("Enter musician's first name: ");
					string first = Console.ReadLine() ?? "";
					Console.Write("Enter musician's last name: ");
					string last = Console.ReadLine() ?? "";
					Console.Write("Can they skate? (Y/N): ");
					bool canSkate = Console.ReadLine()?.ToLower() == "y";
					people = [..people,new Musician(first,last,canSkate)];
				} else if (userInput == "pilot") {
					Console.Write("Enter pilot's first name: ");
					string first = Console.ReadLine() ?? "";
					Console.Write("Enter pilot's last name: ");
					string last = Console.ReadLine() ?? "";
					Console.Write("Can they skate? (Y/N): ");
					bool canSkate = Console.ReadLine()?.ToLower() == "y";
					people = [..people,new Pilot(first,last,canSkate)];
				} else {
					Console.WriteLine("Invalid input!");
					return;
				}
				Console.WriteLine($"Successfully added {people[people.Length - 1]}");
			} },

			{ "2", () => {
				// Removing a person
				if (people.Length == 0) {
					Console.WriteLine("List is empty!");
					return;
				}
				Console.WriteLine("Select a person to remove:");
				for (int i = 0; i < people.Length; i++) {
					Console.WriteLine($"{i + 1}. {people[i]}");
				}
				if (int.TryParse(Console.ReadLine() ?? "0", out int toRemove) && toRemove <= people.Length) {
					people = people.Where((_,index) => index != toRemove - 1).ToArray();
					Console.WriteLine("Successfully removed a person");
				} else {
					Console.WriteLine("Invalid index!");
				}
			} },

			{ "3", () => {
				if (people.Length == 0) {
					Console.WriteLine("The list is empty! Wiping file? (Y/N)");
					if (Console.ReadLine()?.ToLower() == "y") {
						db.SaveToFile(people);
						Console.WriteLine("File cleared!");
					} else Console.WriteLine("Action cancelled");
				} else {
					Console.WriteLine($"Overwrite file with {people.Length} people? (Y/N)");
					if (Console.ReadLine()?.ToLower() == "y") {
						db.SaveToFile(people);
						Console.WriteLine($"Saved {people.Length} people");
					} else Console.WriteLine("Action cancelled");
				}
			} },

			{ "4", () => {
				if (people.Length == 0) {
					Console.WriteLine("List is empty!");
					return;
				}
				Console.WriteLine($"Append {people.Length} people to the file? (Y/N)");
				if (Console.ReadLine()?.ToLower() == "y") {
					db.AppendToFile(people);
					Console.WriteLine($"Appended {people.Length} people");
				} else Console.WriteLine("Action cancelled");
			} },

			{ "5", () => {
				people = db.LoadFromFile();
				Console.WriteLine($"Loaded {people.Length} people");
			} },

			{ "6", () => {
				if (people.Length == 0) {
					Console.WriteLine("List is empty");
				} else {
					foreach (var i in people) Console.WriteLine(i);
				}
			} },

			{ "7", () => {
				Console.WriteLine("Search by\n1. Name\n2. ID");
				userInput = Console.ReadLine();
				if (userInput == "1") {
					Console.Write("Enter first name: ");
					string? first = Console.ReadLine();
					Console.Write("Enter last name: ");
					string? last = Console.ReadLine();
					bool isFound = false;
					for (int i = 0; i < people.Length; i++) {
						var person = people[i];
						if (person.FirstName.ToLower() == first?.ToLower() && person.LastName.ToLower() == last?.ToLower()) {
							Console.WriteLine($"{person} is number {i+1}");
							isFound = true;
						}
					}
					if (!isFound) Console.WriteLine("No matching person found");
				} else if (userInput == "2") {
					Console.Write("Enter student ID: ");
					string? id = Console.ReadLine();
					bool isFound = false;
					for (int i = 0; i < people.Length; i++) {
						if (people[i] is Student s && s.StudentId == id) {
							Console.WriteLine($"{s} is number {i+1}");
							isFound = true;
						}
					}
					if (!isFound) Console.WriteLine("No matching student found");
				}
			} },

			{ "8", () => {
				Console.Write("Enter new file path: ");
				userInput = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(userInput)) db.FilePath = userInput;
				else Console.WriteLine("Invalid input");
			} },

			{ "9", () => {
				Student[] match = [];
				foreach (var p in people) {
					if (p is Student s && s.Course == 1 && s.Gender.ToLower() == "f" && s.Residence.Contains(".")) {
						match = [..match,s];
					}
				}
				Console.WriteLine($"Found {match.Length} female students in year 1 living in dorm:");
				foreach (var s in match) Console.WriteLine(s);
			} },

			{ "10", () => {
				Person[] toAdd = [
					new Student("Anna","Koval",1,"ST12345678","F","101.12"),
					new Student("Olya","Shevchenko",1,"ST87654321","F","102.21"),
					new Student("Ivan","Petrenko",2,"ST11223344","M","Home"),
					new Musician("Oleg","Muzik",true),
					new Pilot("Sergiy","Flyer",false)
				];
				people = [..people,..toAdd];
				Console.WriteLine("Added 5 predefined people");
			} },

			{ "11", () => { isRunning = false; } }
		};

		while (isRunning) {
			Console.WriteLine(@"Select an action:
1. Add a person
2. Remove a person
3. Save people to file (overwrite)
4. Append people to file
5. Load people from file
6. View loaded people
7. Search
8. Change file path
9. Calculate number of female 1st-year students in dorm
10. Add predefined people
11. Exit");
			userInput = Console.ReadLine() ?? "";
			if (choices.TryGetValue(userInput, out var action)) action();
			else Console.WriteLine("Invalid input!");

			Console.WriteLine("Press any key to continue...");
			Console.ReadLine();
		}
	}
}
