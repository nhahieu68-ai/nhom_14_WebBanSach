using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.ViewModels
{
    public class BookDetailViewModel
    {
        public Book Book { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Book> RelatedBooks { get; set; }

        // Dùng cho form Add-to-Cart
        public int Quantity { get; set; } = 1;
    }
}