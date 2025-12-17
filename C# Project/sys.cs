using System;
using System.Collections.Generic;

namespace HospitalManagementSystem
{
    // Delegate for billing
    public delegate double BillingStrategy(double amount);

    // Event arguments
    public class PatientEventArgs : EventArgs
    {
        public string PatientName { get; set; }
        public double BillAmount { get; set; }
    }

    // Base Patient class
    public class Patient
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Type { get; set; }

        public Patient(string name, int age, string type)
        {
            Name = name;
            Age = age;
            Type = type;
        }

        public virtual double GetBaseBill()
        {
            if (Type == "Regular")
                return 1000;
            else if (Type == "Emergency")
                return 3000;
            else
                return 5000; // ICU
        }
    }

    // Hospital System
    public class Hospital
    {
        // Event declaration
        public event EventHandler<PatientEventArgs> PatientAdmitted;
        public event EventHandler<PatientEventArgs> BillGenerated;

        private List<Patient> patients = new List<Patient>();

        public void AdmitPatient()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIT PATIENT ===\n");

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Age: ");
            int age = int.Parse(Console.ReadLine());

            Console.WriteLine("\nSelect Type:");
            Console.WriteLine("1. Regular");
            Console.WriteLine("2. Emergency");
            Console.WriteLine("3. ICU");
            Console.Write("Choice: ");
            string choice = Console.ReadLine();

            string type = "";
            if (choice == "1") type = "Regular";
            else if (choice == "2") type = "Emergency";
            else type = "ICU";

            Patient patient = new Patient(name, age, type);
            patients.Add(patient);

            // Raise event - Patient Admitted
            if (PatientAdmitted != null)
            {
                PatientAdmitted(this, new PatientEventArgs 
                { 
                    PatientName = patient.Name 
                });
            }

            // Calculate Bill
            CalculateBill(patient);
        }

        public void CalculateBill(Patient patient)
        {
            double baseBill = patient.GetBaseBill();

            Console.WriteLine($"\nBase Bill: ₹{baseBill}");
            Console.WriteLine("\nSelect Billing Type:");
            Console.WriteLine("1. Normal");
            Console.WriteLine("2. Insurance (20% off)");
            Console.Write("Choice: ");
            string choice = Console.ReadLine();

            // Delegate for billing strategy
            BillingStrategy billing;

            if (choice == "2")
                billing = (amount) => amount * 0.80; // Insurance
            else
                billing = (amount) => amount; // Normal

            double finalBill = billing(baseBill);

            Console.WriteLine("\n--- BILL DETAILS ---");
            Console.WriteLine($"Patient-Name: {patient.Name}");
            Console.WriteLine($"Type: {patient.Type}");
            Console.WriteLine($"Final Bill: ₹{finalBill}");

            // Raise event - Bill Generated
            if (BillGenerated != null)
            {
                BillGenerated(this, new PatientEventArgs 
                { 
                    PatientName = patient.Name,
                    BillAmount = finalBill 
                });
            }

            Console.WriteLine("\n✓ Patient admitted successfully!");
            Console.ReadKey();
        }

        public void ViewPatients()
        {
            Console.Clear();
            Console.WriteLine("=== ALL PATIENTS ===\n");

            if (patients.Count == 0)
            {
                Console.WriteLine("No patients found.");
            }
            else
            {
                int count = 1;
                foreach (Patient p in patients)
                {
                    Console.WriteLine($"{count}. Name: {p.Name} | Age: {p.Age} | Type: {p.Type}");
                    count++;
                }
            }

            Console.ReadKey();
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("   HOSPITAL MANAGEMENT SYSTEM");
                Console.WriteLine("====================================");
                Console.WriteLine("\n1. Admit Patient");
                Console.WriteLine("2. View All Patients");
                Console.WriteLine("3. Exit");
                Console.Write("\nEnter Choice: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    AdmitPatient();
                }
                else if (choice == "2")
                {
                    ViewPatients();
                }
                else if (choice == "3")
                {
                    Console.WriteLine("\nThank you!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice!");
                    Console.ReadKey();
                }
            }
        }
    }

    // Department class for event handling
    public class Department
    {
        public void Subscribe(Hospital hospital)
        {
            hospital.PatientAdmitted += OnPatientAdmitted;
            hospital.BillGenerated += OnBillGenerated;
        }

        private void OnPatientAdmitted(object sender, PatientEventArgs e)
        {
            Console.WriteLine($"\n[NOTIFICATION] Patient {e.PatientName} has been admitted.");
        }

        private void OnBillGenerated(object sender, PatientEventArgs e)
        {
            Console.WriteLine($"[NOTIFICATION] Bill of ₹{e.BillAmount} generated for {e.PatientName}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Hospital hospital = new Hospital();
            Department dept = new Department();
            
            // Subscribe department to hospital events
            dept.Subscribe(hospital);

            hospital.ShowMenu();
        }
    }
}