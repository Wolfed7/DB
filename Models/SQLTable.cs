namespace WebApplication2.Models;
using System.Data;

public class SQLTable
{
    public DataTable DataTable { get; set; }

    public SQLTable()
    {
        DataTable = new DataTable();
    }
}