using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            {
                switch (crudOperation)
                {
                    case "create":
                        db.Employees.InsertOnSubmit(employee);
                        db.SubmitChanges();
                        break;
                    case "read":
                        employee = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();
                        break;
                    case "update":
                        Employee employeefromdb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();
                        employeefromdb.FirstName = employee.FirstName;
                        employeefromdb.LastName = employee.LastName;
                        employeefromdb.UserName = employee.UserName;
                        employeefromdb.Password = employee.Password;
                        employeefromdb.EmployeeNumber = employee.EmployeeNumber;
                        employeefromdb.Email = employee.Email;
                        db.SubmitChanges();
                        break;
                    case "delete":
                        Employee deleteEmployeefromdb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();
                        db.Employees.DeleteOnSubmit(deleteEmployeefromdb);
                        db.SubmitChanges();
                        break;
                }
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
           var animal = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
           return animal;
            
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            var animal = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();
            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        var updateCategory = db.Categories.Where(c => c.Name == update.Value).FirstOrDefault();
                        animal.CategoryId = updateCategory.CategoryId;
                        break;
                    case 2:
                        animal.Name = update.Value;
                        break;
                    case 3:
                        animal.Age = Int32.Parse(update.Value);
                        break;
                    case 4:
                        animal.Demeanor = update.Value;
                        break;
                    case 5:
                        if (update.Value == "yes")
                        {
                            animal.KidFriendly = true;
                        }
                        else
                        {
                            animal.KidFriendly = false;
                        }
                        break;
                    case 6:
                        if (update.Value == "yes")
                        {
                            animal.PetFriendly = true;
                        }
                        else
                        {
                            animal.PetFriendly = false;
                        }
                        break;
                    case 7:
                        animal.Weight = Int32.Parse(update.Value);
                        break;
                }
            }
        }



        internal static void RemoveAnimal(Animal animal)
        {
            Animal deleteAnimalfromdb = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
            db.Animals.DeleteOnSubmit(deleteAnimalfromdb);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            var filteredAnimals = db.Animals.Select(a => a);
            foreach (KeyValuePair<int, string> update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        var searchedCategory = db.Categories.Where(c => c.Name == update.Value).FirstOrDefault();
                        filteredAnimals = filteredAnimals.Where(a => a.Name == searchedCategory.Name);
                        break;
                    case 2:
                        filteredAnimals = filteredAnimals.Where(a => a.Name == update.Value);
                        break;
                    case 3:
                        filteredAnimals = filteredAnimals.Where(a => a.Age == Int32.Parse(update.Value));
                        break;
                    case 4:
                        filteredAnimals = filteredAnimals.Where(a => a.Demeanor == update.Value);
                        break;
                    case 5:
                        if (update.Value == "yes")
                        {
                            bool friendly = true;
                            filteredAnimals = filteredAnimals.Where(a => a.KidFriendly == friendly);
                        }
                        if (update.Value == "no")
                        {
                            bool friendly = false;
                            filteredAnimals = filteredAnimals.Where(a => a.KidFriendly == friendly);
                        }
                        break;
                    case 6:
                        if (update.Value == "yes")
                        {
                            bool friendly = true;
                            filteredAnimals = filteredAnimals.Where(a => a.PetFriendly == friendly);
                        }
                        if (update.Value == "no")
                        {
                            bool friendly = false;
                            filteredAnimals = filteredAnimals.Where(a => a.PetFriendly == friendly);
                        }
                        break;
                    case 7:
                        filteredAnimals = filteredAnimals.Where(a => a.Weight == Int32.Parse(update.Value));
                        break;
                    case 8:
                        filteredAnimals = filteredAnimals.Where(a => a.AnimalId == Int32.Parse(update.Value));
                        break;
                }
            }
            return filteredAnimals;

        }









        // TODO: Misc Animal Things
         internal static int GetCategoryId(string categoryName)
        {
            return db.Categories.Where(a => a.Name == categoryName).FirstOrDefault().CategoryId;           
        }
        
        internal static Room GetRoom(int animalId)
        {
            return db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
            
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            return db.DietPlans.Where(d => d.Name == dietPlanName).FirstOrDefault().DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption { ClientId = client.ClientId, AnimalId = animal.AnimalId, ApprovalStatus = "Pending", AdoptionFee = 75, PaymentCollected = false };
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
           return db.Adoptions.Where(a => a.ApprovalStatus == "Pending" );
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption deleteAdoptionfromdb = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(deleteAdoptionfromdb);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            return db.AnimalShots.Where(a => a.ShotId == animal.AnimalId);
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}