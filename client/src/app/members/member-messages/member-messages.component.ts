import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Message } from 'src/app/_models/message';

@Component({
   selector: 'app-member-messages',
   standalone: true,
   templateUrl: './member-messages.component.html',
   styleUrls: ['./member-messages.component.css'],
   imports: [CommonModule, FormsModule],
})
export class MemberMessagesComponent implements OnInit {
   @Input() username?: string;
   @Input() messages: Message[] = [];
   messageContent = '';

   constructor() {}

   ngOnInit(): void {}

   sendMessage() {}
}
