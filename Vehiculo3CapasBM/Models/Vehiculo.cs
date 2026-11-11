using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vehiculo3CapasBM.Models
{
    public class Vehiculo
    {
   
       public int Id { get; set; }

        [Required(ErrorMessage = "Placa Incorrecta")]
        [StringLength(10,MinimumLength =10, ErrorMessage ="Numero de digitos incompletos")]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Marca Incorrecta")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "Modelo Incorrecta")]
        public string Modelo { get; set; } = string.Empty;
        
        [Range(1900,2100,ErrorMessage = "Rango Incorrecto")]
        public int Anio { get; set; }

        [Range(0,999999,ErrorMessage ="Kilometraje no esta en el rango")]
        public int Kilometraje { get; set; }

    }
}
