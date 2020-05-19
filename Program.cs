using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace WorkingWithCSV
{
    class Program
    {
        static void Main(string[] args)
        {

            //I had the same issue.I fixed it by opening the reference properties and setting both "Copy Local" and "Embed Interop Types" to "Yes".
            string[] names = { "Abak", "Babak", "Cabak", "Dabak", "Ebak", "Fabak", "Gabak", "Habak", "Ibak", "Jabak" };
            string[] surnames = { "Abacki", "Babacki", "Cabacki", "Dabacki", "Ebacki", "Fabacki", "Gabacki", "Habacki", "Ibacki", "Jabacki" };
            string[] gender = { "Male", "Female" };
            string[] nationalId = { "78082969784","91051765424","68060587682","68070148844","01291626548","04241729384","65092855637","87020328664","50121456695","53062386124"};
            string[] telephonnumers = { "65487654", "147852369", "123654789", "951357951", "35852369", "357852147", "4569874" };

            List<Person> personList = new List<Person>();

            try
            {
                personList = GetDataFromCSV("C:\\Users\\Zaicew\\Desktop\\persons.csv");
            }
            catch (FileNotFoundException e)
            {
                Random rnd = new Random();
                Console.WriteLine("Random creating 500 peoples!");
                for (int i = 0; i < 500; i++)
                    personList.Add(new Person(names[rnd.Next(1, 10)], surnames[rnd.Next(1, 10)], nationalId[rnd.Next(1, 10)], telephonnumers[rnd.Next(1, 7)], gender[rnd.Next(0, 2)]));
                SaveDataToCSV(personList, "C:\\Users\\Zaicew\\Desktop\\persons.csv");

            }

            foreach (var e in personList)
                Console.WriteLine(e.NationalId);

            personList = ValidationPersonsData(personList);

            Console.WriteLine("---------------------------------");

            foreach (var e in personList)
                //Console.WriteLine(e.NationalId);

            SaveDataToCSV(personList, "C:\\Users\\Zaicew\\Desktop\\persons.csv");

            Console.ReadKey();

        }


     


        static void SaveDataToCSV(List<Person> _personList, string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.HasHeaderRecord = false;
                csv.WriteRecords(_personList);
            }
        }

        static List<Person> GetDataFromCSV(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.HasHeaderRecord = false;
                try
                {
                    var list = csv.GetRecords<Person>().ToList();
                    return list;
                }
                catch (CsvHelper.BadDataException e)
                {
                    Console.WriteLine(e.ToString());
                    return null;
                }
            }
        }

        static List<Person> ValidationPersonsData(List<Person> _personList)
        {
            List<Person> personsToDelete = new List<Person>();
            foreach (var e in _personList)
            {
                if (CheckNationalId(e) == false) personsToDelete.Add(e);
                else if (IsTelephonNummerLenghtEqualsNine(e) == false) personsToDelete.Add(e);
            }

            foreach (var e in personsToDelete)
            {
                Console.WriteLine($"Deleting item: {e}");
                _personList.Remove(e);
            }


            return _personList;
        }
        static bool CheckNationalId(Person _p)
        {
            string firstTwo = _p.NationalId[0].ToString();
            firstTwo += _p.NationalId[1].ToString();
            string secondTwo = _p.NationalId[2].ToString();
            secondTwo += _p.NationalId[3].ToString();
            string thirdTwo = _p.NationalId[4].ToString();
            thirdTwo += _p.NationalId[5].ToString();

            if (_p.NationalId.Length != 11) { Console.WriteLine($"Wrong lenght of pesel! : {_p.NationalId}"); return false; }
            else if (!Enumerable.Range(0, 99).Contains(Convert.ToInt16(firstTwo))) { Console.WriteLine($"Wrong year! {_p.NationalId}"); return false; }
            else if (!Enumerable.Range(1, 12).Contains(Convert.ToInt16(secondTwo))) { Console.WriteLine($"Wrong month! {_p.NationalId}"); return false; }
            else if (!Enumerable.Range(1, 31).Contains(Convert.ToInt16(thirdTwo))) { Console.WriteLine($"Wrong day number! {_p.NationalId}"); return false; }
            else if (CheckSexFromPesel(_p) == false) { Console.WriteLine($"Wrong sex number! {_p.NationalId} {_p.Gender}"); return false; }
            else if (ValidationChecksumOfPesel(_p) != Convert.ToInt32(_p.NationalId.ToString().Last().ToString())) { Console.WriteLine($"Wrong walidation number! {_p.NationalId}"); return false; }
            else return true;
        }

        static int ValidationChecksumOfPesel(Person _p)
        {
            string result = "";
            int resultint = 0;

            //1 - 3 - 7 - 9 - 1 - 3 - 7 - 9 - 1 - 3

            for (int i = 0; i < 11; i++)
            {
                if (i == 0 || i == 4 || i == 8) result += (Convert.ToInt32(_p.NationalId[i].ToString()) * 1).ToString().Last(); 
                if (i == 1 || i == 5 || i == 9) result += (Convert.ToInt32(_p.NationalId[i].ToString()) * 3).ToString().Last();
                if (i == 2 || i == 6) result += (Convert.ToInt32(_p.NationalId[i].ToString()) * 7).ToString().Last(); 
                if (i == 3 || i == 7) result += (Convert.ToInt32(_p.NationalId[i].ToString()) * 9).ToString().Last(); 
            }

            foreach (var e in result)
                resultint += int.Parse(e.ToString());

            return 10 - int.Parse(resultint.ToString().Last().ToString());
        }

        static bool CheckSexFromPesel(Person _p)
        {
            string tmp = _p.NationalId;
            if (Convert.ToInt16(tmp[9]) % 2 == 0 && _p.Gender == "Female") return true; 
            if (Convert.ToInt16(tmp[9]) % 2 == 1 && _p.Gender == "Male") return true; 
            else return false;
        }

        static bool IsTelephonNummerLenghtEqualsNine(Person _p)
        {
            if (_p.TelephonNumber.Length != 9) { Console.WriteLine("Telephonnumber is not valid!"); return false; }
            return true;
        }

    }


    public class Person
    {
        [Name("Name")]
        public string Name { get; set; }
        [Name("Surname")]
        public string Surname { get; set; }
        [Name("NationalId")]
        public string NationalId { get; set; }
        [Name("Telephon Number")]
        public string TelephonNumber { get; set; }
        [Name("Gender")]
        public string Gender { get; set; }

        public Person(string _name, string _surname, string _nationalId, string _telephonnNumer, string _gender)
        {
            this.Name = _name;
            this.Surname = _surname;
            this.NationalId = _nationalId;
            this.TelephonNumber = _telephonnNumer;
            this.Gender = _gender;
        }
    }
}