using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ECommerce.Catalog;
using Npgsql;
using NpgsqlTypes;

namespace ECommerce.Migrations;

internal static class AttributeOptionMigrationBackfill
{
    internal static void Run()
    {
        var connectionString = ResolveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Could not resolve ConnectionStrings:Default for attribute option backfill. " +
                "Run database migrations from ECommerce.DbMigrator or set ConnectionStrings__Default.");
        }

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        var definitions = new List<(Guid Id, Guid? TenantId, string Json)>();
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = """
                SELECT "Id", "TenantId", "AllowedValuesJson"
                FROM "AppAttributeDefinitions"
                WHERE "DataType" = 4 AND "AllowedValuesJson" IS NOT NULL AND trim("AllowedValuesJson") <> ''
                """;
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                definitions.Add((
                    reader.GetGuid(0),
                    reader.IsDBNull(1) ? null : reader.GetGuid(1),
                    reader.GetString(2)));
            }
        }

        foreach (var (definitionId, tenantId, json) in definitions)
        {
            var ordered = AttributeAllowedValuesParser.ParseOrdered(json);
            for (var i = 0; i < ordered.Count; i++)
            {
                var value = ordered[i];
                var optionId = AttributeOptionIdFactory.Create(definitionId, value);
                InsertOption(connection, optionId, tenantId, definitionId, value, i);
            }
        }

        using (var del = connection.CreateCommand())
        {
            del.CommandText = """
                DELETE FROM "AppAttributeOptionTranslations" t
                WHERE NOT EXISTS (
                    SELECT 1 FROM "AppAttributeOptions" o WHERE o."Id" = t."AttributeOptionId")
                """;
            del.ExecuteNonQuery();
        }
    }

    private static void InsertOption(
        NpgsqlConnection connection,
        Guid id,
        Guid? tenantId,
        Guid attributeDefinitionId,
        string value,
        int displayOrder)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO "AppAttributeOptions" (
                "Id", "TenantId", "AttributeDefinitionId", "Value", "DisplayOrder", "IsActive",
                "ExtraProperties", "ConcurrencyStamp", "CreationTime")
            VALUES (
                @id, @tenantId, @defId, @value, @displayOrder, true, '{}', @stamp, @created)
            ON CONFLICT ("Id") DO NOTHING
            """;
        cmd.Parameters.AddWithValue("id", id);
        var tenantParam = new NpgsqlParameter("tenantId", NpgsqlDbType.Uuid)
        {
            Value = tenantId.HasValue ? tenantId.Value : DBNull.Value
        };
        cmd.Parameters.Add(tenantParam);
        cmd.Parameters.AddWithValue("defId", attributeDefinitionId);
        cmd.Parameters.AddWithValue("value", value);
        cmd.Parameters.AddWithValue("displayOrder", displayOrder);
        // ConcurrencyStamp max length 40; "N" format GUID is only 32 characters.
        cmd.Parameters.AddWithValue(
            "stamp",
            (Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"))[..40]);
        cmd.Parameters.AddWithValue("created", DateTime.UtcNow);
        cmd.ExecuteNonQuery();
    }

    private static string? ResolveConnectionString()
    {
        var fromEnv = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            return fromEnv;
        }

        var candidates = new[]
        {
            AppContext.BaseDirectory,
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ECommerce.DbMigrator")),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "ECommerce.DbMigrator")),
        };

        foreach (var dir in candidates)
        {
            var path = Path.Combine(dir, "appsettings.json");
            if (!File.Exists(path))
            {
                continue;
            }

            try
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                if (doc.RootElement.TryGetProperty("ConnectionStrings", out var cs) &&
                    cs.TryGetProperty("Default", out var def))
                {
                    var s = def.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        return s;
                    }
                }
            }
            catch (JsonException)
            {
                // try next path
            }
        }

        return null;
    }
}
