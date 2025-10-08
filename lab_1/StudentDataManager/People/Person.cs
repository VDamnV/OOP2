using System.Text.RegularExpressions;

namespace StudentDataManager.People;

public abstract class Person {
	private string firstName = "";
	private string lastName = "";

	public string FirstName {
		get => this.firstName;
		set {
			if (!Regex.IsMatch(value,"^[A-Za-z'-]+$")) throw new FormatException("Invalid first name format");
			this.firstName = value;
		}
	}
	public string LastName {
		get => this.lastName;
		set {
			if (!Regex.IsMatch(value,"^[A-Za-z'-]+$")) throw new FormatException("Invalid last name format");
			this.lastName = value;
		}
	}

	protected Person(string first, string last) {
		this.FirstName = first;
		this.LastName = last;
	}

	public override string ToString() => $"{this.GetType().Name}: {this.firstName} {this.lastName}";
}