using System.Collections.Generic;
using System.Linq;
using HotelBookingSystem.BLL.Exceptions;
using HotelBookingSystem.BLL.Interfaces;
using HotelBookingSystem.BLL.Models;
using HotelBookingSystem.DAL.Interfaces;

namespace HotelBookingSystem.BLL.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _clientRepository;

        public ClientService(IRepository<Client> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public void AddClient(Client client)
        {
            if (client == null)
            {
                throw new ValidationException("Client cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(client.FirstName))
            {
                throw new ValidationException("Client first name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(client.LastName))
            {
                throw new ValidationException("Client last name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(client.PhoneNumber))
            {
                throw new ValidationException("Client phone number cannot be empty.");
            }

            if (_clientRepository.GetAll().Any(c => c.Id == client.Id))
            {
                throw new ValidationException($"Client with ID {client.Id} already exists.");
            }

            client.Id = _clientRepository.GetAll().Any() ? _clientRepository.GetAll().Max(c => c.Id) + 1 : 1;
            _clientRepository.Add(client);
        }

        public void DeleteClient(int clientId)
        {
            if (clientId <= 0)
            {
                throw new ValidationException("Invalid input: Client ID must be positive.");
            }

            var clientToRemove = _clientRepository.GetById(clientId);
            if (clientToRemove == null)
            {
                throw new ValidationException($"Client with ID {clientId} not found.");
            }

            _clientRepository.Delete(clientId);
        }

        public void UpdateClient(Client client)
        {
            if (client == null)
            {
                throw new ValidationException("Client cannot be null.");
            }
            if (client.Id <= 0)
            {
                throw new ValidationException("Invalid input: Client ID must be positive for update.");
            }
            if (string.IsNullOrWhiteSpace(client.FirstName))
            {
                throw new ValidationException("Client first name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(client.LastName))
            {
                throw new ValidationException("Client last name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(client.PhoneNumber))
            {
                throw new ValidationException("Client phone number cannot be empty.");
            }

            var existingClient = _clientRepository.GetById(client.Id);
            if (existingClient == null)
            {
                throw new ValidationException($"Client with ID {client.Id} not found for update.");
            }

            _clientRepository.Update(client);
        }

        public Client GetClientDetails(int clientId)
        {
            if (clientId <= 0)
            {
                throw new ValidationException("Invalid input: Client ID must be positive.");
            }
            return _clientRepository.GetById(clientId);
        }

        public IEnumerable<Client> GetAllClients()
        {
            return _clientRepository.GetAll();
        }

        public IEnumerable<Client> GetClientsSortedByName()
        {
            return _clientRepository.GetAll().OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();
        }

        public IEnumerable<Client> GetClientsSortedByLastName()
        {
            return _clientRepository.GetAll().OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
        }

        public IEnumerable<Client> SearchClients(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ValidationException("Search keyword cannot be empty.");
            }
            
            string lowerKeyword = keyword.ToLower();
            return _clientRepository.GetAll()
                .Where(c => c.FirstName.ToLower().Contains(lowerKeyword) ||
                            c.LastName.ToLower().Contains(lowerKeyword) ||
                            c.PhoneNumber.ToLower().Contains(lowerKeyword))
                .ToList();
        }
    }
}