using System;

namespace StudentDataManager.People;

public class Pilot : Person, ISkater {
    public bool CanSkate { get; private set; }

    public Pilot(string first, string last, bool canSkate) : base(first, last) {
        this.CanSkate = canSkate;
    }

    public void Skate() {
        if (!CanSkate) {
            Console.WriteLine($"{FirstName} {LastName} cannot skate.");
            return;
        }
        Console.WriteLine($"{FirstName} {LastName} is skating!");
    }

    public override string ToString() {
        return $"Pilot: {FirstName} {LastName}, Can skate: {CanSkate}";
    }
}
