<<<<<<< HEAD
import { Component, OnInit } from '@angular/core';
import { ReadingService } from '../../services/reading.service';
import { Reading } from '../../models/reading.model';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-reading',
  templateUrl: './reading.component.html',
  styleUrls: ['./reading.component.css'],
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, MatListModule]
})
export class ReadingComponent implements OnInit {
  readings: Reading[] = [];

  constructor(private readingService: ReadingService) { }

  ngOnInit(): void {
    this.loadReadings();
  }

  loadReadings(): void {
    this.readingService.getReadings().subscribe(
      data => this.readings = data,
      error => console.error('Error loading readings', error)
    );
  }
}
=======
import { Component } from '@angular/core';

@Component({
  selector: 'app-reading',
  imports: [],
  templateUrl: './reading.component.html',
  styleUrl: './reading.component.css'
})
export class ReadingComponent {

}
>>>>>>> 957651ab69f38f9c9b52999ab9d286d402ce51e6
