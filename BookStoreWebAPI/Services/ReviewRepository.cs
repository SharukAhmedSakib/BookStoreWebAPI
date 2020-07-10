using BookStoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreWebAPI.Services
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly BookDbContext reviewContext;

        public ReviewRepository(BookDbContext reviewContext)
        {
            this.reviewContext = reviewContext;
        }

        public bool CreateReview(Review review)
        {
            reviewContext.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            reviewContext.Remove(review);
            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            reviewContext.RemoveRange(reviews);
            return Save();
        }

        public Book GetBookOfAReview(int reviewId)
        {
            return reviewContext.Reviews.Where(r => r.Id == reviewId).Select(r => r.Book).FirstOrDefault();
        }

        public Review GetReview(int reviewId)
        {
            return reviewContext.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return reviewContext.Reviews.OrderBy(r => r.Headline).ToList();
        }

        public ICollection<Review> GetReviewsOfABook(int bookId)
        {
            return reviewContext.Reviews.Where(r => r.Book.Id == bookId).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return reviewContext.Reviews.Any(r=>r.Id==reviewId);
        }

        public bool Save()
        {
            var saved = reviewContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            reviewContext.Update(review);
            return Save();
        }
    }
}
