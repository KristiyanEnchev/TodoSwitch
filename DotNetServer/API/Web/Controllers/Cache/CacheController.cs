namespace Web.Controllers.Health
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Swashbuckle.AspNetCore.Annotations;

    using Application.Interfaces.Services;

    public class CacheController : ApiController
    {
        private readonly ICachedTodoService _cachedTodoService;

        public CacheController( ICachedTodoService cachedTodoService)
        {
            _cachedTodoService = cachedTodoService;
        }

        [HttpDelete]
        [AllowAnonymous]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Invalid or missing data supplied")]
        [SwaggerResponse(500, "Internal Server error")]
        [SwaggerOperation("Conroller to invalidate the local cache", "Invalidate the localCache.")]
        public async Task<IActionResult> InvalidateCache()
        {
            _cachedTodoService.InvalidateAllCache();

            return Ok();
        }
    }
}