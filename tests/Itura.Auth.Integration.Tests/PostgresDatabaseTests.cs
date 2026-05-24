using FluentAssertions;
using Itura.Auth.Integration.Tests.Infrastructure;
using Npgsql;

namespace Itura.Auth.Integration.Tests;

[Collection("PostgresCollection")]
public sealed class PostgresDatabaseTests(PostgresFixture db) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task PostgresContainer_StartsSuccessfully_ConnectionStringIsValid()
    {
        db.ConnectionString.Should().NotBeNullOrEmpty();
        db.ConnectionString.Should().Contain("itura_test");
    }

    [Fact]
    public async Task PostgresContainer_CanCreateTableAndInsertRow()
    {
        await using var conn = new NpgsqlConnection(db.ConnectionString);
        await conn.OpenAsync();

        await using var createCmd = conn.CreateCommand();
        createCmd.CommandText = """
            CREATE TABLE IF NOT EXISTS test_table (
                id SERIAL PRIMARY KEY,
                name TEXT NOT NULL
            );
            """;
        await createCmd.ExecuteNonQueryAsync();

        await using var insertCmd = conn.CreateCommand();
        insertCmd.CommandText = "INSERT INTO test_table (name) VALUES ('test') RETURNING id";
        var id = await insertCmd.ExecuteScalarAsync();

        ((int)id!).Should().BeGreaterThan(0);
    }
}
