using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using HotelBookingSystem.BLL.Exceptions;
using HotelBookingSystem.BLL.Models;
using HotelBookingSystem.BLL.Services;
using HotelBookingSystem.DAL.Interfaces;

namespace HotelBookingSystem.Tests
{
    [TestFixture]
    public class ClientServiceTests
    {
        private Mock<IRepository<Client>> _mockClientRepository;
        private ClientService _clientService;

        [SetUp]
        public void Setup()
        {
            _mockClientRepository = new Mock<IRepository<Client>>();
            _clientService = new ClientService(_mockClientRepository.Object);
        }

        // --- AddClient Tests (100% coverage) ---

        [Test]
        public void AddClient_ValidClient_AddsClientToRepository()
        {
            var newClient = new Client { FirstName = "John", LastName = "Doe", PhoneNumber = "12345" };
            _mockClientRepository.Setup(r => r.GetAll()).Returns(new List<Client>()); 
            _mockClientRepository.Setup(r => r.Add(It.IsAny<Client>()));

            _clientService.AddClient(newClient);

            _mockClientRepository.Verify(r => r.Add(It.Is<Client>(c => c.FirstName == "John" && c.Id == 1)), Times.Once);
        }

        [Test]
        public void AddClient_ClientWithExistingId_ThrowsValidationException()
        {
            var existingClients = new List<Client> { new Client { Id = 1, FirstName = "Existing", LastName = "Client", PhoneNumber = "54321" } };
            var newClient = new Client { Id = 1, FirstName = "John", LastName = "Doe", PhoneNumber = "12345" };
            
            _mockClientRepository.Setup(r => r.GetAll()).Returns(existingClients);

            Assert.Throws<ValidationException>(() => _clientService.AddClient(newClient));
        }

