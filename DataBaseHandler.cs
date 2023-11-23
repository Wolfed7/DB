using System.Data;
using Microsoft.VisualBasic.CompilerServices;
using NpgsqlTypes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Numerics;

namespace WebApplication2;

public class DataBaseHandler
{
	private readonly string connectionString = "Server=students.ami.nstu.ru;Database=students;User ID=pmi-b0202;Password=viakKec0;SearchPath=pmib0202";

	private NpgsqlConnection _connection;
	private NpgsqlCommand _command;
	private NpgsqlDataReader _reader;
	private DataTable _dataTable;

	public string? ErrorMessage;

	public DataBaseHandler()
	{
		_connection = new NpgsqlConnection(connectionString);
		_dataTable = new DataTable();
		ErrorMessage = null;
	}

	public List<SelectListItem> GetIzds()
	{
		ErrorMessage = null;

		List<SelectListItem> items = new List<SelectListItem>();
		string cmd = @"SELECT n_izd
                       FROM j";

		_connection.Open();
		using (_command = new NpgsqlCommand(cmd, _connection))
		{
			NpgsqlTransaction? transaction = null;

			try
			{
				transaction = _connection.BeginTransaction();
				_reader = _command.ExecuteReader();

				while (_reader.Read())
				{
					items.Add(new SelectListItem(_reader[0].ToString(), _reader[0].ToString()));
				}

				_reader.Close();

				transaction.Commit();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				transaction?.Rollback();
				ErrorMessage = $"Cannot complete transaction: {e.Message}";
				//throw new NpgsqlException("Cannot complete transaction.");
			}
		}
		_connection.Close();

		return items;
	}

	public List<SelectListItem> GetClients()
	{
		ErrorMessage = null;
		List<SelectListItem> items = new List<SelectListItem>();
		string cmd = @"SELECT n_cl
                       FROM c";

		_connection.Open();
		using (_command = new NpgsqlCommand(cmd, _connection))
		{
			NpgsqlTransaction? transaction = null;

			try
			{
				transaction = _connection.BeginTransaction();
				_reader = _command.ExecuteReader();

				while (_reader.Read())
				{
					items.Add(new SelectListItem(_reader[0].ToString(), _reader[0].ToString()));
				}

				_reader.Close();

				transaction.Commit();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				transaction?.Rollback();
				ErrorMessage = $"Cannot complete transaction: {e.Message}";
				//throw new NpgsqlException("Cannot complete transaction.");
			}
		}
		_connection.Close();

		return items;
	}

	public DataTable Query1(string n_izd)
	{
		ErrorMessage = null;
		NpgsqlParameter par_n_izd = new NpgsqlParameter("@vn_izd", NpgsqlDbType.Varchar);
		par_n_izd.Value = n_izd;

		string cmd = @"SELECT lastpost.n_det, spj1.cost 
                       FROM ( SELECT n_det, MAX(date_post)
                              FROM spj1
                              WHERE n_izd = @vn_izd
                              GROUP BY n_det
                            ) AS lastpost
                       JOIN spj1 ON spj1.n_det = lastpost.n_det 
                                 AND spj1.date_post = lastpost.max";

		_connection.Open();
		using (_command = new NpgsqlCommand(cmd, _connection))
		{
			_command.Parameters.Add(par_n_izd);

			NpgsqlTransaction transaction = null;

			try
			{
				transaction = _connection.BeginTransaction();
				_reader = _command.ExecuteReader();
				_dataTable.Load(_reader);

				_reader.Close();

				transaction.Commit();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				transaction?.Rollback();
				ErrorMessage = $"Cannot complete transaction: {e.Message}";
				//throw new NpgsqlException($"Cannot complete transaction: {e.Message}");
			}
		}
		_connection.Close();

		return _dataTable;
	}



