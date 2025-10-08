using System.Reflection;
using System.Runtime.Serialization;
using StudentDataManager.People;

namespace StudentDataManager.FileManager;

public class DatabaseManager(string filePath) {
    public string FilePath { get; set; } = filePath;
    public bool IsLocked { get; private set; } = false;

    public void SaveToFile(Person[] people) => this.writeToFile(people,false);
    public void AppendToFile(Person[] people) => this.writeToFile(people,true);

    public Person[] LoadFromFile() {
        if (this.IsLocked) throw new DatabaseLockedException("Database locked for another operation! Cannot read!");
        if (!File.Exists(this.FilePath)) throw new FileNotFoundException();
        this.IsLocked = true;
        using var reader = new StreamReader(this.FilePath);
        var result = new Person[0];
        int readingState = 0;

        while (!reader.EndOfStream) {
            string? line = reader.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line[0] == '{') {
                readingState = 1;
                continue;
            } else if (line[0] == '}') {
                readingState = 0;
                continue;
            }

            if (readingState == 0) {
                Type type = Type.GetType(line) ?? throw new TypeLoadException($"Could not find type {line}");
                if (!typeof(Person).IsAssignableFrom(type)) {
                    readingState = 2;
                    continue;
                }
#pragma warning disable
                result = [..result,(Person)FormatterServices.GetSafeUninitializedObject(type)!];
#pragma warning restore
            } else if (readingState == 1) {
                string[] split = line.Split(':',StringSplitOptions.TrimEntries);
                if (split.Length < 2) throw new InvalidDataException();
                Person person = result[result.Length - 1];
                PropertyInfo property = person.GetType().GetProperty(split[0].Substring(1,split[0].Length - 2))
                    ?? throw new InvalidDataException($"Invalid property name {split[0]}");

                var valueStr = split[1];
                if (valueStr[0] == '"') property.SetValue(person,valueStr.Substring(1,valueStr.Length - 2));
                else if (valueStr[0] == '\'') property.SetValue(person,valueStr[1]);
                else if (valueStr == "True") property.SetValue(person,true);
                else if (valueStr == "False") property.SetValue(person,false);
                else if (valueStr == "null") property.SetValue(person,null);
                else if (int.TryParse(valueStr,out int integer)) property.SetValue(person,integer);
                else if (float.TryParse(valueStr,out float floating)) property.SetValue(person,floating);
                else {
                    Type? typeEnum = Type.GetType(valueStr.Substring(0,valueStr.LastIndexOf('.')));
                    if (typeEnum != null && typeEnum.IsEnum)
                        property.SetValue(person,Enum.Parse(typeEnum,valueStr.Substring(valueStr.LastIndexOf('.') + 1)));
                    else throw new InvalidDataException("Invalid property type");
                }
            } else if (readingState == 2) continue;
        }
        this.IsLocked = false;
        return result;
    }

    private void writeToFile(Person[] people,bool append) {
        if (this.IsLocked) throw new DatabaseLockedException("Database locked for another operation! Cannot write!");
        this.IsLocked = true;
        using var writer = new StreamWriter(this.FilePath,append);

        foreach (var person in people) {
            var type = person.GetType();
            writer.Write(type.FullName);
            writer.Write("\n{\n");
            foreach (var property in type.GetProperties()) {
                string toWrite;
                var value = property.GetValue(person);
                if (value == null) toWrite = "null";
                else if (property.PropertyType.IsEnum) toWrite = $"{property.PropertyType.FullName}.{value}";
                else if (property.PropertyType == typeof(string)) toWrite = $"\"{value}\"";
                else if (property.PropertyType == typeof(char)) toWrite = $"'{value}'";
                else toWrite = value.ToString() ?? throw new Exception($"Could not convert property {property.Name} to string");

                writer.Write($"\"{property.Name}\": {toWrite}\n");
            }
            writer.Write("};\n");
        }

        this.IsLocked = false;
    }
}

public class DatabaseLockedException : Exception {
    public DatabaseLockedException() : base("Database locked for another operation!") { }
    public DatabaseLockedException(string message) : base(message) { }
    public DatabaseLockedException(string message,Exception? innerException) : base(message,innerException) { }
}
