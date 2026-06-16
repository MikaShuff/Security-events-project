//open-event.component.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { API_BASE } from '../../config';

type LookupItem = { id: number; name: string };

@Component({
  standalone: true,
  selector: 'app-open-event',
  templateUrl: './open-event.component.html',
  styleUrls: ['./open-event.component.css'],
  imports: [CommonModule, ReactiveFormsModule],
})

export class OpenEventComponent implements OnInit {
  submitted = false;
  saving = false;
  
  private lookupsLoaded = { formats: false, zones: false };
  private pendingFormatId: number | null = null;
  private pendingSecurityAreaId: number | null = null;

  // פונקציה עוזרת – להציב כשמוכן
  
private applyPendingLookupsIfReady() {
  if (this.lookupsLoaded.formats && this.lookupsLoaded.zones) {
    if (this.pendingFormatId != null) {
      this.f.formatId.setValue(this.pendingFormatId, { emitEvent: false });
      this.f.formatId.disable({ emitEvent: false });      // נשאר נעול
    }
    if (this.pendingSecurityAreaId != null) {
      this.f.securityAreaId.setValue(this.pendingSecurityAreaId, { emitEvent: false });
      this.f.securityAreaId.disable({ emitEvent: false }); // נשאר נעול
    }

    this.pendingFormatId = null;
    this.pendingSecurityAreaId = null;
  }
}


  // Lookups
  branches: LookupItem[] = [];
  formats: LookupItem[] = [];
  securityAreas: LookupItem[] = [];
  eventTypes: LookupItem[] = [];
  subEventTypes: LookupItem[] = [];
  officers: LookupItem[] = [];
  handlings: LookupItem[] = [];
  statuses: LookupItem[] = [];

  //  מספר אירוע
  eventNumber: string | null = null;

