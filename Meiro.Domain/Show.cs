using MongoDB.Bson.Serialization.Attributes;

namespace Meiro.Domain;

public record Show
{
    [BsonId]
    public required int Id { get; init; }
    
    public required string Name { get; init; }
    
    public required Cast[] Cast { get; init; }
}