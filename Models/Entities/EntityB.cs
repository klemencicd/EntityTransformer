using Models.Attributes;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

public class EntityB : ITransformable
{

    [TextTransform(TransformTypes.Uppercase)]
    public string Title { get; private set; } = string.Empty;
    [TextTransform(TransformTypes.DashReplacement)]
    [CharacterCount]
    public string Content { get; private set; } = string.Empty;
}
