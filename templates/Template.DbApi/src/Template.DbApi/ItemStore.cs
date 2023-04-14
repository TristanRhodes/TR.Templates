using Dapper;
using Template.DbApi.Model;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.DbApi;

public interface IItemStore
{
    Task<IEnumerable<Item>> GetItems();

    Task<Item> AddItem(Item itemDto);

    Task<Item?> GetItem(Guid itemId);
}

public class ItemStore : IItemStore
{
    private readonly ILogger _logger;
    private readonly IConnectionFactory _connectionFactory;

    public ItemStore(ILogger logger, IConnectionFactory connectionFactory)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Item>> GetItems()
    {
        using var conn = _connectionFactory.CreateConnection();

        var results = (await conn
            .QueryAsync("SELECT * FROM items"))
            .Select(e => new Item
            {
                ItemId = e.item_id,
                Name = e.name,
                Description = e.description,
            });

        return results;
    }

    public async Task<Item?> GetItem(Guid itemId)
    {
        using var conn = _connectionFactory.CreateConnection();

        var results = (await conn
            .QueryAsync("SELECT * FROM items WHERE item_id = @ItemId", new { ItemId = itemId }))
            .Select(e => new Item
            {
                ItemId = e.item_id,
                Name = e.name,
                Description = e.description,
            });

        return results.SingleOrDefault();
    }

    public async Task<Item> AddItem(Item item)
    {
        using var conn = _connectionFactory.CreateConnection();

        var recordId = await conn.ExecuteScalarAsync<Guid>("INSERT INTO items (name, description) VALUES (@Name, @Description) RETURNING item_id;", item);

        item.ItemId = recordId;

        return item;
    }
}
