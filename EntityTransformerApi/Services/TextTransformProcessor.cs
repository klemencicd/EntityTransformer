using EntityTransformerApi.Endpoints.Responses;
using EntityTransformerApi.Services.Interfaces;
using Models.Attributes;
using Models.Enums;
using Models.Interfaces;
using System.Reflection;
using System.Text.Json.Nodes;

namespace EntityTransformerApi.Services;

public class TextTransformProcessor(ILogger<TextTransformProcessor> _logger) : ITextTransformProcessor
{
    public TransformResponse ProcessRequest(JsonObject data)
    {
        try
        {
            List<Type> allEntities = Assembly.Load(new AssemblyName("Models")).GetTypes()
                .Where(predicate: type => typeof(ITransformable).IsAssignableFrom(type))
                .ToList();

            Dictionary<string, string> modifiedEntity = [];
            Dictionary<string, MetaData> metadata = [];

            foreach (Type entity in allEntities)
            {
                PropertyInfo[] properties = entity.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    string? value = data[property.Name.ToLower()]?.GetValue<string>();
                    if (string.IsNullOrWhiteSpace(value)) continue;

                    List<string> transformations = [];
                    int? charactersCount = null;

                    if (Attribute.IsDefined(property, typeof(TextTransformAttribute)))
                    {
                        TextTransformAttribute attribute = (TextTransformAttribute)Attribute
                            .GetCustomAttribute(property, typeof(TextTransformAttribute))!;
                        value = Transform(value, attribute.TransformType);
                        transformations.Add($"{attribute.TransformType}".ToLower());
                    }

                    if (Attribute.IsDefined(property, typeof(CharacterCountAttribute)))
                    {
                        charactersCount = value.Length;
                    }

                    modifiedEntity.Add(property.Name, value);
                    if (transformations.Count > 0 || charactersCount is not null)
                    {
                        metadata.Add(property.Name, new MetaData
                        {
                            Transformations = transformations,
                            CharacterCount = charactersCount
                        });
                    }
                }
            }

            return new TransformResponse
            {
                ModifiedEntity = modifiedEntity,
                Metadata = metadata
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Errpr -> ProcessRequest: {Message}; {StackTrace}", ex.Message, ex.StackTrace);
            throw;
        }
    }

    private static string Transform(string text, TransformTypes transformType)
    {
        return transformType switch
        {
            TransformTypes.Uppercase => text.ToUpper(),
            TransformTypes.Lowercase => text.ToLower(),
            TransformTypes.DashReplacement => text.Replace(" ", "-"),
            _ => throw new Exception($"Unknown transform type '{transformType}'"),
        };
    }
}
