import { Component } from '@angular/core';
import { EmployeeService } from './employee.service';
import { Employee } from './employee';
import Chart from 'chart.js/auto';

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

        this.createPieChart(this.employees);
      },
      error: (error: any) => {
        console.error('Error fetching employees:', error);
      }
    });
  }
  
  isLessThan100Hours(hours: number): boolean {
    return hours < 100;
  }

  createPieChart(data: any[]): void {
    // Calculate total hours worked across all employees
    const totalHours = data.reduce((acc, employee) => acc + employee.totalTimeWorked, 0);
  
    const labels = data.map(employee => employee.employeeName);
    const percentages = data.map(employee => (employee.totalTimeWorked / totalHours) * 100);
  
    const canvas: any = document.getElementById('pieChart');
    const ctx = canvas.getContext('2d');
  
    new Chart(ctx, {
      type: 'pie',
      data: {
        labels: labels,
        datasets: [{
          label: 'Percentage of Total Time Worked',
          data: percentages,
          backgroundColor: [
            'rgba(255, 99, 132, 0.6)',
            'rgba(54, 162, 235, 0.6)',
            'rgba(255, 206, 86, 0.6)',
            'rgba(75, 192, 192, 0.6)',
            'rgba(153, 102, 255, 0.6)',
            'rgba(255, 159, 64, 0.6)'
          ]
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false
      }
    });
  }
  

}
