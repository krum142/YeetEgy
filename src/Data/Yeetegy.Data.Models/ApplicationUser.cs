// ReSharper disable VirtualMemberCallInConstructor
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.PostVotes = new HashSet<UserPostVote>();
            this.Posts = new HashSet<Post>();
            this.Comments = new HashSet<Comment>();
        }

        // Yeetegy Additional
        public ICollection<UserPostVote> PostVotes { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public string AvatarUrl { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
    }
}
