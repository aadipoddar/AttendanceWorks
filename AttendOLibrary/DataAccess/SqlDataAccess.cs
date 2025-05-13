using System.Data;

using Dapper;

using Microsoft.Data.SqlClient;

namespace AttendOLibrary.DataAccess;

public static class SqlDataAccess
{
	public static async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters)
	{
		using IDbConnection connection = new SqlConnection(ConnectionStrings.Azure);

		List<T> rows = [.. await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure)];

		return rows;
	}

	public static async Task SaveData<T>(string storedProcedure, T parameters)
	{
		using IDbConnection connection = new SqlConnection(ConnectionStrings.Azure);

		await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
	}
}

public class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
	public override void SetValue(IDbDataParameter parameter, DateOnly date)
		=> parameter.Value = date.ToDateTime(new TimeOnly(0, 0));
	public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);
}

public class SqlTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
	public override void SetValue(IDbDataParameter parameter, TimeOnly time)
	{
		parameter.Value = time.ToString();
	}

	public override TimeOnly Parse(object value) => TimeOnly.FromTimeSpan((TimeSpan)value);
}
