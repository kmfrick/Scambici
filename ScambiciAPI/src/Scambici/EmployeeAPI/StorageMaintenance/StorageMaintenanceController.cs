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

using System.Linq;

namespace Scambici.EmployeeAPI.StorageMaintenance
{
	public abstract class StorageMaintenanceController : Scambici.API.Controller
	{
		//Get all the storage maintenances
		public System.Collections.Generic.List<Scambici.Domain.StorageMaintenance> GetStorageMaintenances(Scambici.Domain.Store currentStore)
		{
			return dbContext.StorageMaintenances.Where(s => ((s.Completed != true) && (s.Bike.Store == currentStore))).ToList();

		}
		//Assign the specified delivery to the employee
		public bool TakeTask(Scambici.Domain.Employee employee, int storageMaintenanceId)
		{
			//Get the employee and verify that he doesn't have an actve task
			var dbEmployee = dbContext.Employees.Where(e => e.EmployeeId == employee.EmployeeId).First();
			//The employe can take the task
			if(dbEmployee.CurrentTask == null)
			{
				//Assign the task to the employee
				dbEmployee.CurrentTask = dbContext.StorageMaintenances.Where(s => s.TaskId == storageMaintenanceId).First();
				dbContext.SaveChanges();
				return true;
			}
			//The employee has a task already active
			else
			{  
				return false;
			}

		}
		//Terminate the current active task of the employee
		public void TerminateTask(Scambici.Domain.Employee employee)
		{
			//Get the employee and verify that he has an active task
			var dbEmployee = dbContext.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Where(e => e.CurrentTask != null).First();
			//Set the task as completed
			var completedTask = (Scambici.Domain.StorageMaintenance) dbEmployee.CurrentTask;
			completedTask.Completed = true;
			//Add the completed task to the list of completed task
			dbEmployee.Tasks.Add(completedTask);
			//Remove the task from the employee
			dbEmployee.CurrentTask = null;
			dbContext.SaveChanges();
		}
		//Scrap the bike
		public void ScrapBike(Scambici.Domain.Store currentStore, int bikeId)
		{
			//Get the store
			var store = dbContext.Stores.Where(s => s.StoreId == currentStore.StoreId).First();
			//Get the bike from the store
			var bike = store.Bikes.Where(b => b.BikeId == bikeId).First();
			//Remove the bike
			store.Bikes.Remove(bike);
			dbContext.SaveChanges();
		}
	}
}

