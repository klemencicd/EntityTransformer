using EntityTransformerApi.Endpoints.Responses;
using EntityTransformerApi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json.Nodes;

namespace Tests;

public class TransformationTests
{
    private readonly Mock<ILogger<TextTransformProcessor>> loggerMock = new();

    [Fact]
    public void Name_To_Lower_Case_With_Characters_Count()
    {
        TextTransformProcessor textTransformProcessor = new(loggerMock.Object);
        TransformResponse response = textTransformProcessor.ProcessRequest(new JsonObject
        {
            ["name"] = "John Doe",
            ["description"] = "Senior Developer"
        });

        Assert.NotEmpty(response.ModifiedEntity);
        Assert.Equal(2, response.ModifiedEntity.Count);
        Assert.Equal("john doe", response.ModifiedEntity["Name"]);
        Assert.Equal("Senior Developer", response.ModifiedEntity["Description"]);
        Assert.Single(response.Metadata);
        List<string> transformations = response.Metadata["Name"].Transformations;
        Assert.Single(transformations);
        Assert.Equal("lowercase", transformations[0]);
        Assert.Equal(8, response.Metadata["Name"].CharacterCount);
    }

    [Fact]
    public void Title_To_Upper_Case_Content_Dashed_With_Characters_Count()
    {
        TextTransformProcessor textTransformProcessor = new(loggerMock.Object);
        TransformResponse response = textTransformProcessor.ProcessRequest(new JsonObject
        {
            ["title"] = "Hello World",
            ["content"] = "This is content"
        });

        Assert.NotEmpty(response.ModifiedEntity);
        Assert.Equal(2, response.ModifiedEntity.Count);
        Assert.Equal("HELLO WORLD", response.ModifiedEntity["Title"]);
        Assert.Equal("This-is-content", response.ModifiedEntity["Content"]);
        Assert.Equal(2, response.Metadata.Count);
        List<string> transformations = response.Metadata["Title"].Transformations;
        Assert.Single(transformations);
        Assert.Equal("uppercase", transformations[0]);
        transformations = response.Metadata["Content"].Transformations;
        Assert.Single(transformations);
        Assert.Equal("dashreplacement", transformations[0]);
        Assert.Equal(15, response.Metadata["Content"].CharacterCount);
    }

    [Fact]
    public void Unknown_Entity()
    {
        TextTransformProcessor textTransformProcessor = new(loggerMock.Object);
        TransformResponse response = textTransformProcessor.ProcessRequest(new JsonObject
        {
            ["propertyOne"] = "Hello World",
            ["propertyTwo"] = "This is content"
        });

        Assert.Empty(response.ModifiedEntity);
        Assert.Empty(response.Metadata);
    }
}