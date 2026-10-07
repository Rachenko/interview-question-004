import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RegisterPersonRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  profile: string;
  birthDay: string;
  occupation: string;
  sex: string;
}

export interface RegisterPersonResponse {
  id: number;
}

@Injectable({ providedIn: 'root' })
export class PersonService {
  private readonly http = inject(HttpClient);
  // Local dev: the API runs on localhost:5134. In hosted previews the API is
  // exposed on the same host pattern with the 5134 port prefix.
  private readonly apiUrl = (() => {
    const host = window.location.hostname;
    if (host === 'localhost') return 'http://localhost:5134/api/persons';
    const match = /^4200--(.+)$/.exec(host);
    return match
      ? `${window.location.protocol}//5134--${match[1]}/api/persons`
      : `${window.location.protocol}//${host.replace('4200', '5134')}/api/persons`;
  })();

  register(request: RegisterPersonRequest): Observable<RegisterPersonResponse> {
    return this.http.post<RegisterPersonResponse>(this.apiUrl, request);
  }
}
