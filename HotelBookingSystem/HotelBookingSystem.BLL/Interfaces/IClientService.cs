using System.Collections.Generic;
using HotelBookingSystem.BLL.Models;

namespace HotelBookingSystem.BLL.Interfaces
{
    public interface IClientService
    {
        void AddClient(Client client);
        void DeleteClient(int clientId);
        void UpdateClient(Client client);
        Client GetClientDetails(int clientId);
        IEnumerable<Client> GetAllClients();
        IEnumerable<Client> GetClientsSortedByName();
        IEnumerable<Client> GetClientsSortedByLastName();
        IEnumerable<Client> SearchClients(string keyword);
    }
}