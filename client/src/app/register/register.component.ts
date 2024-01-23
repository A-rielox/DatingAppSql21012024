import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import {
   AbstractControl,
   FormBuilder,
   FormGroup,
   ValidatorFn,
   Validators,
} from '@angular/forms';

@Component({
   selector: 'app-register',
   templateUrl: './register.component.html',
   styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
   @Output() cancelRegister = new EventEmitter();
   @Input() usersFromHomeComp: any;
   model: any = {};

   constructor(
      private accountService: AccountService // private toastr: ToastrService
   ) {}

   ngOnInit(): void {}

   register() {
      this.accountService.register(this.model).subscribe({
         next: (res) => {
            this.cancel(); // cierro el register form
         },
         error: (err) => {
            console.log(err);
            // this.toastr.error(err.error + '  💩');
         },
      });
   }

   cancel() {
      this.cancelRegister.emit(false);
   }
}
