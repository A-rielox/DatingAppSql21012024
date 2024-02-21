import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
   selector: 'app-member-list',
   templateUrl: './member-list.component.html',
   styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
   members: Member[] = [];
   pagination: Pagination | undefined;
   userParams: UserParams | undefined;
   user: User | undefined;

   genderList = [
      { value: 'male', display: 'Males' },
      { value: 'female', display: 'Females' },
   ];

   constructor(
      private memberService: MembersService,
      private accountService: AccountService
   ) {
      // this.userParams = this.memberService.getUserParams();

      this.accountService.currentUser$.pipe(take(1)).subscribe({
         next: (user) => {
            if (user) {
               this.userParams = new UserParams(user);
               this.user = user;
            }
         },
      });
   }

   ngOnInit(): void {
      this.loadMembers();
   }

   loadMembers() {
      // if (this.userParams) {
      //    this.memberService.setUserParams(this.userParams);
      //    this.memberService.getMembers(this.userParams).subscribe({
      //       next: (res) => {
      //          if (res.result && res.pagination) {
      //             this.members = res.result;
      //             this.pagination = res.pagination;
      //          }
      //       },
      //    });
      // }

      if (!this.userParams) return;

      this.memberService.getMembers(this.userParams).subscribe({
         next: (res) => {
            if (res.result && res.pagination) {
               this.members = res.result;
               this.pagination = res.pagination;
            }
         },
      });
   }

   // applyFilters() {
   //    if (!this.userParams) return;

   //    this.userParams.pageNumber = 1;

   //    this.loadMembers();
   // }

   // resetFilters() {
   //    this.userParams = this.memberService.resetUserParams();

   //    this.loadMembers();
   // }

   pageChanged(e: any) {
      if (this.userParams && this.userParams?.pageNumber !== e.page) {
         this.userParams.pageNumber = e.page;
         this.loadMembers();
      }

      // if (this.userParams && this.userParams?.pageNumber !== e.page) {
      //    this.userParams.pageNumber = e.page;
      //    // para que tambien cambie en el memberService q es donde esta "respaldado" p' el caso en q se cambia de pagina
      //    this.memberService.setUserParams(this.userParams);
      //    this.loadMembers();
      // }
   }
}
