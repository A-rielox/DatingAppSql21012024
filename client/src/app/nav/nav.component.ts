import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
   selector: 'app-nav',
   templateUrl: './nav.component.html',
   styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
   model: any = { userName: 'pepi', password: 'P@ssw0rd' };

   constructor(public accountService: AccountService) {}

   ngOnInit(): void {}

   login() {
      console.log(this.model);
      this.accountService.login(this.model).subscribe({
         next: (res) => {
            console.log(res, 'api respuesta');
            this.accountService.login(res);
         },
         error: (err) => console.log(err),
      });
   }

   logout() {
      this.accountService.logout();
   }
}
