using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.EntityFramework
{
    class BookConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Domain.Book>
    {
        public BookConfiguration()
        {
            Property(m => m.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(m => m.Name).IsRequired().HasMaxLength(50).IsUnicode(false);
        }
    }
}
