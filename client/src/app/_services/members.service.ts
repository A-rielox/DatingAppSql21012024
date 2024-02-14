import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { HttpClient } from '@angular/common/http';
import { map, of } from 'rxjs';

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
      if (this.members.length > 0) return of(this.members);

      return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
         map((res) => {
            this.members = res;

            return res;
         })
      );
   }

   getMember(userName: string) {
      const member = this.members.find((m) => m.userName === userName);
      if (member) return of(member);

      return this.http.get<Member>(this.baseUrl + 'users/' + userName);
   }

   updateMember(member: Member) {
      return this.http.put(this.baseUrl + 'users', member).pipe(
         map(() => {
            const index = this.members.indexOf(member);

            this.members[index] = { ...this.members[index], ...member };
         })
      );
   }
}
