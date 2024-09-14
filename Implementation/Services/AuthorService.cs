using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;

        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            return await _context.Authors
                         .Select(d => new AuthorDto
                         {
                             Id = d.Id,
                             Name = d.Name,
                             Biography = d.Biography
                         }).ToListAsync();
        }


        public async Task<AuthorDto> GetAuthorByIdAsync(Guid id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return null;

            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography
            };
        }

        public async Task<AuthorDto> CreateAuthorAsync(AuthorRequestModel authorRequest)
        {
            var author = new Author
            {
                Name = authorRequest.Name,
                Biography = authorRequest.Biography
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography
            };
        }

        public async Task<AuthorDto> UpdateAuthorAsync(Guid id, AuthorRequestModel authorRequest)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return null;

            author.Name = authorRequest.Name;
            author.Biography = authorRequest.Biography;
            await _context.SaveChangesAsync();

            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography
            };
        }

        public async Task<bool> DeleteAuthorAsync(Guid id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return true;
        }

        //public async Task<IEnumerable<SelectListItem>> GetAuthorSelectList()
        //{
        //    var authors = await GetAllAuthorsAsync();
        //    var authorList = authors.Select(d => new SelectListItem
        //    {
        //        Value = d.Id.ToString(),
        //        Text = d.Name
        //    });

        //    return new SelectList(authorList, "Value", "Text");
        //}
        public async Task<IEnumerable<SelectListItem>> GetAuthorSelectList()
        {
            var authors = await _context.Authors.ToListAsync();
            return authors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name // Or any other property you want to display
            });
        }
    }


}
