using lab_5.DataAccessLevel.Models;

namespace lab_5.BusinessLogic.DTOs;

public abstract class PersonDTO
{
        public string FirstName { get; set; } = "John";
    public string LastName { get; set; } = "Doe";
    public int PassportId { get; set; }
    public string Gender { get; set; } = "Unknown";
    public abstract Person ToEntity();
}

public class StudentDTO : PersonDTO
{
    public string StudentId { get; set; } = "KV12345678";
    public int Year { get; set; } // "Курс"
    public string Residence { get; set; } = "Hostel 1.101";

    public override Person ToEntity() => new Student(
        this.FirstName,
        this.LastName,
        this.PassportId,
        this.Gender,
        this.StudentId,
        this.Year,
        this.Residence
    );
}

public class MusicianDTO : PersonDTO
{
    public string Instrument { get; set; } = "Piano";
    public int SkillLevel { get; set; } = 0;

    public override Person ToEntity() => new Musician(
        this.FirstName,
        this.LastName,
        this.PassportId,
        this.Gender,
        this.Instrument,
        this.SkillLevel
    );
}

public class PilotDTO : PersonDTO
{
    public string LicenseId { get; set; } = "PLT-001";
    public int FlightHours { get; set; } = 0;

    public override Person ToEntity() => new Pilot(
        this.FirstName,
        this.LastName,
        this.PassportId,
        this.Gender,
        this.LicenseId,
        this.FlightHours
    );
}

public class SkaterDTO : PersonDTO
{
    public string SkateType { get; set; } = "Ice"; // e.g., Ice, Roller
    public int SkillLevel { get; set; } = 0;

    public override Person ToEntity() => new Skater(
        this.FirstName,
        this.LastName,
        this.PassportId,
        this.Gender,
        this.SkateType,
        this.SkillLevel
    );
}