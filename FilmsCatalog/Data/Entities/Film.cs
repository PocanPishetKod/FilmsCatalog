using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Data.Entities
{
    public class Film
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Director { get; set; }

        public DateTime ReleaseYear { get; set; }

        public virtual FilmPoster Poster { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
