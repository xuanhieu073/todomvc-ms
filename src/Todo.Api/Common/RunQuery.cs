using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Entities;

namespace Common;

public class QueryRequest
{
    public string? Filter { get; set; }       // raw Mongo filter as JSON, e.g. {"Category":"Shoes","Price":{"$gt":50}}
    public string? SortField { get; set; }
    public bool SortDescending { get; set; }
    public int? Skip { get; set; }
    public int? Limit { get; set; }

    public static async Task<object> RunQuery<T>(QueryRequest req) where T : IEntity
    {
        var filter = string.IsNullOrWhiteSpace(req.Filter)
            ? "{}"
            : req.Filter;

        var bsonFilter = BsonDocument.Parse(filter);

        var query = DB.Find<T>()
            .Match(bsonFilter);

        if (!string.IsNullOrWhiteSpace(req.SortField))
        {
            query = req.SortDescending
                ? query.Sort(x => Builders<T>.Sort.Descending(req.SortField))
                : query.Sort(x => Builders<T>.Sort.Ascending(req.SortField));
        }

        if (req.Skip is > 0) query = query.Skip(req.Skip.Value);
        if (req.Limit is > 0) query = query.Limit(req.Limit.Value);

        return await query.ExecuteAsync();
    }
}
