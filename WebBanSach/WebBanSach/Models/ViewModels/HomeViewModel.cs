using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebBanSach.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Book> NewBooks { get; set; }
        public List<Book> PopularBooks { get; set; }
        public Dictionary<Category, List<Book>> BooksByCategory { get; set; }
        public List<Category> Categories { get; set; }
    }
}
