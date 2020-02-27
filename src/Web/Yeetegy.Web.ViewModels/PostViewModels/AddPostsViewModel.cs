﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class AddPostsViewModel
    {
        public IFormFile File { get; set; }

        [MaxLength(200)]
        public string Tittle { get; set; }

        public string Category { get; set; }
    }
}