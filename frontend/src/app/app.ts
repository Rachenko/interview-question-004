import { Component, signal } from '@angular/core';
import { PersonFormComponent } from './person-form/person-form.component';

@Component({
  imports: [PersonFormComponent],
  selector: 'app-root',
  templateUrl: './app.html',
})
export class App {
  readonly header = signal('IT 04-1');

  onSaved(): void {
    this.header.set('IT 04-2');
  }

  onCleared(): void {
    this.header.set('IT 04-1');
  }
}
