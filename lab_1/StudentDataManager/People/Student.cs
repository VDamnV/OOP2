using System.Text.RegularExpressions;

namespace StudentDataManager.People;

public class Student : Person {
	public int Course { get; set; }           
	public string StudentId { get; init; }    
	public string Gender { get; set; }
	public string Residence { get; set; }   

	public Student(
		string first,
		string last,
		int course,
		string studentId,
		string gender,
		string residence
	) : base(first, last) {

		this.Course = course;

		this.StudentId = Regex.IsMatch(studentId,"^[A-Z]{2}[0-9]{8}$")
			? studentId
			: throw new FormatException("Invalid student ID format");

		this.Gender = Regex.IsMatch(gender,"^[MF]$") 
			? gender
			: throw new FormatException("Gender must be 'M' or 'F'");

		this.Residence = residence;
	}

	public void AdvanceYear() => this.Course++;

	public override string ToString() {
		return $"Student: {FirstName} {LastName}, Course: {Course}, ID: {StudentId}, Gender: {Gender}, Residence: {Residence}";
	}
}