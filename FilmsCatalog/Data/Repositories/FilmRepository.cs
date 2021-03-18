using FilmsCatalog.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Data.Repositories
{
    public class FilmRepository : IFilmRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FilmRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private void WriteStreamToArray(Stream stream, byte[] array)
        {
            int index = 0, bt;
            while ((bt = stream.ReadByte()) != -1)
            {
                array[index++] = (byte)bt;
            }
        }

        public Task<Film> GetById(int id)
        {
            return _dbContext.Films
                .Include(f => f.Poster)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public Task<FilmPoster> GetFilmPoster(int filmId)
        {
            return _dbContext.FilmPosters
                .AsNoTracking()
                .FirstOrDefaultAsync(fp => fp.FilmId == filmId);
        }

        public async Task<IReadOnlyCollection<Film>> GetFilms(int pageNum, int pageSize)
        {
            if (pageNum < 1 || pageSize < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            var result = await _dbContext.Films
                .AsNoTracking()
                .OrderBy(f => f.Id)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return result;
        }

        public async Task<int> GetFilmsCount()
        {
            return await _dbContext.Films.CountAsync();
        }

        public async Task Create(Film film, IFormFile formFile)
        {
            _dbContext.Films.Add(film);
            await _dbContext.SaveChangesAsync();

            using (var stream = formFile.OpenReadStream())
            {
                var poster = new FilmPoster()
                {
                    FilmId = film.Id,
                    ContentType = formFile.ContentType,
                    Data = new byte[stream.Length]
                };

                WriteStreamToArray(stream, poster.Data);

                _dbContext.FilmPosters.Add(poster);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task Update(Film film, IFormFile formFile)
        {
            if (_dbContext.ChangeTracker.Entries<Film>().FirstOrDefault(f => f.Entity == film) == null)
            {
                _dbContext.Attach(film);
            }

            _dbContext.Films.Update(film);
            
            if (formFile != null)
            {
                if (film.Poster == null)
                {
                    film.Poster = await GetFilmPoster(film.Id);
                }

                using (var stream = formFile.OpenReadStream())
                {
                    film.Poster.Data = new byte[stream.Length];
                    film.Poster.ContentType = formFile.ContentType;
                    WriteStreamToArray(stream, film.Poster.Data);
                }

                _dbContext.FilmPosters.Update(film.Poster);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Film film)
        {
            _dbContext.Films.Remove(film);
            await _dbContext.SaveChangesAsync();
        }
    }
}
