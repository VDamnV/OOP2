using Newtonsoft.Json;

namespace lab_5.DataAccessLevel.Models;

public abstract class Person(string first, string last, int passportId, string gender)
{
    public string FirstName { get; set; } = first;
    public string LastName { get; set; } = last;
    public int PassportId { get; set; } = passportId;
    
    public string Gender { get; set; } = gender; 

    [JsonConstructor]
    public Person() : this("NA", "NA", -1, "Unknown") { }
    
    public override string ToString() => $"{this.FirstName} {this.LastName} (Gender: {this.Gender}) | Passport number: {this.PassportId}";
}