using System.Runtime.Serialization;

namespace DataAccessLevel.DataProviders;

public class BinaryProvider(string filePath) : DataProvider(filePath)
{
    protected override void saveToFileLogic<T>(ICollection<T> objects)
    {
        using var stream = new FileStream(this.FilePath, FileMode.Create);
        using var writer = new BinaryWriter(stream);
        foreach (T obj in objects)
        {
            if (obj == null) continue;
            var type = obj.GetType();
            if (type.AssemblyQualifiedName == null) continue;
            
            writer.Write(type.AssemblyQualifiedName);
            writer.Write("{");
            foreach (var prop in type.GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(CustomProviderIgnore))) continue;
                writer.Write($"{prop.Name}:{prop.GetValue(obj)}");
            }
            writer.Write("}");
        }
    }

    protected override ICollection<T>? loadFromFileLogic<T>()
    {
        if (!File.Exists(this.FilePath)) throw new FileNotFoundException($"Не вдалося відкрити файл за шляхом {this.FilePath}");
        
        using var stream = new FileStream(this.FilePath, FileMode.Open);
        using var reader = new BinaryReader(stream);
        var objects = new List<T>();
        int mode = 0; // 0 - очікуємо тип, 1 - очікуємо властивості
        
        try
        {
            while (true)
            {
                string line = reader.ReadString().Trim();
                if (line == "{") continue;
                if (line == "}")
                {
                    mode = 0;
                    continue;
                }
                
                if (mode == 0)
                {
                    Type type = Type.GetType(line) ?? throw new TypeLoadException($"Недійсний тип '{line}'");
                    if (!typeof(T).IsAssignableFrom(type))
                        throw new TypeLoadException($"Тип {line} не може бути присвоєний {typeof(T).Name}");
                    
                    objects.Add((T)Activator.CreateInstance(type)!);
                    mode = 1;
                }
                else if (mode == 1)
                {
                    string[] split = line.Split(':', StringSplitOptions.TrimEntries);
                    if (split.Length != 2) throw new SerializationException($"Недійсний формат рядка '{line}'");
                    
                    var propName = split[0];
                    var value = split[1];
                    var obj = objects[objects.Count - 1];
                    var property = obj!.GetType().GetProperty(propName) ?? throw new SerializationException($"Недійсна властивість '{propName}'");
                    
                    if (Attribute.IsDefined(property, typeof(CustomProviderIgnore))) continue;
                    
                    property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                }
            }
#pragma warning disable CS0168
        }
        catch (EndOfStreamException _)
        {
#pragma warning restore
                        return objects;
        }
        catch (SerializationException e)
        {
            throw new SerializationException(e.Message, e);
        }
        catch (TypeLoadException e)
        {
            throw new TypeLoadException(e.Message, e);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}