  // חשוב: any כדי לא להיתקע על Strict typing בשלב הראשון
  form!: any;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
  ) {
    // יצירת הטופס בתוך constructor כדי לא לקבל "fb used before initialization"
    this.form = this.fb.group({
      occurredAt: ['', Validators.required],
      branchId: [null, Validators.required],
      formatId: [{ value: null, disabled: true }],
      securityAreaId: [{ value: null, disabled: true }],

      eventTypeId: [null, Validators.required],
      subEventTypeId: [{ value: null, disabled: true }, Validators.required],

      officerId: [null, Validators.required],
      customerId: ['', [Validators.required, Validators.pattern(/^\d{5,9}$/)]],

      description: [
        '',
        [
          Validators.required,
          Validators.minLength(5),
          Validators.maxLength(2000),
        ],
      ],
      amountNis: [null, [Validators.min(0)]],

      handlingId: [null, Validators.required],
      handlingDescription: ['', [Validators.maxLength(2000)]],

      remarks: ['', [Validators.maxLength(2000)]],
      statusId: [null, Validators.required],
    });
  }

  // Helper נוח לטמפלט: f.fieldName
  get f(): any {
    return this.form.controls;
  }

  ngOnInit(): void {
    // ברירת מחדל: היום
    this.form.patchValue({ occurredAt: this.toDateTimeLocalValue(new Date()) });

    // תלות: סוג אירוע -> תת סוג אירוע
    this.f.eventTypeId.valueChanges.subscribe((eventTypeId: any) => {
      // מאפסים תת-סוג בכל שינוי סוג
      this.subEventTypes = [];
      this.f.subEventTypeId.reset(null);

      // אם אין סוג אירוע - ננעל ונצא
      if (!eventTypeId) {
        this.f.subEventTypeId.disable();
        return;
      }

      // יש סוג אירוע - טוענים תתי סוג מהשרת
      this.loadSubEventTypes(Number(eventTypeId));
    });

    // טעינת סניפים ל Lookup
    this.http.get<any[]>(`${API_BASE}/api/branches`).subscribe({
      next: (data) => {
        // התאמה לשמות שדות: אם זה {id,name} או {branchId,branchName}
        this.branches = (data ?? []).map((x: any) => ({
          id: x.id ?? x.branchId ?? x.value ?? x.key,
          name: x.name ?? x.branchName ?? x.text ?? x.label,
        }));
      },
      error: (err) => {
        console.error('Failed to load branches', err);
        console.log('לא הצלחתי לטעון סניפים (בדקי CORS/כתובת API)');
      },
    });

    this.f.branchId.valueChanges.subscribe((branchId: any) => {
      this.f.formatId.disable({ emitEvent: false });
      this.f.securityAreaId.disable({ emitEvent: false });
      this.f.formatId.setValue(null, { emitEvent: false });
      this.f.securityAreaId.setValue(null, { emitEvent: false });

      if (!branchId) return;

      this.http
        .get<any>(`${API_BASE}/api/branches/${branchId}/details`)
        .subscribe({
          next: (d) => {
            // ⬅️ נציב IDs, לא שמות:
            this.pendingFormatId = d.companyId ?? null;
            this.pendingSecurityAreaId = d.securityZoneId ?? null;

            // ננסה להציב עכשיו; אם הלוקאפים עוד לא נטענו – נשלים בצעד 2
            this.applyPendingLookupsIfReady();
          },
          error: (err) => console.error('Failed to load branch details', err),
        });
    });

    // טעינת פורמטים ל Lookup
    this.http.get<any[]>(`${API_BASE}/api/formats`).subscribe({
      next: (data) => {
        this.formats = (data ?? []).map((x: any) => ({
          id: x.id ?? x.formatId ?? x.value ?? x.key,
          name: x.name ?? x.formatName ?? x.text ?? x.label,
        }));
        this.lookupsLoaded.formats = true;
        this.applyPendingLookupsIfReady(); // ⬅️ חשוב
      },
      error: (err) => {
        console.error('Failed to load formats', err);
        console.log('לא הצלחתי לטעון פורמטים');
      },
    });

    // טעינת אזורי ביטחון (Zones) ל Lookup
    this.http.get<any[]>(`${API_BASE}/api/zones`).subscribe({
      next: (data) => {
        this.securityAreas = (data ?? []).map((x: any) => ({
          id: x.id ?? x.zoneId ?? x.value ?? x.key,
          name: x.name ?? x.zoneName ?? x.text ?? x.label,
        }));
        this.lookupsLoaded.zones = true;
        this.applyPendingLookupsIfReady(); // ⬅️ חשוב
      },
      error: (err) => {
        console.error('Failed to load zones', err);
        console.log('לא הצלחתי לטעון אזורי ביטחון');
      },
    });

    // טעינת סוגי אירוע ל Lookup
    this.http.get<any[]>(`${API_BASE}/api/event-types`).subscribe({
      next: (data) => {
        this.eventTypes = (data ?? []).map((x: any) => ({
          id: x.id ?? x.eventTypeId ?? x.value ?? x.key,
          name: x.name ?? x.eventTypeName ?? x.text ?? x.label,
        }));
      },
      error: (err) => {
        console.error('Failed to load event types', err);
        console.log('לא הצלחתי לטעון סוגי אירוע');
      },
    });

    // טעינת קב"טים ל Lookup
    this.http.get<any[]>(`${API_BASE}/api/officers`).subscribe({
      next: (data) => {
        this.officers = (data ?? []).map((x: any) => ({
          id: x.id ?? x.officerId ?? x.value ?? x.key,
          name: x.name ?? x.officerName ?? x.fullName ?? x.text ?? x.label,
        }));
      },
      error: (err) => {
        console.error('Failed to load officers', err);
        console.log('לא הצלחתי לטעון קב"טים');
      },
    });

    // טעינת סטטוסים ל Lookup
    this.http.get<any[]>(`${API_BASE}/api/statuses`).subscribe({
      next: (data) => {
        this.statuses = (data ?? []).map((x: any) => ({
          id: x.id ?? x.statusId ?? x.value ?? x.key,
          name: x.name ?? x.statusName ?? x.text ?? x.label,
        }));
      },
      error: (err) => {
        console.error('Failed to load statuses', err);
        console.log('לא הצלחתי לטעון סטטוסים');
      },
    });

    // טעינת אופני טיפול ל Lookup
    this.http.get<any[]>(`${API_BASE}/api/handlings`).subscribe({
      next: (data) => {
        this.handlings = (data ?? []).map((x: any) => ({
          id: x.id ?? x.handlingId ?? x.value ?? x.key,
          name: x.name ?? x.handlingName ?? x.text ?? x.label,
        }));
      },
      error: (err) => {
        console.error('Failed to load handlings', err);
        console.log('לא הצלחתי לטעון אופני טיפול');
      },
    });
  }

  private loadSubEventTypes(eventTypeId: number): void {
    // מנקים בחירה קודמת ורשימה קודמת
    this.subEventTypes = [];
    this.f.subEventTypeId.reset(null);
    this.f.subEventTypeId.disable();

    this.http
      .get<
        any[]
      >(`${API_BASE}/api/sub-event-types?eventType=${eventTypeId}`)
      .subscribe({
        next: (data) => {
          // התאמה לשמות שדות אפשריים
          this.subEventTypes = (data ?? []).map((x: any) => ({
            id: x.id ?? x.subEventTypeId ?? x.value ?? x.key,
            name: x.name ?? x.subEventTypeName ?? x.text ?? x.label,
          }));

          // אם חזרו תוצאות - מאפשרים לבחור
          if (this.subEventTypes.length > 0) {
            this.f.subEventTypeId.enable();
          } else {
            this.f.subEventTypeId.disable();
          }
        },
        error: (err) => {
          console.error('Failed to load sub event types', err);
          this.subEventTypes = [];
          this.f.subEventTypeId.disable();
          console.log('לא הצלחתי לטעון תתי-סוג אירוע');
        },
      });
  }
  onSubmit(): void {
    this.submitted = true;
    this.form.markAllAsTouched();

    if (this.form.invalid) return;

    this.saving = true;

    // ✅ מיפוי מהטופס לשמות שהשרת מצפה להם (EventCreateDto)
    const payload = {
      eventDate: new Date(this.f.occurredAt.value).toISOString(),
      branchNum: this.f.branchId.value,
      eventType: this.f.eventTypeId.value,
      subEventId: this.f.subEventTypeId.value,
      officerId: this.f.officerId.value,
      handleType: this.f.handlingId.value,
      statusId: this.f.statusId.value,

      eventSum: this.f.amountNis.value ?? null,
      eventDesc: (this.f.description.value ?? '').trim(),
      handleDesc: (this.f.handlingDescription.value ?? '').trim(),
      remark: (this.f.remarks.value ?? '').trim(),
      ceoRemark: null, // כרגע אין שדה בטופס – אפשר להוסיף בעתיד
      customerTz: (this.f.customerId.value ?? '').trim(),
    };

    // open-event in db
    this.http.post<any>(`${API_BASE}/api/Events`, payload).subscribe({
      next: (created) => {
        this.saving = false;

        // אם השרת מחזיר את הישות – אמור להיות שם eventId
        this.eventNumber = created?.eventId ?? created?.eventID ?? null;

        alert(
          `האירוע נשמר בהצלחה ✅${this.eventNumber ? ` (מספר: ${this.eventNumber})` : ''}`,
        );
        this.router.navigate(['/events']);

        // אופציונלי: איפוס הטופס אחרי שמירה
        this.onCancel();
      },
      error: (err) => {
        this.saving = false;

        // ניסיון להציג שגיאה “אנושית”
        const msg =
          err?.error?.message ||
          (typeof err?.error === 'string' ? err.error : null) ||
          'שמירה נכשלה. בדקי שהנתונים תקינים (OfficerId וכו’)';

        console.error('Create event failed', err);
        alert(msg);
      },
    });
  }

  onCancel(): void {
    this.form.reset({
      occurredAt: this.toDateTimeLocalValue(new Date()),
      branchId: null,
      formatId: null,
      securityAreaId: null,
      eventTypeId: null,
      subEventTypeId: null,
      officerId: null,
      customerId: '',
      description: '',
      amountNis: null,
      handlingId: null,
      handlingDescription: '',
      remarks: '',
      statusId: null,
    });

    this.f.subEventTypeId.disable();
    this.subEventTypes = [];
    this.submitted = false;
  }

  private toDateTimeLocalValue(date: Date) {
    const pad = (n: number) => String(n).padStart(2, '0');
    const yyyy = date.getFullYear();
    const mm = pad(date.getMonth() + 1);
    const dd = pad(date.getDate());
    const hh = pad(date.getHours());
    const min = pad(date.getMinutes());
    return `${yyyy}-${mm}-${dd}T${hh}:${min}`;
  }
}
