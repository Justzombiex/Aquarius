import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-ph-display',
  templateUrl: './ph-display.component.html',
  styleUrls: ['./ph-display.component.css'],
  imports: [CommonModule]
})
export class PhDisplayComponent {
  phLevel: number = 7.5;
  
  get status(): string {
    if (this.phLevel < 6.5) return 'Ácido';
    if (this.phLevel > 7.5) return 'Alcalino';
    return 'Neutral';
  }

  get statusColor(): string {
    if (this.phLevel < 6.5) return 'acid';
    if (this.phLevel > 7.5) return 'alkaline';
    return 'neutral';
  }

  calculatePhPosition(): string {
    return `${(this.phLevel / 14) * 100}%`;
  }

  bubbles = Array(5).fill(0).map((_, i) => ({
    size: Math.random() * 10 + 5,
    left: Math.random() * 100,
    delay: Math.random() * 3,
    duration: Math.random() * 3 + 3
  }));

  getBubbleStyle(bubble: any): string {
    return `
      width: ${bubble.size}px;
      height: ${bubble.size}px;
      left: ${bubble.left}%;
      animation-delay: ${bubble.delay}s;
      animation-duration: ${bubble.duration}s;
    `;
  }
}