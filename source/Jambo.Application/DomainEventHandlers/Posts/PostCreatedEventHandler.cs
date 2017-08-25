﻿using Jambo.Domain.Aggregates.Blogs;
using Jambo.Domain.Aggregates.Posts;
using Jambo.Domain.Aggregates.Posts.Events;
using MediatR;
using System;


namespace Jambo.Application.DomainEventHandlers.Posts
{
    public class PostCreatedEventHandler : INotificationHandler<PostCreatedDomainEvent>
    {
        private readonly IPostReadOnlyRepository _postReadOnlyRepository;
        private readonly IPostWriteOnlyRepository _postWriteOnlyRepository;
        private readonly IBlogReadOnlyRepository _blogReadOnlyRepository;
        private readonly IBlogWriteOnlyRepository _blogWriteOnlyRepository;

        public PostCreatedEventHandler(
            IPostReadOnlyRepository postReadOnlyRepository,
            IPostWriteOnlyRepository postWriteOnlyRepository,
            IBlogReadOnlyRepository blogReadOnlyRepository,
            IBlogWriteOnlyRepository blogWriteOnlyRepository)
        {
            _postReadOnlyRepository = postReadOnlyRepository ??
                throw new ArgumentNullException(nameof(postReadOnlyRepository));
            _postWriteOnlyRepository = postWriteOnlyRepository ??
                throw new ArgumentNullException(nameof(postWriteOnlyRepository));
            _blogReadOnlyRepository = blogReadOnlyRepository ??
                throw new ArgumentNullException(nameof(blogReadOnlyRepository));
            _blogWriteOnlyRepository = blogWriteOnlyRepository ??
                throw new ArgumentNullException(nameof(blogWriteOnlyRepository));
        }
        public void Handle(PostCreatedDomainEvent message)
        {

            Blog blog = _blogReadOnlyRepository.GetBlog(message.BlogId).Result;

            if (blog.Version == message.BlogVersion)
            {
                Post post = new Post();
                post.Apply(message);

                _postWriteOnlyRepository.AddPost(post).Wait();

                blog.Apply(message);
                _blogWriteOnlyRepository.UpdateBlog(blog).Wait();
            }
        }
    }
}
