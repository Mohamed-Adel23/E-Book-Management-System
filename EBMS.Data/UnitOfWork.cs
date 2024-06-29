﻿using EBMS.Data.DataAccess;
using EBMS.Data.Services;
using EBMS.Infrastructure;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.IServices.IAuth;
using Microsoft.AspNetCore.Hosting;

namespace EBMS.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookDbContext _context;

        // wwwroot Location
        private readonly IWebHostEnvironment _webHostEnvironment;
        // Services (Repositories)
        public ICategoryService Categories { get; private set; }
        public IAuthorService Authors { get; private set; }
        public IBookService Books { get; private set; }

        // Inject The Context and Initialize The Services (Repositories)
        public UnitOfWork(BookDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            Categories = new CategoryService(_context);
            Authors = new AuthorService(_context);
            Books = new BookService(_context, _webHostEnvironment);
        }

        // Save Changes into Database with UoW, returns number of affected rows
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Damage The Current Context and Release its ralated resources
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}