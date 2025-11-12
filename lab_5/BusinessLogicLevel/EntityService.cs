using System.Text;
using lab_5.BusinessLogic.DTOs;
using lab_5.DataAccessLevel.FileRepositories;
using lab_5.DataAccessLevel.Models;

namespace lab_5.BusinessLogic;

public class EntityService
{
    private List<Person> _people = new();
    private IRepository _fileRepo;

    private int _maleRoomCounter = 101;
    private int _femaleRoomCounter = 201;

    public string FilePath
    {
        get => this._fileRepo.FilePath;
        set => this._fileRepo.FilePath = value;
    }
    
    public int PeopleCount => this._people.Count;
    
    public EntityService(IRepository fileRepo) => this._fileRepo = fileRepo;
    
    public void ChangeRepo(IRepository newRepo) => this._fileRepo = newRepo;
    
    public FileAccessResult SaveToFile()
    {
        try
        {
            this._fileRepo.SaveToFile(this._people);
            return FileAccessResult.EmptySuccess;
        }
        catch (Exception e)
        {
            return new FileAccessResult(false, $"Error while saving: {e.Message}");
        }
    }
    
    public FileAccessResult LoadFromFile()
    {
        try
        {
            var loaded = this._fileRepo.GetFromFile<Person>();
            if (loaded == null) return new FileAccessResult(false, "File repository returned null");
            this._people = loaded.ToList();
            return FileAccessResult.EmptySuccess;
        }
        catch (Exception e)
        {
            return new FileAccessResult(false, $"Error while loading: {e.Message}");
        }
    }
    
    public void AddPerson(Person person) => this._people.Add(person);
    
    public void AddPerson(PersonDTO personDTO) => this._people.Add(personDTO.ToEntity());
    
    public bool RemovePerson(int index)
    {
        if (index < 0 || index >= this._people.Count) return false;
        this._people.RemoveAt(index);
        return true;
    }
    
    public void Clear() => this._people.Clear();

    public string GetFirstYearFemaleHostelResidentsInfo()
    {
        var result = new StringBuilder();
        bool found = false;
        int count = 0;
        
        for (int i = 0; i < this._people.Count; i++)
        {
            if (this._people[i] is Student student)
            {
                bool isHostelResident = student.Residence != null && student.Residence.Contains(".");
                
                if (student.Year == 1 && student.Gender == "Female" && isHostelResident)
                {
                    result.Append($"{student.ToString()}, under index {i} in the list\n");
                    found = true;
                    count++;
                }
            }
        }
        
        if (!found) return "None";
        
        result.Insert(0, $"Total found: {count}\n---\n");
        return result.ToString();
    }

    public string SettleStudentsInHostel()
    {
        var result = new StringBuilder();
        bool settledAnyone = false;

        foreach (var person in this._people)
        {
            if (person is Student student)
            {
                bool isSettled = student.Residence != null && student.Residence.Contains(".");
                if (!isSettled)
                {
                    string newRoom;
                    if (student.Gender == "Male")
                    {
                        newRoom = $"Hostel 1.{_maleRoomCounter++}";
                    }
                    else if (student.Gender == "Female")
                    {
                        newRoom = $"Hostel 2.{_femaleRoomCounter++}";
                    }
                    else
                    {
                        newRoom = "Hostel 3.301"; // Для невідомої статі
                    }
                    
                    result.Append($"Settled {student.FirstName} {student.LastName} (Gender: {student.Gender}) into room {newRoom}\n");
                    student.Residence = newRoom;
                    settledAnyone = true;
                }
            }
        }
        
        if (!settledAnyone) return "Everyone is already settled.";
        return result.ToString();
    }

    public string PracticeMusic(int personIndex)
    {
        if (personIndex < 0 || personIndex >= this._people.Count)
        {
            return "Error: Invalid index.";
        }

        if (this._people[personIndex] is Musician musician)
        {
            musician.Practice();
            return $"{musician.FirstName} practiced {musician.Instrument}. Skill level is now: {musician.SkillLevel}";
        }

        return "Error: This person is not a musician.";
    }

    public string GoSkating(int personIndex)
    {
        
        return "не потрібно виконувати для оцінки добре";
    }

    public string GetPeopleFullInfo()
    {
        if (this._people.Count == 0) return "None";
        var result = new StringBuilder();
        for (int i = 0; i < this._people.Count; i++)
        {
            var person = this._people[i];
            result.Append($"{new string('=', 10)}\n{i})\n");
            foreach (var prop in person.GetType().GetProperties())
            {
                result.Append($"{prop.Name} = {prop.GetValue(person)}\n");
            }
        }
        return result.ToString();
    }
    
    public static EntityService CreateJsonEntityService(string filePath) => new EntityService(new JsonRepository(filePath));
}