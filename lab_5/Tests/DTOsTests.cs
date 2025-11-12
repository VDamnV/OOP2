using lab_5.BusinessLogic.DTOs;
using lab_5.DataAccessLevel.Models;
using Xunit;

namespace lab_5.Tests;

public class DTOsTests
{
    [Fact]
    public void ToEntity_StudentDTO_To_Student()
    {
        var dto = new StudentDTO()
        {
            FirstName = "Maria",
            LastName = "Ivanenko",
            PassportId = 12345,
            Gender = "Female",
            StudentId = "KV111",
            Year = 1,
            Residence = "Hostel 2.201"
        };

        Student? student = dto.ToEntity() as Student;

        Assert.NotNull(student);
        Assert.Equal("Maria", student.FirstName);
        Assert.Equal("Ivanenko", student.LastName);
        Assert.Equal(12345, student.PassportId);
        Assert.Equal("Female", student.Gender);
        Assert.Equal("KV111", student.StudentId);
        Assert.Equal(1, student.Year);
        Assert.Equal("Hostel 2.201", student.Residence);
    }

    [Fact]
    public void ToEntity_MusicianDTO_To_Musician()
    {
        var dto = new MusicianDTO()
        {
            FirstName = "Mykola",
            LastName = "Leontovych",
            PassportId = 888,
            Gender = "Male",
            Instrument = "Piano",
            SkillLevel = 50
        };

        Musician? musician = dto.ToEntity() as Musician;

        Assert.NotNull(musician);
        Assert.Equal("Mykola", musician.FirstName);
        Assert.Equal("Leontovych", musician.LastName);
        Assert.Equal(888, musician.PassportId);
        Assert.Equal("Male", musician.Gender);
        Assert.Equal("Piano", musician.Instrument);
        Assert.Equal(50, musician.SkillLevel);
    }

    [Fact]
    public void ToEntity_PilotDTO_To_Pilot()
    {
        var dto = new PilotDTO()
        {
            FirstName = "Leonid",
            LastName = "Kadeniuk",
            PassportId = 999,
            Gender = "Male",
            LicenseId = "UKR-001",
            FlightHours = 1000
        };

        Pilot? pilot = dto.ToEntity() as Pilot;

        Assert.NotNull(pilot);
        Assert.Equal("Leonid", pilot.FirstName);
        Assert.Equal("Kadeniuk", pilot.LastName);
        Assert.Equal(999, pilot.PassportId);
        Assert.Equal("Male", pilot.Gender);
        Assert.Equal("UKR-001", pilot.LicenseId);
        Assert.Equal(1000, pilot.FlightHours);
    }

    [Fact]
    public void ToEntity_SkaterDTO_To_Skater()
    {
        var dto = new SkaterDTO()
        {
            FirstName = "Oksana",
            LastName = "Baiul",
            PassportId = 777,
            Gender = "Female",
            SkateType = "Ice",
            SkillLevel = 99
        };

        Skater? skater = dto.ToEntity() as Skater;

        Assert.NotNull(skater);
        Assert.Equal("Oksana", skater.FirstName);
        Assert.Equal("Baiul", skater.LastName);
        Assert.Equal(777, skater.PassportId);
        Assert.Equal("Female", skater.Gender);
        Assert.Equal("Ice", skater.SkateType);
        Assert.Equal(99, skater.SkillLevel);
    }
}