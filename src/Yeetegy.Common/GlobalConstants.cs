using System.Collections.Generic;

namespace Yeetegy.Common
{
    public static class GlobalConstants
    {
        public const string SystemName = "Yeetegy";

        public const string AdministratorRoleName = "Administrator";

        public const string UserRoleName = "User";

        public const int LoadPostCountAjax = 5;

        public const int LoadUserPostsCountAjax = 5;

        public const int LoadCommentsCountAjax = 10;

        public const string TagValidationRegex = "#[A-Za-z0-9]{1,30}";

        public const string DefaultUserImg = "https://res.cloudinary.com/cloudnameiscool/image/upload/v1586729131/YeetegyDefaultAvatar_c0pefi.jpg"; // save it in the cloudinary

        public static Dictionary<string, string> ConstantCategories = new Dictionary<string, string>()
        {
            { "Newest", "https://external-content.duckduckgo.com/iu/?u=http%3A%2F%2Fimage.syracuse.com%2Fhome%2Fsyr-media%2Fwidth620%2Fimg%2Fnews%2Fphoto%2F2014%2F03%2Fne-140304-daylightsavingsjpg-055c1d8de9cfcc67.jpg&f=1&nofb=1" },
            { "Popular", "https://external-content.duckduckgo.com/iu/?u=http%3A%2F%2F2.bp.blogspot.com%2F-R0trJG1LsKQ%2FT8zg8U4bZ4I%2FAAAAAAAADoo%2Fk_C_PmI3SQc%2Fs1600%2FFire%2BFlames%2B3.jpg&f=1&nofb=1" },
            { "Discussed", "https://external-content.duckduckgo.com/iu/?u=http%3A%2F%2Fwww.quickmeme.com%2Fimg%2F6b%2F6b2049ba637ec47baf0e879cf88a73811940cfa6999f0b85e21cf3954e47d999.jpg&f=1&nofb=1" },
        };
    }
}
