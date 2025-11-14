using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using HotelBookingSystem.DAL.Interfaces;
using HotelBookingSystem.BLL.Models; 

namespace HotelBookingSystem.DAL.Repositories
{
    public class FileRepository<T> : IRepository<T> where T : class
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public FileRepository(string filePath)
        {
            _filePath = filePath;
            _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
            InitializeFile();
        }

        private void InitializeFile()
        {
            if (!File.Exists(_filePath))
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(_filePath, JsonSerializer.Serialize(new List<T>(), _jsonSerializerOptions));
            }
        }

        private List<T> LoadAll()
        {
            string jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(jsonString, _jsonSerializerOptions) ?? new List<T>();
        }

        private void SaveAll(IEnumerable<T> entities)
        {
            string jsonString = JsonSerializer.Serialize(entities.ToList(), _jsonSerializerOptions);
            File.WriteAllText(_filePath, jsonString);
        }

        public T GetById(int id)
        {
            var allEntities = LoadAll();
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have an 'Id' property.");
            }
            return allEntities.FirstOrDefault(e => (int)idProperty.GetValue(e) == id);
        }

        public IEnumerable<T> GetAll()
        {
            return LoadAll();
        }

        public void Add(T entity)
        {
            var allEntities = LoadAll();
            allEntities.Add(entity);
            SaveAll(allEntities);
        }

        public void Update(T entity)
        {
            var allEntities = LoadAll();
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have an 'Id' property.");
            }

            var entityId = (int)idProperty.GetValue(entity);
            var existingEntity = allEntities.FirstOrDefault(e => (int)idProperty.GetValue(e) == entityId);

            if (existingEntity != null)
            {
                var index = allEntities.IndexOf(existingEntity);
                if (index != -1)
                {
                    allEntities[index] = entity;
                }
            }
            else
            {
                throw new InvalidOperationException($"Entity with ID {entityId} not found for update.");
            }
            SaveAll(allEntities);
        }

        public void Delete(int id)
        {
            var allEntities = LoadAll();
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have an 'Id' property.");
            }

            var entityToRemove = allEntities.FirstOrDefault(e => (int)idProperty.GetValue(e) == id);
            if (entityToRemove != null)
            {
                allEntities.Remove(entityToRemove);
            }
            else
            {
                throw new InvalidOperationException($"Entity with ID {id} not found for deletion.");
            }
            SaveAll(allEntities);
        }
    }
}