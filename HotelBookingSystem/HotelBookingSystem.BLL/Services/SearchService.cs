using System.Collections.Generic;
using HotelBookingSystem.BLL.Interfaces;
using HotelBookingSystem.BLL.Models;

namespace HotelBookingSystem.BLL.Services
{
    public class SearchService 
    {
        private readonly IHotelService _hotelService;
        private readonly IClientService _clientService;

        public SearchService(IHotelService hotelService, IClientService clientService)
        {
            _hotelService = hotelService;
            _clientService = clientService;
        }

        public IEnumerable<Hotel> SearchHotels(string keyword)
        {
            return _hotelService.SearchHotels(keyword);
        }

        public IEnumerable<Client> SearchClients(string keyword)
        {
            return _clientService.SearchClients(keyword);
        }
    }
}