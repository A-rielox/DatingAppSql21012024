import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { User } from '../_models/user';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
   providedIn: 'root',
})
export class MembersService {
   baseUrl = environment.apiUrl;
   members: Member[] = [];
   userParams: UserParams | undefined;
   user: User | undefined;
   memberCache = new Map();

   constructor(private http: HttpClient, accountService: AccountService) {
      accountService.currentUser$.pipe(take(1)).subscribe({
         next: (user) => {
            if (user) {
               this.userParams = new UserParams(user);
               this.user = user;
            }
         },
      });
   }

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
      const response = this.memberCache.get(
         Object.values(userParams).join('-')
      );

      if (response) return of(response);

      let params = getPaginationHeaders(
         userParams.pageNumber,
         userParams.pageSize
      );

      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);

      // { observe: 'response', params } pq me pase toda la respuesta y no solo el body
      return getPaginatedResult<Member[]>(
         this.baseUrl + 'users',
         params,
         this.http
      ).pipe(
         map((res) => {
            this.memberCache.set(Object.values(userParams).join('-'), res);

            return res;
         })
      );
   }

   getMember(userName: string) {
      const member = [...this.memberCache.values()]
         .reduce((t, i) => {
            return t.concat(i.result);
         }, [])
         .find((member: Member) => member.userName === userName);

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

   //////////////////////////////////////
   //////////     PARAMS
   //////////////////////////////////////
   getUserParams() {
      return this.userParams;
   }

   setUserParams(params: UserParams) {
      this.userParams = params;
   }

   resetUserParams() {
      if (this.user) {
         this.userParams = new UserParams(this.user);

         return this.userParams;
      }

      return;
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

   //////////////////////////////////////
   //////////     LIKES
   //////////////////////////////////////
   addLike(username: string) {
      return this.http.post(this.baseUrl + 'likes/' + username, {});
   }

   //                                                 NO PAGINE VIDEO 181
   getLikes(predicate: string /* , pageNumber: number, pageSize: number */) {
      return this.http.get<Member[]>(
         this.baseUrl + 'likes?predicate=' + predicate
      );

      // let params = getPaginationHeaders(pageNumber, pageSize);

      // params = params.append('predicate', predicate);

      // return getPaginatedResult<Member[]>(
      //    this.baseUrl + 'likes',
      //    params,
      //    this.http
      // );
   }

   //////////////////////////////////////////////
   //////////////////////////////////////////////
   //          PAGINACION
   // private getPaginationHeaders(pageNumber: number, pageSize: number) {
   //    // p' poner query string parameters
   //    let params = new HttpParams();

   //    params = params.append('pageNumber', pageNumber);
   //    params = params.append('pageSize', pageSize);

   //    return params;
   // }

   // private getPaginatedResult<T>(url: string, params: HttpParams) {
   //    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

   //    // { observe: 'response', params } pq me pase toda la respuesta y no solo el body
   //    return this.http.get<T>(url, { observe: 'response', params }).pipe(
   //       map((res) => {
   //          if (res.body) {
   //             paginatedResult.result = res.body;
   //          }

   //          const pagination = res.headers.get('Pagination');

   //          if (pagination) {
   //             paginatedResult.pagination = JSON.parse(pagination);
   //          }

   //          return paginatedResult;
   //       })
   //    );
   // }
}
