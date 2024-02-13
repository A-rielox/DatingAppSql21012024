import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
   selector: 'app-member-edit',
   templateUrl: './member-edit.component.html',
   styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
   // | undefined xq al construir el componente puede estar indefinido
   @ViewChild('editForm') editForm: NgForm | undefined;
   member: Member | undefined;
   user: User | null = null;

   constructor(
      private accountService: AccountService,
      private memberService: MembersService,
      private toastr: ToastrService
   ) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
         next: (user) => {
            this.user = user;
         },
      });
   }

   ngOnInit(): void {
      this.loadMember();
   }

   loadMember() {
      if (!this.user) return;

      this.memberService.getMember(this.user.userName).subscribe({
         next: (res) => {
            this.member = res;
         },
      });
   }

   updateMember() {
      console.log(this.member, 'member');

      // this.memberService.updateMember(this.editForm?.value).subscribe({
      //    next: () => {
      this.toastr.success('Profile updated successfully');
      this.editForm?.reset(this.member);
      //    },
      // });
   }
}
