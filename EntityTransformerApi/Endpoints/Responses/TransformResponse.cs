namespace EntityTransformerApi.Endpoints.Responses;

public record TransformResponse
{
    public Dictionary<string, string> ModifiedEntity { get; set; } = [];
    public Dictionary<string, MetaData> Metadata { get; set; } = [];
}


public record MetaData
{
    public List<string> Transformations { get; set; } = [];
    public int? CharacterCount { get; set; } = null;
}

public record ErrorResponse(string Error);