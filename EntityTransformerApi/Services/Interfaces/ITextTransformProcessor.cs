using EntityTransformerApi.Endpoints.Responses;
using System.Text.Json.Nodes;

namespace EntityTransformerApi.Services.Interfaces;

public interface ITextTransformProcessor
{
    TransformResponse ProcessRequest(JsonObject data);
}
