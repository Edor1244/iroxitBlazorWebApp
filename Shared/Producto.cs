using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iroxitBlazorApp.Shared
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Esto indica que la propiedad es una identidad
        public int IDProductos { get; set; }

        [Required] // Esto indica que el campo no puede ser nulo
        [MaxLength(255)] // Esto establece la longitud máxima del campo
        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(7,2)")] // Esto especifica el tipo de columna en la base de datos
        public decimal PrecioUnitario { get; set; }

        [Required]
        public int Existencias { get; set; }
    }
}
