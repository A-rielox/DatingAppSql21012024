import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

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

   // getMembers() {       -------> SIN PAGINAR
   //    if (this.members.length > 0) return of(this.members);

   //    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
   //       map((res) => {
   //          this.members = res;

   //          return res;
   //       })
   //    );
   // }
   getMembers(userParams: UserParams) {
      let params = this.getPaginationHeaders(
         userParams.pageNumber,
         userParams.pageSize
      );

      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);

      // { observe: 'response', params } pq me pase toda la respuesta y no solo el body
      return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params);
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

   //////////////////////////////////////////////
   //////////////////////////////////////////////
   //          FOTOS
   setMainPhoto(photoId: number) {
      return this.http.put(
         this.baseUrl + 'users/set-main-photo/' + photoId,
         {}
      );
   }

   deletePhoto(photoId: number) {
      return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
   }

   //////////////////////////////////////////////
   //////////////////////////////////////////////
   //          PAGINACION
   private getPaginationHeaders(pageNumber: number, pageSize: number) {
      // p' poner query string parameters
      let params = new HttpParams();

      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);

      return params;
   }

   private getPaginatedResult<T>(url: string, params: HttpParams) {
      const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

      // { observe: 'response', params } pq me pase toda la respuesta y no solo el body
      return this.http.get<T>(url, { observe: 'response', params }).pipe(
         map((res) => {
            if (res.body) {
               paginatedResult.result = res.body;
            }

            const pagination = res.headers.get('Pagination');

            if (pagination) {
               paginatedResult.pagination = JSON.parse(pagination);
            }

            return paginatedResult;
         })
      );
   }
}
