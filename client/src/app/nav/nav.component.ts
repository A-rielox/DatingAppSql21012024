import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';

@Component({
   selector: 'app-nav',
   templateUrl: './nav.component.html',
   styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
   model: any = { userName: 'pepi', password: 'P@ssw0rd' };

   constructor(public accountService: AccountService, private router: Router) {}

   ngOnInit(): void {}

   login() {
      this.accountService.login(this.model).subscribe({
         next: () => {
            // console.log(res, 'api respuesta');
            this.router.navigateByUrl('/members');
         },
         error: (err) => console.log(err),
      });
   }

   logout() {
      this.accountService.logout();

      this.router.navigateByUrl('/');
   }
}
