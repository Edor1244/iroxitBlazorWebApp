using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace iroxitBlazorApp.Shared
{
    public class Venta 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDVentas { get; set; }

        [Required]
        public int IDProductos { get; set; } 

        [ForeignKey("IDProductos")]
        public Producto Producto { get; set; } = new Producto();

        [Required]
        public int CantidadVendida { get; set; }

        [Required]
        public DateTime Fecha { get; set; }
    }
}
