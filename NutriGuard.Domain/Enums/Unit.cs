using System.Runtime.Serialization;

public enum Unit
{
    [EnumMember(Value = "g")]
    Gram,

    [EnumMember(Value = "ml")]
    Milliliter,

    [EnumMember(Value = "tbsp")]
    Tablespoon,

    [EnumMember(Value = "tsp")]
    Teaspoon,

    [EnumMember(Value = "piece")]
    Piece,

    [EnumMember(Value = "cup")]
    Cup
}