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
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
   selector: 'app-register',
   templateUrl: './register.component.html',
   styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
   @Output() cancelRegister = new EventEmitter();
   @Input() usersFromHomeComp: any;
   model: any = {};
   registerForm: FormGroup = new FormGroup({});

   constructor(
      private accountService: AccountService,
      private fb: FormBuilder,
      private toastr: ToastrService,
      private router: Router
   ) {}

   ngOnInit(): void {
      this.initializeForm();

      // this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
   }

   // prettier-ignore
   initializeForm() {
      this.registerForm = this.fb.group({
         // gender: ['male'],
         username: ['', Validators.required],
         // knownAs: ['', Validators.required],
         // dateOfBirth: ['', Validators.required],
         // city: ['', Validators.required],
         // country: ['', Validators.required],
         password: [ '', [ Validators.required, Validators.minLength(4), Validators.maxLength(12) ] ],
         confirmPassword: [ '', [Validators.required, this.matchValues('password')] ],
      });

      // por si cambia el password despues de poner el confirmPassword y pasar la validacion
      this.registerForm.controls['password'].valueChanges.subscribe({
         next: () =>
            this.registerForm.controls[
               'confirmPassword'
            ].updateValueAndValidity(),
      });
   }

   matchValues(matchTo: string): ValidatorFn {
      return (control: AbstractControl) => {
         return control.value === control.parent?.get(matchTo)?.value
            ? null
            : { notMatching: true };
      };
   }

   register() {
      console.log(this.registerForm.value);
      // this.accountService.register(this.model).subscribe({
      //    next: (res) => {
      //       this.cancel(); // cierro el register form
      //    },
      //    error: (err) => {
      //       console.log(err);
      //       this.toastr.error(err.error + '  💩');
      //    },
      // });
   }

   cancel() {
      this.cancelRegister.emit(false);
   }
}
