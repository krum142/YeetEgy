using System.Collections.Generic;

namespace Yeetegy.Web.ViewModels
{
    public class CategorysViewModel
    {
        public string CurrentName { get; set; }

        public string CurrentUrl { get; set; }

        public IEnumerable<CategoryViewModel> CategoryViewModels { get; set; }
    }
}