using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Models
{
    public class UpdateFilmViewModel
    {
        [Required]
        public FilmViewModel Film { get; set; }

        public IFormFile PosterFile { get; set; }
    }
}
