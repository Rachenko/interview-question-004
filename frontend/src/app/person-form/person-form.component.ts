import { Component, ElementRef, ViewChild, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { PersonService } from './person.service';

@Component({
  selector: 'app-person-form',
  imports: [ReactiveFormsModule],
  templateUrl: './person-form.component.html',
  styleUrl: './person-form.component.css',
})
export class PersonFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly persons = inject(PersonService);

  @ViewChild('profileInput') profileInput!: ElementRef<HTMLInputElement>;
  @ViewChild('birthDayPicker') birthDayPicker!: ElementRef<HTMLInputElement>;

  readonly occupations = [
    'Software Developer',
    'System Analyst',
    'Project Manager',
    'QA Engineer',
    'UX/UI Designer',
    'Business Analyst',
    'DevOps Engineer',
    'Data Engineer',
  ];

  readonly profileFileName = signal('');
  readonly toastMessage = signal('');
  readonly saving = signal(false);
  submitted = false;

  private toastTimer: ReturnType<typeof setTimeout> | null = null;

  readonly form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.pattern(/^\+?[0-9][0-9\- ]{7,14}$/)]],
    profile: ['', Validators.required],
    birthDay: ['', [Validators.required, birthDayValidator]],
    occupation: ['', Validators.required],
    sex: ['', Validators.required],
  });

  error(control: keyof typeof this.form.controls): string {
    const c = this.form.controls[control];
    if (!this.submitted && !c.touched) return '';
    if (!c.errors) return '';
    if (c.errors['required']) {
      switch (control) {
        case 'email':
          return 'Please provide a valid Email';
        case 'phone':
          return 'Please provide a valid Phone';
        case 'birthDay':
          return 'Please provide a valid Birth Day';
        case 'profile':
          return 'Please selected any profile';
        case 'occupation':
          return 'Please selected any Occupation';
        default:
          return `${labelOf(control)} is required.`;
      }
    }
    switch (control) {
      case 'email':
        return 'Please provide a valid Email';
      case 'phone':
        return 'Please provide a valid Phone';
      case 'birthDay':
        return 'Please provide a valid Birth Day';
      default:
        return 'Invalid value';
    }
  }

  openProfilePicker(): void {
    this.profileInput.nativeElement.click();
  }

  onProfileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
      this.form.controls.profile.setValue(reader.result as string);
      this.profileFileName.set(file.name);
    };
    reader.readAsDataURL(file);
  }

  openBirthDayPicker(): void {
    const picker = this.birthDayPicker.nativeElement;
    const current = this.form.controls.birthDay.value;
    if (current && isValidBirthDay(current)) {
      const [d, m, y] = current.split('/');
      picker.value = `${y}-${m.padStart(2, '0')}-${d.padStart(2, '0')}`;
    }
    picker.showPicker();
  }

  onBirthDayPicked(event: Event): void {
    const value = (event.target as HTMLInputElement).value; // yyyy-MM-dd
    if (!value) return;
    const [y, m, d] = value.split('-');
    this.form.controls.birthDay.setValue(`${d}/${m}/${y}`);
    this.form.controls.birthDay.markAsTouched();
  }

  save(): void {
    this.submitted = true;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.persons.register(this.form.getRawValue() as never).subscribe({
      next: (res) => {
        this.saving.set(false);
        this.showToast(`save data success Id : ${res.id}`);
        this.clear();
      },
      error: () => {
        this.saving.set(false);
        this.showToast('save data failed');
      },
    });
  }

  clear(): void {
    this.form.reset();
    this.profileFileName.set('');
    this.submitted = false;
    if (this.profileInput) this.profileInput.nativeElement.value = '';
  }

  private showToast(message: string): void {
    this.toastMessage.set(message);
    if (this.toastTimer) clearTimeout(this.toastTimer);
    this.toastTimer = setTimeout(() => this.toastMessage.set(''), 5000);
  }
}

function labelOf(control: string): string {
  const labels: Record<string, string> = {
    firstName: 'First Name',
    lastName: 'Last Name',
    email: 'Email',
    phone: 'Phone',
    profile: 'Profile',
    birthDay: 'Birth Day',
    occupation: 'Occupation',
    sex: 'Sex',
  };
  return labels[control] ?? control;
}

export function isValidBirthDay(value: string): boolean {
  const match = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(value);
  if (!match) return false;
  const [, d, m, y] = match;
  const date = new Date(+y, +m - 1, +d);
  return date.getFullYear() === +y && date.getMonth() === +m - 1 && date.getDate() === +d;
}

export const birthDayValidator: ValidatorFn = (control: AbstractControl) => {
  const value: string | null = control.value;
  if (!value) return null;
  return isValidBirthDay(value) ? null : { birthDay: true };
};
