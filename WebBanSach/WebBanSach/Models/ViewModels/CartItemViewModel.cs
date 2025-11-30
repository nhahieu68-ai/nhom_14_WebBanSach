using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.ViewModels
{
    public class CartItemViewModel
    {
        public int BookID { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal Total
        {
            get { return Quantity * Price; }
        }
    }
}