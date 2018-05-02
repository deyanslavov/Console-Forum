namespace Forum.App.Commands
{
    using Forum.App.Contracts;
    using System;

    public class SubmitCommand : ICommand
    {
        private ISession session;
        private IPostService postService;
        private ICommandFactory commandFactory;

        public SubmitCommand(ISession session, IPostService postService, ICommandFactory commandFactory)
        {
            this.session = session;
            this.postService = postService;
            this.commandFactory = commandFactory;
        }

        public IMenu Execute(params string[] args)
        {
            string replyContent = args[0];
            int postId = int.Parse(args[1]);

            int userId = this.session.UserId;

            if (string.IsNullOrWhiteSpace(replyContent))
            {
                throw new ArgumentException("Reply cannot be empty!");
            }

            this.postService.AddReplyToPost(postId, replyContent, userId);

            ICommand command = this.commandFactory.CreateCommand("ViewReplyMenu");

            return this.session.Back();
        }
    }
}
