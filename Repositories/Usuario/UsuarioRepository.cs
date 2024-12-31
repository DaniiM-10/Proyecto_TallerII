using Microsoft.Data.Sqlite;
using Proyecto_TallerII.Models;
namespace Proyecto_TallerII.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ILogger<UsuarioRepository> _logger;
    private readonly string connectionString;

    public UsuarioRepository(string CadenaDeConexion)
    {
        connectionString = CadenaDeConexion;
    }

    public void CrearUsuario(Usuario usuario)
    {
        var query = "INSERT INTO Usuario (nombre_de_usuario, password, rol_usuario) VALUES (@nombreU, @passwordU, @rolU);";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@nombreU", usuario.NombreUsuario);
                command.Parameters.AddWithValue("@passwordU", usuario.Password);
                command.Parameters.AddWithValue("@rolU", usuario.RolUsuario);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al CrearUsuario."));
                throw new Exception("Hubo un problema con la consulta a la BD al CrearUsuario.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public void EditarUsuario(int idUsuario, Usuario usuario)
    {   
        string query;
        if(usuario.Password == null) 
        {
            query = "UPDATE Usuario SET nombre_de_usuario = @nombreU, rol_usuario = @rolU WHERE id = @idU;";
        } else {
            query = "UPDATE Usuario SET nombre_de_usuario = @nombreU, password = @passwordU, rol_usuario = @rolU WHERE id = @idU;";
        }

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@nombreU", usuario.NombreUsuario);
                if(usuario.Password != null) 
                {
                    command.Parameters.AddWithValue("@passwordU", usuario.Password);
                }
                command.Parameters.AddWithValue("@rolU", usuario.RolUsuario);
                command.Parameters.AddWithValue("@idU", idUsuario);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ModificarUsuario."));
                throw new Exception("Hubo un problema con la consulta a la BD al ModificarUsuario.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public void EliminarUsuario(int idUsuario)
    {
        var queryTableros = "UPDATE Tablero SET id_usuario_propietario = NULL WHERE id_usuario_propietario = @idU;";
        var queryTareas = "UPDATE Tarea SET estado = 1, color = '737373', id_usuario_asignado = NULL WHERE id_usuario_asignado = @idU;";
        var query = "DELETE FROM Usuario WHERE id = @idU;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // Actualizar Tableros
                    var commandTableros = new SqliteCommand(queryTableros, connection, transaction);
                    commandTableros.Parameters.AddWithValue("@idU", idUsuario);
                    commandTableros.ExecuteNonQuery();

                    // Actualizar Tareas
                    var commandTareas = new SqliteCommand(queryTareas, connection, transaction);
                    commandTareas.Parameters.AddWithValue("@idU", idUsuario);
                    commandTareas.ExecuteNonQuery();

                    // Eliminar Usuario
                    var commandUsuario = new SqliteCommand(query, connection, transaction);
                    commandUsuario.Parameters.AddWithValue("@idU", idUsuario);
                    commandUsuario.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(LoggerMsj.MensajeEx(ex, "Error al interactuar con la base de datos en EliminarUsuario."));
                    transaction.Rollback(); // Revertir cambios si hay un error
                    throw new Exception("Hubo un problema con la consulta a la base de datos.", ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
    public List<Usuario> ObtenerTodosLosUsuarios()
    {
        List<Usuario> listaUsuarios = new List<Usuario>();

        var query = "SELECT * FROM Usuario;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var usuario = new Usuario
                        {
                            IdUsuario = Convert.ToInt32(reader["id"]),
                            NombreUsuario = reader["nombre_de_usuario"].ToString()!,
                            Password = reader["password"].ToString()!, 
                            RolUsuario = (Rol)Convert.ToInt32(reader["rol_usuario"])
                        };

                        listaUsuarios.Add(usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ObtenerTodosLosUsuarios."));
                throw new Exception("Hubo un problema con la consulta a la BD al ObtenerTodosLosUsuarios.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        if (listaUsuarios == null)
        {
            throw new Exception("Lista de Usuarios vacia.");
        }

        return listaUsuarios;
    }
    public Usuario ObtenerUsuarioPorId(int idUsuario)
    {
        var query = "SELECT * FROM Usuario WHERE id = @idU;";

        Usuario? usuario = null;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@idU", idUsuario);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            IdUsuario = Convert.ToInt32(reader["id"]),
                            NombreUsuario = reader["nombre_de_usuario"].ToString()!,
                            Password = reader["password"].ToString()!, 
                            RolUsuario = (Rol)Convert.ToInt32(reader["rol_usuario"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ObtenerUsuarioPorId."));
                throw new Exception("Hubo un problema con la consulta a la BD al ObtenerUsuarioPorId.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        if (usuario == null)
        {
            throw new Exception($"Usuario con id:{idUsuario} NO existe.");
        }

        return usuario!;
    }
    public Usuario ObtenerUsuarioPorCredenciales(string nombreUsuario, string passwordUsuario)
    {
        string query = "SELECT * FROM Usuario WHERE nombre_de_usuario = @nombreU AND password = @passwordU";
        
        Usuario usuario = null;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@nombreU", nombreUsuario));
                    command.Parameters.Add(new SqliteParameter("@passwordU", passwordUsuario));

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["id"]),
                                NombreUsuario = reader["nombre_de_usuario"].ToString()!,
                                Password = reader["password"].ToString()!, 
                                RolUsuario = (Rol)Convert.ToInt32(reader["rol_usuario"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ObtenerUsuarioPorCredenciales."));
                throw new Exception("Hubo un problema con la consulta a la BD al ObtenerUsuarioPorCredenciales.", ex);
            }
            finally
            {
                connection.Close();
            }
        }
        
        return usuario;
    }
    public string ObtenerNombreUsuario(int idUsuario) 
    {
        var query = "SELECT nombre_de_usuario FROM Usuario WHERE id = @idU;";

        string nombreUsuario = "Sin usuario.";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@idU", idUsuario);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nombreUsuario = reader["nombre_de_usuario"].ToString()!;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ObtenerNombreUsuario."));
                throw new Exception("Hubo un problema con la consulta a la BD al ObtenerNombreUsuario.", ex);
            }
            finally
            {
                connection.Close();
            }
        }

        return nombreUsuario;
    }
    public bool ExisteUsuario(string nombreUsuario, int idUsuario)
    {
        if(nombreUsuario == null) 
        {
            return false;
        }

        var query = "SELECT * FROM Usuario WHERE nombre_de_usuario = @nombreU AND id != @idU;";

        bool existe = false;

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@nombreU", nombreUsuario));
                command.Parameters.Add(new SqliteParameter("@idU", idUsuario));
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
                _logger.LogError(LoggerMsj.MensajeEx(ex, "Hubo un problema con la consulta a la BD al ExisteUsuario."));
                throw new Exception("Hubo un problema con la consulta a la BD al ExisteUsuario.");
            }
            finally
            {
                connection.Close();
            }
        }
        return existe;
    }
}