using System;
using System.Collections.Concurrent;
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
        private ConcurrentQueue<ICommand> Commands { get; set; }

        /// <summary>
        /// 配置读取的连接串名称
        /// </summary>
        private string ConnectionName { get; set; }

        /// <summary>
        /// 创建 Entity Framework 的实体上下文
        /// </summary>
        public Context()
        {
            Commands = new ConcurrentQueue<ICommand>();
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
        /// 保存改变
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
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
        /// 提交
        /// </summary>
        /// <returns></returns>
        public int Commit()
        {
            var result = 0;
            ICommand command;
            while (Commands.TryDequeue(out command))
            {
                if (command.Execute()) result++;
            }
            return result;
        }
    }
}
