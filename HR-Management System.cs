using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Deployment.Internal;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace HR_project
{
    internal class Program
    {
        static List<Employee> employees = new List<Employee>();
       
        class Employee
        {
            public int ID;
            public string Name;
            public string Department;
            private decimal salary;
            public string Role;

            public decimal GetSalary()
            {
                return salary;
            }
            public void Setsalary(decimal sal)
            {
                salary = sal;
            }
            
            public virtual decimal CalculatePay(int hoursworked, int id)
            {

                return GetSalary();

            }
        }
        class LeaveRequest
        {
            public int ID;
            public string reason;
            public int days;
            public string status;

            public LeaveRequest()

            {
                status = "Pending";
            }
        }
        class Manager : Employee
        {
            public int Bonus = 500;
            public override decimal CalculatePay(int hoursworked, int id)
            {decimal salary = Payroll.findsalary(id);
                return salary + Bonus;
            }
            public static void ApproveLeave(int iD)
            {


                List<string> lines = File.ReadAllLines(@"E:\request.txt").ToList();
                int i = 0;
                while (i < lines.Count)
                {
                    string[] array = lines[i].Split(',');
                    int idfile = int.Parse(array[0].Split(':')[1]);
                    if (iD == idfile)
                    {
                        array[3] = "approve";

                        lines[i] = ($"id:{iD},reason:{array[1]},days:{array[2]},status:{array[3]}");
                        WriteLog($"Manager approve LeaveRequest---->id:{iD},reason:{array[1]},days:{array[2]},status:{array[3]}");
                        break;
                    }

                    i++;
                }
                File.WriteAllLines(@"E:\request.txt", lines);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Leave request for Employee {iD} Approved by Manager");
            }



            public static void RejectLeave(int id)
            {


                List<string> lines = File.ReadAllLines(@"E:\request.txt").ToList();
                int i = 0;
                while (i < lines.Count)
                {
                    string[] array = lines[i].Split(',');
                    int idfile = int.Parse(array[0].Split(':')[1]);
                    if (id == idfile)
                    {
                        array[3] = "Reject";

                        lines[i] = ($"id:{id},reason:{array[1]},days:{array[2]},status:{array[3]}");
                        WriteLog($"Manager Reject--->id:{id},reason:{array[1]},days:{array[2]},status:{array[3]}");
                        break;
                    }

                    i++;
                }
                File.WriteAllLines(@"E:\request.txt", lines);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Leave request for Employee {id} Rejected by Manager");
            }

        }
        class Developer : Employee
        {
            public override decimal CalculatePay(int hoursworked, int id)
            {

                int overtime = hoursworked - 160;
                decimal salary = Payroll.findsalary(id);
                decimal finalsalary = salary;
                if (overtime > 0)
                {

                    finalsalary += overtime * 50;

                }
                Setsalary(finalsalary);
                return finalsalary;
            }

        }
        class Intern : Employee
        {
            public override decimal CalculatePay(int hoursworked, int id)
            {
                decimal salary = Payroll.findsalary(id);
                return salary;
            }
        }
        public static bool check(int ID)
        {

            using (StreamReader fs = new StreamReader(@"E:\EMPdata.txt"))
            {
                string line = fs.ReadLine();
                while (line != null)
                {

                    string[] array = line.Split(',');
                    string EmpID = array[0].Trim();
                    if ($"ID:{ID}" == EmpID)
                    {

                        return true;
                    }
                    line = fs.ReadLine();

                }

                return false;
            }
        }
        class Attendance
        {
            static Dictionary<int, DateTime> attributes = new Dictionary<int, DateTime>();

            public void CheckIn(int Id)
            {

                if (check(Id))
                {

                    DateTime dateTimein = DateTime.Now;
                    attributes[Id] = dateTimein;
                    using (StreamWriter sw = new StreamWriter(@"E:\Attendance.txt", true))
                    {
                        sw.WriteLine($"id:{Id},checkin:{dateTimein}");
                    }
                    WriteLog($"checkin----->id:{Id},checkin:{dateTimein}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("checkin successful");

                }
            }
            public void CheckOut(int ID)
            {

                if (check(ID))
                {
                    DateTime dateTimeout = DateTime.Now;

                    TimeSpan total = dateTimeout - attributes[ID];
                    List<string> lines = File.ReadAllLines(@"E:\Attendance.txt").ToList();
                    int i = 0;
                    while (i < lines.Count)
                    {
                        string[] array = lines[i].Split(',');
                        int id = int.Parse(array[0].Split(':')[1]);
                        if (ID == id)
                        {

                            lines[i] = ($"id:{ID},checkin:{attributes[ID]},checkout:{dateTimeout},totalhourday:{total}");
                            WriteLog($"check out------>id:{ID},checkin:{attributes[ID]},checkout:{dateTimeout},totalhourday:{total}");
                            break;
                        }

                        i++;
                    }
                    File.WriteAllLines(@"E:\Attendance.txt", lines);

                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("checkout successful");
            }






            public static int GetTotalHours(int ID)
            {
                TimeSpan totalhour = new TimeSpan(0, 0, 0);
                if (check(ID))
                {
                    using (StreamReader fs = new StreamReader(@"E:\Attendance.txt"))
                    {

                        string line = fs.ReadLine();
                        while (line != null)
                        {

                            string[] array = line.Split(',');
                            if ($"id:{ID}" == array[0])
                            {

                                TimeSpan hours = TimeSpan.Parse(array[3].Replace("totalhourday:", ""));
                                totalhour += hours;
                            }
                            line = fs.ReadLine();
                        }

                    }
                }
                return (int)totalhour.TotalHours;
            }
        }
        class Payroll
        {
            public static int findsalary(int id)
            {
                int salary = 0;
                using (StreamReader fs = new StreamReader(@"E:\EMPdata.txt"))
                {
                    string line = fs.ReadLine();
                    while (line != null)
                    {

                        string[] array = line.Split(',');
                        string EmpID = array[0].Trim();

                        if ($"id:{id}" == EmpID)
                        {
                            salary = int.Parse(array[4].Split(':')[1].Trim());
                            break;

                        }
                        line = fs.ReadLine();

                    }


                }
                return salary;
            }
        }


        static void Main(string[] args)
        {//password admin=123
         //password manager=456
         //pasword developer/intern=789

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("==================================");
            Console.WriteLine("HR_Management_System_Requirements");
            Console.WriteLine("=================================");
            Console.Write("Enter type(Admin / Manager / Developer / Intern):");
            string Role = Console.ReadLine();
            Console.Write("Enter Password:");
            int Password = int.Parse(Console.ReadLine());
            if (Role.Trim().ToLower() == "admin".Trim().ToLower())
            {
            StartLoop:
                if (Password == 123)
                {

                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("---------------------");
                        Console.WriteLine("1.Add Employee");
                        Console.WriteLine("2.Update Employee Salary");
                        Console.WriteLine("3.Display Employee(by ID)");
                        Console.WriteLine("4.Calculate Payroll");
                        Console.WriteLine("5.View Log File");
                        Console.WriteLine("6.Exit");
                        Console.WriteLine("---------------------");
                        Console.Write("Enter number your choice:");
                        int choice = int.Parse(Console.ReadLine());
                        while (choice > 6 || choice < 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("invalid choice try again");
                            Console.Write("Enter number your choice:");
                            choice = int.Parse(Console.ReadLine());
                        }
                        switch (choice)
                        {
                            case 1:
                                AddEmployee();
                                break;
                            case 2:
                                UpdateSalary();
                                break;
                            case 3:
                                DisplayEmployee();
                                break;

                            case 4:
                                Console.Write("Eenter ID:");
                                int id = int.Parse(Console.ReadLine());
                                int totalHours = Attendance.GetTotalHours(id);
                                Console.WriteLine($"totalhourwork:{totalHours}");
                                using (StreamReader fs = new StreamReader(@"E:\EMPdata.txt"))
                                {
                                    string line = fs.ReadLine();
                                    while (line != null)
                                    {

                                        string[] array = line.Split(',');
                                        string EmpID = array[0].Trim();
                                        string role = array[3].Split(':')[1];
                                        if ($"ID:{id}" == EmpID)
                                        {
                                            if (role.Trim().ToLower() == "developer".Trim().ToLower())
                                            {
                                                Employee e1 = new Developer();
                                                decimal pay = e1.CalculatePay(totalHours, id);
                                                Console.WriteLine($"Final Salary: {pay}");

                                            }
                                            else if (role.Trim().ToLower() == "intern".Trim().ToLower())
                                            {
                                                Employee e1 = new Intern();
                                                decimal pay = e1.CalculatePay(totalHours, id);
                                                Console.WriteLine($"Final Salary: {pay}");

                                            }
                                            else if (role.Trim().ToLower() == "manager".Trim().ToLower())
                                            {
                                                Employee e1 = new Manager();

                                                decimal pay = e1.CalculatePay(totalHours, id);
                                                Console.WriteLine($"Final Salary: {pay}");

                                            }
                                            break;

                                        }
                                        line = fs.ReadLine();

                                    }
                                }
                                break;
                            case 5:
                                string[] log = File.ReadAllLines(@"E:\SystemLog.txt");
                                foreach (string logLine in log)
                                {
                                    Console.WriteLine(logLine);
                                }
                                break;
                            case 6:
                                return;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("invalid choice.....");
                                break;

                        }

                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sorry password not correct try again");
                    int i = 1;
                    bool pass = false;
                    while (i <= 3)
                    {
                        Console.Write("Enter Password:");
                        Password = int.Parse(Console.ReadLine());
                        if (Password == 123)
                        {
                            pass = true;
                            break;
                        }
                        i++;
                    }
                    if (pass)
                    {
                        goto StartLoop;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("sorry you not admin");
                        return;
                    }

                }



            }




            if (Role.Trim().ToLower() == "Developer".Trim().ToLower() || Role.Trim().ToLower() == "Intern".Trim().ToLower())
            {
            StartLoop:
                if (Password == 789)
                {
                    Attendance a = new Attendance();

                    while (true)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("---------------------");
                        Console.WriteLine("1.Check -in");
                        Console.WriteLine("2.Check -out");
                        Console.WriteLine("3.Request Leave");
                        Console.WriteLine("4.Exit");
                        Console.WriteLine("---------------------");
                        Console.WriteLine("Enter number your choice:");
                        int choice = int.Parse(Console.ReadLine());

                        switch (choice)
                        {
                            case 1:
                                Console.Write("Enter id:");
                                int id = int.Parse(Console.ReadLine());
                                a.CheckIn(id);

                                break;
                            case 2:
                                Console.Write("Enter id:");
                                int ID = int.Parse(Console.ReadLine());

                                a.CheckOut(ID);
                                break;
                            case 3:

                                LeaveRequest request = new LeaveRequest();
                                Console.Write("Enter Id:");
                                ID = int.Parse(Console.ReadLine());
                                if (check(ID))
                                {
                                    request.ID = ID;
                                    Console.Write("Please Enter reason:");
                                    request.reason = Console.ReadLine();
                                    Console.Write("Enter Number of leave days:");
                                    request.days = int.Parse(Console.ReadLine());
                                    using (StreamWriter f = new StreamWriter(@"E:\request.txt", true))
                                    {
                                        f.WriteLine($"id:{request.ID},reason:{request.reason},days:{request.days},status:{request.status}");
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Sorry, your ID is not found");
                                }
                                break;
                            case 4:
                                return;

                            default:
                                Console.WriteLine("invalid choice.....");
                                break;



                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sorry password not correct try again");
                    int i = 1;
                    bool pass = false;
                    while (i <= 3)
                    {
                        Console.Write("Enter Password:");
                        Password = int.Parse(Console.ReadLine());
                        if (Password == 789)
                        {
                            pass = true;
                            break;
                        }
                        i++;
                    }
                    if (pass)
                    {
                        goto StartLoop;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("sorry you not Developer/intern");
                        return;
                    }
                }

            }


            if (Role.Trim().ToLower() == "Manager".Trim().ToLower())
            {
            StartLoop:
                if (Password == 456)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("*********************************");
                    Console.WriteLine("1.Approve / Reject Leave Requests");
                    Console.WriteLine("2.Exit");
                    Console.WriteLine("*********************************");
                    Console.Write("Enter choice:");
                    int choice = int.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            Manager m = new Manager();
                            Console.Write("Enter ID:");
                            int id = int.Parse(Console.ReadLine());
                            bool checkcase = false;
                            string reason = null;
                            int days = 0;
                            string status = null;
                            if (check(id))
                            {
                                using (StreamReader f = new StreamReader(@"E:\request.txt"))
                                {

                                    string line = f.ReadLine();


                                    while (line != null)
                                    {
                                        string[] array = line.Split(',');
                                        int iD = int.Parse(array[0].Split(':')[1]);
                                        if (id == iD)
                                        {
                                            checkcase = true;
                                            reason = array[1];
                                            days = int.Parse(array[2].Split(':')[1]);
                                            status = array[3];
                                            break;


                                        }
                                        line = f.ReadLine();

                                    }
                                }
                            }
                            if (checkcase)
                            {
                                Console.WriteLine($"ID:{id}{reason},days:{days},{status}");
                                Console.Write("Please Enter your Decision(approve/reject):");
                                string Decision = Console.ReadLine();
                                if (Decision.Trim().ToLower() == "approve".Trim().ToLower())
                                {
                                    Manager.ApproveLeave(id);
                                }
                                else if (Decision.Trim().ToLower() == "reject".Trim().ToLower())
                                {
                                    Manager.RejectLeave(id);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Invalid decision! Please type 'approve' or 'reject'.");
                                    Console.ResetColor();
                                }



                            }



                            break;
                        case 2:
                            return;
                        default:
                            Console.WriteLine("invalid choice........");
                            break;



                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sorry password not correct try again");
                    int i = 1;
                    bool pass = false;
                    while (i <= 3)
                    {
                        Console.Write("Enter Password:");
                        Password = int.Parse(Console.ReadLine());
                        if (Password == 456)
                        {
                            pass = true;
                            break;
                        }
                        i++;
                    }
                    if (pass)
                    {
                        goto StartLoop;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("sorry you not Manager");
                        return;
                    }
                }
            }
        }    


            public static void AddEmployee()
            {
                bool exist = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Employee employee = new Employee();
                Console.Write("Enter ID:");
                int ID = int.Parse(Console.ReadLine());
                if (File.Exists(@"E:\EMPdata.txt"))
                {

                    var line = File.ReadAllLines(@"E:\EMPdata.txt");
                    foreach (var lines in line)
                    {
                        string[] array = lines.Split(',');
                        int id = int.Parse(array[0].Split(':')[1]);

                        if (ID == id)
                        {
                            exist = true;
                            break;

                        }
                    }
                }

                if (exist)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Employee with ID {ID} already exists! Cannot add duplicate.");
                }
                else
                {

                    employee.ID = ID;
                    Console.Write("Enter Name:");
                    employee.Name = Console.ReadLine();
                    Console.Write("Enter Department:");
                    employee.Department = Console.ReadLine();
                    Console.Write("Enter Salary:");
                    employee.Setsalary(int.Parse(Console.ReadLine()));
                    Console.Write("Enter type(Admin / Manager / Developer / Intern):");
                    employee.Role = Console.ReadLine();
                    employees.Add(employee);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Employee Added Successfully....");
                    using (StreamWriter fs = new StreamWriter(@"E:\EMPdata.txt", true))
                    {

                        fs.WriteLine($"id:{ID},Name:{employee.Name},Department:{employee.Department},Role:{employee.Role},Salary:{employee.GetSalary()}");

                    }
                    WriteLog($"Add Employee-----> ID:{ID},Name:{employee.Name},Department:{employee.Department},Role:{employee.Role},Salary:{employee.GetSalary()}");


                }
            }
            public static void UpdateSalary()
            {
                Console.Write("Enter ID:");
                int id = int.Parse(Console.ReadLine());
                var line = File.ReadAllLines(@"E:\EMPdata.txt");
                bool check = false;
                Console.Write("Enter Newsalary:");
                int newsalary = int.Parse(Console.ReadLine());
                List<string> lines = File.ReadAllLines(@"E:\EMPdata.txt").ToList();
                int i = 0;
                while (i < lines.Count)
                {
                    string[] array = lines[i].Split(',');
                    int idfile = int.Parse(array[0].Split(':')[1]);
                    if (id == idfile)
                    {


                        lines[i] = ($"id:{id},{array[1]},{array[2]},{array[3]},Salary:{newsalary}");
                        WriteLog($"modify salary--->id:{id},{array[1]},{array[2]},{array[3]},Salary:{newsalary}");

                        check = true;
                        break;
                    }

                    i++;
                }
                if (check)
                {
                    File.WriteAllLines(@"E:\EMPdata.txt", lines);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("done Update salary");
                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("id does not exist please try again");

                }

            }
            public static void DisplayEmployee()
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("**********************");
                Console.WriteLine("1.Diplay one Employeee");
                Console.WriteLine("2.Display All Employee");
                Console.WriteLine("**********************");
                Console.Write("number your choice:");
                int numcase = int.Parse(Console.ReadLine().Trim());
                while (numcase > 2 || numcase < 1)
                {
                    Console.WriteLine("invalid number of choice try again");
                    Console.Write("number your choice:");
                    numcase = int.Parse(Console.ReadLine());
                }
                if (numcase == 1)
                {
                    bool found = false;
                    Console.WriteLine("Enter ID:");
                    int id = int.Parse(Console.ReadLine());

                    using (StreamReader fs = new StreamReader(@"E:\EMPdata.txt"))
                    {

                        string line = fs.ReadLine();
                        while (line != null)
                        {
                            string[] array = line.Split(',');
                            int IDfile = int.Parse(array[0].Split(':')[1]);
                            if (id == IDfile)
                            {
                                Console.WriteLine(line);
                                found = true;
                                break;
                            }
                            line = fs.ReadLine();
                         
                        }
                           if (!found)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Employee not found!");
                            }
                    }
                }
                if (numcase == 2)
                {
                    using (StreamReader fs = new StreamReader(@"E:\EMPdata.txt"))
                    {
                        string line = fs.ReadLine();
                        while (line != null)
                        {
                            Console.WriteLine(line);
                            line = fs.ReadLine();
                        }
                    }
                }
            }
            public static void WriteLog(string message)
            {
                using (StreamWriter sw = new StreamWriter(@"E:\SystemLog.txt", true))
                {
                    sw.WriteLine($"{DateTime.Now}:{message}");
                }
            }
        }   
} 


