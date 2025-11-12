using Newtonsoft.Json;

namespace lab_5.DataAccessLevel.Models;

public class Pilot(
    string first,
    string last,
    int passportId,
    string gender,
    string licenseId,
    int flightHours) : Person(first, last, passportId, gender), ISkater
{
    public string LicenseId { get; set; } = licenseId;
    public int FlightHours { get; set; } = flightHours;
    public bool IsOnFlight { get; private set; } = false;

    [JsonConstructor]
    public Pilot() : this("NA", "NA", -1, "Unknown", "PLT-000", 0) { }

    public void StartFlight()
    {
        if (this.IsOnFlight) throw new Exception("Pilot is already on a flight.");
        this.IsOnFlight = true;
    }

    public void FinishFlight(int hours)
    {
        if (!this.IsOnFlight) throw new Exception("Pilot is not on a flight.");
        if (hours <= 0) throw new ArgumentException("Flight hours must be positive.", nameof(hours));
        
        this.IsOnFlight = false;
        this.FlightHours += hours;
    }

    public void Skate()

}