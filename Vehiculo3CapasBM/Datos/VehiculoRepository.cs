using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows.Input;
using Vehiculo3CapasBM.Models;

namespace Vehiculo3CapasBM.Datos
{
    public class VehiculoRepository
    {
        private readonly string _cadenaConexion;
        //constructor
        public VehiculoRepository(IConfiguration configuration)
        {
            _cadenaConexion = configuration.GetConnectionString("VehiculoConnection")
                ?? throw new InvalidOperationException("No se encontreo la conexion");
        }
        //Programacionn estilo landa
        private SqlConnection CrearConexion() => new(_cadenaConexion);

        private static Vehiculo LeerVehiculo(SqlDataReader lector) => new()
        {
            Id = lector.GetInt32(0),
            Placa = lector.GetString(1),
            Marca = lector.GetString(2),
            Modelo = lector.GetString(3),
            Anio = lector.GetInt32(4),
            Kilometraje = lector.GetInt32(5)
        };

        public static void AgregarParametros(SqlCommand command,Vehiculo vehiculo)
        {
            command.Parameters.Add("@Placa", SqlDbType.NVarChar, 10).Value = vehiculo.Placa;
            command.Parameters.Add("@Marca", SqlDbType.NVarChar, 100).Value = vehiculo.Marca;
            command.Parameters.Add("@Modelo", SqlDbType.NVarChar, 100).Value = vehiculo.Modelo;
            command.Parameters.Add("@Anio", SqlDbType.Int).Value = vehiculo.Anio;
            command.Parameters.Add("@Kilometreje", SqlDbType.Int).Value = vehiculo.Kilometraje;
        }
        public List<Vehiculo> ObtenerVehiculo()
        {
            var vehiculo = new List<Vehiculo>();
            using var conexion = CrearConexion();
            using var comando = new SqlCommand(
                "Select Id, Placa, Modelo ,Anio, kilometraje from dbo.vehiculo order by Id"
                , conexion
                );
            conexion.Open();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
                vehiculo.Add(LeerVehiculo(lector));
            return vehiculo;
        }
        public Vehiculo? BuscarPorId(int id)
        {
            using var conexion = CrearConexion();
            using var comando = new SqlCommand(
                "Select Id, Placa, Marca, Modelo,Anio, Kilometraje from dbo.Vehiculo where Id = @Id",
                conexion
                );
            conexion.Open();
            using var lector = comando.ExecuteReader();
            return lector.Read() ? LeerVehiculo(lector) : null ;
        } 
        public bool ExistePlaca(string placa,int exceptoId = 0)
        {
            using var conexion = CrearConexion();
            using var comando = new SqlCommand(
                "Select count(1) from dbo.Vehiculo where placa = @Placa and Id <> @ExceptoId", conexion
                );
           comando.Parameters.Add("@Placa",SqlDbType.NVarChar,10).Value = placa;
            comando.Parameters.Add("@ExeptoId", SqlDbType.Int).Value = exceptoId;
            conexion.Open();
            return Convert.ToInt32(comando.ExecuteScalar()) > 0;
        }
        public void Agregar(Vehiculo vehiculo)
        {
            using var conexion = CrearConexion();
            using var comando = new SqlCommand(
                """
                Insert into dbo.Vehiculo (Placa, Marca, Modelo, Anio, kilometraje)
                output insertd.id
                values(@Placa,@Marca,@Modelo, @Anio, @Kilometraje)

                """,
                conexion
                );
            AgregarParametros(comando, vehiculo);
            conexion.Open();
            vehiculo.Id = Convert.ToInt32(comando.ExecuteScalar());
            
        }
        public bool Actualizar(Vehiculo vehiculo)
        {
            using var conexion = CrearConexion();
            using var comando = new SqlCommand(
                """
                Update dbo.Vehiculos
                set Placa = @Placa ,Marca = @Marca, Modelo =@Modelo,
                    Anio = @Anio , kilometraje = @kilometraje
                    where Id = @Id
                """,
                conexion);

            AgregarParametros(comando, vehiculo);
            comando.Parameters.Add("@Id", SqlDbType.Int).Value = vehiculo.Id;
            conexion.Open();
            return comando.ExecuteNonQuery() > 0;
        }

    }
}
