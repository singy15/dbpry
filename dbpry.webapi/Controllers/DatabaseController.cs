using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace dbpry.webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseController : ControllerBase
{
  private readonly ILogger<DatabaseController> _logger;

  public DatabaseController(ILogger<DatabaseController> logger)
  {
    _logger = logger;
  }

  private IDbTransaction CreateTransaction(IDbConnection conn)
  {
    return conn.BeginTransaction();
  }

  private DataTable Query(IDbTransaction tx, string sql)
  {
    using (var cmd = tx.Connection.CreateCommand())
    {
      cmd.CommandText = sql;
      var reader = cmd.ExecuteReader();
      var dt = new DataTable();
      dt.Load(reader);
      return dt;
    }
  }

  [HttpGet(Name = "GetQueryResult")]
  public string Get(string query)
  {
    string result = null;
    using(IDbConnection conn = new OracleConnection("User ID=XXX; Password=XXX; Data Source=XXXX/XXXX")) {
      conn.Open();
      using(var tx = CreateTransaction(conn)) {
        var dt = Query(tx, query);
        result = JsonConvert.SerializeObject(dt);
      }
      conn.Close();
    }

    return result;
  }
}
