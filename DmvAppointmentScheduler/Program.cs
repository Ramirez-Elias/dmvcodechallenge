using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DmvAppointmentScheduler
{
    class Program
    {
        public static Random random = new Random();
        public static List<Appointment> appointmentList = new List<Appointment>();
        public static List<TellerDuration> tellerSpecialtyList1 = new List<TellerDuration>();
        public static List<TellerDuration> tellerSpecialtyList2 = new List<TellerDuration>();
        public static List<TellerDuration> tellerSpecialtyList3 = new List<TellerDuration>();
        public static List<TellerDuration> tellerSpecialtyList0 = new List<TellerDuration>();
        static void Main(string[] args)
        {
            CustomerList customers = ReadCustomerData();
            TellerList tellers = ReadTellerData();
            CreateSpecialtyList(tellers);
            Calculation(customers, tellers);
            OutputTotalLengthToConsole();

        }
        private static CustomerList ReadCustomerData()
        {
            string fileName = "CustomerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            CustomerList customerData = JsonConvert.DeserializeObject<CustomerList>(jsonString);
            return customerData;

        }
        private static TellerList ReadTellerData()
        {
            string fileName = "TellerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            TellerList tellerData = JsonConvert.DeserializeObject<TellerList>(jsonString);
            return tellerData;

        }
        //FindTheIndex will call FindTheTeller and provides the customer type
        //in return it receives the most efficient teller
        private static int FindTheIndex(Customer customer)
        {
            int tellerIndex = 0;
            switch(customer.type)
            {
                case "1":
                    tellerIndex = FindTheTeller(customer.duration, tellerSpecialtyList1);
                    break;
                case "2":
                    tellerIndex = FindTheTeller(customer.duration, tellerSpecialtyList2);
                    break;
                case "3":
                    tellerIndex = FindTheTeller(customer.duration, tellerSpecialtyList3);
                    break;
                default:
                    tellerIndex = FindTheTeller(customer.duration, tellerSpecialtyList0);
                    break;
            }
            return tellerIndex;
        }
        private static int FindTheTeller(string duration, List<TellerDuration> list)
        {
            //This int will return the Teller Index
            int TellerID = 0;
            //TODO: Add this either in a class or as a separate method
            //Check if every teller has an appoinment
            bool isFull = false;
            double counter = 0;
            foreach (TellerDuration teller in list)
            {
                //if duration is == 0 it means teller has not customers assigned
                if (teller.duration == 0)
                    counter++;
            }
            //once counter == 0, then every teller in the list has a customer
            if (counter == 0)
                isFull=true;

            //Iteration to find best teller
            foreach (TellerDuration teller in list)
            {
                //If duration is not equal to 0, it means teller already has customers
                //assigned. If so, then function should be able to find the next teller
                //available, either without customers or with less amount of time assigned
                if(teller.duration!= 0)
                {
                    //Check if full
                    if(isFull)
                    {

                        // This is amazing! Sorting list to find first available teller
                        list = list.OrderBy(x => x.duration).ToList();
                        //TellerID gets Teller Index
                        TellerID = list[0].index;
                        //Duration is updated for teller
                        list[0].duration += Convert.ToDouble(duration) * list[0].multiplier;
                        //It breaks as it's not necessary to keep iterating
                        break;
                    }
                }
                //This should take care of the first time each teller is assigned a customer
                else
                {
                    //TellerID gets Teller Index
                    TellerID = teller.index;
                    //Duration is updated for teller
                    teller.duration = Convert.ToDouble(duration) * teller.multiplier;
                    //It breaks as it's not necessary to keep iterating
                    break;
                }
            }

            return TellerID;
        }
        //This function will split of teller list into smaller lists increasing
        //efficiency by reducing the size of list before iteration. 
        static void CreateSpecialtyList(TellerList tellers)
        {
            //index decleared. It will be assigned to NewTeller via non-default constructor 
            int index = 0;

            //Iteration where Teller list is split into 4 lists
            foreach(Teller teller in tellers.Teller)
            {
                if (teller.specialtyType == "1")
                {
                    //Creates a new teller by specialty
                    TellerDuration NewTeller = new TellerDuration(index, teller);
                    //Assigns the new teller to list 1
                    tellerSpecialtyList1.Add(NewTeller);
                }
                else if (teller.specialtyType == "2")
                {
                    //Create a new teller by specialty
                    TellerDuration NewTeller = new TellerDuration(index, teller);
                    //Assigns the new teller to list 2
                    tellerSpecialtyList2.Add(NewTeller);
                }
                else if(teller.specialtyType == "3")
                {
                    //Create a new teller by specialty
                    TellerDuration NewTeller = new TellerDuration(index, teller);
                    //Assigns the new teller to list 3
                    tellerSpecialtyList3.Add(NewTeller);
                }
                else
                {
                    //Create a new teller by specialty
                    TellerDuration NewTeller = new TellerDuration(index, teller);
                    //Assigns the new teller to list 0
                    tellerSpecialtyList0.Add(NewTeller);
                }
                index++;
            }

        }
        static void Calculation(CustomerList customers, TellerList tellers)
        {
            // Added the function FinTheIndex as it returns the index number corresponding
            //to the most efficient teller
            foreach(Customer customer in customers.Customer)
            {
                var appointment = new Appointment(customer, tellers.Teller[FindTheIndex(customer)]);
                appointmentList.Add(appointment); 
            }
        }
        static void OutputTotalLengthToConsole()
        {
            var tellerAppointments =
                from appointment in appointmentList
                group appointment by appointment.teller into tellerGroup
                select new
                {
                    teller = tellerGroup.Key,
                    totalDuration = tellerGroup.Sum(x => x.duration),
                };
            var max = tellerAppointments.OrderBy(i => i.totalDuration).LastOrDefault();
            Console.WriteLine("Teller " + max.teller.id + " will work for " + max.totalDuration + " minutes!");
        }

    }
}
