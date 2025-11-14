using System;

namespace HotelBookingSystem.BLL.Models
{
    public class Client : IComparable<Client>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public int CompareTo(Client other)
        {
            if (other == null) return 1;

            int lastNameComparison = LastName.CompareTo(other.LastName);
            if (lastNameComparison != 0)
            {
                return lastNameComparison;
            }
            return FirstName.CompareTo(other.FirstName);
        }
    }
}