using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Yeetegy.Web.ViewModels
{
    public class CategorysViewModel
    {
        public string CurrentName { get; set; }

        public string CurrentUrl { get; set; }

        public IEnumerable<CategoryViewModel> CategoryViewModels { get; set; }
    }
}