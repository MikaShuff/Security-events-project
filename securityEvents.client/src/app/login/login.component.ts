//login.component.ts
import { Component } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth/auth.service';
import { AuthState } from '../auth/auth-state.service';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  form: any;
  error = '';
  loading = false;
  showPassword = false;

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private authState: AuthState,
    private router: Router,
  ) {
    this.form = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  private setLoading(v: boolean) {
    this.loading = v;
    if (v) this.form.disable({ emitEvent: false });
    else this.form.enable({ emitEvent: false });
  }

  onSubmit() {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.setLoading(true);
    this.error = '';

    const { username, password } = this.form.value;

    this.auth.login(username, password).subscribe({
      next: () => {
        this.authState.refreshMe().subscribe({
          next: () => {
            this.setLoading(false);
            this.router.navigate(['/']);
          },
          error: () => {
            this.setLoading(false);
            this.router.navigate(['/']);
          },
        });
      },
      error: () => {
        this.error = 'שם משתמש או סיסמה שגויים';
        this.setLoading(false);
      },
    });
  }
}