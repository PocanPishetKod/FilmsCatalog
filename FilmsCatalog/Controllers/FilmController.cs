using FilmsCatalog.Data.Entities;
using FilmsCatalog.Data.Repositories;
using FilmsCatalog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Controllers
{
    [Authorize]
    public class FilmController : Controller
    {
        private readonly IFilmRepository _filmRepository;
        private readonly UserManager<User> _userManager;

        public const int PageSize = 10;

        public FilmController(IFilmRepository filmRepository, UserManager<User> userManager)
        {
            _filmRepository = filmRepository;
            _userManager = userManager;
        }

        private bool CheckContentType(string contentType)
        {
            return contentType.Contains("jpeg") || contentType.Contains("jpg") || contentType.Contains("png");
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]int pageNum, [FromQuery]int pageSize)
        {
            if (pageNum < 1)
                pageNum = 1;

            if (pageSize < 1)
                pageSize = PageSize;

            var result = new FilmsListViewModel()
            {
                PageNum = pageNum,
                PageSize = pageSize
            };

            result.Films = (await _filmRepository.GetFilms(pageNum, pageSize)).Select(e => new FilmViewModel()
            {
                Id = e.Id,
                Description = e.Description,
                Director = e.Director,
                Name = e.Name,
                ReleaseYear = e.ReleaseYear,
                UserId = e.UserId
            }).ToList();

            result.TotalCount = await _filmRepository.GetFilmsCount();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPosterData(int filmId)
        {
            var poster = await _filmRepository.GetFilmPoster(filmId);

            if (poster == null)
                return NotFound();

            return File(poster.Data, poster.ContentType);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var film = await _filmRepository.GetById(id);

            if (film == null)
                return NotFound();

            return View(new FilmViewModel()
            {
                Id = film.Id,
                Name = film.Name,
                Description = film.Description,
                Director = film.Director,
                ReleaseYear = film.ReleaseYear,
                UserId = film.UserId
            });
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var film = await _filmRepository.GetById(id);

            if (film == null)
                return NotFound();

            if (user == null || user.Id != film.UserId)
            {
                return Forbid();
            }

            return View(new UpdateFilmViewModel()
            {
                Film = new FilmViewModel()
                {
                    Id = film.Id,
                    Description = film.Description,
                    Director = film.Director,
                    Name = film.Name,
                    ReleaseYear = film.ReleaseYear,
                    UserId = film.UserId
                }
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required] UpdateFilmViewModel viewModel)
        {
            if (viewModel.PosterFile == null)
                return BadRequest();

            if (!CheckContentType(viewModel.PosterFile.ContentType))
                return BadRequest();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Forbid();

            var entity = new Film()
            {
                Name = viewModel.Film.Name,
                Description = viewModel.Film.Description,
                Director = viewModel.Film.Director,
                ReleaseYear = viewModel.Film.ReleaseYear,
                UserId = user.Id
            };

            await _filmRepository.Create(entity, viewModel.PosterFile);

            return RedirectToAction("Index", "Film", new { pageNum = 1, pageSize = PageSize });
        }

        [HttpPost]
        public async Task<IActionResult> Update([Required] UpdateFilmViewModel viewModel)
        {
            if (viewModel.PosterFile != null && !CheckContentType(viewModel.PosterFile.ContentType))
                return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            var film = await _filmRepository.GetById(viewModel.Film.Id);

            if (film == null)
                return NotFound();

            if (user == null || user.Id != film.UserId)
            {
                return Forbid();
            }

            film.Name = viewModel.Film.Name;
            film.Description = viewModel.Film.Description;
            film.Director = viewModel.Film.Director;
            film.ReleaseYear = viewModel.Film.ReleaseYear;

            await _filmRepository.Update(film, viewModel.PosterFile);

            return RedirectToAction("Index", "Film", new { pageNum = 1, pageSize = 10 });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var film = await _filmRepository.GetById(id);

            if (film == null)
                return NotFound();

            if (user == null || user.Id != film.UserId)
            {
                return Forbid();
            }

            await _filmRepository.Delete(film);

            return RedirectToAction("Index", new { pageNum = 1, pageSize = PageSize });
        }
    }
}
