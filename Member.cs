using System;

namespace LibraryManagementSystem2
{
    public class Member
    {
        public int MemberID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }

        public Member() { }

        public Member(int memberID, string name, string address, string phone, string email, DateTime registrationDate)
        {
            MemberID = memberID;
            Name = name;
            Address = address;
            Phone = phone;
            Email = email;
            RegistrationDate = registrationDate;
        }

        public override string ToString()
        {
            return $"{Name} ({Email}) - Registered on {RegistrationDate:yyyy-MM-dd}";
        }
    }
}
