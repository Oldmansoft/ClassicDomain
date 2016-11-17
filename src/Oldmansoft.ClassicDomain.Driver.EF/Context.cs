using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.EF
{
    /// <summary>
    /// Entity Framework 的实体上下文
    /// </summary>
    public abstract class Context : System.Data.Entity.DbContext, IUnitOfWorkManagedItem
    {
        /// <summary>
        /// 配置读取的连接串名称
        /// </summary>
        protected string ConnectionName { get; set; }

        /// <summary>
        /// 创建 Entity Framework 的实体上下文
        /// </summary>
        public Context()
        {
            ConnectionName = GetType().FullName;
        }

        /// <summary>
        /// 获取连接串名称
        /// </summary>
        /// <returns></returns>
        public string GetConnectionName()
        {
            return ConnectionName;
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public int Commit()
        {
            try
            {
                return SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException is System.Data.SqlClient.SqlException)
                {
                    int errorNumber = (ex.InnerException.InnerException as System.Data.SqlClient.SqlException).Number;
                    if (errorNumber == 2601 || errorNumber == 2627)
                    {
                        throw new UniqueException(ex.InnerException.InnerException);
                    }
                }
                throw;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var entityError = ex.EntityValidationErrors.FirstOrDefault();
                if (entityError != null)
                {
                    var validationError = entityError.ValidationErrors.FirstOrDefault();
                    if (validationError != null)
                    {
                        throw new ClassicDomainException(validationError.ErrorMessage);
                    }
                }
                throw;
            }
        }
        
        void IUnitOfWorkManagedItem.ModelCreating()
        {
        }
    }
}
