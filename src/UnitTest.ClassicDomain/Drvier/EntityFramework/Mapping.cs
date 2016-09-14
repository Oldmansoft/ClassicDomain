using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.EntityFramework
{
    class Mapping : Oldmansoft.ClassicDomain.Driver.EF.Context
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new BookConfiguration());
            modelBuilder.Configurations.Add(new AuthorConfiguration());
        }
    }
}
