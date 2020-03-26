using System;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ImageGallery.API.Authorization.MustOwnImage.Handlers
{
    public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGalleryRepository _galleryRepository;

        public MustOwnImageHandler(IHttpContextAccessor httpContextAccessor, IGalleryRepository galleryRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _galleryRepository = galleryRepository;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       MustOwnImageRequirement requirement)
        {
            //Busca o id da imagem na rota que chamou a conexão.
            var imageId = _httpContextAccessor.HttpContext.GetRouteValue("id").ToString();

            if (!Guid.TryParse(imageId, out Guid imageIdAsGuid))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            //Busca o Id nos claims
            var ownerId = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            //Verifica se a imagem é da pessoa que esta fazendo a requisição
            if(!_galleryRepository.IsImageOwner(imageIdAsGuid, ownerId))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            //Se tudo passar, ta liberado a badernah.
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
