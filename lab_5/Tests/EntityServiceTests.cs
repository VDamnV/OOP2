using Xunit;
using Moq;
using lab_5.BusinessLogic;
using lab_5.DataAccessLevel.FileRepositories;
using lab_5.DataAccessLevel.Models;
using lab_5.BusinessLogic.DTOs;
using System.Runtime.Serialization;

namespace lab_5.Tests;

public class EntityServiceTests
{

    [Fact]
    public void Ctor_SetsFileRepo()
    {
        var mockRepo = new Mock<IRepository>();
        mockRepo.SetupProperty(r => r.FilePath, "testfile");
        
        var service = new EntityService(mockRepo.Object);
        
        Assert.Equal("testfile", service.FilePath);
        Assert.Equal(0, service.PeopleCount);
    }

    [Fact]
    public void FilePath_GetSet_UpdatesUnderlyingRepo()
    {
        var mockRepo = new Mock<IRepository>();
        mockRepo.SetupProperty(r => r.FilePath, "init");
        var service = new EntityService(mockRepo.Object);

        service.FilePath = "newpath";
        
        Assert.Equal("newpath", mockRepo.Object.FilePath);
    }

    [Fact]
    public void ChangeRepo_ReplacesRepository()
    {
        var repo1 = new Mock<IRepository>();
        repo1.SetupProperty(r => r.FilePath, "initfile");
        var repo2 = new Mock<IRepository>();
        repo2.SetupProperty(r => r.FilePath, "newfile");
        var service = new EntityService(repo1.Object);

        service.ChangeRepo(repo2.Object);

        Assert.Equal("newfile", service.FilePath);
    }

    [Fact]
    public void SaveToFile_ReturnsSuccess_WhenOk()
    {
        var mockRepo = new Mock<IRepository>();
        var service = new EntityService(mockRepo.Object);

        var result = service.SaveToFile();

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void SaveToFile_ReturnsError_WhenException()
    {
        var mockRepo = new Mock<IRepository>();
        mockRepo.Setup(r => r.SaveToFile(It.IsAny<List<Person>>())).Throws(new SerializationException("Cannot serialize object"));
        var service = new EntityService(mockRepo.Object);

        var result = service.SaveToFile();

        Assert.False(result.IsSuccess);
        Assert.Contains("Error while saving", result.Message);
    }

    [Fact]
    public void LoadFromFile_SetsPeople_WhenValid()
    {
        var mockRepo = new Mock<IRepository>();
        mockRepo.Setup(r => r.GetFromFile<Person>())
                .Returns(new List<Person> { new Student("A", "B", 1, "Male", "A1", 2, "Hostel 1.101") });
        var service = new EntityService(mockRepo.Object);

        var result = service.LoadFromFile();

        Assert.True(result.IsSuccess);
        Assert.Equal(1, service.PeopleCount);
    }

    [Fact]
    public void LoadFromFile_ReturnsError_WhenNull()
    {
        var mockRepo = new Mock<IRepository>();
        mockRepo.Setup(r => r.GetFromFile<Person>()).Returns((List<Person>?)null);
        var service = new EntityService(mockRepo.Object);

        var result = service.LoadFromFile();

        Assert.False(result.IsSuccess);
        Assert.Equal("File repository returned null", result.Message);
    }

    [Fact]
    public void LoadFromFile_ReturnsError_WhenException()
    {
        var mockRepo = new Mock<IRepository>();
        mockRepo.Setup(r => r.GetFromFile<Person>()).Throws(new SerializationException("Cannot deserialize objects"));
        var service = new EntityService(mockRepo.Object);

        var result = service.LoadFromFile();

        Assert.False(result.IsSuccess);
        Assert.Contains("Error while loading", result.Message);
    }

    [Fact]
    public void AddPerson_AddsEntity()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        
        s.AddPerson(new Student("A", "B", 1, "Male", "C1", 2, "Hostel 1.101"));
        
        Assert.Equal(1, s.PeopleCount);
    }

    [Fact]
    public void AddPerson_FromDTO_AddsEntity()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        
        s.AddPerson(new StudentDTO
        {
            FirstName = "A",
            LastName = "a",
            PassportId = 1,
            Gender = "Female",
            Year = 2,
            Residence = "Hostel 2.201",
            StudentId = "A1"
        });

