using Microsoft.Data.Sqlite;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Helpers;
namespace Proyecto_TallerII.Repositories;

public class TableroRepository : ITableroRepository
{
    private readonly ILogger<TableroRepository> _logger;
    private readonly string connectionString;

    public TableroRepository(string CadenaDeConexion)
    {
        connectionString = CadenaDeConexion;
    }

    public void CrearTablero(Tablero tablero)
    {
        string query = @"INSERT INTO Tablero (id_usuario_propietario, nombre, descripcion) 
                    VALUES (@idPropietarioT, @nombreT, @descripcionT);";
        
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idPropietarioT", tablero.IdUsuarioPropietario));
                command.Parameters.Add(new SqliteParameter("@nombreT", tablero.NombreTablero));
                command.Parameters.Add(new SqliteParameter("@descripcionT", tablero.DescripcionTablero));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al CrearTablero."));
                throw new Exception("Hubo un problema con la consulta a la BD al CrearTablero.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public void EditarTablero(Tablero tablero)
    {
        string query = @"UPDATE Tablero SET id_usuario_propietario = @idPropietarioT, nombre = @nombreT, descripcion = @descripcionT 
                    WHERE id = @idTablero;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idPropietarioT", tablero.IdUsuarioPropietario));
                command.Parameters.Add(new SqliteParameter("@nombreT", tablero.NombreTablero));
                command.Parameters.Add(new SqliteParameter("@descripcionT", tablero.DescripcionTablero));
                command.Parameters.Add(new SqliteParameter("@idTablero", tablero.IdTablero));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ModificarTablero."));
                throw new Exception("Hubo un problema con la consulta a la BD al ModificarTablero.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public void EliminarTablero(int idTablero)
    {
        string queryEliminarTareas = "DELETE FROM Tarea WHERE id_tablero = @idTablero;";
        string queryEliminarTablero = "DELETE FROM Tablero WHERE id = @idTablero;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(queryEliminarTareas, connection))
                {
                    command.Parameters.AddWithValue("@idTablero", idTablero);
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = new SqliteCommand(queryEliminarTablero, connection))
                {
                    command.Parameters.AddWithValue("@idTablero", idTablero);
                    command.ExecuteNonQuery();
                }   
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al EliminarTablero."));
                throw new Exception("Hubo un problema con la consulta a la BD al EliminarTablero.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public Tablero ObtenerTableroPorId(int idTablero)
    {
        var query = @"SELECT * FROM Tablero 
                WHERE id = @idTablero;";

        Tablero tablero = null;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();

                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idTablero", idTablero));

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tablero = new Tablero();
                        tablero.IdTablero = Convert.ToInt32(reader["id"]);
                        tablero.IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]);
                        tablero.NombreTablero = reader["nombre"].ToString()!;
                        tablero.DescripcionTablero = reader["descripcion"].ToString()!;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ObtenerTableroPorId."));
                throw new Exception("Hubo un problema con la consulta a la BD al ObtenerTableroPorId.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        if (tablero == null)
        {
            throw new Exception($"Tablero con id: {idTablero} NO existe.");
        }
        return tablero;
    }
    public List<Tablero> ListaTablerosAjenos(int idUsuarioMio)
    {
        var query = @"
        SELECT DISTINCT Tablero.id, Tablero.id_usuario_propietario, Tablero.nombre, Tablero.descripcion
        FROM Tablero 
        INNER JOIN Tarea ON Tarea.id_tablero = Tablero.id 
        WHERE (Tablero.id_usuario_propietario IS NULL AND Tarea.id_usuario_asignado = @idUsuarioMio) 
        OR (Tablero.id_usuario_propietario != @idUsuarioMio AND Tarea.id_usuario_asignado = @idUsuarioMio);
        ";

        List<Tablero> tablerosAjenos = null;
        
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idUsuarioMio", idUsuarioMio));

                using (var reader = command.ExecuteReader())
                {
                    tablerosAjenos = new List<Tablero>();
                    while (reader.Read())
                    {
                        var tablero = new Tablero
                        {
                            IdTablero = Convert.ToInt32(reader["id"]),
                            IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]),
                            NombreTablero = reader["nombre"]?.ToString()!,
                            DescripcionTablero = reader["descripcion"]?.ToString()!
                        };

                        tablerosAjenos.Add(tablero);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ListarTablerosAjenos."));
                throw new Exception("Hubo un problema con la consulta a la BD al ListarTablerosAjenos.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        if(tablerosAjenos == null)
        {
            throw new Exception($"Lista de tableros vacia.");
        }

        return tablerosAjenos;
    }
    public List<Tablero> ListaTableros(int idUsuarioMio)
    {
        string query;
        if (idUsuarioMio > 0)
        {
            query = "SELECT * FROM Tablero WHERE Tablero.id_usuario_propietario = @idPropietarioT";
        }
        else
        {
            query = "SELECT * FROM Tablero";
        }

        List<Tablero> misTableros = null;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                if (idUsuarioMio > 0)
                {
                    command.Parameters.Add(new SqliteParameter("@idPropietarioT", idUsuarioMio));
                }

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    misTableros = new List<Tablero>();
                    while (reader.Read())
                    {
                        var tablero = new Tablero
                        {
                            IdTablero = Convert.ToInt32(reader["id"]),
                            IdUsuarioPropietario = Convert.ToInt32(reader["id_usuario_propietario"]),
                            NombreTablero = reader["nombre"].ToString()!,
                            DescripcionTablero = reader["descripcion"].ToString()!
                        };

                        misTableros.Add(tablero);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ListaTableros."));
                throw new Exception("Hubo un problema con la consulta a la BD al ListaTableros.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        if (misTableros == null)
        {
            throw new Exception($"Lista de tableros vacía.");
        }

        return misTableros;
    }
    public bool ExisteTablero(string nombreDeTablero, int idTablero)
    {
        var query = "SELECT Tablero.nombre FROM Tablero WHERE nombre = @nombreT AND id != @idT;";

        bool existe = false;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@nombreT", nombreDeTablero));
                command.Parameters.Add(new SqliteParameter("@idT", idTablero));
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        existe = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ExisteTablero."));
                throw new Exception("Hubo un problema con la consulta a la BD al ExisteTablero.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        return existe;
    }
}