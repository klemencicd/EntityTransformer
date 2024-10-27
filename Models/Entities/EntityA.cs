using Models.Attributes;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

public class EntityA : ITransformable
{

    [TextTransform(TransformTypes.Lowercase)]
    [CharacterCount]
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
}