	public int? QueryUpdate(string n_izd, string n_cl, string date_order, string date_pay, string date_ship, string kol, string cost)
	{
		int i = 0;

		//NpgsqlParameter par_n_real = new NpgsqlParameter("@vn_real", NpgsqlDbType.Varchar);
		//par_n_real.Value = n_real;

		NpgsqlParameter par_n_izd = new NpgsqlParameter("@vn_izd", NpgsqlDbType.Char);
		par_n_izd.Value = n_izd;

		NpgsqlParameter par_n_cl = new NpgsqlParameter("@vn_cl", NpgsqlDbType.Char);
		par_n_cl.Value = n_cl;

		NpgsqlParameter par_date_order = new NpgsqlParameter("@vdate_order", NpgsqlDbType.Date);
		par_date_order.Value = date_order is null ? DBNull.Value
		 : DateOnly.Parse(date_order);

		NpgsqlParameter par_date_pay = new NpgsqlParameter("@vdate_pay", NpgsqlDbType.Date);
		par_date_pay.Value = date_pay is null ? DBNull.Value
		   : DateOnly.Parse(date_pay);

		NpgsqlParameter par_date_ship = new NpgsqlParameter("@vdate_ship", NpgsqlDbType.Date);
		par_date_ship.Value = date_ship is null ? DBNull.Value
		   : DateOnly.Parse(date_ship);

		NpgsqlParameter par_kol = new NpgsqlParameter("@vkol", NpgsqlDbType.Integer);
		par_kol.Value = kol is null ? DBNull.Value : Convert.ToInt32(kol);

		NpgsqlParameter par_cost = new NpgsqlParameter("@vcost", NpgsqlDbType.Integer);
		par_cost.Value = cost is null ? DBNull.Value : Convert.ToInt32(cost);


      //string cmd = @"insert into r values
      //                 (@vn_real, @vn_izd, @vn_cl, @vdate_order, @vdate_pay, @vdate_ship, @vkol, @vcost)";

      string cmd = @"SELECT ins_r(@vn_izd, @vn_cl, @vdate_order, @vdate_pay, @vdate_ship, @vkol, @vcost)";

      _connection.Open();
		using (_command = new NpgsqlCommand(cmd, _connection))
		{
			//_command.Parameters.Add(par_n_real);
			_command.Parameters.Add(par_n_izd);
			_command.Parameters.Add(par_n_cl);
			_command.Parameters.Add(par_date_order);
			_command.Parameters.Add(par_date_pay);
			_command.Parameters.Add(par_date_ship);
			_command.Parameters.Add(par_kol);
			_command.Parameters.Add(par_cost);

			NpgsqlTransaction transaction = null;

			try
			{
				transaction = _connection.BeginTransaction();

				i = _command.ExecuteNonQuery();

				transaction.Commit();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				transaction?.Rollback();
				ErrorMessage = $"Cannot complete transaction: {e.Message}";
				//throw new NpgsqlException($"Cannot complete transaction: {e.Message}");
			}
		}
		_connection.Close();

		return i;
	}

