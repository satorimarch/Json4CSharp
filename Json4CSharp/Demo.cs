using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json4CSharp
{
    public class Demo
    {
        public static void Test(string str)
        {
            try {
                JsonData jsonData = Json4CSharp.Json(str);
                Console.WriteLine(jsonData);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Test1()
        {
            StreamReader streamReader = new StreamReader("Test1.txt");
            int index = 1;
            for (string? str; (str = streamReader.ReadLine()) != null; index++) {
                Console.WriteLine($"Case{index}:");
                Test(str);
                Console.WriteLine();
            }
        }

        public static void Test2()
        {
            StreamReader streamReader = new StreamReader("Test2.txt");
            string str = streamReader.ReadToEnd();

            JsonData jsonData = Json4CSharp.Json(str);
            Console.WriteLine(jsonData["configurations"][0]["name"]);
            //Console.WriteLine(jsonData);
        }

        public static void Test3()
        {
            StreamReader streamReader = new StreamReader("Test3.txt");
            string str = streamReader.ReadToEnd();
            //Console.WriteLine(Json4CSharp.Json(str));
        }

        public static void Main()
        {
            Test1();
            Test2();
            Test3();
        }

    }
}
