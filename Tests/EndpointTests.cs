using EntityTransformerApi.Endpoints;
using EntityTransformerApi.Endpoints.Responses;
using EntityTransformerApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System.Text.Json.Nodes;

namespace Tests;
public class EndpointTests
{
    private readonly Mock<ITextTransformProcessor> textTransformProcessor = new();

    [Fact]
    public void No_Transformable_Entity_Match_Error()
    {
        textTransformProcessor.Setup(x => x.ProcessRequest(It.IsAny<JsonObject>()))
            .Returns(new TransformResponse
            {
                ModifiedEntity = [],
                Metadata = []
            });

        TransformEndpoint transformEndpoint = new();
        IResult result = transformEndpoint.Transform(new JsonObject
        {
            ["name"] = "John Doe",
            ["description"] = "Senior Developer"
        }, textTransformProcessor.Object);

        Assert.IsType<BadRequest<ErrorResponse>>(result);
        BadRequest<ErrorResponse>? badRequestResult = result as BadRequest<ErrorResponse>;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("Payload does not match any known transformable entity", badRequestResult.Value.Error);
    }

    [Fact]
    public void Process_Request_Exception()
    {
        textTransformProcessor.Setup(x => x.ProcessRequest(It.IsAny<JsonObject>()))
            .Throws(new InvalidOperationException("An error occured"));

        TransformEndpoint transformEndpoint = new();
        IResult result = transformEndpoint.Transform(new JsonObject
        {
            ["name"] = "John Doe",
            ["description"] = "Senior Developer"
        }, textTransformProcessor.Object);

        Assert.IsType<BadRequest<ErrorResponse>>(result);
        BadRequest<ErrorResponse>? badRequestResult = result as BadRequest<ErrorResponse>;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorResponse>(badRequestResult.Value);
        Assert.Equal("An error ocurred, please contact system administrator", badRequestResult.Value.Error);
    }

    [Fact]
    public void EntityA_Should_Return_Ok_Result()
    {
        textTransformProcessor.Setup(x => x.ProcessRequest(It.IsAny<JsonObject>()))
            .Returns(new TransformResponse
            {
                ModifiedEntity = new Dictionary<string, string> { { "Name", "john doe" }, { "Description", "Senior Developer" } },
                Metadata = new Dictionary<string, MetaData> { { "Name", new MetaData { Transformations = ["lowercase"], CharacterCount = 8 } } }
            });

        TransformEndpoint transformEndpoint = new();
        IResult result = transformEndpoint.Transform(new JsonObject
        {
            ["name"] = "John Doe",
            ["description"] = "Senior Developer"
        }, textTransformProcessor.Object);

        Assert.IsType<Ok<TransformResponse>>(result);
        Ok<TransformResponse>? okResult = result as Ok<TransformResponse>;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<TransformResponse>(okResult.Value);
        Assert.Equal(2, okResult.Value.ModifiedEntity.Count);
        Assert.Equal("john doe", okResult.Value.ModifiedEntity["Name"]);
        Assert.Equal("Senior Developer", okResult.Value.ModifiedEntity["Description"]);
        Assert.Single(okResult.Value.Metadata);
        List<string> transformations = okResult.Value.Metadata["Name"].Transformations;
        Assert.Single(transformations);
        Assert.Equal("lowercase", transformations[0]);
        Assert.Equal(8, okResult.Value.Metadata["Name"].CharacterCount);
    }

    [Fact]
    public void EntityA_Uppercase_Properties_Should_Return_Ok_Result()
    {
        textTransformProcessor.Setup(x => x.ProcessRequest(It.IsAny<JsonObject>()))
            .Returns(new TransformResponse
            {
                ModifiedEntity = new Dictionary<string, string> { { "Name", "john doe" }, { "Description", "Senior Developer" } },
                Metadata = new Dictionary<string, MetaData> { { "Name", new MetaData { Transformations = ["lowercase"], CharacterCount = 8 } } }
            });

        TransformEndpoint transformEndpoint = new();
        IResult result = transformEndpoint.Transform(new JsonObject
        {
            ["NAME"] = "John Doe",
            ["DESCRIPTION"] = "Senior Developer"
        }, textTransformProcessor.Object);

        Assert.IsType<Ok<TransformResponse>>(result);
        Ok<TransformResponse>? okResult = result as Ok<TransformResponse>;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<TransformResponse>(okResult.Value);
        Assert.Equal(2, okResult.Value.ModifiedEntity.Count);
        Assert.Equal("john doe", okResult.Value.ModifiedEntity["Name"]);
        Assert.Equal("Senior Developer", okResult.Value.ModifiedEntity["Description"]);
        Assert.Single(okResult.Value.Metadata);
        List<string> transformations = okResult.Value.Metadata["Name"].Transformations;
        Assert.Single(transformations);
        Assert.Equal("lowercase", transformations[0]);
        Assert.Equal(8, okResult.Value.Metadata["Name"].CharacterCount);
    }

    [Fact]
    public void EntityB_Should_Return_Ok_Result()
    {
        textTransformProcessor.Setup(x => x.ProcessRequest(It.IsAny<JsonObject>()))
            .Returns(new TransformResponse
            {
                ModifiedEntity = new Dictionary<string, string> { { "Title", "HELLO WORLD" }, { "Content", "This-is-content" } },
                Metadata = new Dictionary<string, MetaData> 
                { 
                    { "Title", new MetaData { Transformations = ["uppercase"] } },
                    { "Content", new MetaData { Transformations = ["dashreplacement"], CharacterCount = 15 }}
                }
            });

        TransformEndpoint transformEndpoint = new();
        IResult result = transformEndpoint.Transform(new JsonObject
        {
            ["title"] = "Hello World",
            ["content"] = "This is content"
        }, textTransformProcessor.Object);

        Assert.IsType<Ok<TransformResponse>>(result);
        Ok<TransformResponse>? okResult = result as Ok<TransformResponse>;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<TransformResponse>(okResult.Value);
        Assert.Equal(2, okResult.Value.ModifiedEntity.Count);
        Assert.Equal("HELLO WORLD", okResult.Value.ModifiedEntity["Title"]);
        Assert.Equal("This-is-content", okResult.Value.ModifiedEntity["Content"]);
        Assert.Equal(2, okResult.Value.Metadata.Count);
        List<string> transformations = okResult.Value.Metadata["Title"].Transformations;
        Assert.Single(transformations);
        Assert.Equal("uppercase", transformations[0]);
        transformations = okResult.Value.Metadata["Content"].Transformations;
        Assert.Equal("dashreplacement", transformations[0]);
        Assert.Equal(15, okResult.Value.Metadata["Content"].CharacterCount);
    }
}
