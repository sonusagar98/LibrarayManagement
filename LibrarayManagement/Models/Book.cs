using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarayManagement.Models
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }

        public string Publication { get; set; }

        public Book(string title, string author, string publication)
        {
            Title = title;
            Author = author;
            Publication = publication;
        }
    }
}
