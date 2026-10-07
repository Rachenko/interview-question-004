import { Component } from '@angular/core';
import { PersonFormComponent } from './person-form/person-form.component';

@Component({
  imports: [PersonFormComponent],
  selector: 'app-root',
  templateUrl: './app.html',
})
export class App {}
