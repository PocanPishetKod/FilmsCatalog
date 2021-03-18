using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Models
{
    public class FilmsListViewModel
    {
        public IReadOnlyCollection<FilmViewModel> Films { get; set; }

        public int TotalCount { get; set; }

        public int PageNum { get; set; }

        public int PageSize { get; set; }
    }
}
