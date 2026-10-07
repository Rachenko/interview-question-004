import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { isValidBirthDay, PersonFormComponent } from './person-form.component';

describe('isValidBirthDay', () => {
  it('accepts dd/MM/yyyy', () => {
    expect(isValidBirthDay('15/08/1995')).toBe(true);
  });

  it('rejects wrong formats and impossible dates', () => {
    expect(isValidBirthDay('1995-08-15')).toBe(false);
    expect(isValidBirthDay('32/01/1995')).toBe(false);
    expect(isValidBirthDay('15/13/1995')).toBe(false);
    expect(isValidBirthDay('abc')).toBe(false);
  });
});

describe('PersonFormComponent', () => {
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonFormComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  function validForm(c: PersonFormComponent) {
    c.form.setValue({
      firstName: 'Somchai',
      lastName: 'Jaidee',
      email: 'somchai@example.com',
      phone: '081-234-5678',
      profile: 'data:image/png;base64,AQID',
      birthDay: '15/08/1995',
      occupation: '1',
      sex: 'male',
    });
  }

  it('is invalid when empty', () => {
    const fixture = TestBed.createComponent(PersonFormComponent);
    expect(fixture.componentInstance.form.invalid).toBe(true);
  });

  it('rejects an invalid email', () => {
    const fixture = TestBed.createComponent(PersonFormComponent);
    const c = fixture.componentInstance;
    validForm(c);
    c.form.controls.email.setValue('bad');
    expect(c.form.invalid).toBe(true);
  });

  it('posts the form and shows a success toast with the id', async () => {
    const fixture = TestBed.createComponent(PersonFormComponent);
    const c = fixture.componentInstance;
    validForm(c);

    c.save();

    const req = http.expectOne('http://localhost:5134/api/persons');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.email).toBe('somchai@example.com');
    expect(req.request.body.occupationId).toBe(1);
    req.flush({ id: 7 });

    expect(c.toastMessage()).toBe('save data success Id : 7');
    expect(c.form.pristine).toBe(true);
  });

  it('does not call the API when the form is invalid', () => {
    const fixture = TestBed.createComponent(PersonFormComponent);
    const c = fixture.componentInstance;

    c.save();

    http.expectNone('http://localhost:5134/api/persons');
  });
});
