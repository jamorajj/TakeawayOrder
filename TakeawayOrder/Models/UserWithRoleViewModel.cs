﻿namespace TakeawayOrder.Models
{
    public class UserWithRoleViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int CheckoutTotal { get; set; }
    }
}
