using Microsoft.AspNetCore.Authorization;

namespace ImageGallery.API.Authorization.MustOwnImage
{
    public class MustOwnImageRequirement : IAuthorizationRequirement
    {
        public MustOwnImageRequirement()
        {
        }
    }
}
