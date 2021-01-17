using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Internal;
using Yeetegy.Data.Models;

namespace Yeetegy.Data.Seeding
{
    public class CategoriesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            var categories = new List<(string Name, string ImageUrl)>
            {
                ("Funny", "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fcdn.images.express.co.uk%2Fimg%2Fdynamic%2F10%2F285x214%2F70400_1.jpg&f=1&nofb=1"),
                ("Coronavirus", "https://cdn.wccftech.com/wp-content/uploads/2020/01/https___cdn.cnn_.com_cnnnext_dam_assets_200108214800-coronavirus.jpg"),
                ("WTF", "https://skybambi.files.wordpress.com/2011/01/wtf-1920x1200-01v01.jpg"),
                ("Cool", "http://img.dreamdealer.nl/dd/cool/001.jpg"),
                ("Programming", "https://learnworthy.net/wp-content/uploads/2019/12/Why-programming-is-the-skill-you-have-to-learn.jpg"),
                ("Cats", "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi.ytimg.com%2Fvi%2FbDVZW14fo-4%2Fhqdefault.jpg&f=1&nofb=1"),
                ("Dogs", "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Ftse2.mm.bing.net%2Fth%3Fid%3DOIP.1U4LN4zQGhYu58a66AL0bQHaEK%26pid%3DApi&f=1"),
            };
            foreach (var category in categories)
            {
                await dbContext.Categories.AddAsync(new Category
                {
                    Name = category.Name,
                    ImageUrl = category.ImageUrl,
                });
            }
        }
    }
}