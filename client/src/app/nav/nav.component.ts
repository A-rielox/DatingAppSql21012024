import { Component } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
   selector: 'app-nav',
   templateUrl: './nav.component.html',
   styleUrls: ['./nav.component.css'],
})
export class NavComponent {
   model: any = { userName: 'pepi', password: 'P@ssw0rd' };
   loggedIn = false;

   constructor(private accountService: AccountService) {}

   login() {
      console.log(this.model);
      this.accountService.login(this.model).subscribe({
         next: (res) => {
            console.log(res, 'api respuesta');
            this.loggedIn = true;
         },
         error: (err) => console.log(err),
      });
   }

   logout() {
      this.loggedIn = false;
   }
}
