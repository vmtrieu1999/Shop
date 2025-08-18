using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public UserSession(int userId, string username, string role, bool isActive, string company)
        {
            this.UserId = userId;
            this.Username = username;
            this.Role = role;
            this.IsActive = isActive;
            this.CompanyCode = company;
        }
    }
}