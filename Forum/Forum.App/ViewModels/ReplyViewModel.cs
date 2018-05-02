using Forum.App.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.App.ViewModels
{
    class ReplyViewModel : ContentViewModel, IReplyViewModel
    {
        public ReplyViewModel(string author, string content) : base(content)
        {
            this.Author = author;
            this.Content = this.GetLines(content);
        }

        public string Author { get; }

        public string[] Content { get; }
    }
}
