
import { Component, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-open-ticket',
  templateUrl: './open-ticket.component.html',
  styleUrls: ['./open-ticket.component.css'], // אם יש לך קובץ CSS
  imports: [CommonModule, ReactiveFormsModule],
})

export class OpenTicketComponent {
  constructor(private location: Location, private router: Router) { }

  private fb = inject(FormBuilder);

  form = this.fb.group({
    subject: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required, Validators.minLength(10)]],
    priority: ['Medium', Validators.required],
  });

  isSubmitting = false;
  serverMessage: string | null = null;

  submit() {
    if (this.form.invalid) {
      this.serverMessage = 'נא להשלים את כל השדות.';
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.serverMessage = null;

    // סימולציה – בהמשך נחבר לשרת
    setTimeout(() => {
      this.isSubmitting = false;
      this.serverMessage = 'הקריאה נפתחה (סימולציה).';
      this.form.reset({ priority: 'Medium' });
    }, 600);
  }
}
