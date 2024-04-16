using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PuntodeVentaAliMon.Models
{
    public class PuntoVentaContext : DbContext
    {
        public PuntoVentaContext() : base("PuntoVentaDatabase")
        {
        }

        public DbSet<UsuUsuario> UsuUsuarios { get; set; }
        public DbSet<UsuCatEstado> UsuCatEstado { get; set; }
        public DbSet<UsuCatTipoUsuario> UsuCatTipoUsuarios { get; set; }
        public DbSet<ProCatCategoria> ProCatCategorias { get; set; }
        public DbSet<ProCatSubcategoria> ProCatSubcategorias { get; set; }
        public DbSet<ProProducto> ProProductos { get; set; }
        public DbSet<VenCatEstado> VenCatEstado { get; set; }
        public DbSet<VenVenta> VenVenta { get; set; }
        public DbSet<VenVentaProducto> VenVentaProducto { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuUsuario>()
                .HasRequired(u => u.Estado)
                .WithMany()
                .HasForeignKey(u => u.usuCatEstado);

            modelBuilder.Entity<UsuUsuario>()
                .HasRequired(u => u.TipoUsuario)
                .WithMany()
                .HasForeignKey(u => u.usuCatTipoUsuario);

            // Configuración de las relaciones para ProProducto
            modelBuilder.Entity<ProProducto>()
                .HasRequired(p => p.ProCatCategoria)
                .WithMany(c => c.ProProductos)
                .HasForeignKey(p => p.idProCatCategoria);

            modelBuilder.Entity<ProProducto>()
                .HasRequired(p => p.ProCatSubcategoria)
                .WithMany(s => s.ProProductos)
                .HasForeignKey(p => p.idProCatSubcategoria);

            // Configuración de las relaciones para VenVenta
            modelBuilder.Entity<VenVenta>()
                .HasRequired(v => v.VenCatEstado)
                .WithMany(e => e.VenVentas)
                .HasForeignKey(v => v.idVenCatEstado);

            // Configuración de las relaciones para VenVentaProducto
            modelBuilder.Entity<VenVentaProducto>()
                .HasRequired(vp => vp.VenVenta)
                .WithMany(v => v.VenVentaProductos)
                .HasForeignKey(vp => vp.idVenVenta);


        }
    }
}
