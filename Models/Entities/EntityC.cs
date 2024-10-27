using Models.Attributes;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

public class EntityC : ITransformable
{

    [TextTransform(TransformTypes.Uppercase)]
    [CharacterCount]
    public string Occupation { get; private set; } = string.Empty;
    [TextTransform(TransformTypes.DashReplacement)]
    [CharacterCount]
    public string Subject { get; private set; } = string.Empty;
    [TextTransform(TransformTypes.Lowercase)]
    public string FirstName { get; private set; } = string.Empty;
}
