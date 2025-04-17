using Microsoft.Data.Sqlite;
using Proyecto_TallerII.Models;
using Proyecto_TallerII.Helpers;
namespace Proyecto_TallerII.Repositories;

public class TareaRepository : ITareaRepository
{
    private readonly ILogger<TareaRepository> _logger;
    private readonly string connectionString;

    public TareaRepository(string CadenaDeConexion)
    {
        connectionString = CadenaDeConexion;
    }
    public void CrearTarea(Tarea nuevaTarea)
    {
        string query;
        if(nuevaTarea.IdUsuarioAsignado == null)
        {
            query = @"INSERT INTO Tarea (id_tablero, nombre, estado, descripcion, color) 
                    VALUES (@idTablero, @nombreTarea, @estadoTarea, @descripcionTarea, @colorTarea);";
        } else {
            query = @"INSERT INTO Tarea (id_tablero, nombre, estado, descripcion, color, id_usuario_asignado) 
                    VALUES (@idTablero, @nombreTarea, @estadoTarea, @descripcionTarea, @colorTarea, @idUsuarioAsignado);";
        }

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idTablero", nuevaTarea.IdTablero));
                command.Parameters.Add(new SqliteParameter("@nombreTarea", nuevaTarea.NombreTarea));
                command.Parameters.Add(new SqliteParameter("@estadoTarea", (int)nuevaTarea.EstadoTarea));
                command.Parameters.Add(new SqliteParameter("@descripcionTarea", nuevaTarea.DescripcionTarea));
                command.Parameters.Add(new SqliteParameter("@colorTarea", nuevaTarea.ColorTarea));
                if(nuevaTarea.IdUsuarioAsignado != null)
                {
                    command.Parameters.Add(new SqliteParameter("@idUsuarioAsignado", nuevaTarea.IdUsuarioAsignado));
                }

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al CrearTarea."));
                throw new Exception("Hubo un problema con la consulta a la BD al CrearTarea.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public void EditarTarea(Tarea tareaModificada)
    {
        var query = @"UPDATE Tarea SET nombre = @nombreTarea, descripcion = @descripcionTarea WHERE id = @idTarea;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@nombreTarea", tareaModificada.NombreTarea));
                command.Parameters.Add(new SqliteParameter("@descripcionTarea", tareaModificada.DescripcionTarea));
                command.Parameters.Add(new SqliteParameter("@idTarea", tareaModificada.IdTarea));
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ModificarTarea."));
                throw new Exception("Hubo un problema con la consulta a la BD al ModificarTarea.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public void EliminarTarea(int idTarea)
    {
        var query = "DELETE FROM Tarea WHERE id = @idTarea;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idTarea", idTarea));
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al EliminarTarea."));
                throw new Exception("Hubo un problema con la consulta a la BD al EliminarTarea.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public List<Tarea> ListarTodasLasTareas(int idTablero)
    {
        var query = @"SELECT Tarea.id, Tarea.id_tablero, Tarea.nombre, Tarea.estado, Tarea.descripcion, Tarea.color, Tarea.id_usuario_asignado 
                    FROM Tarea INNER JOIN Tablero ON Tablero.id = Tarea.id_tablero 
                    WHERE Tarea.id_tablero = @idTablero;";

        List<Tarea> listaTareas = null;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idTablero", idTablero));
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    listaTareas = new List<Tarea>();
                    while (reader.Read())
                    {
                        var tarea = new Tarea
                        {
                            IdTarea = Convert.ToInt32(reader["id"]),
                            IdTablero = Convert.ToInt32(reader["id_tablero"]),
                            NombreTarea = reader["nombre"].ToString()!,
                            DescripcionTarea = reader["descripcion"].ToString()!,
                            ColorTarea = reader["color"].ToString()!,
                            EstadoTarea = (EstadoTarea)Enum.Parse(typeof(EstadoTarea), reader["estado"].ToString()!),
                            IdUsuarioAsignado = (reader["id_usuario_asignado"] == DBNull.Value) 
                                ? null 
                                : Convert.ToInt32(reader["id_usuario_asignado"])
                        };

                        listaTareas.Add(tarea);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ListarTodasLasTareas."));
                throw new Exception("Hubo un problema con la consulta a la BD al ListarTodasLasTareas.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        if (listaTareas == null)
        {
            throw new Exception("Lista de Tareas vacia.");
        }
        return listaTareas;
    }
    public List<Tarea> ListarTareasDeUsuario(int idUsuario, bool sa)
    {
        string query;
        if(sa) // para traer las tareas (Sin Asignar) de los tableros de un usuario en particular
        {
            query = @"SELECT Tarea.id, Tarea.id_tablero, Tarea.nombre, Tarea.estado, Tarea.descripcion, Tarea.color, Tarea.id_usuario_asignado 
                    FROM Tarea 
                    INNER JOIN Tablero ON Tarea.id_tablero = Tablero.id
                    INNER JOIN Usuario ON Tablero.id_usuario_propietario = Usuario.id
                    WHERE Tarea.id_usuario_asignado IS NULL AND Usuario.id = @id_usuario;";
        } else { // obtengo las tareas asignadas o sin asignar de un usuario en particular
            query = @"SELECT Tarea.id, Tarea.nombre, Tarea.descripcion, Tarea.estado, Tarea.color, Tarea.id_tablero, Tarea.id_usuario_asignado 
                    FROM Tarea 
                    INNER JOIN Tablero ON Tarea.id_tablero = Tablero.id
                    WHERE id_usuario_asignado = @id_usuario";
        }

        List<Tarea> listaTareas = null;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@id_usuario", idUsuario));
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    listaTareas = new List<Tarea>();
                    while (reader.Read())
                    {
                        var tarea = new Tarea
                        {
                            IdTarea = Convert.ToInt32(reader["id"]),
                            NombreTarea = reader["nombre"].ToString()!,
                            DescripcionTarea = reader["descripcion"].ToString()!,
                            EstadoTarea = (EstadoTarea)Enum.Parse(typeof(EstadoTarea), reader["estado"].ToString()!),
                            ColorTarea = reader["color"].ToString()!,
                            IdTablero = Convert.ToInt32(reader["id_tablero"]),
                            IdUsuarioAsignado = (reader["id_usuario_asignado"] == DBNull.Value) 
                                ? (int?)null 
                                : Convert.ToInt32(reader["id_usuario_asignado"])
                        };

                        listaTareas.Add(tarea);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ListarTareasDeUsuario."));
                throw new Exception("Hubo un problema con la consulta a la BD al ListarTareasDeUsuario.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        if (listaTareas == null)
        {
            throw new Exception("Lista de Tareas vacia");
        }
        return listaTareas;
    }
    public List<Tarea> ListarTareasSinAsignarAdmin()
    {
        var query = @"SELECT Tarea.id, Tarea.id_tablero, Tarea.nombre, Tarea.estado, Tarea.descripcion, Tarea.color, Tarea.id_usuario_asignado 
                    FROM Tarea INNER JOIN Tablero ON Tarea.id_tablero = Tablero.id
                    INNER JOIN Usuario ON Tablero.id_usuario_propietario = Usuario.id
                    WHERE Tarea.id_usuario_asignado IS NULL";

        List<Tarea> listaTareas = null;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    listaTareas = new List<Tarea>();
                    while (reader.Read())
                    {
                        var tarea = new Tarea
                        {
                            IdTarea = Convert.ToInt32(reader["id"]),
                            IdTablero = Convert.ToInt32(reader["id_tablero"]),
                            NombreTarea = reader["nombre"].ToString()!,
                            DescripcionTarea = reader["descripcion"].ToString()!,
                            ColorTarea = reader["color"].ToString()!,
                            EstadoTarea = (EstadoTarea)Enum.Parse(typeof(EstadoTarea), reader["estado"].ToString()!),
                            IdUsuarioAsignado = (reader["id_usuario_asignado"] == DBNull.Value) 
                                ? (int?)null 
                                : Convert.ToInt32(reader["id_usuario_asignado"])
                        };

                        listaTareas.Add(tarea);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ListarTareasSinAsignarAdmin."));
                throw new Exception("Hubo un problema con la consulta a la BD al ListarTareasSinAsignarAdmin.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        if (listaTareas == null)
        {
            throw new Exception("Lista de Tareas vacia.");
        }
        return listaTareas;
    }
    public Tarea ObtenerTareaPorId(int idTarea)
    {
        var query = "SELECT * FROM Tarea WHERE id = @idTarea;";

        Tarea tarea = null;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idTarea", idTarea));
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tarea = new Tarea();
                        tarea.IdTarea = Convert.ToInt32(reader["id"]);
                        tarea.IdTablero = Convert.ToInt32(reader["id_tablero"]);
                        tarea.NombreTarea = reader["nombre"].ToString()!;
                        tarea.DescripcionTarea = reader["descripcion"].ToString()!;
                        tarea.ColorTarea = reader["color"].ToString()!;
                        tarea.EstadoTarea = (EstadoTarea)Enum.Parse(typeof(EstadoTarea), reader["estado"].ToString()!);
                        tarea.IdUsuarioAsignado = (reader["id_usuario_asignado"] == DBNull.Value) 
                            ? (int?)null 
                            : Convert.ToInt32(reader["id_usuario_asignado"]);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ObtenerTareaPorId."));
                throw new Exception("Hubo un problema con la consulta a la BD al ObtenerTareaPorId.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        if (tarea == null)
        {
            throw new Exception("Tarea no Existe.");
        }
        return tarea;
    }
    public bool ExisteTarea(int idTablero, int idTarea, string nombreTarea)
    {
        if(nombreTarea == null) {
            return true;
        }
        
        var query = "SELECT Tarea.nombre FROM Tarea WHERE nombre = @nombreT AND id_tablero = @idT AND id != @idTarea;";

        bool existe = false;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@nombreT", nombreTarea));
                command.Parameters.Add(new SqliteParameter("@idT", idTablero));
                command.Parameters.Add(new SqliteParameter("@idTarea", idTarea));
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
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ExisteTarea."));
                throw new Exception("Hubo un problema con la consulta a la BD al ExisteTarea.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        return existe;
    }
    public void CambiarEstadoTarea(int idTarea, int codigoEstado, string colorEstado)
    {
        var query = "UPDATE Tarea SET estado = @estadoTarea, color = @colorTarea WHERE id = @idTarea;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@estadoTarea", codigoEstado));
                command.Parameters.Add(new SqliteParameter("@colorTarea", colorEstado));
                command.Parameters.Add(new SqliteParameter("@idTarea", idTarea));
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al CambiarEstadoTarea."));
                throw new Exception("Hubo un problema con la consulta a la BD al CambiarEstadoTarea.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public int CantidadDeTareas(int idTablero)
    {
        int cantidad_tareas = 0;

        var query = "SELECT COUNT(id) FROM Tarea WHERE id_tablero = @idTablero";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();

                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idTablero", idTablero));

                cantidad_tareas = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al CantidadDeTareas."));
                throw new Exception("Hubo un problema con la consulta a la BD al CantidadDeTareas.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        return cantidad_tareas;
    }
    public int CantidadDeTareasRealizadas(int idTablero)
    {
        int cantidad_tareas_realizadas = 0;

        var query = "SELECT COUNT(id) FROM Tarea WHERE id_tablero = @idTablero AND estado = 4";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();

                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idTablero", idTablero));

                cantidad_tareas_realizadas = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al CantidadDeTareasRealizadas."));
                throw new Exception("Hubo un problema con la consulta a la BD al CantidadDeTareasRealizadas.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        return cantidad_tareas_realizadas;
    }
    public void CambiarPropietarioTarea(int? idUsuario, int idTarea)
    {
        var query = @"UPDATE Tarea SET id_usuario_asignado = @idUsuarioAsignado WHERE id = @idTarea;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@idUsuarioAsignado", idUsuario ?? (object)DBNull.Value));
                command.Parameters.Add(new SqliteParameter("@idTarea", idTarea));
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al CambiarPropietarioTarea."));
                throw new Exception("Hubo un problema con la consulta a la BD al CambiarPropietarioTarea.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}