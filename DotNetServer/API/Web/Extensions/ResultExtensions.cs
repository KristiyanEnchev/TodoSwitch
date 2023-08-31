namespace Web.Extensions
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Shared;

    public static class ResultExtensions
    {
        public static async Task<ActionResult> ToActionResult<TData>(this Task<Result<TData>> resultTask)
        {
            var result = await resultTask;

            var response = new
            {
                success = result.Success,
                data = result.Data,
                errors = !result.Success ? result.Errors : null
            };

            if (!result.Success)
            {
                if (result.Errors != null)
                {
                    return new BadRequestObjectResult(new { Errors = response.errors });
                }
                return new BadRequestObjectResult("Unexpected Error occured");
            }

            else if (result.Data == null)
            {
                return new NoContentResult();
            }

            if (response.data is string)
            {
                return new OkObjectResult(new
                {
                    message = result.Data,
                });
            }

            return new OkObjectResult(response.data);
        }
    }
}