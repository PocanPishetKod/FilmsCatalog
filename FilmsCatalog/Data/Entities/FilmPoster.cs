using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Data.Entities
{
    public class FilmPoster
    {
        public int Id { get; set; }

        public byte[] Data { get; set; }

        public string ContentType { get; set; }

        public int FilmId { get; set; }

        public virtual Film Film { get; set; }
    }
}
