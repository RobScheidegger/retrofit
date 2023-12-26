using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Retrofit.Data;

public enum RetrofitType
{
    Image,
    Video,
    Unknown
}

public class RetrofitEntry
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public RetrofitType Type { get; set; }
    public required string Extension { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
    [Column(TypeName = "vector(1024)")]
    public Vector? Embedding { get; set; }
    public string? Caption { get; set; }
}