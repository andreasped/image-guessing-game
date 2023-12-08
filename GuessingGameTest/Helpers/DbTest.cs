using System.Data.Common;
using GuessingGame.SharedKernel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit.Abstractions;

namespace GuessingGameTest.Helpers;
public class DbTest
{
	private readonly DbContextOptions<GameContext> _contextOptions;
	private readonly DbConnection _connection;
	private readonly ITestOutputHelper _output;

	public DbTest(ITestOutputHelper output)
	{
		_output = output;

		_contextOptions = new DbContextOptionsBuilder<GameContext>()
				.UseSqlite(CreateInMemoryDatabase())
				.LogTo(_output.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, Microsoft.Extensions.Logging.LogLevel.Information)
				.EnableSensitiveDataLogging()
				.Options;
		_connection = RelationalOptionsExtension.Extract(_contextOptions).Connection;
	}

	public DbContextOptions<GameContext> ContextOptions => _contextOptions;

	private static DbConnection CreateInMemoryDatabase()
	{
		var connection = new SqliteConnection("Filename=:memory:");

		connection.Open();

		return connection;
	}

	public void Dispose() => _connection.Dispose();
}
