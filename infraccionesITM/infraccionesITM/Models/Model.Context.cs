﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace infraccionesITM.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBExamenEntities3 : DbContext
    {
        public DBExamenEntities3()
            : base("name=DBExamenEntities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<FotoInfraccion> FotoInfraccions { get; set; }
        public virtual DbSet<Infraccion> Infraccions { get; set; }
        public virtual DbSet<Vehiculo> Vehiculoes { get; set; }
    }
}
