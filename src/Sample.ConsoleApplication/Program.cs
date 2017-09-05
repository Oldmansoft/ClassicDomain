using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Guid id;
            Console.Write("开始添加");
            Console.ReadLine();
            if (new Applications.Persons().Add("Oldman", out id))
            {
                Console.Write("开始修改");
                Console.ReadLine();
                new Applications.Persons().Edit(id, "Oldmansoft");
            }
            else
            {
                Console.WriteLine("添加失败");
            }

            int totalCount;
            var persons = new Applications.Persons().Page(1, 10, out totalCount);
            foreach (var person in persons)
            {
                Console.Write(person.Id);
                Console.Write(" ");
                Console.WriteLine(person.Name);
            }
            Console.Write("开始移除");
            Console.ReadLine();
            new Applications.Persons().Remove(id);
            Console.WriteLine("移除完成");
            Console.ReadLine();
        }
    }
}
