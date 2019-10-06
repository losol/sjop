﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppur.Models
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public ICollection<CartItem> CartItems { get; set; }
    }
}