        Assert.Equal(1, s.PeopleCount);
    }

    [Fact]
    public void Clear_ClearsList()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Student("A", "a", 1, "Male", "a1", 2, "Home"));

        s.Clear();

        Assert.Equal(0, s.PeopleCount);
    }

    [Fact]
    public void RemovePerson_WorksAndFails_WhenExpected()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Student("A", "B", 1, "Male", "C1", 2, "Home"));

        Assert.True(s.RemovePerson(0));
        Assert.False(s.RemovePerson(10));
        Assert.False(s.RemovePerson(-1));
    }

    [Fact]
    public void GetPeopleFullInfo_ReturnsNone_WhenEmpty()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        
        Assert.Equal("None", s.GetPeopleFullInfo());
    }

    [Fact]
    public void GetPeopleFullInfo_ReturnsDetails()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Student("A", "B", 1, "Male", "C1", 2, "Home"));
        
        var result = s.GetPeopleFullInfo();
        
        Assert.Contains("A", result);
    }

    [Fact]
    public void CreateJsonEntityService_ReturnsValidInstance()
    {
        var s = EntityService.CreateJsonEntityService("abc.json");

        Assert.Equal("abc.json", s.FilePath);
    }


    [Fact]
    public void GetFirstYearFemaleHostelResidentsInfo_ReturnsMatches()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Student("Maria", "Ivanenko", 1, "Female", "KV1", 1, "Hostel 2.201"));
        s.AddPerson(new Student("Ivan", "Petrenko", 2, "Male", "KV2", 1, "Hostel 1.101"));
        s.AddPerson(new Student("Anna", "Boiko", 3, "Female", "KV3", 2, "Hostel 2.202"));
        s.AddPerson(new Student("Olena", "Shevchenko", 4, "Female", "KV4", 1, "Kyiv, Main St. 5"));

        var result = s.GetFirstYearFemaleHostelResidentsInfo();

        Assert.Contains("Maria Ivanenko", result);
        Assert.Contains("Total found: 1", result);
        Assert.DoesNotContain("Ivan Petrenko", result);
        Assert.DoesNotContain("Anna Boiko", result);
        Assert.DoesNotContain("Olena Shevchenko", result);
    }

    [Fact]
    public void GetFirstYearFemaleHostelResidentsInfo_ReturnsNone_WhenNoMatch()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Student("Ivan", "Petrenko", 2, "Male", "KV2", 1, "Hostel 1.101"));
        s.AddPerson(new Student("Anna", "Boiko", 3, "Female", "KV3", 2, "Hostel 2.202"));
        
        var result = s.GetFirstYearFemaleHostelResidentsInfo();
        
        Assert.Equal("None", result);
    }

    [Fact]
    public void SettleStudentsInHostel_SettlesCorrectlyByGender()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Student("Andriy", "Melnyk", 1, "Male", "KV1", 3, "Needs hostel"));
        s.AddPerson(new Student("Kateryna", "Boiko", 2, "Female", "KV2", 2, ""));
        s.AddPerson(new Student("Petro", "Ivanov", 3, "Male", "KV3", 1, "Hostel 1.101"));
        
        var result = s.SettleStudentsInHostel();

        Assert.Contains("Settled Andriy Melnyk (Gender: Male) into room Hostel 1.101", result);
        Assert.Contains("Settled Kateryna Boiko (Gender: Female) into room Hostel 2.201", result);
        Assert.DoesNotContain("Petro Ivanov", result);
    }

    [Fact]
    public void SettleStudentsInHostel_ReturnsNone_WhenAllSettled()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Student("Petro", "Ivanov", 3, "Male", "KV3", 1, "Hostel 1.101"));
        
        var result = s.SettleStudentsInHostel();
        
        Assert.Equal("Everyone is already settled.", result);
    }

    
    [Fact]
    public void PracticeMusic_IncreasesSkill_ForMusician()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Musician("Mykola", "Leontovych", 1, "Male", "Piano", 50));
        
        var result = s.PracticeMusic(0);

        Assert.Equal("Mykola practiced Piano. Skill level is now: 55", result);
    }

    [Fact]
    public void PracticeMusic_ReturnsError_WhenNotMusician()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        s.AddPerson(new Student("A", "B", 1, "Male", "C1", 2, "Home"));
        
        var result = s.PracticeMusic(0);
        
        Assert.Equal("Error: This person is not a musician.", result);
    }

    [Fact]
    public void PracticeMusic_ReturnsError_WhenIndexInvalid()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        
        var result = s.PracticeMusic(99);
        
        Assert.Equal("Error: Invalid index.", result);
    }
    

    [Fact]
    public void GoSkating_ReturnsNotRequiredMessage()
    {
        var s = new EntityService(new Mock<IRepository>().Object);
        
        var result = s.GoSkating(0);
        
        Assert.Equal("не потрібно виконувати для оцінки добре", result);
    }
}