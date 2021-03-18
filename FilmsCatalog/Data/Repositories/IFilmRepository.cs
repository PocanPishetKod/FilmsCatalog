using FilmsCatalog.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Data.Repositories
{
    public interface IFilmRepository
    {
        Task<IReadOnlyCollection<Film>> GetFilms(int pageNum, int pageSize);

        Task<int> GetFilmsCount();

        Task<Film> GetById(int id);

        Task<FilmPoster> GetFilmPoster(int filmId);

        Task Create(Film film, IFormFile formFile);

        Task Update(Film film, IFormFile formFile);

        Task Delete(Film film);
    }
}
