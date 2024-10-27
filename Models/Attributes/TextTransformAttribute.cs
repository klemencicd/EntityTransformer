using Models.Enums;

namespace Models.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class TextTransformAttribute(TransformTypes transformType) : Attribute
{
    public TransformTypes TransformType { get; set; } = transformType;
}
