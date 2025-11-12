using lab_5.DataAccessLevel.Models;
using Xunit;

namespace lab_5.Tests;

public class EntityTests
{

    [Fact]
    public void Student_Ctor_NoParams_SetsValuesToDefault()
    {
        var student = new Student();

        Assert.Equal("NA", student.FirstName);
        Assert.Equal("NA", student.LastName);
        Assert.Equal(-1, student.PassportId);
        Assert.Equal("Unknown", student.Gender);
        Assert.Equal("NA", student.StudentId);
        Assert.Equal(-1, student.Year);
        Assert.Equal("NA", student.Residence);
    }

    [Fact]
    public void Student_IncreaseYear_ShouldIncrease_WhenValidYear()
    {
        var student = new Student { Year = 1 };

        student.IncreaseYear();

        Assert.Equal(2, student.Year);
    }

    [Fact]
    public void Student_IncreaseYear_Throws_WhenInvalidYear()
    {
        var student = new Student { Year = 6 };

        var e = Assert.Throws<InvalidOperationException>(student.IncreaseYear);

        Assert.Equal("Student is on the last year", e.Message);
    }


    [Fact]
    public void Musician_Ctor_NoParams_SetsValuesToDefault()
    {
        var musician = new Musician();

        Assert.Equal("NA", musician.FirstName);
        Assert.Equal("NA", musician.LastName);
        Assert.Equal(-1, musician.PassportId);
        Assert.Equal("Unknown", musician.Gender);
        Assert.Equal("NA", musician.Instrument);
        Assert.Equal(0, musician.SkillLevel);
    }

    [Fact]
    public void Musician_Practice_IncreasesSkill()
    {
        var musician = new Musician { SkillLevel = 50 };

        musician.Practice();

        Assert.Equal(55, musician.SkillLevel);
    }

    [Fact]
    public void Musician_Practice_DoesNotExceedMaxSkill()
    {
        var musician = new Musician { SkillLevel = 98 };

        musician.Practice();
        
        Assert.Equal(100, musician.SkillLevel);
        
        musician.Practice();

        Assert.Equal(100, musician.SkillLevel);
    }

    
    [Fact]
    public void Pilot_Ctor_NoParams_SetsValuesToDefault()
    {
        var pilot = new Pilot();
        
        Assert.Equal("NA", pilot.FirstName);
        Assert.Equal("NA", pilot.LastName);
        Assert.Equal(-1, pilot.PassportId);
        Assert.Equal("Unknown", pilot.Gender);
        Assert.Equal("PLT-000", pilot.LicenseId);
        Assert.Equal(0, pilot.FlightHours);
        Assert.False(pilot.IsOnFlight);
    }

    [Fact]
    public void Pilot_StartFlight_Works()
    {
        var pilot = new Pilot();

        pilot.StartFlight();

        Assert.True(pilot.IsOnFlight);
    }
    
    [Fact]
    public void Pilot_StartFlight_Throws_WhenAlreadyOnFlight()
    {
        var pilot = new Pilot();
        pilot.StartFlight();

        var e = Assert.Throws<Exception>(() => pilot.StartFlight());

        Assert.Equal("Pilot is already on a flight.", e.Message);
    }
    
    [Fact]
    public void Pilot_FinishFlight_Works()
    {
        var pilot = new Pilot { FlightHours = 100 };
        pilot.StartFlight();
        
        pilot.FinishFlight(5);
        
        Assert.False(pilot.IsOnFlight);
        Assert.Equal(105, pilot.FlightHours);
    }
    
    [Fact]
    public void Pilot_FinishFlight_Throws_WhenNotOnFlight()
    {
        var pilot = new Pilot();

        var e = Assert.Throws<Exception>(() => pilot.FinishFlight(5));

        Assert.Equal("Pilot is not on a flight.", e.Message);
    }
    
    [Fact]
    public void Pilot_FinishFlight_Throws_WhenHoursAreInvalid()
    {
        var pilot = new Pilot();
        pilot.StartFlight();
        
        var e = Assert.Throws<ArgumentException>(() => pilot.FinishFlight(0)); // 0 годин
        var e2 = Assert.Throws<ArgumentException>(() => pilot.FinishFlight(-5)); // -5 годин
        
        Assert.Contains("Flight hours must be positive.", e.Message);
        Assert.Equal("hours", e.ParamName);
        Assert.Contains("Flight hours must be positive.", e2.Message);
        Assert.Equal("hours", e2.ParamName);
    }
}
