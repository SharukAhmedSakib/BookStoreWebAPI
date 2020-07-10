using BookStoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreWebAPI.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BookDbContext categoryContext;

        public CategoryRepository(BookDbContext categoryContext)
        {
            this.categoryContext = categoryContext;
        }
        public bool CategoryExists(int categoryId)
        {
            return categoryContext.Categories.Any(c => c.Id == categoryId);
        }

        public bool CreateCategory(Category category)
        {
            categoryContext.Add(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            categoryContext.Remove(category);
            return Save();
        }

        public ICollection<Book> GetBooksForCategory(int categoryId)
        {
            return categoryContext.BookCategories.Where(bc => bc.CategoryId == categoryId).Select(b => b.Book).ToList();
        }

        public ICollection<Category> GetCategories()
        {
            return categoryContext.Categories.OrderBy(c => c.Name).ToList();
        }

        public ICollection<Category> GetCategoriesOfABook(int bookId)
        {
            return categoryContext.BookCategories.Where(bc => bc.BookId == bookId).Select(c => c.Category).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return categoryContext.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }

        public bool IsDuplicateCategoryName(int categoryId, string categoryName)
        {
            var category = categoryContext.Categories.Where(c => c.Name.Trim().ToUpper() == categoryName.Trim().ToUpper() && c.Id != categoryId);

            return category == null ? false : true;
        }

        public bool Save()
        {
            var saved = categoryContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            categoryContext.Update(category);

            return Save();
        }
    }
}
