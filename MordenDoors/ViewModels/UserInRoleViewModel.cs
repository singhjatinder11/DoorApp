using System;

namespace MordenDoors.ViewModels
{
    public class UserInRoleViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Skills { get; set; }
        public bool Status { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }

        public string EmployeeNumber { get; set; }

    }
}