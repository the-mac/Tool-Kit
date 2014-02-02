namespace MyCompany.Visitors.Data.Infrastructure.Initializers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using MyCompany.Visitors.Model;
    using System.Configuration;

    /// <summary>
    /// The default initializer for context. You can learn more 
    /// about EF initializers here
    /// http://msdn.microsoft.com/en-us/library/gg696323(v=VS.103).aspx
    /// </summary>
    class MyCompanyContextInitializer :
        DropCreateDatabaseIfModelChanges<MyCompanyContext>
    {

        private static readonly Random _randomize = new Random();
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private string _picturePath = "FakeImages\\{0}.jpg";
        private string _smallPicturePath = "FakeImages\\{0} - small.jpg";
        private List<string> _employeeEmails = new List<string>() 
        {
            String.Format("Andrew.Davis@{0}", tenant),
            String.Format("Christen.Anderson@{0}", tenant),
            String.Format("David.Alexander@{0}", tenant),
            String.Format("Robin.Counts@{0}", tenant),
            String.Format("Thomas.Andersen@{0}", tenant),
            String.Format("Josh.Bailey@{0}", tenant),
            String.Format("Adam.Barr@{0}", tenant),
            String.Format("Christa.Geller@{0}", tenant),
            String.Format("Carole.Poland@{0}", tenant),
            String.Format("Cristina.Potra@{0}", tenant)
        };
        private List<string> _employeeNames = new List<string>() 
        {
            "Andrew Davis",
            "Christen Anderson", 
            "David Alexander",
            "Robin Counts",
            "Thomas Andersen", 
            "Josh Bailey", 
            "Adam Barr",
            "Christa Geller",
            "Carole Poland",
            "Cristina Potra"
        };
        private List<string> _visitorNames = new List<string>()
        {
            "Garth Fort",
            "Chloe Smith",
            "Dorothy White"
        };

        /// <summary>
        /// Seed
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(MyCompanyContext context)
        {
            CreateTeamManagers(context);
            CreateEmployees(context);
            CreateEmployeePictures(context);
            CreateVisitors(context);
            CreateVisits(context);
            CreateQuestions(context);
        }


        private void CreateQuestions(MyCompanyContext context)
        {
            context.Questions.Add(new Question()
            {
                Text = "Why are software updates necessary?",
                Answer = "Microsoft is committed to providing its customers with software that has been tested for safety and security. Although no system is completely secure, we use processes, technology, and several specially focused teams to investigate, fix, and learn from security issues to help us meet this goal and to provide guidance to customers on how to help protect their PCs."
            });

            context.Questions.Add(new Question()
            {
                Text = "How can I keep my software up to date?",
                Answer = "Microsoft offers a range of online services to help you keep your computer up to date. Windows Update finds updates that you might not even be aware of and provides you with the simplest way to install updates that help prevent or fix problems, improve how your computer works, or enhance your computing experience. Visit Windows Update to learn more."
            });

            context.Questions.Add(new Question()
            {
                Text = "How do I find worldwide downloads?",
                Answer = "Microsoft delivers downloads in more than 118 languages worldwide. The Download Center now combines all English downloads into a single English Download Center. We no longer offer separate downloads for U.S. English, U.K. English, Australian English, or Canadian English."
            });

            context.Questions.Add(new Question()
            {
                Text = "How do I install downloaded software?",
                Answer = "Before you can use any software that you download, you must install it. For example, if you download a security update but do not install it, the update will not provide any protection for your computer."
            });

            context.Questions.Add(new Question()
            {
                Text = "Can I try Office before I buy?",
                Answer = "Yes. You can sign up to try Office 365 Home Premium for one month free."
            });

            context.Questions.Add(new Question()
            {
                Text = "What do I get with the Office 365 Home Premium trial? ",
                Answer = "The Office trial gives you access to all the features of Office 365 Home Premium except the additional. 20 GB of SkyDrive storage. You can install the Office trial alongside your existing version of Office."
            });

            context.Questions.Add(new Question()
            {
                Text = "How do I make sure I always have the latest Office applications?",
                Answer = "Office 365 customers with an active subscription always get the newest versions of the Office applications when they are available. When we release a new version of Office, you will be notified that you have the option to update your software to the latest version. "
            });

            context.Questions.Add(new Question()
            {
                Text = "Can I install the new Office on my Mac?",
                Answer = " you have an active Office 365 Home Premium or Office 365 University subscription, and available installs, you can install Office applications including Word, Excel, PowerPoint and Outlook on your Mac. The applications available for Mac users and the version numbers may be different from those available for PC users."
            });
            context.SaveChanges();
        }

        private void CreateTeamManagers(MyCompanyContext context)
        {
            int managersCount = 2;

            for (int i = 0; i < managersCount; i++)
            {
                int id = i + 1;
                var name = _employeeNames[i];
                var split = name.Split(' ');
                context.Employees.Add(new Employee()
                {
                    EmployeeId = id,
                    FirstName = split[0],
                    LastName = split[1],
                    Email = _employeeEmails[i],
                    JobTitle = "Team Lead",
                });

                context.Teams.Add(new Team() { TeamId = id, ManagerId = id });
            }

            context.SaveChanges();
        }

        private void CreateEmployees(MyCompanyContext context)
        {
            int initialId = context.Employees.Count() + 1;
            int employeesCount = _employeeEmails.Count + 1;

            int teamOneId = context.Teams.OrderBy(t => t.TeamId).First().TeamId;
            int teamTwoId = context.Teams.OrderByDescending(t => t.TeamId).First().TeamId;

            for (int i = initialId; i < employeesCount; i++)
            {
                int index = i - 1;
                var name = _employeeNames[index];

                var split = name.Split(' ');
                context.Employees.Add(new Employee()
                {
                    EmployeeId = i,
                    FirstName = split[0],
                    LastName = split[1],
                    Email = _employeeEmails[index],
                    JobTitle = GetPosition(i),
                    TeamId = i > (_employeeEmails.Count / 2) ? teamOneId : teamTwoId
                });
            }

            context.SaveChanges();
        }

        private void CreateVisitors(MyCompanyContext context)
        {
            int visitorsCount = 3;
            int visitorPictureId = 1;

            for (int i = 0; i < visitorsCount; i++)
            {
                int id = i + 1;
                var name = _visitorNames[i];
                var split = name.Split(' ');

                context.Visitors.Add(new Visitor()
                    {
                        VisitorId = id,
                        FirstName = split[0],
                        LastName = split[1],
                        Email = name.Replace(" ", ".") + "@mycompany.com",
                        Position = GetPosition(i),
                        Company = "MyCompany",
                        CreatedDateTime = DateTime.UtcNow,
                        LastModifiedDateTime = DateTime.UtcNow
                    }
                );

                string path = string.Format(_smallPicturePath, name);
                context.VisitorPictures.Add(new VisitorPicture()
                {
                    VisitorPictureId = visitorPictureId,
                    VisitorId = id,
                    PictureType = PictureType.Small,
                    Content = GetPicture(path)
                });
                visitorPictureId++;

                path = string.Format(_picturePath, name);
                context.VisitorPictures.Add(new VisitorPicture()
                {
                    VisitorPictureId = visitorPictureId,
                    VisitorId = id,
                    PictureType = PictureType.Big,
                    Content = GetPicture(path)
                });
                visitorPictureId++;
            }

            context.SaveChanges();
        }

        private void CreateVisits(MyCompanyContext context)
        {
            var employeesIds = context.Employees.Select(e => e.EmployeeId).ToList();
            foreach (var visitor in context.Visitors)
            {
                int visits = _randomize.Next(15, 40);

                for (int i = 0; i < visits; i++)
                {
                    int index = 0; //half of the visits go to Andrew Davids.
                    if (i % 2 != 0)
                    {
                        index = _randomize.Next(0, employeesIds.Count);
                    }

                    int employeeId = employeesIds.ElementAt(index);
                    int days = _randomize.Next(-3, 15);
                    int minutes = _randomize.Next(5, 480);

                    var visit = new Visit()
                    {
                        CreatedDateTime = DateTime.UtcNow,
                        VisitDateTime = DateTime.UtcNow.AddDays(days).AddMinutes(minutes),
                        Comments = string.Empty,
                        EmployeeId = employeeId,
                        VisitorId = visitor.VisitorId,
                        Status = VisitStatus.Pending
                    };

                    if (i % 3 == 0)
                    {
                        visit.HasCar = true;
                        visit.Plate = string.Format("B{0}E-6610", visitor.FirstName.Substring(0, 1));
                    }

                    context.Visits.Add(visit);
                }
            }

            context.SaveChanges();
        }

        private void CreateEmployeePictures(MyCompanyContext context)
        {
            int employeePictureId = 1;

            foreach (var employee in context.Employees)
            {
                string employeeName = string.Format("{0} {1}", employee.FirstName, employee.LastName);
                string path = string.Format(_smallPicturePath, employeeName);
                context.EmployeePictures.Add(new EmployeePicture()
                {
                    EmployeePictureId = employeePictureId,
                    EmployeeId = employee.EmployeeId,
                    PictureType = PictureType.Small,
                    Content = GetPicture(path)
                });
                employeePictureId++;

                path = string.Format(_picturePath, employeeName);
                context.EmployeePictures.Add(new EmployeePicture()
                {
                    EmployeePictureId = employeePictureId,
                    EmployeeId = employee.EmployeeId,
                    PictureType = PictureType.Big,
                    Content = GetPicture(path)
                });
                employeePictureId++;
            }

            context.SaveChanges();
        }

        private static byte[] GetPicture(string fileName)
        {
            string path = new Uri(Assembly.GetAssembly(typeof(MyCompanyContextInitializer)).CodeBase).LocalPath;
            FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(path),
                                                         fileName), FileMode.Open, FileAccess.Read);

            using (BinaryReader br = new BinaryReader(fs))
            {
                return br.ReadBytes((int)fs.Length);
            }
        }

        private static string GetPosition(int index)
        {
            List<string> positions = new List<string>() {
                "Development advisor", 
                "Software engineer", 
                "Frontend developer", 
                "Backend developer", 
            };

            return index / 3 < positions.Count ? positions[index / 3] : positions[0];
        }
    }
}