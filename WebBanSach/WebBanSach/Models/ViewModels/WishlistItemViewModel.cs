using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WebBanSach.Models;

namespace WebBanSach.Models.ViewModels
{
    public class WishlistItemViewModel
{
    public int BookID { get; set; }
    public Book Book { get; set; }
}
}