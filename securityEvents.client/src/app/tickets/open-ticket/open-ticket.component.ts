// src/app/tickets/open-ticket/open-ticket.component.ts
import { Component, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-open-ticket',
  templateUrl: './open-ticket.component.html',
  styleUrls: ['./open-ticket.component.css'],
  imports: [CommonModule, ReactiveFormsModule],
})
export class OpenTicketComponent {
  private fb = inject(FormBuilder);

  constructor(
    private location: Location,
    private router: Router
  ) { }

  // טופס
  form = this.fb.group({
    subject: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required, Validators.minLength(10)]],
    priority: ['Medium', Validators.required],
  });

  isSubmitting = false;
  serverMessage: string | null = null;
  serverSuccess = false;

  submit() {
    if (this.form.invalid) {
      this.serverSuccess = false;
      this.serverMessage = 'נא להשלים את כל השדות.';
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.serverMessage = null;
    this.serverSuccess = false;

    // סימולציה — בהמשך נחבר לשרת (POST)
    setTimeout(() => {
      this.isSubmitting = false;
      this.serverSuccess = true;
      this.serverMessage = 'הקריאה נפתחה (סימולציה).';
      this.form.reset({ priority: 'Medium' });
      // אם תרצי אחרי פתיחה לנווט לדף האירועים:
      // this.router.navigate(['/events']);
    }, 600);
  }

  // ניווטים שימושיים
  goBack() {
    this.location.back();
  }

  goHome() {
    this.router.navigate(['/']);
  }
}
