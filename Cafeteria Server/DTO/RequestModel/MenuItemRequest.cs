﻿using System;

namespace CafeteriaServer.DTO
{
    public class MenuItemRequest
    {
        public string ItemName { get; set; }
        public double Price { get; set; }
        public bool AvailabilityStatus { get; set; }
    }
}
