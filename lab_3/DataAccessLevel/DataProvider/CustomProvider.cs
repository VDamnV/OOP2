using System.Runtime.Serialization;

namespace DataAccessLevel.DataProviders;

public class CustomProvider(string filePath) : DataProvider(filePath)
{
    protected override void saveToFileLogic<T>(ICollection<T> objects)
    {
        using var writer = new StreamWriter(this.FilePath);
        foreach (T i in objects)
        {
            if (i == null) continue;
            var type = i.GetType();
            writer.WriteLine(type.AssemblyQualifiedName);
            writer.WriteLine("{");
            foreach (var property in type.GetProperties())
            {
                if (Attribute.IsDefined(property, typeof(CustomProviderIgnore))) continue;
                writer.WriteLine($"	{property.Name}: {property.GetValue(i)}");
            }
            writer.WriteLine("};");
        }
    }

    protected override ICollection<T>? loadFromFileLogic<T>()
    {
        if (!File.Exists(this.FilePath)) throw new FileNotFoundException($"Не вдалося відкрити файл за шляхом {this.FilePath}");
        
        var objects = new List<T>();
        string fileText = File.ReadAllText(this.FilePath);
        if (string.IsNullOrWhiteSpace(fileText)) return null;

        foreach (string objectText in fileText.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] lines = objectText.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (lines.Length == 0) continue;
            Type type = Type.GetType(lines[0]) ?? throw new TypeLoadException($"Недійсний тип '{lines[0]}'");
            if (!typeof(T).IsAssignableFrom(type)) throw new TypeLoadException($"Тип {type.Name} не може бути присвоєний {typeof(T).Name}");
            
            object current = Activator.CreateInstance(type)!;
            
            for (int i = 1; i < lines.Length - 1; i++)             {
                string line = lines[i].Trim();
                if (line == "{" || line == "}") continue;
                
                string[] parts = line.Split(':', StringSplitOptions.TrimEntries);
                if (parts.Length != 2) throw new SerializationException($"Недійсний формат у рядку \"{line}\"");
                
                string propName = parts[0];
                string value = parts[1];
                
                var property = type.GetProperty(propName) ?? throw new SerializationException($"Недійсна назва властивості \"{propName}\"");
                
                if (Attribute.IsDefined(property, typeof(CustomProviderIgnore))) continue;
                
                property.SetValue(current, Convert.ChangeType(value, property.PropertyType));
            }
            objects.Add((T)current);
        }
        return objects;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class CustomProviderIgnore : Attribute { }