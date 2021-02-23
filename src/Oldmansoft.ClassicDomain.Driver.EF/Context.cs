using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Oldmansoft.ClassicDomain.Driver.EF
{
    /// <summary>
    /// Entity Framework 的实体上下文
    /// </summary>
    public abstract class Context : System.Data.Entity.DbContext, IUnitOfWorkManagedItem
    {
        private ConcurrentQueue<ICommand> Commands { get; set; }

        /// <summary>
        /// 保存改变
        /// </summary>
        /// <param name="domainType">领域实体类型</param>
        /// <returns></returns>
        public int SaveChanges(Type domainType)
        {
            try
            {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.InnerException is System.Data.SqlClient.SqlException)
                {
                    int errorNumber = (ex.InnerException.InnerException as System.Data.SqlClient.SqlException).Number;
                    if (errorNumber == 2601 || errorNumber == 2627)
                    {
                        throw new UniqueException(domainType, ex.InnerException.InnerException);
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
                        throw new ClassicDomainException(domainType, validationError.ErrorMessage);
                    }
                }
                throw;
            }
        }

        void IUnitOfWorkManagedItem.Init(ConcurrentQueue<ICommand> commands)
        {
            Commands = commands;
        }

        void IUnitOfWorkManagedItem.ModelCreating()
        {
        }

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="domain"></param>
        public void RegisterAdd<TDomain>(TDomain domain)
            where TDomain : class
        {
            Commands.Enqueue(new Commands.AddCommand<TDomain>(this, domain));
        }

        /// <summary>
        /// 注册替换
        /// </summary>
        /// <param name="domain"></param>
        public void RegisterReplace<TDomain>(TDomain domain)
            where TDomain : class
        {
            Commands.Enqueue(new Commands.ReplaceCommand<TDomain>(this, domain));
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="domain"></param>
        public void RegisterRemove<TDomain>(TDomain domain)
            where TDomain : class
        {
            Commands.Enqueue(new Commands.RemoveCommand<TDomain>(this, domain));
        }

        /// <summary>
        /// 注册执行
        /// </summary>
        /// <param name="execute"></param>
        public void RegisterExecute<TDomain>(Func<Context, bool> execute)
            where TDomain : class
        {
            Commands.Enqueue(new Commands.ActionCommand<TDomain>(this, execute));
        }
    }
}
