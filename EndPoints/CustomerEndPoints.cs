using Dapper;
using Microsoft.Data.SqlClient;
using Web.APi.Dapper.Models;
using Web.APi.Dapper.Services;

namespace Web.APi.Dapper.EndPoints
{
    public static class   CustomerEndPoints
    {
        public static void CustomerEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("orders", async (SqlConnectionFactory sqlConnectionFactory) =>
            {

                using var connection = sqlConnectionFactory.Create();
                const string sql = "SELECT * FROM [dbo].[Order]";
                var orders = await connection.QueryAsync<Order>(sql);
                return Results.Ok(orders);
            });
            builder.MapGet("orders/{id}", async (int id , SqlConnectionFactory sqlConnectionFactory) => {
                using var connection = sqlConnectionFactory.Create();
                const string sql = "SELECT * FROM [dbo].[Order] where id = @OrderId";
                var order = await connection.QuerySingleOrDefaultAsync<Order>(sql,
                    new {OrderID=id});
                return Results.Ok(order);

            });
           
            builder.MapPost("orders", async (Order order, SqlConnectionFactory sqlConnectionFactory) =>
            {
                using var connection = sqlConnectionFactory.Create();
                const string sql = @"
        INSERT INTO [dbo].[Order] (CustomerId, OrderDate, TotalPrice, CreationDate, ModificationDate)
        VALUES (@CustomerId, @OrderDate, @TotalPrice, @CreationDate, @ModificationDate);
        SELECT CAST(SCOPE_IDENTITY() as int);"; // This will return the ID of the inserted order

                var orderId = await connection.ExecuteScalarAsync<int>(sql, order);
                return Results.Created($"/orders/{orderId}", order);
            });
            builder.MapPut("orders/{id}", async (int id, Order order, SqlConnectionFactory sqlConnectionFactory) =>
            {
                using var connection = sqlConnectionFactory.Create();
                const string sql = @"
        UPDATE [dbo].[Order]
        SET CustomerId = @CustomerId, OrderDate = @OrderDate, TotalPrice = @TotalPrice, 
            CreationDate = @CreationDate, ModificationDate = @ModificationDate
        WHERE Id = @Id";

                var affectedRows = await connection.ExecuteAsync(sql, new
                {
                    order.CustomerId,
                    order.OrderDate,
                    order.TotalPrice,
                    order.CreationDate,
                    order.ModificationDate,
                    Id = id
                });

                if (affectedRows > 0)
                    return Results.Ok(order);
                else
                    return Results.NotFound();
            });

            builder.MapDelete("orders/{id}", async (int id, SqlConnectionFactory sqlConnectionFactory) =>
            {
                using var connection = sqlConnectionFactory.Create();
                const string sql = "DELETE FROM [dbo].[Order] WHERE Id = @Id";

                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });

                if (affectedRows > 0)
                    return Results.Ok($"Order {id} deleted.");
                else
                    return Results.NotFound();
            });



        }
    }
}
