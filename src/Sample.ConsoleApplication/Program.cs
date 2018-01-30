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
            if (new Application.Persons().Add("A", "Oldman", out id))
            {
                Console.Write("开始修改");
                Console.ReadLine();
                new Application.Persons().Change(id, "B", "Oldmansoft");
            }
            else
            {
                Console.WriteLine("添加失败");
            }

            int totalCount;
            var persons = new Application.Persons().Page(1, 10, out totalCount);
            foreach (var person in persons)
            {
                Console.Write(person.Id);
                Console.Write(" ");
                Console.WriteLine(person);
            }
            Console.Write("开始移除");
            Console.ReadLine();
            new Application.Persons().Remove(id);
            Console.WriteLine("移除完成");
            Console.ReadLine();
        }
    }
}
