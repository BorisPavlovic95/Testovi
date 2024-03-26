import { Component } from '@angular/core';
import { EmployeeService } from './employee.service';
import { Employee } from './employee';

@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.css']
})
export class EmployeeComponent {
  employees: Employee[]= [];
  error: string | null = null;

  constructor(private employeeService: EmployeeService) { }

  ngOnInit(): void {
    this.fetchEmployees(); 
  }

  fetchEmployees(): void {
    this.employeeService.getEmployees().subscribe({
      next: (data: any[]) => {
        const groupedEmployees: {[key: string]: Employee} = {};
  
        data.forEach((item: any) => {
          const startTime = new Date(item.StarTimeUtc);
          const endTime = new Date(item.EndTimeUtc);
          const totalHoursWorked = Math.round((endTime.getTime() - startTime.getTime()) / (1000 * 60 * 60));
  
          if (!groupedEmployees[item.EmployeeName]) {
            groupedEmployees[item.EmployeeName] = {
              id: item.Id,
              employeeName: item.EmployeeName,
              starTimeUtc: startTime,
              endTimeUtc: endTime,
              totalTimeWorked: 0 
          
            };
          }
  
          groupedEmployees[item.EmployeeName].totalTimeWorked += totalHoursWorked;
        });
  
        
        this.employees = Object.values(groupedEmployees);
        this.employees.sort((a, b) => b.totalTimeWorked - a.totalTimeWorked);
        console.log('Employees:', this.employees);
      },
      error: (error: any) => {
        console.error('Error fetching employees:', error);
      }
    });
  }
  
  isLessThan100Hours(hours: number): boolean {
    return hours < 100;
  }

}
