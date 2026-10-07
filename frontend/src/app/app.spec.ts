import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { App } from './app';

describe('App', () => {
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  function flushOccupations() {
    http.match('http://localhost:5134/api/occupations').forEach((r) => r.flush([]));
  }

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    flushOccupations();
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the IT 04-1 header', async () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    flushOccupations();
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.app-header')?.textContent).toContain('IT 04-1');
  });
});