	public void Reset()
	{
		string command = @"truncate r;
                            insert into R (N_REAL, N_IZD, N_CL, DATE_ORDER, DATE_PAY, DATE_SHIP, KOL, COST) values 
							('R1','J1','C1',to_date('10-02-2011','dd-mm-yyyy'),to_date('13-02-2011','ddmm-yyyy'),to_date('03-03-2011','dd-mm-yyyy'),15,275),
							('R2','J6','С2',to_date('01-03-2011','dd-mm-yyyy'),to_date('06-03-2011','dd-mmyyyy'),to_date('13-03-2011','dd-mm-yyyy'),30,300),
							('R3','J1','C3',to_date('05-03-2011','dd-mm-yyyy'),to_date('07-03-2011','dd-mmyyyy'),to_date('10-03-2011','dd-mm-yyyy'),30,280),
							('R4','J3','C2',to_date('03-04-2011','dd-mm-yyyy'),to_date('07-04-2011','dd-mmyyyy'),to_date('09-04-2011','dd-mm-yyyy'),25,300),
							('R5','J2','C5',to_date('22-04-2011','dd-mm-yyyy'),to_date('24-04-2011','dd-mmyyyy'),to_date('26-04-2011','dd-mm-yyyy'),15,400),
							('R6','J7','C5',to_date('08-05-2011','dd-mm-yyyy'),to_date('11-05-2011','dd-mmyyyy'),to_date('13-05-2011','dd-mm-yyyy'),70,325),
							('R7','J5','С2',to_date('15-05-2011','dd-mm-yyyy'),to_date('19-05-2011','dd-mmyyyy'),to_date('22-05-2011','dd-mm-yyyy'),40,290),
							('R8','J3','C4',to_date('20-05-2011','dd-mm-yyyy'),to_date('23-05-2011','dd-mmyyyy'),to_date('24-05-2011','dd-mm-yyyy'),15,290),
							('R9','J1','C6',to_date('09-06-2011','dd-mm-yyyy'),to_date('15-06-2011','dd-mmyyyy'),to_date('20-06-2011','dd-mm-yyyy'),45,285),
							('R10','J6','С2',to_date('15-06-2011','dd-mm-yyyy'),to_date('16-06-2011','dd-mmyyyy'),to_date('16-06-2011','dd-mm-yyyy'),20,300),
							('R11','J4','C3',to_date('19-06-2011','dd-mm-yyyy'),to_date('27-06-2011','dd-mmyyyy'),to_date('27-06-2011','dd-mm-yyyy'),30,445), 
							('R12','J1','C5',to_date('01-07-2011','dd-mm-yyyy'),to_date('03-07-2011','dd-mmyyyy'),to_date('05-07-2011','dd-mm-yyyy'),50,277),
							('R13','J5','С1',to_date('01-09-2011','dd-mm-yyyy'),to_date('04-09-2011','dd-mmyyyy'),to_date('04-09-2011','dd-mm-yyyy'),100,285),
							('R14','J4','C4',to_date('20-09-2011','dd-mm-yyyy'),to_date('21-09-2011','dd-mmyyyy'),to_date('21-09-2011','dd-mm-yyyy'),120,420),
							('R15','J3','C2',to_date('20-10-2011','dd-mm-yyyy'),to_date('23-10-2011','dd-mmyyyy'),to_date('24-10-2011','dd-mm-yyyy'),25,295),
							('R16','J7','C6',to_date('01-11-2011','dd-mm-yyyy'),to_date('03-11-2011','dd-mmyyyy'),to_date('06-11-2011','dd-mm-yyyy'),80,415),
							('R17','J1','C1',to_date('11-01-2012','dd-mm-yyyy'),to_date('13-01-2012','dd-mmyyyy'),to_date('15-01-2012','dd-mm-yyyy'),10,280),
							('R18','J6','С2',to_date('15-04-2012','dd-mm-yyyy'),to_date('22-04-2012','dd-mmyyyy'),to_date('23-04-2012','dd-mm-yyyy'),60,350),
							('R19','J4','C6',to_date('13-05-2012','dd-mm-yyyy'),to_date('15-05-2012','dd-mmyyyy'),to_date('20-05-2012','dd-mm-yyyy'),50,500),
							('R20','J2','C6',to_date('01-06-2012','dd-mm-yyyy'),to_date('05-06-2012','dd-mmyyyy'),null,30,490),
							('R21','J7','C2',to_date('20-06-2012','dd-mm-yyyy'),to_date('21-06-2012','dd-mmyyyy'),to_date('25-06-2012','dd-mm-yyyy'),150,415),
							('R22','J3','C4',to_date('10-08-2012','dd-mm-yyyy'),to_date('12-08-2012','dd-mmyyyy'),to_date('13-08-2012','dd-mm-yyyy'),60,330),
							('R23','J5','C3',to_date('15-08-2012','dd-mm-yyyy'),to_date('19-08-2012','dd-mmyyyy'),to_date('21-08-2012','dd-mm-yyyy'),120,350),
							('R24','J4','C3',to_date('20-08-2012','dd-mm-yyyy'),null,null,70,520),
							('R25','J3','C2',to_date('20-08-2012','dd-mm-yyyy'),to_date('22-08-2012','dd-mmyyyy'),to_date('26-08-2012','dd-mm-yyyy'),45,340),
							('R26','J1','C3',to_date('25-08-2012','dd-mm-yyyy'),to_date('27-08-2012','dd-mmyyyy'),null,50,290),
							('R27','J6','С2',to_date('15-09-2012','dd-mm-yyyy'),to_date('17-09-2012','dd-mmyyyy'),null,70,350);

                     DROP SEQUENCE rea;
                     CREATE SEQUENCE rea 
                     START 1
                     MAXVALUE 99999;
                     SELECT setval('rea', 27)";

		_connection.Open();
		using (_command = new NpgsqlCommand(command, _connection))
		{
			NpgsqlTransaction? transaction = null;

			try
			{
				transaction = _connection.BeginTransaction();

				int i = _command.ExecuteNonQuery();

				transaction.Commit();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				transaction?.Rollback();
				ErrorMessage = $"Cannot complete transaction: {e.Message}";
				//throw new NpgsqlException($"Cannot complete transaction: {e.Message}");
			}
		}
		_connection.Close();
	}
}