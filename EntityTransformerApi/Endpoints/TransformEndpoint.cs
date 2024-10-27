using Carter;
using EntityTransformerApi.Endpoints.Responses;
using EntityTransformerApi.Services.Interfaces;
using System.Text.Json.Nodes;

namespace EntityTransformerApi.Endpoints;

public class TransformEndpoint() : CarterModule("/transform")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/", Transform)
            .WithName("Transform")
            .WithOpenApi(); ;
    }

    public IResult Transform(JsonObject data, ITextTransformProcessor textTransformProcessor)
    {
        try
        {
            TransformResponse result = textTransformProcessor.ProcessRequest(data);
            if (result.ModifiedEntity.Count == 0 && result.Metadata.Count == 0)
            {
                return TypedResults.BadRequest(new ErrorResponse("Payload does not match any known transformable entity" ));
            }

            return TypedResults.Ok(result);
        }
        catch
        {
            return TypedResults.BadRequest(new ErrorResponse("An error ocurred, please contact system administrator"));
        }
    }
}
