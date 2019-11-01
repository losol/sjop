using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sjop.Models;

namespace Sjop.ViewModels
{
    public class AddItemToCartVM
    {
        public int ProductId { get; set; }
    }
}