        [Test]
        public void AddClient_NullClient_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _clientService.AddClient(null));
        }

        [Test]
        public void AddClient_EmptyFirstName_ThrowsValidationException()
        {
            var newClient = new Client { FirstName = "", LastName = "Doe", PhoneNumber = "12345" };
            Assert.Throws<ValidationException>(() => _clientService.AddClient(newClient));
        }

        [Test]
        public void AddClient_EmptyLastName_ThrowsValidationException()
        {
            var newClient = new Client { FirstName = "John", LastName = "", PhoneNumber = "12345" };
            Assert.Throws<ValidationException>(() => _clientService.AddClient(newClient));
        }

        [Test]
        public void AddClient_EmptyPhoneNumber_ThrowsValidationException()
        {
            var newClient = new Client { FirstName = "John", LastName = "Doe", PhoneNumber = "" };
            Assert.Throws<ValidationException>(() => _clientService.AddClient(newClient));
        }

        // --- DeleteClient Tests ---

        [Test]
        public void DeleteClient_ExistingClient_DeletesClientFromRepository()
        {
            var clientToDelete = new Client { Id = 1, FirstName = "John" };
            _mockClientRepository.Setup(r => r.GetById(1)).Returns(clientToDelete);
            _mockClientRepository.Setup(r => r.Delete(1));

            _clientService.DeleteClient(1);

            _mockClientRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [Test]
        public void DeleteClient_ClientNotFound_ThrowsValidationException()
        {
            _mockClientRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Client)null);

            Assert.Throws<ValidationException>(() => _clientService.DeleteClient(999));
        }

        [Test]
        public void DeleteClient_InvalidClientId_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _clientService.DeleteClient(0));
        }

        // --- UpdateClient Tests ---

        [Test]
        public void UpdateClient_ExistingClient_UpdatesClientInRepository()
        {
            var existingClient = new Client { Id = 1, FirstName = "Old", LastName = "Name", PhoneNumber = "111" };
            var updatedClient = new Client { Id = 1, FirstName = "New", LastName = "Name", PhoneNumber = "222" };
            
            _mockClientRepository.Setup(r => r.GetById(1)).Returns(existingClient);
            _mockClientRepository.Setup(r => r.Update(It.IsAny<Client>()));

            _clientService.UpdateClient(updatedClient);

            _mockClientRepository.Verify(r => r.Update(It.Is<Client>(c => c.FirstName == "New" && c.Id == 1)), Times.Once);
        }

        [Test]
        public void UpdateClient_ClientNotFound_ThrowsValidationException()
        {
            var updatedClient = new Client { Id = 999, FirstName = "New", LastName = "Name", PhoneNumber = "222" };
            _mockClientRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Client)null);

            Assert.Throws<ValidationException>(() => _clientService.UpdateClient(updatedClient));
        }

        [Test]
        public void UpdateClient_NullClient_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _clientService.UpdateClient(null));
        }

        [Test]
        public void UpdateClient_InvalidClientId_ThrowsValidationException()
        {
            var updatedClient = new Client { Id = 0, FirstName = "New", LastName = "Name", PhoneNumber = "222" };
            Assert.Throws<ValidationException>(() => _clientService.UpdateClient(updatedClient));
        }

        [Test]
        public void UpdateClient_EmptyFirstName_ThrowsValidationException()
        {
            var existingClient = new Client { Id = 1, FirstName = "Old", LastName = "Name", PhoneNumber = "111" };
            var updatedClient = new Client { Id = 1, FirstName = "", LastName = "Name", PhoneNumber = "222" };
            _mockClientRepository.Setup(r => r.GetById(1)).Returns(existingClient);
            Assert.Throws<ValidationException>(() => _clientService.UpdateClient(updatedClient));
        }

        // --- GetClientDetails Tests ---

        [Test]
        public void GetClientDetails_ExistingClient_ReturnsClient()
        {
            var client = new Client { Id = 1, FirstName = "Test" };
            _mockClientRepository.Setup(r => r.GetById(1)).Returns(client);

            var result = _clientService.GetClientDetails(1);

            Assert.That(result, Is.EqualTo(client));
        }

        [Test]
        public void GetClientDetails_ClientNotFound_ReturnsNull()
        {
            _mockClientRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Client)null);

            var result = _clientService.GetClientDetails(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetClientDetails_InvalidClientId_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _clientService.GetClientDetails(0));
        }

        // --- GetAllClients Tests ---

        [Test]
        public void GetAllClients_ReturnsAllClients()
        {
            var clients = new List<Client> { new Client { Id = 1 }, new Client { Id = 2 } };
            _mockClientRepository.Setup(r => r.GetAll()).Returns(clients);

            var result = _clientService.GetAllClients();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(clients));
        }

        // --- GetClientsSortedByName Tests ---

        [Test]
        public void GetClientsSortedByName_ReturnsClientsSortedByFirstNameThenLastName()
        {
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "Bob", LastName = "Smith" },
                new Client { Id = 2, FirstName = "Alice", LastName = "Johnson" },
                new Client { Id = 3, FirstName = "Alice", LastName = "Zebra" }
            };
            _mockClientRepository.Setup(r => r.GetAll()).Returns(clients);

            var sortedClients = _clientService.GetClientsSortedByName().ToList();

            Assert.That(sortedClients[0].FirstName, Is.EqualTo("Alice"));
            Assert.That(sortedClients[0].LastName, Is.EqualTo("Johnson"));
            Assert.That(sortedClients[1].FirstName, Is.EqualTo("Alice"));
            Assert.That(sortedClients[1].LastName, Is.EqualTo("Zebra"));
            Assert.That(sortedClients[2].FirstName, Is.EqualTo("Bob"));
        }

        // --- GetClientsSortedByLastName Tests ---

        [Test]
        public void GetClientsSortedByLastName_ReturnsClientsSortedByLastNameThenFirstName()
        {
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "Bob", LastName = "Smith" },
                new Client { Id = 2, FirstName = "Alice", LastName = "Johnson" },
                new Client { Id = 3, FirstName = "Charlie", LastName = "Johnson" }
            };
            _mockClientRepository.Setup(r => r.GetAll()).Returns(clients);

            var sortedClients = _clientService.GetClientsSortedByLastName().ToList();

            Assert.That(sortedClients[0].LastName, Is.EqualTo("Johnson"));
            Assert.That(sortedClients[0].FirstName, Is.EqualTo("Alice"));
            Assert.That(sortedClients[1].LastName, Is.EqualTo("Johnson"));
            Assert.That(sortedClients[1].FirstName, Is.EqualTo("Charlie"));
            Assert.That(sortedClients[2].LastName, Is.EqualTo("Smith"));
        }

        // --- SearchClients Tests ---

        [Test]
        public void SearchClients_WithMatchingKeyword_ReturnsFilteredClients()
        {
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", PhoneNumber = "111" },
                new Client { Id = 2, FirstName = "Jane", LastName = "Smith", PhoneNumber = "222" },
                new Client { Id = 3, FirstName = "Peter", LastName = "Jones", PhoneNumber = "333" }
            };
            _mockClientRepository.Setup(r => r.GetAll()).Returns(clients);

            var result = _clientService.SearchClients("john").ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].FirstName, Is.EqualTo("John"));
        }

        [Test]
        public void SearchClients_NoMatchingKeyword_ReturnsEmptyList()
        {
            var clients = new List<Client>
            {
                new Client { Id = 1, FirstName = "John", LastName = "Doe", PhoneNumber = "111" }
            };
            _mockClientRepository.Setup(r => r.GetAll()).Returns(clients);

            var result = _clientService.SearchClients("xyz").ToList();

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void SearchClients_EmptyKeyword_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _clientService.SearchClients(""));
        }
    }
}