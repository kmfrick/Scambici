// This file is part of Scambici.
// Copyright (C) 2020 Giovanni Lucia, Stefano Fantazzini and Kevin Michael Frick
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https:// www.gnu.org/licenses/>.

// See ScambiciContext_CreateNewUser for explanation of this using directive
using System.Linq;

namespace Scambici.Tests.EmployeeAPI
{
	public class GetRenatlInterruptionControllerTestable : Scambici.EmployeeAPI.ConfirmRentalInterruption.ConfirmRentalInterruptionController 
	{
		public GetRenatlInterruptionControllerTestable(Scambici.Domain.ScambiciContext context)
		{	dbContext = context; }
	}

	[NUnit.Framework.TestFixture]
	public class ConfirmRenatlInterruptionController_GetInterruptions
	{
		Scambici.Domain.ScambiciContext dbContext;
		Scambici.EmployeeAPI.ConfirmRentalInterruption.ConfirmRentalInterruptionController controller;
		[NUnit.Framework.SetUp]
		public void SetUp()
		{
			//Setup mock
			dbContext = new Scambici.Tests.ScambiciContextMock();
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();
			controller = new GetRenatlInterruptionControllerTestable(dbContext);
			//Employee
            Scambici.Domain.Employee mazzini = new Scambici.Domain.Employee { Name = "Giuseppe", Surname = "Mazzini", EMailAddress = "mazzini@giovane.it", PasswordHash = "12345", Tasks = new System.Collections.Generic.List<Scambici.Domain.Task>() };
			dbContext.Add(mazzini);
			dbContext.SaveChanges();
			//Store
			Scambici.Domain.Store miliani = new Scambici.Domain.Store { StoreId = 123, Address = "Via Pietro Miliani 7, Bologna", Administrator = mazzini, Employees = new System.Collections.Generic.List<Scambici.Domain.Employee>(), Users =  new System.Collections.Generic.List<Scambici.Domain.User>() };
            dbContext.Add(miliani);
			dbContext.SaveChanges();
			//Bike
			var bike = new Scambici.Domain.Bike { BikeId = 5566, StoreId = 123 };
			dbContext.Add(bike);
			dbContext.SaveChanges();
			//User 1
            Scambici.Domain.User garibaldi = new Scambici.Domain.User { Name = "Giuseppe", Surname = "Garibaldi", Address = "Via dei Mille 1000, Nizza", 
				EMailAddress = "garibaldi@italia.it", PasswordHash = "farelitalia12345", RentedBike = bike, 
				MaintenanceRequests = new System.Collections.Generic.List<Scambici.Domain.UserMaintenance>(),
				RentalInterruptions = new System.Collections.Generic.List<Scambici.Domain.RentalInterruption>(),
				TheftReports = new System.Collections.Generic.List<Scambici.Domain.TheftReport>(),
				Deliveries = new System.Collections.Generic.List<Scambici.Domain.Delivery>(),
				Store = miliani };
			dbContext.Add(garibaldi);
			dbContext.SaveChanges();
			//User 2
            Scambici.Domain.User carboni = new Scambici.Domain.User { Name = "Luca", Surname = "Carboni", Address = "Via dei Duemila 2000, Nizza", 
				EMailAddress = "Carboni@italia.it", PasswordHash = "sonobrabo1!1!", RentedBike = null, 
				MaintenanceRequests = new System.Collections.Generic.List<Scambici.Domain.UserMaintenance>(),
				RentalInterruptions = new System.Collections.Generic.List<Scambici.Domain.RentalInterruption>(),
				TheftReports = new System.Collections.Generic.List<Scambici.Domain.TheftReport>(),
				Deliveries = new System.Collections.Generic.List<Scambici.Domain.Delivery>(),
				Store = miliani };
			//Interruption
            Scambici.Domain.RentalInterruption interrupt1 = new Scambici.Domain.RentalInterruption { RentalInterruptionId = 1, Confirmed = true, CreatedBy = garibaldi };
            Scambici.Domain.RentalInterruption interrupt2 = new Scambici.Domain.RentalInterruption { RentalInterruptionId = 2, Confirmed = false, CreatedBy = garibaldi };
            Scambici.Domain.RentalInterruption interrupt3 = new Scambici.Domain.RentalInterruption { RentalInterruptionId = 3, Confirmed = false, CreatedBy = carboni };
			dbContext.Add(interrupt1);
			dbContext.Add(interrupt2);
			dbContext.Add(interrupt3);
			dbContext.SaveChanges();		
		}

		[NUnit.Framework.Test]
		public void ConfirmRenatlInterruptionController_DBHasInterruptions_GetInterruptions()
		{
			NUnit.Framework.Assert.That(() => controller.GetRentalInterruptionRequests(dbContext.Stores.First()), NUnit.Framework.Throws.Nothing);
			var interruptions = controller.GetRentalInterruptionRequests(dbContext.Stores.First());
			var carboni = dbContext.Users.Where(u => u.Surname == "Carboni").First();
			NUnit.Framework.Assert.That(interruptions.Count, NUnit.Framework.Is.EqualTo(2));
			NUnit.Framework.Assert.That(interruptions[0].RentalInterruptionId, NUnit.Framework.Is.EqualTo(2));
			NUnit.Framework.Assert.That(interruptions[1].CreatedBy, NUnit.Framework.Is.EqualTo(carboni));
		}
	}
}