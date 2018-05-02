﻿namespace Forum.App.Services
{
    using Forum.App.Contracts;
    using Forum.App.ViewModels;
    using Forum.Data;
    using Forum.DataModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class PostService : IPostService
    {
        private ForumData forumData;
        private IUserService userService;

        public PostService(ForumData forumData, IUserService userService)
        {
            this.forumData = forumData;
            this.userService = userService;
        }

        public int AddPost(int userId, string postTitle, string postCategory, string postContent)
        {
            bool emptyCategory = string.IsNullOrWhiteSpace(postCategory);
            bool emptyTitle = string.IsNullOrWhiteSpace(postTitle);
            bool emptyContent = string.IsNullOrWhiteSpace(postContent);

            if (emptyCategory || emptyContent || emptyTitle)
            {
                throw new ArgumentException("All fields must be filled!");
            }

            Category category = this.EnsureCategory(postCategory);
            int postId = this.forumData.Posts.Any() ? this.forumData.Posts.Last().Id + 1 : 1;
            User author = this.userService.GetUserById(userId);

            Post post = new Post(postId, postTitle, postContent, category.Id, userId, new List<int>());

            this.forumData.Posts.Add(post);
            author.Posts.Add(postId);
            category.Posts.Add(postId);
            this.forumData.SaveChanges();

            return postId;
        }

        private Category EnsureCategory(string postCategory)
        {
            if (this.forumData.Categories.Any(c => c.Name == postCategory))
            {
                return this.forumData.Categories.First(c => c.Name == postCategory);
            }
            else
            {
                int categoryId = this.forumData.Categories.Any() ? this.forumData.Categories.Last().Id + 1 : 1;
                Category category = new Category(categoryId,postCategory, new List<int>());
                this.forumData.Categories.Add(category);
                return category;
            }
        }

        public void AddReplyToPost(int postId, string replyContents, int userId)
        {
            bool emptyContent = string.IsNullOrWhiteSpace(replyContents);

            if (emptyContent)
            {
                throw new ArgumentException("All fields must be filled!");
            }

            int replyId = this.forumData.Replies.Any() ? this.forumData.Replies.Last().Id + 1 : 1;

            Reply reply = new Reply(replyId, replyContents, userId, postId);

            Post post = this.forumData.Posts.FirstOrDefault(p => p.Id == postId);
            post.Replies.Add(replyId);

            this.forumData.Replies.Add(reply);
            this.forumData.SaveChanges();
        }

        public IEnumerable<ICategoryInfoViewModel> GetAllCategories()
        {
            IEnumerable<ICategoryInfoViewModel> categories = this.forumData
                .Categories.Select(c => new CategoryInfoViewModel(c.Id, c.Name, c.Posts.Count));
            return categories;
        }

        public string GetCategoryName(int categoryId)
        {
            string categoryName = this.forumData.Categories.Find(c => c.Id == categoryId)?.Name;

            if (categoryName == null)
            {
                throw new ArgumentException($"Category with id {categoryId} not found!");
            }

            return categoryName;
        }

        public IEnumerable<IPostInfoViewModel> GetCategoryPostsInfo(int categoryId)
        {
            return this.forumData.Posts
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new PostInfoViewModel(p.Id, p.Title, p.Replies.Count));
        }

        public IPostViewModel GetPostViewModel(int postId)
        {
            Post post = this.forumData.Posts.FirstOrDefault(p => p.Id == postId);

            return new PostViewModel(post.Title, this.userService.GetUserName(post.AuthorId), post.Content, this.GetPostReplies(postId));
        }

        private IEnumerable<IReplyViewModel> GetPostReplies(int postId)
        {
            return this.forumData.Replies.Where(p => p.PostId == postId)
                .Select(r => new ReplyViewModel(this.userService.GetUserName(r.AuthorId), r.Content));
        }
    }
}
