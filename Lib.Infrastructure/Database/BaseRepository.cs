using Lib.Core.Enums;
using Microsoft.Data.Sqlite;

namespace Lib.Infrastructure.Database;

public abstract class BaseRepository
{
    protected readonly string _connectionString = "Data Source=game.db";
}