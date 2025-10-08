using System;
using System.Media;
using System.Runtime.Versioning;

namespace StudentDataManager.People;

public class Musician : Person, ISkater {
    public bool CanSkate { get; private set; }
    
    public Musician(string first, string last, bool canSkate) : base(first, last) {
        this.CanSkate = canSkate;
    }

    [SupportedOSPlatform("windows")]
    public void PlayMusic() {
        SystemSounds.Asterisk.Play();
        Console.WriteLine($"{FirstName} {LastName} is playing music!");
    }

    public void Skate() {
        if (!CanSkate) {
            Console.WriteLine($"{FirstName} {LastName} cannot skate.");
            return;
        }
        Console.WriteLine($"{FirstName} {LastName} is skating!");
    }

    public override string ToString() {
        return $"Musician: {FirstName} {LastName}, Can skate: {CanSkate}";
    }
}