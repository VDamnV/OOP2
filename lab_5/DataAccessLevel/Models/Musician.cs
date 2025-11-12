using Newtonsoft.Json;

namespace lab_5.DataAccessLevel.Models;

public class Musician(
    string first,
    string last,
    int passportId,
    string gender,
    string instrument,
    int skillLevel) : Person(first, last, passportId, gender), ISkater
{
    public string Instrument { get; set; } = instrument;
    public int SkillLevel { get; set; } = skillLevel;

    [JsonConstructor]
    public Musician() : this("NA", "NA", -1, "Unknown", "NA", 0) { }

    public void Practice()
    {
        if (this.SkillLevel < 100) // Обмеження для логіки
        {
            this.SkillLevel += 5;
        }
    }

    public void Skate()
}