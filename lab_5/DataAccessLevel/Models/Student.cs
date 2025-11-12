using Newtonsoft.Json;

namespace lab_5.DataAccessLevel.Models;

public class Student : Person, ISkater
{
    public string StudentId { get; }
    public int Year { get; set; }
    public string Residence { get; set; }

    public Student(
        string first,
        string last,
        int passportId,
        string gender,
        string studentId,
        int year,
        string residence)
        : base(first, last, passportId, gender)
    {
        StudentId = studentId;
        Year = year;
        Residence = residence;
    }

    [JsonConstructor]
    public Student() : this("NA", "NA", -1, "Unknown", "NA", -1, "NA") { }

    public void IncreaseYear()
    {
        if (this.Year > 5) throw new InvalidOperationException("Student is on the last year");
        this.Year++;
    }

    public void Skate()

}