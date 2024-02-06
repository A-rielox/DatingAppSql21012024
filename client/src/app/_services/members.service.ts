import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { HttpClient } from '@angular/common/http';

@Injectable({
   providedIn: 'root',
})
export class MembersService {
   baseUrl = environment.apiUrl;
   members: Member[] = [];
   memberCache = new Map();

   user: User | undefined;
   // userParams: UserParams | undefined;

   constructor(private http: HttpClient) {}

   getMembers() {
      return this.http.get<Member[]>(this.baseUrl + 'users');
   }

   getMember(userName: string) {
      return this.http.get<Member>(this.baseUrl + 'users/' + userName);
   }
}